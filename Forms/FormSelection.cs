using System;
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

        private void btnGearConfig_Click(object sender, EventArgs e)
        {
            FormGearConfig formGearConfig = new FormGearConfig();
            this.Hide();
            formGearConfig.ShowDialog(this);
            this.Show();
        }
    }
}
