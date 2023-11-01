using Accessibility;
using System.Diagnostics;
using System.Runtime.Versioning;
using WinFormsHolder.Properties;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Timer = System.Windows.Forms.Timer;

namespace WinFormsHolder
{
    public partial class MainForm : Form
    {
        UserControl m_ToolBar;
        static MainForm m_MainForm;
        Button m_AddButton,m_DelButton;
        List<TrapezoidButton> m_Buttons = new List<TrapezoidButton>();
        int m_SelectedIndex = 0;
        Timer m_Timer;
        public MainForm()
        {
            m_MainForm = this;
            InitializeComponent();
            this.Text = "";
            string explorerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(explorerPath);

            UserControl tb = new UserControl();
            this.BackColor = Color.White;
            tb.BackColor = this.BackColor;
            tb.Left = 0;
            tb.Top = 0;
            tb.Width = ClientSize.Width;
            tb.Height = 31;
            this.Controls.Add(tb);
            m_ToolBar = tb;

            Button cfgBtn = new Button();
            cfgBtn.FlatStyle = FlatStyle.Flat;
            cfgBtn.FlatAppearance.BorderSize = 0;
            cfgBtn.TabStop = false;

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
            exit.Click += (s, e) =>
            {
                this.Close();
            };
            menu.Items.Add(bookmark);
            menu.Items.Add(allBookmarks);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exit);
            cfgBtn.Click += (s, e) =>
            {
                menu.Show(this, new Point(cfgBtn.Left, cfgBtn.Bottom));
            };
            tb.Controls.Add(cfgBtn);

            Button addBtn = new Button();
            addBtn.FlatStyle = FlatStyle.Flat;
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.TabStop = false;
            addBtn.Left = 30;
            addBtn.Top = 3;
            addBtn.Width = 24;
            addBtn.Height = 24;
            addBtn.BackgroundImage = (Image)Resources.ResourceManager.GetObject("add");
            tb.Controls.Add(addBtn);
            m_AddButton = addBtn;
            m_AddButton.Click += (s, e) =>
            {
                OpenExplorerWindow(string.Empty);
            };

            Button rmBtn = new Button();
            rmBtn.FlatStyle = FlatStyle.Flat;
            rmBtn.FlatAppearance.BorderSize = 0;
            rmBtn.TabStop = false;
            rmBtn.Left = this.ClientSize.Width-28;
            rmBtn.Top = 3;
            rmBtn.Width = 24;
            rmBtn.Height = 24;
            rmBtn.BackgroundImage = (Image)Resources.ResourceManager.GetObject("delete");
            tb.Controls.Add(rmBtn);
            rmBtn.Click += (s, e) =>
            {
                if(m_Buttons.Count > 1)
                    CPPInterface.CloseExplorerWin(m_Buttons[m_SelectedIndex].WinHandle);
            };
            m_DelButton = rmBtn;
            //m_Timer = new Timer();
            //m_Timer.Interval = 500;
            //m_Timer.Enabled = true;
            //m_Timer.Tick += (s, e) =>
            //{
            //    foreach(var item in tb.Controls)
            //    {
            //        TrapezoidButton? btn = item as TrapezoidButton;
            //        if(btn != null && CPPInterface.IsWindowVisible(btn.WinHandle))
            //            btn.Text = CPPInterface.GetWindowText(btn.WinHandle);
            //    }
            //};
            //m_Timer.Start();

            FormClosing += (s, e) =>
            {
                if (e.CloseReason != CloseReason.UserClosing) // ctrl+W
                {
                    e.Cancel = true;
                }
            };

            this.SizeChanged += MainForm_SizeChanged;
        }

        private void MainForm_SizeChanged(object? sender, EventArgs e)
        {
            CPPInterface.UpdateMainWinSize();
            m_DelButton.Left = this.ClientSize.Width-28;
            m_ToolBar.Width = ClientSize.Width;
            Invalidate();
        }

        private void AddButton(TrapezoidButton btn)
        {
            m_Buttons.Add(btn);
            m_ToolBar.Controls.Add(btn);
        }
        private void RemoveButton(TrapezoidButton btn)
        {
            m_Buttons.Remove(btn);
            m_ToolBar.Controls.Remove(btn);
            m_ToolBar.ResumeLayout();
        }

