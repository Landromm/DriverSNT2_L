using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSNT2_L.ComPort
{
    internal class CommunicationManager
    {
        public enum TransmissionType { Text, Hex }

        //property variables
        private string _baudRate = string.Empty;
        private string _parity = string.Empty;
        private string _stopBits = string.Empty;
        private string _dataBits = string.Empty;
        private string _portName = string.Empty;
        private int count;
        private List<string> _dataByteList;

        #region Manager Properties
        public List<string> DataByteList
        {
            get { return _dataByteList; }
            set { _dataByteList = value; }
        }
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }
        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }
        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public string StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }
        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public string DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }
        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }
        #endregion

        private SerialPort comPort = new SerialPort();

        public CommunicationManager(string baud, string par, string sBits, string dBits, string name)
        {
            _baudRate = baud;
            _parity = par;
            _stopBits = sBits;
            _dataBits = dBits;
            _portName = name;
            _dataByteList = new List<string>();

            //now add an event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }
        public CommunicationManager()
        {
            _baudRate = string.Empty;
            _parity = string.Empty;
            _stopBits = string.Empty;
            _dataBits = string.Empty;
            _portName = "COM2";
            _dataByteList = new List<string>();

            //add event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        /// <summary>
        /// Метод проверки открытия COM-порта.
        /// </summary>
        public bool ComPortIsOpen()
        {
            if (comPort.IsOpen && !comPort.BreakState)
                return true;
            else
                return false;
        }

        #region OpenPort
        /// <summary>
        /// Метод открытия COM-порта.
        /// </summary>
        public bool OpenPort()
        {
            try
            {
                if (comPort.IsOpen) comPort.Close();
                comPort.BaudRate = int.Parse(_baudRate);    //BaudRate
                comPort.DataBits = int.Parse(_dataBits);    //DataBits
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);    //StopBits
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);    //Parity
                comPort.PortName = _portName;   //PortName
                //Console.WriteLine("Read_TimeOut = " + comPort.ReadTimeout);
                //comPort.ReadTimeout = 1000;
                comPort.Open();

                Console.WriteLine(("Открытие COM-порта: " + comPort.PortName));
                Thread.Sleep(5000);

                Console.WriteLine("COM-порт открыт в |" + DateTime.Now + "|\n");
                return true;
            }
            catch (Exception ex)
            {
                ConsoleColorOutputError("Не удалось открыть COM-Порт!", ex);
                return false;
            }
        }
        #endregion

        #region ClosePort
        /// <summary>
        /// Метод закрытия COM-порта.
        /// </summary>
        public bool ClosePort()
        {
            try
            {
                if (comPort.IsOpen == true || comPort.BreakState)
                {
                    comPort.BreakState = false;
                    comPort.Close();
                }
                Console.WriteLine(("Закрытие COM-порта: " + comPort.PortName));
                Console.WriteLine("COM-порт закрыт в |" + DateTime.Now + "|\n");
                return true;
            }
            catch (Exception ex)
            {
                ConsoleColorOutputError("Не удалось корректно закрыть COM-Порт!", ex);
                return false;
            }
        }
        #endregion

        #region HexToByte
        /// <summary>
        /// Метод конвертации строки-hex в массив байтов.
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        private byte[] HexToByte(string msg)
        {
            try
            {
                msg = msg.Replace(" ", "");
                byte[] comBuffer = new byte[msg.Length / 2];
                for (int i = 0; i < msg.Length; i += 2)
                    comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
                return comBuffer;
            }
            catch (Exception ex)
            {
                ConsoleColorOutputError("Ошибка конвертации полученной строки в массив байтов!", ex);
                //протетсировать данноое возвращение!!!!!!!!!!!! Возможно наличие багов.
                throw;
            }
        }
        #endregion

        #region ByteToHex
        /// <summary>
        /// Метод конвертации массива байт в строку-hex.
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a hex string</returns>
        private string ByteToHex(byte[] comByte)
        {
            try
            {
                StringBuilder builder = new StringBuilder(comByte.Length * 3);
                foreach (byte data in comByte)
                    builder.Append(Convert.ToString(data, 16).PadLeft(2, '0'));

                return builder.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                ConsoleColorOutputError("Ошибка конвертации массива байт в строку-hex!", ex);
                return string.Empty; 
            }
        }
        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// Метод, который будет вызываться, когда в буфере ожидают данные.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!comPort.BreakState)
            {
                try
                {
                    int bytes = comPort.BytesToRead;
                    byte[] comBuffer = new byte[bytes];
                    comPort.Read(comBuffer, 0, bytes);
                    //Console.WriteLine("{0} " + ByteToHex(comBuffer), count);
                    //Console.WriteLine(ByteToHex(comBuffer));
                    count += bytes;
                    foreach (var item in comBuffer)
                    {
                        _dataByteList.Add(Convert.ToString(item, 16).PadLeft(2, '0').ToUpper());
                    }
                    //Console.WriteLine("Размер коллекции: " + _dataByteList.Count);
                }
                catch (Exception ex)
                {
                    ConsoleColorOutputError("Ошибка чтения с COM-порта.!", ex);
                }
            }
            else
            {
                string str = comPort.ReadExisting();
                Console.WriteLine("Разрыв связи с COM-портом: " + str);
            }
        }
        #endregion

        #region WriteData
        /// <summary>
        /// Метод отправки данных-hex счетчику.
        /// </summary>
        /// <param name="msg">string hex</param>
        public void WriteData(string msg)
        {
            try
            {
                if (!(comPort.IsOpen == true))
                    comPort.Open();
                byte[] newMsg = HexToByte(msg);
                //Console.WriteLine("\n" + "ОТПРАВКА hex-сообщения счетчику: |" + msg);
                comPort.Write(newMsg, 0, newMsg.Length);
            }
            catch (FormatException ex)
            {
                ConsoleColorOutputError("Ошибка отправки hex - сообщения счетчику: ", ex);
                byte[] newMsg = HexToByte(msg);
                Console.WriteLine(ByteToHex(newMsg) + "\n\n");
            }
        }
        #endregion

        //Метод форматировванного цветного вывода текста ошибки.
        private void ConsoleColorOutputError(string str, Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"{str}\n");
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(ex.Message + "\n");
            Console.ResetColor();
        }
    }
}
