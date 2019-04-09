using System.IO.Ports;
using System.Linq;
using System.Timers;

namespace PortsEx
{
    public class SerialPortEx : SerialPort
    {
        public SerialPortEx()
        {
            DataReceived += CallDataReceived;
            _t.Interval = 1;
            _t.Elapsed += (o,e) =>
            {
                if (_i++ <= DataReceivedDelay) return;
                _t.Stop();
                _i = 0;
                OnDataReceivedEx(_bytes);
                _bytes = null;
            };
        }
        public delegate void DeleDataReceived(byte[] b);
        public event DeleDataReceived DataReceivedEx;
        public int DataReceivedDelay { get; set; } = 66;
        private readonly Timer _t = new Timer();
        private int _i;
        private byte[] _bytes;
        protected virtual void CallDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _i = 0;
            var data = new byte[BytesToRead];
            Read(data, 0, data.Length);
            _bytes = _bytes?.Concat(data).ToArray() ?? data;
            _t.Start();
        }
        protected virtual void OnDataReceivedEx(byte[] b)
        {
            DataReceivedEx?.Invoke(b);
        }
    }
}