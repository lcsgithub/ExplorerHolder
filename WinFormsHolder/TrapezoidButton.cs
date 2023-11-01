using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsHolder
{
    internal class TrapezoidButton : Button
    {
        IntPtr window;
        public bool Active
        { get; set; }
        public TrapezoidButton() :base()
        {
            this.TabStop = false;
            this.BackColor = Color.White;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Active = false;
        }

        public IntPtr WinHandle 
        {
            get { return this.window; } 
            set { this.window = value; }
        }
        GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float r2 = radius / 2f;
            float w = rect.Width;
            path.StartFigure();
            float o = rect.Height / 1.732f;
            path.AddLine(rect.X + o, rect.Y, rect.X + rect.Width - o, rect.Y);
            path.AddLine(rect.X + rect.Width - o, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            //path.AddBezier(rect.X + rect.Width - o, rect.Y, 
            //    rect.X + rect.Width - o/2, rect.Y + rect.Height / 2, 
            //    rect.X + rect.Width - o, rect.Y + rect.Height/2, 
            //    rect.X + rect.Width, rect.Y + rect.Height);
            path.AddLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X, rect.Y + rect.Height);
            // path.AddArc(rect.X + o, rect.Y, radius, radius, 150, 90);
            // path.AddLine(rect.X + o + r2, rect.Y, rect.X + rect.Width - r2 - o, rect.Y);
            // path.AddArc(rect.X + rect.Width - o - radius, rect.Y, radius, radius, 270, 90);
            // path.AddLine(rect.Width, rect.Y + r2, rect.Width - w / 3, rect.Height);
            // path.AddLine(rect.Width - w / 3, rect.Height, rect.X + w / 3, rect.Height);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height);
            int radius = 16;

            GraphicsPath buttonPath = GetFigurePath(rect, radius);
            Region region = new Region(buttonPath);
            this.Region = region;

            Pen pen = new Pen(Active ? Color.Purple : Color.Green, 4);

            float o = rect.Height / 1.732f;
            PointF[] pts = new PointF[4] {
                new PointF(rect.X, rect.Y + rect.Height),
                new PointF(rect.X + o, rect.Y),
                new PointF(rect.X + rect.Width - o, rect.Y ) ,
                new PointF(rect.X + rect.Width, rect.Y + rect.Height )};

            e.Graphics.DrawLines(pen, pts);
            //e.Graphics.DrawPath(pen, buttonPath);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                BringToFront();
                this.Invalidate();
            }
        }
    }
}
