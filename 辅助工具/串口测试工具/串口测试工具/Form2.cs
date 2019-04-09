using System;
using System.Windows.Forms;

namespace 串口测试工具
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        //保存按钮
        private void button1_Click(object sender, EventArgs e)
        {
            var index = checkedListBox1.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show(@"请选择要修改的按钮!");
                return;
            }
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
            Properties.Settings.Default.zdy[index] = $"{textBox1.Text}|{richTextBox1.Text}";
            Properties.Settings.Default.Save();
            MessageBox.Show($@"按钮:{textBox1.Text} 保存成功!");
        }
        //退出按钮
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        //左侧列表点击
        private void checkedListBox1_MouseDown(object sender, MouseEventArgs e)
        {
            var index = checkedListBox1.SelectedIndex;
            if (index < 0) return;
            var tmp = Properties.Settings.Default.zdy[index].Split('|');
            textBox1.Text = tmp[0];
            richTextBox1.Text = tmp[1];
        }
        //删除按钮
        private void button3_Click(object sender, EventArgs e)
        {
            for (var i = checkedListBox1.CheckedIndices.Count - 1; i >=0 ; i--)
            {
                var v = checkedListBox1.CheckedIndices[i];
                Properties.Settings.Default.zdy.RemoveAt(v);
            }
            Properties.Settings.Default.Save();
            Close();
        }
        //界面初始化
        private void Form2_Load(object sender, EventArgs e)
        {
            foreach (var v in Properties.Settings.Default.zdy)
            {
                var tmp = v.Split('|');
                checkedListBox1.Items.Add(tmp[0], false);
            }
        }
        private bool _isqx = true;
        //全选/取消全选
        private void button4_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, _isqx);
            }
            _isqx = !_isqx;
        }
    }
}
