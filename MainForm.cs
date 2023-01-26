using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        readonly int[] BAUD_RATES_LAST = { 110, 1200, 2400, 4800, 14400, 19200, 38400, 57600, 115200 }; //, 128000, 256000 }; //most devices don't support over 115200

        DataStore ds;
        int frameCount;

        public MainForm()
        {
            InitializeComponent();

            ds = new DataStore();
            frameCount = 0;

            //TODO: optimize each timer for their purpose, more may need to be added
            timer1.Start();
            timer2.Start();
        }

        /// <summary>
        /// Validate data states, ensure devices are still connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //process available cached data
            while (ds.dataQueue.Any()) 
            {
                double[] data = ds.dataQueue.Dequeue();
            }

            //TODO: check on a cycle if ports are open, remove port1IsOpen clause
            if (frameCount % 100 == 0 && !serialPort1.IsOpen) 
            {
                //TODO: allow for many serial ports to be connected to at once if available
                //query serial ports available
                //  load names and info into UI
                foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
                {
                    bool testBauds = true;
                    //cycle through possible baud rates until combination is found
                    for (int i = 0; i < BAUD_RATES_FIRST.Length + BAUD_RATES_LAST.Length && testBauds; i++)
                    {
                        //test first set of rates, most likely, then second set
                        int baudRate = 0;
                        if (i < BAUD_RATES_FIRST.Length)
                            baudRate = BAUD_RATES_FIRST[i];
                        else
                            baudRate = BAUD_RATES_LAST[i - BAUD_RATES_FIRST.Length];

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
                                testBauds = false;
                        }
                    }

                    if (testBauds == false)
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
            double[] data = new double[2]; //dataIn
            //search serial ports for active ports -- TODO: later - check id's for this
            ds.dataQueue.Enqueue(data);
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
    }
}
