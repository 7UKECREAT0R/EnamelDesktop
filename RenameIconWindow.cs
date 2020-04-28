using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnamelDesktop_Injector
{
    public partial class RenameIconWindow : Form
    {
        public RenameIconWindow(string iconname)
        {
            InitializeComponent();
            ogname = iconname;
            name = iconname;
        }
        public string ogname;
        public string name;

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text.Length < 2)
            {
                MessageBox.Show("Please pick a valid icon name.", "Icon Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox.Text.Contains('~') || textBox.Text.Contains('@') || textBox.Text.Contains('`'))
            {
                MessageBox.Show("Icon names cannot contain a \"~\", \"@\", or \"`\". Please choose a different name.", "Icon Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            name = textBox.Text;
            Close();
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            name = ogname;
            Close();
        }
    }
}
