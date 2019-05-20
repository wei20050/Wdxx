using System.IO.Ports;
using System.Linq;
using System.Timers;

namespace Wdxx.Tool
{
    
    /// <summary>
    /// 串口核心
    /// </summary>
    public class CoreSerialPort
    {

        /// <summary>
        /// 串行端口资源对象
        /// </summary>
        public  SerialPort Sp { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 串口核心封装类实现了定时获取串口数据
        /// </summary>
        public CoreSerialPort()
        {
            if (Sp == null)
            {
                Sp = new SerialPort();
            }
            //默认的多次触发时间 若串口数据过大 每次传输数据量耗时长 需要修改此时间
            DataReceivedDelay = 168;
            Sp.DataReceived += CallDataReceived;
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
        public int DataReceivedDelay { get; set; }

        /// <summary>
        /// 封装返回的定时器
        /// </summary>
        private readonly Timer _t = new Timer();

        /// <summary>
        /// 封装返回定时器计数
        /// </summary>
        private int _i;

        /// <summary>
        /// 封装后返回的数据
        /// </summary>
        private byte[] _bytes;

        /// <summary>
        /// 封装前的事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void CallDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _i = 0;
            var data = new byte[Sp.BytesToRead];
            Sp.Read(data, 0, data.Length);
            _bytes = _bytes.Concat(data).ToArray();
            _t.Start();
        }

        /// <summary>
        /// 封装后的事件触发
        /// </summary>
        /// <param name="b"></param>
        protected virtual void OnDataReceivedEx(byte[] b)
        {
            if (DataReceivedEx != null) DataReceivedEx.Invoke(b);
        }

    }
}