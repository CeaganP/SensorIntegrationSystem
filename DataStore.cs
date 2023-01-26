using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseSys
{
    class DataStore
    {
        private double?[] lineData; //the current contents within the line
        private bool lineFull;      //true when the length of the blocks is the same total as the maximum size
        private int lineSize;       //TODO: line size after each assignment
        private int lineCount;      //the total lines that have been printed
        private double lineAverage; //TODO: the average time taken to write each line
        
        //TODO: validate functionality on single point data sets       must be (re)calculated through method SetBlockSize
        private readonly int smallestBlockSize; //initial datasize
        private readonly int smallestLineMaxSize; //initial smallest line max size
        private int blockSize;
        private int lineMaxSize;

        public int entryCount; //number of entries in the data set
        public readonly int blockCount;
        public readonly string fileExtension;

        //TODO: convert to hashset based off unique IDS for each serial port
        public Queue<double[]> dataQueue;
        

        public enum DataTypes : int
        {
            DATA = 0
        };

        public DataStore()
        {
            blockCount = Enum.GetNames(typeof(DataTypes)).Length; //dynamic sizing based off how many storage groups we have

            #region LegacyDataStorage
            //manually copy smallest block sizes 
            SetBlockSize(3); 
            smallestBlockSize = blockSize;
            smallestLineMaxSize = lineMaxSize;
            #endregion

            //initialize chart
            dataQueue = new Queue<double[]>();
            lineSize = 0;

            SetBlockSize(4);    //set current block size based off how many data points in each set we have
            
            lineData = new double?[lineMaxSize];
            lineFull = false;

            fileExtension = ".DAT";
        }

        //calculate or recalculate the total line and block size
        public void SetBlockSize(int size)
        {
            blockSize = size;
            //regenerate values dependent on block size
            lineMaxSize = blockCount * blockSize;
        }
        public int GetNextBlockSize(int size)
        {
            return blockCount * size;
        }


        public Queue<double[]> GetQueue(int dtype)
        {
            return GetQueue((DataTypes)dtype);
        }
        public ref Queue<double[]> GetQueue(DataTypes dtype)
        {
            switch (dtype)
            {
                default:
                case DataTypes.DATA: return ref dataQueue;
            }
        }

        /// <summary>
        /// Clean method to retrieve single element within the queue
        /// </summary>
        /// <param name="dtype">The queue type to retrieve element from</param>
        /// <param name="index">The desired element index to retrieve</param>
        /// <returns>Single element at index of type DataTypes</returns>
        public double[] GetQueueSingle(DataTypes dtype, int index)
        {
            Queue<double[]> target;
            switch (dtype)
            {
                default:
                case DataTypes.DATA:
                    target = dataQueue;
                    break;
            }

            return target.ElementAt(index);
        }

        public void SaveData(double[] data, string loc, int dtypeIndex, string ver)
        {
            int blockStart = blockSize * dtypeIndex;
            for (int i = 0; i < blockSize; i++)
            {
                lineData[blockStart + i] = data[i];
            }

            EvaluateLine();

            if (lineFull && loc.Length > 0)
            {
                //                              ((DataTypes)dtypeIndex).ToString()
                string file = Path.Combine(loc, ver + fileExtension);
                StreamWriter stream = File.AppendText(file);
                string dataString = "";
                foreach (double? point in lineData)
                {
                    if (point == null)
                        dataString += $"0,";
                    else
                        dataString += $"{point},";
                }
                dataString = dataString.TrimEnd(',');

                //ensure the file is never left open if unexpectedly fails to write data
                try
                {
                    stream.WriteLine(dataString);
                }
                finally
                {
                    stream.Close();
                    ClearLine();
                }
                lineCount++;
            }
            else if (lineFull && loc.Length == 0) //count the lines recorded to still determine accuracy 
            {
                lineCount++;
            }
        }

        public void ClearAllData()
        {
            for (int i = 0; i < blockCount; i++)
                GetQueue(i).Clear();
        }

        public void LoadFileData(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);

            //load all lines into data sets
            for (int i = 0; i < lines.Length; i++)
                LoadLine(lines[i]);

            entryCount = lines.Length;
        }


        /// <summary>
        /// Take a single line from a data file and parse all of the columns to the appropriate queue
        /// </summary>
        /// <param name="line">Raw string of numbers</param>
        private void LoadLine(string line)
        {
            //blockSize;
            //split and load the numbers from the line
            string[] numbers = line.Split(',');

            //number of points read in is smaller than the current set
            //  determine smallest possible amount of points
            while (numbers.Length < lineMaxSize && numbers.Length <= GetNextBlockSize(blockSize - 1) && lineMaxSize > smallestLineMaxSize)
            { 
                SetBlockSize(blockSize - 1);        //re-evaluate the line size
            } 

            double?[] dnumbers = new double?[lineMaxSize];
            for (int i = 0; i < numbers.Length; i++)
            {
                double.TryParse(numbers[i], out double dnum);
                dnumbers[i] = dnum;
            }

            for (int i = 0; i < blockCount; i++)
            {
                int start = i * blockSize;
                double[] blockData = new double[blockSize];
                for (int j = 0; j < blockSize; j++)
                {
                    double? targetNum = dnumbers[start + j];
                    if (targetNum == null)
                        targetNum = 0;

                    blockData[j] = (double)targetNum;
                }

                GetQueue(i).Enqueue(blockData);
            }
        }

        private void ClearLine()
        {
            for (int i = 0; i < lineData.Length; i++)
            {
                lineData[i] = null;
                lineSize--;
            }
        }

        private void EvaluateLine()
        {
            int lineSize = lineMaxSize;
            for (int i = 0; i < blockSize; i++)
            {
                if (lineData[i] == null)
                    lineSize--;
            }

            lineFull = lineSize == lineMaxSize;
        }

        public int GetLineCount() => lineCount;
        public void ResetLineCount() => lineCount = 0;

        //add indexer
        public Queue<double[]> this[int i]
        {
            get { return GetQueue(i); }
        }
        public Queue<double[]> this[DataTypes ty]
        {
            get { return GetQueue(ty); }
        }

    }
}
