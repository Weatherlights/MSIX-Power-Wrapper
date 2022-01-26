namespace wcommsixwrap
{
    partial class PrivacyPolicyForm
    {
        private string uri = null;
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

        public void setCaption(string text)
        {
            this.Text = text;
         }

        public void setDoNotShowAgain(string text)
        {
            this.chkDoNotShowAgain.Text = text;
        }

        public void setPrivacyPolicyUrl(string url)
        {
            uri = url;
        }

        public bool getDoNotShowAgainState ()
        {
            return this.chkDoNotShowAgain.Checked;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrivacyPolicyForm));
            this.chkDoNotShowAgain = new System.Windows.Forms.CheckBox();
            this.webPrivacyPolicy = new System.Windows.Forms.WebBrowser();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkDoNotShowAgain
            // 
            this.chkDoNotShowAgain.AutoSize = true;
            this.chkDoNotShowAgain.Checked = true;
            this.chkDoNotShowAgain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDoNotShowAgain.Location = new System.Drawing.Point(25, 544);
            this.chkDoNotShowAgain.Name = "chkDoNotShowAgain";
            this.chkDoNotShowAgain.Size = new System.Drawing.Size(361, 29);
            this.chkDoNotShowAgain.TabIndex = 0;
            this.chkDoNotShowAgain.Text = "Do not show this message again.";
            this.chkDoNotShowAgain.UseVisualStyleBackColor = true;
            // 
            // webPrivacyPolicy
            // 
            this.webPrivacyPolicy.Location = new System.Drawing.Point(12, 12);
            this.webPrivacyPolicy.MinimumSize = new System.Drawing.Size(20, 20);
            this.webPrivacyPolicy.Name = "webPrivacyPolicy";
            this.webPrivacyPolicy.Size = new System.Drawing.Size(935, 509);
            this.webPrivacyPolicy.TabIndex = 1;
            this.webPrivacyPolicy.Url = new System.Uri("https://storeprivacypolicy.azurewebsites.net/generic/Privacy_en.html", System.UriKind.Absolute);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(682, 600);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(237, 53);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PrivacyPolicyForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 700);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.webPrivacyPolicy);
            this.Controls.Add(this.chkDoNotShowAgain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrivacyPolicyForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Privacy Policy";
            this.Load += new System.EventHandler(this.PrivacyPolicyForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDoNotShowAgain;
        private System.Windows.Forms.WebBrowser webPrivacyPolicy;
        private System.Windows.Forms.Button btnOK;
    }


}