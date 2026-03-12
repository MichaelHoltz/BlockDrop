using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockDrop.Forms
{
    public partial class FormSelection : Form
    {
        public FormSelection()
        {
            InitializeComponent();
        }

        private void btnFormBlockDrop_Click(object sender, EventArgs e)
        {
            FormBlockDrop formBlockDrop = new FormBlockDrop();
            this.Hide();
            formBlockDrop.ShowDialog(this);
            this.Show();

        }
    }
}
