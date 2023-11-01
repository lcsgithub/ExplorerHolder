using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsHolder.Properties;

namespace WinFormsHolder
{
    internal class TabButtonContrl : UserControl
    {
        List<Button> buttons;
        int activeIndex = 0;
        public TabButtonContrl(Form form) 
        {
            Left = 0;
            Top = 0;
            Height = 31;
            Width = form.ClientSize.Width;
            form.SizeChanged += (s,e)=> { Width = form.ClientSize.Width; };

            Button cfgBtn = new Button();
            cfgBtn.FlatStyle = FlatStyle.Flat;
            cfgBtn.FlatAppearance.BorderSize = 0;

            cfgBtn.Left = 4;
            cfgBtn.Top = 3;
            cfgBtn.Width = 24;
            cfgBtn.Height = 24;
            cfgBtn.BackgroundImage = (Image)Resources.ResourceManager.GetObject("config");
            ContextMenuStrip menu = new ContextMenuStrip();
            var bookmark = new ToolStripMenuItem("添加到书签");
            var allBookmarks = new ToolStripMenuItem("所有书签");
            allBookmarks.DropDownItems.Add(bookmark);
            var exit = new ToolStripMenuItem("退出");
            menu.Items.Add(bookmark);
            menu.Items.Add(allBookmarks);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exit);
            menu.Items.Add(new ToolStripButtonMenuItem());
            cfgBtn.Click += (s, e) =>
            {
                Point pos = cfgBtn.PointToScreen(cfgBtn.Location);
                menu.Left = pos.X;
                menu.Top = pos.Y + cfgBtn.Height;
                menu.Show();
            };
            this.Controls.Add(cfgBtn);

            Button addBtn = new Button();
            addBtn.FlatStyle = FlatStyle.Flat;
            addBtn.FlatAppearance.BorderSize = 0;

            addBtn.Left = 304;
            addBtn.Top = 3;
            addBtn.Width = 24;
            addBtn.Height = 24;
            addBtn.BackgroundImage = (Image)Resources.ResourceManager.GetObject("add");
            addBtn.Click += (s, e) =>
            {
                var process = new Process { StartInfo = new ProcessStartInfo("explorer.exe") };
                process.Start();
            };
            this.Controls.Add(menu);
        }

        public Button AddButton()
        {
            Button btn = new TrapezoidButton();
            btn.Text = "Button";
            btn.Left = 30 + 70 * buttons.Count;
            btn.Top = 0;
            btn.Width = 100;
            btn.Height = 31;
            btn.Click += (s, e) =>
            {
                for (int i = 0; i < buttons.Count; ++i)
                {
                    if (buttons[i] == btn)
                    {
                        activeIndex = i;
                        break;
                    }
                }
            };
            buttons.Add(btn);
            return btn;
        }
    }
}