        private void ActiveTabButton(TrapezoidButton btn)
        {
            for(int i = 0; i < m_Buttons.Count; i++)
            {
                m_Buttons[i].Active = false;
                if (btn == m_Buttons[i])
                {
                    m_SelectedIndex = i;
                    btn.Active = true;
                }
                m_Buttons[i].Invalidate();
            }
            Invalidate();
        }
        static void WindowEventCallback(bool Create, IntPtr hWnd)
        {
            if(Create)
            {
                TrapezoidButton btn = new TrapezoidButton() { WinHandle = hWnd, Top = 0, Width = 100, Height = 31};
                btn.Text = CPPInterface.GetWindowText(hWnd);
                btn.Left = 30 + (m_MainForm.m_Buttons.Count)*85;
                m_MainForm.m_SelectedIndex = m_MainForm.m_Buttons.Count;
                m_MainForm.AddButton(btn);
                CPPInterface.ShowExplorerWin(hWnd);
                m_MainForm.ActiveTabButton(btn);
                btn.BringToFront();
                btn.Click += (s, e) =>
                {
                    CPPInterface.ShowExplorerWin(hWnd);
                    m_MainForm.ActiveTabButton(btn);
                };
                //btn.DoubleClick += (s, e) =>
                //{
                //    if(m_MainForm.m_ToolBar.Controls.Count > 3) // skip cfbbtn addbtn
                //        CPPInterface.CloseWindow(hWnd);
                //};
            }
            else
            {
                int i = 0;
                for(i = 0; i < m_MainForm.m_Buttons.Count; ++i)
                {
                    TrapezoidButton btn = (TrapezoidButton)m_MainForm.m_Buttons[i];
                    if(btn != null)
                    {
                        if(hWnd == btn.WinHandle)
                        {
                            m_MainForm.RemoveButton(btn);
                            break;
                        }
                    }
                }
                if (i == m_MainForm.m_Buttons.Count)
                {
                    m_MainForm.m_Buttons[i - 1].BringToFront();
                    m_MainForm.m_SelectedIndex = i - 1;
                }
                else
                {
                    while (i < m_MainForm.m_Buttons.Count)
                    {
                        m_MainForm.m_Buttons[i].Left = 30 + i * 85;
                        ++i;
                    }
                    m_MainForm.m_Buttons[i].BringToFront();
                    m_MainForm.m_SelectedIndex = i;
                }
                // TODO: destroy
            }
            CPPInterface.ShowExplorerWin((m_MainForm.m_Buttons[m_MainForm.m_SelectedIndex] as TrapezoidButton).WinHandle);

            m_MainForm.m_AddButton.Left = m_MainForm.m_Buttons[m_MainForm.m_Buttons.Count - 1].Right + 4;
            m_MainForm.Invalidate();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            int winCount = CPPInterface.SetMainWindow(Handle, WindowEventCallback);
            if (winCount == 0)
                OpenExplorerWindow(string.Empty);
            else
            {
                for(int i = 0; i < winCount; ++i)
                {
                    WindowEventCallback(true, CPPInterface.GetExplorerWindow(i));
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            CPPInterface.ReleaseMainWindow();
            base.OnHandleDestroyed(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen pen = new Pen(Color.Purple, 4);
            int padding = 4;
            Rectangle rect = new Rectangle(padding, 31, ClientSize.Width - padding * 2, ClientSize.Height - padding * 2 - 31);
            if (m_Buttons.Count > 0)
            {
                Button btn = m_Buttons[m_SelectedIndex];
                Point[] pts = new Point[6]
                {
                new Point(btn.Left+2, rect.Y),
                new Point(rect.X, rect.Y),
                new Point(rect.X, rect.Bottom),
                new Point(rect.Right, rect.Bottom),
                new Point(rect.Right, rect.Y),
                new Point(btn.Right-2, rect.Y),
                };
                e.Graphics.DrawLines(pen, pts);
            }
            else
                e.Graphics.DrawRectangle(pen, rect);
        }

        void OpenExplorerWindow(string path)
        {
            var process = Process.Start("explorer.exe", path == null ? "" : path);
        }
    }
}