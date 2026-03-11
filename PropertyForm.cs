using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockDrop
{
    public partial class PropertyForm : Form
    {
        BlockDropSettings _blockDropSettings;
        Form1 _form1;
        public PropertyForm(BlockDropSettings blockDropSettings, Form1 form1)
        {
            InitializeComponent();
            _blockDropSettings = blockDropSettings;
            propertyGrid1.SelectedObject = _blockDropSettings;
            _form1 = form1;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PropertyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _blockDropSettings.Save();
            _form1.Close();
        }
    }
}
