using System;
using Test.Client.FormModel;
namespace Test.Client.Form
{
    public partial class FormMain : FormBase
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private readonly FromMainModel _fm = new FromMainModel();
        private void FormMain_Load(object sender, EventArgs e)
        {
            txtUrl.DataBindings.Add(nameof(txtUrl.Text), _fm, nameof(_fm.Url));
            txtId.DataBindings.Add(nameof(txtId.Text), _fm, nameof(_fm.Id));
            txtXxk.DataBindings.Add(nameof(txtXxk.Text), _fm, nameof(_fm.Xxk));
            dgvUsers.DataBindings.Add(nameof(dgvUsers.DataSource), _fm, nameof(_fm.Users));
        }

        private void btnCs_Click(object sender, EventArgs e)
        {
            _fm.ServiceIni();
        }

        private void btnQk_Click(object sender, EventArgs e)
        {
            _fm.Qk();
        }

        private void btnGetTest_Click(object sender, EventArgs e)
        {
            _fm.GetTest();
        }

        private void btnGetTime_Click(object sender, EventArgs e)
        {
            _fm.GetTime();
        }

        private void GetUser_Click(object sender, EventArgs e)
        {
            _fm.GetUser();
        }

        private void btnPostUser_Click(object sender, EventArgs e)
        {
            _fm.PostUser();
        }

        private void btnPostUser2_Click(object sender, EventArgs e)
        {
            _fm.PostUser2();
        }

        private void btnPutUser_Click(object sender, EventArgs e)
        {
            _fm.PutUser();
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            _fm.DeleteUser();
        }

        private void btnGetUser1_Click(object sender, EventArgs e)
        {
            _fm.GetUser1();
        }

        private void btnPutUser2_Click(object sender, EventArgs e)
        {
            _fm.PutUser2();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
