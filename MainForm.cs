﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SenseSys
{
    public partial class MainForm : Form
    {
        //https://www.codeproject.com/Articles/678025/Serial-Comms-in-Csharp-for-Beginners#:~:text=The%20standard%20baud%20rate%20for,connecting%20to%20embedded%20processors%20%26%20microcontrollers.
        /*The standard baud rate for connections is 9600 (the default for the serial port class) the lower baud rates 600 & 300 are for connecting to embedded processors & microcontrollers*/
        readonly int[] BAUD_RATES_FIRST = { 9600, 600, 300 }; //if these fail test the rest        
        readonly int[] BAUD_RATES_SECOND = { 115200, 57600, 38400, 19200, 14400, 4800, 2400, 1200, 110 }; //, 128000, 256000 }; //most devices don't support over 115200

        DataStore ds;
        int frameCount;
        ConcurrentQueue<object> dataIn;

        public MainForm()
        {
            InitializeComponent();

            ds = new DataStore();
            dataIn = new ConcurrentQueue<object>();
            frameCount = 0;

            /*
            CONSIDERATIONS: 
                What other UI elements does a user want? 
                How do I know what COM port I am reading from?
                Can I see the data being read from it?
            
             Ideas:             Add your own if you come up with them!
                [X] View ports 
                [X] Connect port(s)
                    Read data
                    Write data
                    Store data
                    Create Graphical User Interface (GUI)
                    Connect GUI to core methods

             */


            serialPort1.ReadTimeout = 500;

            string[] ports = RefreshListBoxPorts(); //update the UI then grab the port list
            if (ports.Length > 0 && ConnectToPort(ports[0])) //length is valid so connect to first port
            {
                System.Console.WriteLine("Connected to port: " + ports[0]); //$"Connected to port: {ports[0]}"
            }

            //byte[] data = new byte[20];
            List<byte[]> data = new List<byte[]>();


            //TODO: optimize each timer for their purpose, more may need to be added
            timer1.Interval = 100; //10 times a second
            timer1.Start();
            //timer2.Start();
        }

        /// <summary>
        /// Update the UI then return the ports as an array, clear listbox_ports each call
        /// </summary>
        /// <returns>string[] port array</returns>
        public string[] RefreshListBoxPorts() 
        {
            string[] ports = GetPorts();
            bool valid = ports.Length == listBox_Ports.Items.Count;

            //display the ports in the box
            //  clear potential duplicate entries
            for (int i = 0; i < ports.Length && valid; i++) 
            {
                valid = ports[i].Equals(listBox_Ports.Items[i]);
                if (!valid) break;
            }
            
            //if they don't align, reset box and skip checking remaining entries
            if (!valid) 
            {
                listBox_Ports.Items.Clear();
                listBox_Ports.Items.AddRange(ports);
            }
            return ports;
        }

        private void WriteDataToFile(object data, string fileName = "dataCollected.txt") 
        {
            FileInfo fi = new FileInfo(fileName);
            if (!fi.Exists) File.Create(fileName).Close();
            //case for the data type
            if (data is byte[] dataBytes)
                File.AppendAllText(fileName, Convert.ToBase64String(dataBytes));

            /*FileStream fs;
            Encoding writerEncoding = new UTF8Encoding();
            TextWriter tw = new StreamWriter(fs, writerEncoding);
            tw.Write(dataBytes);
            tw.Close();
            fs.Close();*/
        }

        private string ReadDataFromFile(string fileName = "dataCollected.txt")
        {
            FileInfo fi = new FileInfo(fileName);
            if (fi.Exists) return File.ReadAllText(fileName);
            return "";
        }

        /// <summary>
        /// Validate data states, ensure devices are still connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void timer1_Tick(object sender, EventArgs e)
        {
            //process available cached data
            while (dataIn.Any() || ds.dataQueue.Any()) 
            {
                if (ds.dataQueue.Any()) 
                {
                    double[] dataDoubles = ds.dataQueue.Dequeue();
                }

                if (dataIn.Any())
                {
                    dataIn.TryDequeue(out object data);
                    string strBytes = "";
                    if (data is byte[] dataBytes)
                    {
                        strBytes = string.Join("", dataBytes);
                        WriteDataToFile(dataBytes);
                    }
                    if (data is float[] dataFloat)
                    {
                        strBytes = string.Join("", dataFloat);
                        WriteDataToFile(dataFloat);
                    }

                    richTextBox_Data.Text = strBytes + "\t" + richTextBox_Data.Text;
                }
            }

            //TODO: old code, perhaps clean up?
            #region JUNKDELETE
            /*if (frameCount % 100 == 0 && !serialPort1.IsOpen) 
            {
                //TODO: allow for many serial ports to be connected to at once if available -- requires refactor of serial ports
                //query serial ports available
                //  load names and info into UI
                foreach (string portName in GetPorts())
                {
                    if (!ConnectToPort(portName))
                    {
                        Console.WriteLine("Connection to [" + serialPort1.PortName + "] was made");
                        break;
                    }
                }
            }
            */
            #endregion

            //update UI here once every 0.5 seconds
            if (frameCount % 5 == 0) 
            {
                RefreshListBoxPorts();
            }
            frameCount++;
        }

        /// <summary>
        /// Process the data to be stored
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            //double[] data = new double[2]; //dataIn
            //search serial ports for active ports -- TODO: later - check id's for this
            //.Enqueue(data);
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //this implementation is inefficient for real-time processing
            //serial port has no configuration on buffer size, data transmission size and does not asynchronously process data
            int dataSize = serialPort1.BytesToRead;
            int offset = 0;

            //takes all the bytes available in the serial port and copies them to shallow array
            byte[] dataToRead = new byte[dataSize];
            serialPort1.Read(dataToRead, offset, dataSize);
            
            //for is our divisor due to the bytes in each float
            float[] dataFloats = new float[dataSize / 4]; //define size not initial contents
            for (int i = 0; i < dataSize; i += 4) 
            {
                //take 0000 > 0.f
                float value = BitConverter.ToSingle(dataToRead, i);
                dataFloats[i / 4] = value;
            }

            dataIn.Enqueue(dataFloats.ToString());
            Console.WriteLine(dataSize);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO: close all additional processes, clean up before exiting application
            DisconnectFromPort();

            timer1.Stop();
            timer2.Stop();
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            //read textbox_Input add to listbox_Contents
            listBox_Contents.Items.Add(textBox_Input.Text);
            //clear textbox_input text
            textBox_Input.ResetText();
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            //remove the current list item from listbox_Contents
            if (listBox_Contents.SelectedIndex >= 0)
                listBox_Contents.Items.RemoveAt(listBox_Contents.SelectedIndex);
        }

        private void listBox_Contents_DoubleClick(object sender, EventArgs e)
        {
            //double click and select the item so we can update it
            if (listBox_Contents.SelectedIndex >= 0) 
            {
                //take the selected item and move it to the input text box
                textBox_Input.Text = listBox_Contents.SelectedItem.ToString();
                listBox_Contents.Items.RemoveAt(listBox_Contents.SelectedIndex);
            }
        }


        /// <summary>
        /// Query the serial ports connected to the device
        /// </summary>
        /// <returns>An array of connected serial ports</returns>
        private string[] GetPorts() 
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }
        
        /// <summary>
        /// Ensure the serial port disconnects from previous ports if connected
        ///     Attempt to connect to provided port
        /// </summary>
        /// <returns>connected to port successsfully</returns>
        private bool ConnectToPort(string portName) 
        {
            //cycle through possible baud rates until combination is found
            for (int i = 0; i < BAUD_RATES_FIRST.Length + BAUD_RATES_SECOND.Length; i++)
            {
                //test first set of rates, most likely, then second set
                int baudRate = 0;
                if (i < BAUD_RATES_FIRST.Length)
                    baudRate = BAUD_RATES_FIRST[i];
                else
                    baudRate = BAUD_RATES_SECOND[i - BAUD_RATES_FIRST.Length];

                //TODO: allow for many connections at once
                //pick port(s) to connect to
                if (portName.Length > 0)
                {
                    DisconnectFromPort();

                    serialPort1.PortName = portName;
                    serialPort1.BaudRate = baudRate;
                    //TODOO: add more checks 
                    //"System.IO.IOException: 'The semaphore timeout period has expired.
                    //Tries to open serial port, but no devices connected, so it times out.
                    try
                    {
                        serialPort1.Open();
                    }
                    catch (System.IO.IOException timeout) 
                    {
                        label_Log.Text = "Connection timed out " + portName + " " + baudRate;
                        return false;
                    }

                    //port made connection, go to next port
                    if (serialPort1.IsOpen) 
                    {
                        label_Log.Text = "Connected to port " + portName + " " + baudRate;
                        return true;
                    }
                }
            }
            
            label_Log.Text = "Failed to connect to port " + portName;
            return false;
        }

        public void DisconnectFromPort() 
        {
            //TODO: use indexing if many ports exist
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }


        private void portUpdate_Click(object sender, EventArgs e)
        {
            //Clear existing ports from listbox 2, and update with lastest ports
            string[] ports = GetPorts();
            listBox_Ports.Items.Clear();
            listBox_Ports.Items.AddRange(ports);
        }

        //FIXME: Selecting another Serial Port causes program to crash
        private void listBo_Ports_Contents_DoubleClick(object sender, EventArgs e)
        {
            if (sender is ListBox lb) 
            {
                if (lb.SelectedIndex < 0) { return; } //prevent null entries, no selected index = -1
                string selectPort = lb.SelectedItem.ToString();
                if (selectPort != null)
                {
                    if (ConnectToPort(selectPort))
                    {
                        System.Console.WriteLine("Connected to Port: " + selectPort);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] ports = GetPorts();
            ConnectToPort(ports[0]);
            groupBox1.Controls.Add(new PortBox());
        }

        private void listBox_Ports_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            DisconnectFromPort();
        }
    }
    
}
