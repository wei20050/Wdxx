using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CardReading.Core
{
    public partial class CardReadSetting : Form
    {
        public CardReadSetting()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 所有插件信息
        /// </summary>
        private readonly List<CardReaderInfoAttribute> _craList = new List<CardReaderInfoAttribute>();

        /// <summary>
        /// 选中的插件信息
        /// </summary>
        private CardReaderInfoAttribute _cardReaderInfo = new CardReaderInfoAttribute("", "", false);

        /// <summary>
        /// 临时的读卡器类型
        /// </summary>
        private string _cardReaderType;

        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private void CardReadSetting_Load(object sender, EventArgs e)
        {
            _cardReaderType = Settings.CardReaderType;
            Loads();
            DgvDllInfo.DataSource = _craList;
            _timer.Tick += TimerCall;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
        }
        /// <summary>
        /// 端口刷新定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerCall(object sender, EventArgs e)
        {
            CmbPort.Items.Clear();
            var serialPorts = SerialPort.GetPortNames();
            if (serialPorts.Length ==0)
            {
                CmbPort.Text = string.Empty;
            }
            else
            {
                // ReSharper disable once CoVariantArrayConversion
                CmbPort.Items.AddRange(serialPorts);
            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        private void Loads()
        {
            _craList.Clear();
            var location = Directory.GetParent(typeof(CardReaderFactory).Assembly.Location).FullName;
            var dllDirectory = Path.Combine(location, @"IdCardReader");
            foreach (var dir in Directory.GetDirectories(dllDirectory))
            {
                LoadDir(dir);
            }
            CmbPort.Text = Settings.CardReaderComPort;
            LabMsg.Text = @"已选择:" + _cardReaderInfo.Name;
        }
        /// <summary>
        /// 加载具体dll
        /// </summary>
        /// <param name="dir"></param>
        private void LoadDir(string dir)
        {
            var dirs = Directory.GetFiles(dir, "CardReading*.dll");
            foreach (var file in dirs)
            {
                var fullPath = Path.GetFullPath(file);
                var assembly = Assembly.LoadFrom(fullPath);
                var types = assembly.GetTypes().Where(t => typeof(IReadCard).IsAssignableFrom(t)).ToList();
                foreach (var t in types)
                {
                    var attribute = t.GetCustomAttributes(typeof(CardReaderInfoAttribute), false).FirstOrDefault();
                    if (!(attribute is CardReaderInfoAttribute cardReaderInfoAttribute)) continue;
                    cardReaderInfoAttribute.Type = t;
                    _craList.Add(cardReaderInfoAttribute);
                    if (t.FullName == _cardReaderType)
                    {
                        _cardReaderInfo = cardReaderInfoAttribute;
                    }
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_cardReaderInfo.IsComPortRequired && string.IsNullOrEmpty(CmbPort.Text))
            {
                MessageBox.Show(@"请选择端口", @"提示");
                return;
            }
            Settings.CardReaderType = _cardReaderInfo.Type.FullName;
            Settings.CardReaderComPort = CmbPort.Text;
            Close();
        }

        /// <summary>
        /// 选择读卡器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvDllInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex <= -1) return;
            if (DgvDllInfo.CurrentRow == null) return;
            var dgvr = DgvDllInfo.CurrentRow;
            _cardReaderType = dgvr.Cells[3].EditedFormattedValue.ToString();
            Loads();
        }
    }
}
