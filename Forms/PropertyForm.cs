using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlockDrop.Settings;

namespace BlockDrop.Forms
{
    public partial class PropertyForm : Form
    {
        SettingsBase _settingsClassInstance;
        Form _parentForm;
        public PropertyForm(SettingsBase settingsClassInstance, Form parentForm)
        {
            InitializeComponent();
            _settingsClassInstance = settingsClassInstance;
            propertyGrid1.SelectedObject = _settingsClassInstance;
            _parentForm = parentForm;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PropertyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settingsClassInstance.Save();
            _parentForm.Close();
        }
    }
}
