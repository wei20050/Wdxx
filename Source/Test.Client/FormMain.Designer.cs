namespace Test.Client
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCs = new System.Windows.Forms.Button();
            this.btnQk = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtXxk = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCs
            // 
            this.btnCs.BackColor = System.Drawing.Color.Green;
            this.btnCs.ForeColor = System.Drawing.Color.White;
            this.btnCs.Location = new System.Drawing.Point(427, 14);
            this.btnCs.Name = "btnCs";
            this.btnCs.Size = new System.Drawing.Size(75, 27);
            this.btnCs.TabIndex = 1;
            this.btnCs.Text = "测试连接";
            this.btnCs.UseVisualStyleBackColor = false;
            this.btnCs.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnQk
            // 
            this.btnQk.Location = new System.Drawing.Point(508, 14);
            this.btnQk.Name = "btnQk";
            this.btnQk.Size = new System.Drawing.Size(280, 63);
            this.btnQk.TabIndex = 2;
            this.btnQk.Text = "清空消息框";
            this.btnQk.UseVisualStyleBackColor = true;
            this.btnQk.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(19, 45);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(154, 32);
            this.button3.TabIndex = 3;
            this.button3.Text = "无参返回字符串(Test)";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "服务地址:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(82, 18);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(339, 21);
            this.textBox1.TabIndex = 5;
            // 
            // txtXxk
            // 
            this.txtXxk.Location = new System.Drawing.Point(508, 83);
            this.txtXxk.Multiline = true;
            this.txtXxk.Name = "txtXxk";
            this.txtXxk.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtXxk.Size = new System.Drawing.Size(280, 405);
            this.txtXxk.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(183, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(154, 32);
            this.button1.TabIndex = 7;
            this.button1.Text = "有参返回字符串(TestStr)";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(347, 45);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(154, 32);
            this.button2.TabIndex = 8;
            this.button2.Text = "无参返回时间(GetTime)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(347, 83);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(154, 32);
            this.button4.TabIndex = 11;
            this.button4.Text = "修改id:1 name:张修改";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(183, 83);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(154, 32);
            this.button5.TabIndex = 10;
            this.button5.Text = "新增id:随机 name:李四";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(19, 83);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(154, 32);
            this.button6.TabIndex = 9;
            this.button6.Text = "新增id:1 name:张三";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(347, 121);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(154, 32);
            this.button7.TabIndex = 14;
            this.button7.Text = "根据id修改name:根修改";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(183, 121);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(154, 32);
            this.button8.TabIndex = 13;
            this.button8.Text = "button8";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(101, 159);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(72, 32);
            this.button9.TabIndex = 12;
            this.button9.Text = "查询id:1";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(347, 159);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(154, 32);
            this.button10.TabIndex = 17;
            this.button10.Text = "查询所有人";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(19, 159);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(72, 32);
            this.button12.TabIndex = 15;
            this.button12.Text = "删除id:1";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.name});
            this.dataGridView1.Location = new System.Drawing.Point(19, 197);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(482, 291);
            this.dataGridView1.TabIndex = 18;
            // 
            // id
            // 
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            // 
            // name
            // 
            this.name.HeaderText = "Name";
            this.name.Name = "name";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtXxk);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnQk);
            this.Controls.Add(this.btnCs);
            this.Name = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCs;
        private System.Windows.Forms.Button btnQk;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtXxk;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
    }
}

