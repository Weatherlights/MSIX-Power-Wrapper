namespace wcommsixwrap
{
    partial class UpdateHandlerForm
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
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.prgUpdateProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.Location = new System.Drawing.Point(12, 9);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(122, 18);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "App is updating";
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(12, 45);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(296, 38);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "Please wait while we update the app.  This could take some minutes...";
            // 
            // prgUpdateProgressBar
            // 
            this.prgUpdateProgressBar.Location = new System.Drawing.Point(15, 86);
            this.prgUpdateProgressBar.Name = "prgUpdateProgressBar";
            this.prgUpdateProgressBar.Size = new System.Drawing.Size(293, 12);
            this.prgUpdateProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgUpdateProgressBar.TabIndex = 2;
            this.prgUpdateProgressBar.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // UpdateHandlerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(320, 110);
            this.ControlBox = false;
            this.Controls.Add(this.prgUpdateProgressBar);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblHeading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UpdateHandlerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Update";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.UpdateHandlerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label lblHeading;
        public System.Windows.Forms.Label lblMessage;
        public System.Windows.Forms.ProgressBar prgUpdateProgressBar;
    }
}