using System.IO.Ports;
using System.Linq;
using System.Timers;

namespace Wdxx.Core
{
    /// <inheritdoc />
    /// <summary>
    /// 串口核心
    /// </summary>
    public class CoreSerialPort : SerialPort
    {
        /// <inheritdoc />
        /// <summary>
        /// 串口核心封装类实现了定时获取串口数据
        /// </summary>
        public CoreSerialPort()
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
        /// <summary>
        /// 封装的委托
        /// </summary>
        /// <param name="b"></param>
        public delegate void DeleDataReceived(byte[] b);
        /// <summary>
        /// 新的的数据返回事件
        /// </summary>
        public event DeleDataReceived DataReceivedEx;
        /// <summary>
        /// 数据返回间隔 单位毫秒 若多次传输间隔少于这个事件不触发新的数据返回事件
        /// </summary>
        public int DataReceivedDelay { get; set; } = 66;
        private readonly Timer _t = new Timer();
        private int _i;
        private byte[] _bytes;
        /// <summary>
        /// 封装前的事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void CallDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _i = 0;
            var data = new byte[BytesToRead];
            Read(data, 0, data.Length);
            _bytes = _bytes?.Concat(data).ToArray() ?? data;
            _t.Start();
        }
        /// <summary>
        /// 封装后的事件触发
        /// </summary>
        /// <param name="b"></param>
        protected virtual void OnDataReceivedEx(byte[] b)
        {
            DataReceivedEx?.Invoke(b);
        }
    }
}