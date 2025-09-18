namespace SSOConfigManagerUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable, buttons and textboxes added in code-behind.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnCreateApp;
        private System.Windows.Forms.Button btnSetProps;
        private System.Windows.Forms.Button btnGetProps;
        private System.Windows.Forms.Button btnDeleteApp;
        private System.Windows.Forms.Button btnShowApps;
        private System.Windows.Forms.TextBox txtOutput;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "SSO Application Manager";
            this.MinimumSize = new System.Drawing.Size(680, 280); // Set minimum size
            this.MaximizeBox = true; // Allow maximizing

            this.btnCreateApp = new System.Windows.Forms.Button();
            this.btnSetProps = new System.Windows.Forms.Button();
            this.btnGetProps = new System.Windows.Forms.Button();
            this.btnDeleteApp = new System.Windows.Forms.Button();
            this.btnShowApps = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
             
            // btnCreateApp
             
            this.btnCreateApp.Location = new System.Drawing.Point(20, 20);
            this.btnCreateApp.Size = new System.Drawing.Size(220, 30);
            this.btnCreateApp.Text = "Create SSO ConfigStore Application";
            this.btnCreateApp.Click += new System.EventHandler(this.btnCreateApp_Click);
             
            // btnSetProps
             
            this.btnSetProps.Location = new System.Drawing.Point(20, 60);
            this.btnSetProps.Size = new System.Drawing.Size(220, 30);
            this.btnSetProps.Text = "Set ConfigStore Properties";
            this.btnSetProps.Click += new System.EventHandler(this.btnSetProps_Click);
             
            // btnGetProps
             
            this.btnGetProps.Location = new System.Drawing.Point(20, 100);
            this.btnGetProps.Size = new System.Drawing.Size(220, 30);
            this.btnGetProps.Text = "Get ConfigStore Properties";
            this.btnGetProps.Click += new System.EventHandler(this.btnGetProps_Click);
             
            // btnDeleteApp
             
            this.btnDeleteApp.Location = new System.Drawing.Point(20, 140);
            this.btnDeleteApp.Size = new System.Drawing.Size(220, 30);
            this.btnDeleteApp.Text = "Delete Application";
            this.btnDeleteApp.Click += new System.EventHandler(this.btnDeleteApp_Click);
             
            // btnShowApps
             
            this.btnShowApps.Location = new System.Drawing.Point(20, 180);
            this.btnShowApps.Size = new System.Drawing.Size(220, 30);
            this.btnShowApps.Text = "Show All Applications";
            this.btnShowApps.Click += new System.EventHandler(this.btnShowApps_Click);
             
            // txtOutput - Make it adaptive to window size
             
            this.txtOutput.Location = new System.Drawing.Point(260, 20);
            this.txtOutput.Size = new System.Drawing.Size(520, 400);
            this.txtOutput.Multiline = true;
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
             
            // Form1
             
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCreateApp);
            this.Controls.Add(this.btnSetProps);
            this.Controls.Add(this.btnGetProps);
            this.Controls.Add(this.btnDeleteApp);
            this.Controls.Add(this.btnShowApps);
            this.Controls.Add(this.txtOutput);
            this.Text = "SSO ConfigStore Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


    }
}

