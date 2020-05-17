using PackageManager;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialComManager
{
    public class SerialListener
    {
        #region Events

        public delegate void NetworkPackageReceived(NetworkPackage package);
        public event NetworkPackageReceived PackageReceived;

        public delegate void NetworkBytesReceived(byte[] bytes);
        public event NetworkBytesReceived BytesPackageReceived;

        public delegate void NetworkTextReceived(string text);
        public event NetworkTextReceived TextReceived;

        #endregion

        #region Properties...
          
        private NetworkPackageGenerator _packageGenerator;

        private SerialPort _serialPort;

        #endregion


        public SerialListener(NetworkPackageGenerator packageGenerator, string port, int baudRate = 9600)
        {
            _packageGenerator = packageGenerator;  
            _serialPort = new SerialPort();
            _serialPort.PortName = port;
            _serialPort.BaudRate = baudRate;
            _serialPort.DataReceived += SerialPort_DataReceived; 
        }

        public void Connect()
        {
            _serialPort.Open();
        }
         
        public void Disconnect()
        {
            _serialPort.Close();
        }

        public void Send(string message, string ipAddress, int port)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            SendBytes(bytes, ipAddress, port);
        }

        public void Send(byte[] bytes, string ipAddress, int port)
        {
            SendBytes(bytes, ipAddress, port);
        }

        public void Send(NetworkPackage package, string ipAddress, int port)
        {
            byte[] bytes = package.GenerateByteArray();
            SendBytes(bytes, ipAddress, port);
        }

         
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] bytes = GetBytes();
            ReceivePackage(bytes);
            ReceiveBytes(bytes);
            ReceiveText(bytes);
        }
          
        private byte[] GetBytes()
        {
            byte[] bytes = null;

            int byteCount = _serialPort.BytesToRead;
            bytes = new byte[byteCount];
            _serialPort.Read(bytes, 0, byteCount); 

            return bytes;
        }

        private void ReceivePackage(byte[] bytes)
        {
            try
            {
                NetworkPackage networkPackage = _packageGenerator.Generate(bytes);
                PackageReceived?.Invoke(networkPackage);
            }
            catch
            {

            }
        }

        private void ReceiveBytes(byte[] bytes)
        {
            try
            {
                BytesPackageReceived?.Invoke(bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReceiveText(byte[] bytes)
        {
            try
            {
                TextReceived?.Invoke(Encoding.ASCII.GetString(bytes));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        private void SendBytes(byte[] bytes, string ipAddress, int port)
        {
            _serialPort.Write(bytes, 0, bytes.Length);
        } 
         
    }
}
