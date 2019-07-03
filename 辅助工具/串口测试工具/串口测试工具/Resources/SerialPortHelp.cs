using System;
using System.IO.Ports;
using System.Text;

public class SerialPortHelp
{
    /// <summary>
    /// 串口操作对象
    /// </summary>
    private readonly SerialPort _comDevice = new SerialPort();

    /// <summary>
    /// 数据获取到的事件
    /// </summary>
    public event DeviceMeasureFinishedEventHandler DeviceMeasureFinished;

    /// <summary>
    /// 数据获取委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeviceMeasureFinishedEventHandler(object sender, DeviceEventArgs e);

    public SerialPortHelp()
    {
        _comDevice.DataReceived += Com_DataReceived;
    }

    /// <summary>
    /// 打开串口
    /// </summary>
    /// <param name="portName"></param>
    public void OpenSerial(string portName)
    {
        _comDevice.PortName = "[duankou]";
        _comDevice.BaudRate = [botelv];
        _comDevice.Parity = [xiaoyan];
        _comDevice.DataBits = [shuju];
        _comDevice.StopBits = [tingzhi];
        _comDevice.RtsEnable = true;
        _comDevice.ReadTimeout = 66;
        _comDevice.WriteTimeout = 66;
        _comDevice.Open();
    }

    /// <summary>
    /// 发送文本数据
    /// </summary>
    public void Write()
    {
        var str = "[wenben]";
        _comDevice.Write(str);
    }

    /// <summary>
    /// 发送字节数据
    /// </summary>
    public void Write(string s)
    {
        //这里是自定义发送功能与参数:[zdy]
        var str = "[zijie]";
        var strs = str.Split(' ');
        var bs = new byte[strs.Length];
        for (var i = 0; i < strs.Length; i++)
        {
            bs[i] = (byte) Convert.ToInt32(strs[i]);
        }
        _comDevice.Write(bs, 0, bs.Length);
    }

    /// <summary>
    /// 发送字节数据(16进制)
    /// </summary>
    public void Write16(string s)
    {
        //这里是自定义发送功能与参数:[zdy]
        var str = "[zijie]";
        var strs = str.Split(' ');
        var bs = new byte[strs.Length];
        for (var i = 0; i < strs.Length; i++)
        {
            bs[i] = byte.Parse(strs[i], System.Globalization.NumberStyles.HexNumber);
        }
        _comDevice.Write(bs, 0, bs.Length);
    }

    /// <summary>
    /// 关闭串口
    /// </summary>
    public void CloseSerial()
    {
        _comDevice?.Close();
    }

    /// <summary>
    ///     接收数据事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var data = new byte[_comDevice.BytesToRead];
        _comDevice.Read(data, 0, data.Length); //读取数据
        var ret = Encoding.Default.GetString(data);
        OnDeviceMeasureCompletedEventArgs(new DeviceEventArgs
        {
            Bytes = data,
            Content = ret
        });
    }

    protected virtual void OnDeviceMeasureCompletedEventArgs(DeviceEventArgs e)
    {
        DeviceMeasureFinished?.Invoke(this, e);
    }
}

public class DeviceEventArgs : EventArgs
{
    public byte[] Bytes { get; set; }
    public string Content { get; set; }
}

