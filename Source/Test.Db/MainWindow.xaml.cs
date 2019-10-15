﻿using System.Collections.Generic;
using NetFrameWork.Database;
using System.Windows;

namespace Test.Db
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly DbHelper _db = new DbHelper();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var ret = _db.Update( new user { id = 1,name = ""});
            //MessageBox.Show(ret.ToString());
            //var ret = _db.FindListBySql<data>("select * from user join userage on user.aid = userage.id ");
            var s = 12345;
            _db.Delete<user>(p=>p.name==s.ToString() && p.id==2 &&(p.id !=3 || p.name == "李四"));

            _db.Select<user>();
        }

    }
    public class data
    {
        public user user { get; set; }
        public userage userage { get; set; }
    }
    public class user
    {
        public int id { get; set; }
        public string name { get; set; }
        public int aid { get; set; }
    }

    public class userage
    {
        public int id { get; set; }
        public int age { get; set; }
    }
}
