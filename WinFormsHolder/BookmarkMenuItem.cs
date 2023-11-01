using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsHolder.Properties;

namespace WinFormsHolder
{
    public class ToolStripButtonMenuItem : ToolStripControlHost
    {
        public ToolStripButtonMenuItem() : base(new Button(),"button")
        {
        }

        public Button Button
        {
            get { return Control as Button; }
        }
    }

}
