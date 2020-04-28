namespace EnamelDesktop_Injector
{
    partial class IconContext
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rename = new System.Windows.Forms.Button();
            this.delete = new System.Windows.Forms.Button();
            this.runAsAdmin = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rename
            // 
            this.rename.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rename.FlatAppearance.BorderSize = 0;
            this.rename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rename.ForeColor = System.Drawing.Color.White;
            this.rename.Location = new System.Drawing.Point(12, 57);
            this.rename.Name = "rename";
            this.rename.Size = new System.Drawing.Size(108, 25);
            this.rename.TabIndex = 1;
            this.rename.Text = "Rename";
            this.rename.UseVisualStyleBackColor = false;
            this.rename.Click += new System.EventHandler(this.rename_Click);
            // 
            // delete
            // 
            this.delete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.delete.FlatAppearance.BorderSize = 0;
            this.delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.delete.ForeColor = System.Drawing.Color.White;
            this.delete.Location = new System.Drawing.Point(12, 88);
            this.delete.Name = "delete";
            this.delete.Size = new System.Drawing.Size(108, 25);
            this.delete.TabIndex = 1;
            this.delete.Text = "Delete";
            this.delete.UseVisualStyleBackColor = false;
            this.delete.Click += new System.EventHandler(this.delete_Click);
            // 
            // runAsAdmin
            // 
            this.runAsAdmin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.runAsAdmin.FlatAppearance.BorderSize = 0;
            this.runAsAdmin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.runAsAdmin.ForeColor = System.Drawing.Color.White;
            this.runAsAdmin.Location = new System.Drawing.Point(12, 12);
            this.runAsAdmin.Name = "runAsAdmin";
            this.runAsAdmin.Size = new System.Drawing.Size(108, 39);
            this.runAsAdmin.TabIndex = 1;
            this.runAsAdmin.Text = "Run as Administrator";
            this.runAsAdmin.UseVisualStyleBackColor = false;
            this.runAsAdmin.Click += new System.EventHandler(this.runAsAdmin_Click);
            // 
            // IconContext
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(132, 125);
            this.Controls.Add(this.delete);
            this.Controls.Add(this.runAsAdmin);
            this.Controls.Add(this.rename);
            this.Name = "IconContext";
            this.Text = "IconContext";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button rename;
        private System.Windows.Forms.Button delete;
        private System.Windows.Forms.Button runAsAdmin;
    }
}