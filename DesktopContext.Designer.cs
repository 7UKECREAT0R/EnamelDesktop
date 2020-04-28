namespace EnamelDesktop_Injector
{
    partial class DesktopContext
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
            this.snap = new System.Windows.Forms.Button();
            this.createIcon = new System.Windows.Forms.Button();
            this.setWallpaper = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // snap
            // 
            this.snap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.snap.FlatAppearance.BorderSize = 0;
            this.snap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.snap.ForeColor = System.Drawing.Color.White;
            this.snap.Location = new System.Drawing.Point(12, 12);
            this.snap.Name = "snap";
            this.snap.Size = new System.Drawing.Size(132, 38);
            this.snap.TabIndex = 0;
            this.snap.Text = "Enable Snap To Grid";
            this.snap.UseVisualStyleBackColor = false;
            this.snap.Click += new System.EventHandler(this.snap_Click);
            // 
            // createIcon
            // 
            this.createIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.createIcon.FlatAppearance.BorderSize = 0;
            this.createIcon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.createIcon.ForeColor = System.Drawing.Color.White;
            this.createIcon.Location = new System.Drawing.Point(12, 56);
            this.createIcon.Name = "createIcon";
            this.createIcon.Size = new System.Drawing.Size(132, 38);
            this.createIcon.TabIndex = 1;
            this.createIcon.Text = "Create Desktop Icon";
            this.createIcon.UseVisualStyleBackColor = false;
            this.createIcon.Click += new System.EventHandler(this.createIcon_Click);
            // 
            // setWallpaper
            // 
            this.setWallpaper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.setWallpaper.FlatAppearance.BorderSize = 0;
            this.setWallpaper.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setWallpaper.ForeColor = System.Drawing.Color.White;
            this.setWallpaper.Location = new System.Drawing.Point(12, 100);
            this.setWallpaper.Name = "setWallpaper";
            this.setWallpaper.Size = new System.Drawing.Size(132, 38);
            this.setWallpaper.TabIndex = 2;
            this.setWallpaper.Text = "Set Wallpaper";
            this.setWallpaper.UseVisualStyleBackColor = false;
            this.setWallpaper.Click += new System.EventHandler(this.setWallpaper_Click);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.saveButton.FlatAppearance.BorderSize = 0;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.ForeColor = System.Drawing.Color.White;
            this.saveButton.Location = new System.Drawing.Point(12, 144);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(49, 24);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.loadButton.FlatAppearance.BorderSize = 0;
            this.loadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadButton.ForeColor = System.Drawing.Color.White;
            this.loadButton.Location = new System.Drawing.Point(67, 144);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(77, 24);
            this.loadButton.TabIndex = 4;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = false;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // DesktopContext
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(156, 180);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.setWallpaper);
            this.Controls.Add(this.createIcon);
            this.Controls.Add(this.snap);
            this.Name = "DesktopContext";
            this.Text = "DesktopContext";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button snap;
        private System.Windows.Forms.Button createIcon;
        private System.Windows.Forms.Button setWallpaper;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button loadButton;
    }
}