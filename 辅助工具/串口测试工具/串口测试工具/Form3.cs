using System;
using System.Windows.Forms;

namespace 串口测试工具
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show(@"请输入按钮名称!");
                return;
            }
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                MessageBox.Show(@"请输入发送内容!");
                return;
            }
            if (Properties.Settings.Default.zdy.Count == 60)
            {
                MessageBox.Show(@"自定义发送按钮已到上限,无法添加!");
                return;
            }
            Properties.Settings.Default.zdy.Add($"{textBox1.Text}|{richTextBox1.Text}");
            Properties.Settings.Default.Save();
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
