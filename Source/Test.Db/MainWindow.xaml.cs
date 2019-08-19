using System.Collections.Generic;
using System.Windows;
using Tset.Entity;

namespace Test.Db
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Wdxx.Database.DbHelper _db = new Wdxx.Database.DbHelper();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ret = _db.Inserts(new List<user> { new user { id = 1, name = "zhenh" }, new user { id = 2, name = "dafff" } });
            MessageBox.Show(ret.ToString());
        }
    }
}
