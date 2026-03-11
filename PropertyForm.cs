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
        public PropertyForm(BlockDropSettings blockDropSettings)
        {
            InitializeComponent();
            _blockDropSettings = blockDropSettings;
            propertyGrid1.SelectedObject = _blockDropSettings;
        }
    }
}
