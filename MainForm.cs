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


            //#####REVIEW BEGIN#####
            /*
            CONSIDERATIONS: 
                What other UI elements do I want? 
                How do I know what COM port I am reading from?
                Can I see the data being read from it?
             */
            //TODO: populate list box with COM ports
            //TODO: after populating COM ports, attach event for double clicking each line item
            //          upon double click begin reading the data from the COM port
            string[] ports = GetPorts();
            for (int i = 0; i < ports.Length; i++)
            {
                listBox2_Contents.Items.Add(ports[i]);
            }

            //conditional checks without a boolean are implicitly 
            //  checking if the condition is equals to true,
            //  this also happens with numbers in some languages where, 0=false, 1=true
            if (ConnectToPort(ports[0])) //ConnectToPort(ports[0])==true
            {
                System.Console.WriteLine("Connected to port: " + ports[0]); //$"Connected to port: {ports[0]}"
            }

            //byte[] data = new byte[20];
            List<byte[]> data = new List<byte[]>();

            //#####REVIEW END#####            

            //TODO: optimize each timer for their purpose, more may need to be added
            //timer1.Start();
            //timer2.Start();
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

            //TODO: check on a cycle if ports are open, remove port1IsOpen clause
            if (frameCount % 100 == 0 && !serialPort1.IsOpen) 
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

            //update UI here
            if (frameCount % 10 == 0) 
            {
                Label tester = new Label();
                tester.Text = frameCount.ToString();
                tester.Location = new Point(frameCount % 37, frameCount % 117);
                tester.BringToFront();
                
                MainForm.ActiveForm.Controls.Add(tester);
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
            listBox2_Contents.Items.Clear();
            for (int i = 0; i < ports.Length; i++)
            {
                listBox2_Contents.Items.Add(ports[i]);
            }
        }
        //FIXME: Selecting another Serial Port causes program to crash
        private void listBox2_Contents_DoubleClick(object sender, EventArgs e)
        {
            string selectPort = listBox2_Contents.SelectedItem.ToString();
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
