using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

namespace MapNavigation
{
    public partial class Params : UserControl
    {
        public Params()
        {
            InitializeComponent();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void дата_с(object sender, EventArgs e)
        {
            label1.Text = "Дата с " + Convert.ToString(dateTimePicker1.Value);
        }

        private void Время_до(object sender, EventArgs e)
        {
            label2.Text = "Дата с " + Convert.ToString(dateTimePicker1.Value);
        }

        private void Params_Load(object sender, EventArgs e)
        {
           


        }
}
}
