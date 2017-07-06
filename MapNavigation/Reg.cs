using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapNavigation
{
    public partial class Reg : Form
    {
        public Reg()
        {
            InitializeComponent();
        }

        private void show_MapsForm(object sender, EventArgs e)
        {
            MapsForm формакарты = new MapsForm();
            формакарты.Show();
        }
    }
}
