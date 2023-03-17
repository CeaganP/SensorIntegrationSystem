using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
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
        readonly int[] BAUD_RATES_FIRST = { 300, 600, 9600 }; //if these fail test the rest        
        readonly int[] BAUD_RATES_SECOND = { 110, 1200, 2400, 4800, 14400, 19200, 38400, 57600, 115200 }; //, 128000, 256000 }; //most devices don't support over 115200

        DataStore ds;
        int frameCount;
        Queue<byte[]> dataIn;

        public MainForm()
        {
            InitializeComponent();

            ds = new DataStore();
            dataIn = new Queue<byte[]>();
            frameCount = 0;

            /*
            CONSIDERATIONS: 
                What other UI elements does a user want? 
                How do I know what COM port I am reading from?
                Can I see the data being read from it?
             */

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
            //TODO: populate list box with COM ports
            //TODO: after populating COM ports, attach event for double clicking each line item
            //          upon double click begin reading the data from the COM port
            string[] ports = GetPorts();
            //here is the string of ports 
            //display the ports in the box
            listBox_Ports.Items.Clear(); //prevent duplicate entries
            listBox_Ports.Items.AddRange(ports);
            return ports;
        }

        /// <summary>
        /// Validate data states, ensure devices are still connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //process available cached data
            while (dataIn.Any() || ds.dataQueue.Any()) 
            {
                if (ds.dataQueue.Any()) 
                {
                    double[] dataDouble = ds.dataQueue.Dequeue();
                }

                if (dataIn.Any()) 
                {
                    byte[] dataByte = dataIn.Dequeue();
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
            dataIn.Enqueue(dataToRead);
            Console.WriteLine(dataSize);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO: close all additional processes, clean up before exiting application
            if (serialPort1.IsOpen)
                serialPort1.Close();

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
                    if (serialPort1.IsOpen)
                        serialPort1.Close();

                    serialPort1.PortName = portName;
                    serialPort1.BaudRate = baudRate;
                    //TODOO: add more checks 
                    //"System.IO.IOException: 'The semaphore timeout period has expired.
                    //Tries to open serial port, but no devices connected, so it times out.
                    serialPort1.Open();

                    //port made connection, go to next port
                    if (serialPort1.IsOpen)
                        return true;
                }
            }

            return false;
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
    }
    
}
