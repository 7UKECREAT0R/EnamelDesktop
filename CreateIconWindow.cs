using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EnamelDesktop_Injector
{
    public partial class CreateIconWindow : Form
    {
        public string name = null;
        public string appdir = null;

        public bool IsConfirmed()
        {
            return name != null && appdir != null;
        }
        public CreateIconWindow()
        {
            InitializeComponent();
            ofd.FileName = Environment.GetFolderPath
                (Environment.SpecialFolder.DesktopDirectory);
            clearButton.Enabled = false;
            confirmButton.Enabled = false;
            namePicker.Enabled = false;
        }
        private void ofd_FileOk(object sender, CancelEventArgs e)
        {
            clearButton.Enabled = true;
            confirmButton.Enabled = true;
            namePicker.Enabled = true;
            namePicker.Text = Path.GetFileNameWithoutExtension(ofd.FileName)
                .Replace("~", "")
                .Replace("@", "")
                .Replace("`", "")
                .Replace("|", "");
        }
        // ======================================================
        private void pickButton_Click(object sender, EventArgs e)
        {
            ofd.ShowDialog();
            return;
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            appdir = null; name = null;
            clearButton.Enabled = false;
            confirmButton.Enabled = false;
            namePicker.Text = "";
            namePicker.Enabled = false;
        }
        private void confirmButton_Click(object sender, EventArgs e)
        {
            if(namePicker.Text.Length < 2)
            {
                MessageBox.Show("Please pick a valid icon name.", "Icon Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(namePicker.Text.Contains('~') || namePicker.Text.Contains('@') || namePicker.Text.Contains('`') || namePicker.Text.Contains('|'))
            {
                MessageBox.Show("Icon names cannot contain a \"~\", \"@\", \"`\", or \"|\". Please choose a different name.", "Icon Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            appdir = ofd.FileName;
            name = namePicker.Text;
            Close();
        }
    }
}
