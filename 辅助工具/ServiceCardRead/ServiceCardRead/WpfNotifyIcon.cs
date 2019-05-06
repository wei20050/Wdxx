using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace ServiceCardRead
{
    public class WpfNotifyIcon
    {
        /// <summary>
        /// 设置系统托盘
        /// </summary>
        /// <param name="minText">最小化之后显示的文字</param>
        /// <param name="meClick">点击事件</param>
        /// <param name="menuList">右键菜单</param>
        /// <returns></returns>
        public static NotifyIcon SetSystemTray(string minText, MouseEventHandler meClick, List<SystemTrayMenu> menuList)
        {
            var notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Text = minText
            };
            //双击事件
            notifyIcon.MouseClick += meClick; 
            notifyIcon.ContextMenuStrip = GetMenuStrip(menuList);
            return notifyIcon;
        }
        /// <summary>
        /// 设置系统托盘的菜单属性e
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private static ContextMenuStrip GetMenuStrip(ICollection<SystemTrayMenu> menus)
        {
            var menu = new ContextMenuStrip();
            var menuArray = new ToolStripItem[menus.Count];
            var i = 0;
            foreach (var item in menus)
            {
                var menuItem = new ToolStripMenuItem {Text = item.Txt};
                menuItem.Click += item.Click;
                menuArray[i++] = menuItem;
            }
            menu.Items.AddRange(menuArray);
            return menu;
        }
    }
    /// <summary>
    /// 右键菜单
    /// </summary>
    public class SystemTrayMenu
    {
        /// <summary>
        /// 菜单文本
        /// </summary>
        public string Txt { get; set; }
        /// <summary>
        /// 菜单单击事件
        /// </summary>
        public EventHandler Click { get; set; }
    }
}
