using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SSOConfigManagerUI
{
    public partial class CreateAppForm : Form
    {
        public string AppName { get; private set; }
        public string Description { get; private set; }
        public string PropertyKey { get; private set; }
        public string PropertyValue { get; private set; }
        public bool IsPropertyMasked { get; private set; }

        private TextBox txtAppName;
        private TextBox txtDescription;
        private TextBox txtPropertyKey;
        private TextBox txtPropertyValue;
        private CheckBox chkMasked;
        private Button btnOK;
        private Button btnCancel;

        public CreateAppForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtAppName = new TextBox();
            this.txtDescription = new TextBox();
            this.txtPropertyKey = new TextBox();
            this.txtPropertyValue = new TextBox();
            this.chkMasked = new CheckBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // Form
            this.Text = "Create SSO Application";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Labels
            var lblAppName = new Label();
            lblAppName.Text = "Application Name:";
            lblAppName.Location = new System.Drawing.Point(20, 20);
            lblAppName.Size = new System.Drawing.Size(120, 23);
            this.Controls.Add(lblAppName);

            var lblDescription = new Label();
            lblDescription.Text = "Description:";
            lblDescription.Location = new System.Drawing.Point(20, 60);
            lblDescription.Size = new System.Drawing.Size(120, 23);
            this.Controls.Add(lblDescription);

            var lblPropertyKey = new Label();
            lblPropertyKey.Text = "Property Key:";
            lblPropertyKey.Location = new System.Drawing.Point(20, 100);
            lblPropertyKey.Size = new System.Drawing.Size(120, 23);
            this.Controls.Add(lblPropertyKey);

            var lblPropertyValue = new Label();
            lblPropertyValue.Text = "Property Value:";
            lblPropertyValue.Location = new System.Drawing.Point(20, 140);
            lblPropertyValue.Size = new System.Drawing.Size(120, 23);
            this.Controls.Add(lblPropertyValue);

            // TextBoxes
            this.txtAppName.Location = new System.Drawing.Point(150, 20);
            this.txtAppName.Size = new System.Drawing.Size(200, 23);
            this.Controls.Add(this.txtAppName);

            this.txtDescription.Location = new System.Drawing.Point(150, 60);
            this.txtDescription.Size = new System.Drawing.Size(200, 23);
            this.Controls.Add(this.txtDescription);

            this.txtPropertyKey.Location = new System.Drawing.Point(150, 100);
            this.txtPropertyKey.Size = new System.Drawing.Size(200, 23);
            this.Controls.Add(this.txtPropertyKey);

            this.txtPropertyValue.Location = new System.Drawing.Point(150, 140);
            this.txtPropertyValue.Size = new System.Drawing.Size(200, 23);
            this.Controls.Add(this.txtPropertyValue);

            // CheckBox
            this.chkMasked.Text = "Mask this property";
            this.chkMasked.Location = new System.Drawing.Point(150, 180);
            this.chkMasked.Size = new System.Drawing.Size(150, 23);
            this.Controls.Add(this.chkMasked);

            // Buttons
            this.btnOK.Text = "Create";
            this.btnOK.Location = new System.Drawing.Point(200, 220);
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Click += BtnOK_Click;
            this.Controls.Add(this.btnOK);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(285, 220);
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAppName.Text))
            {
                MessageBox.Show("Application name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Description is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppName = txtAppName.Text.Trim();
            Description = txtDescription.Text.Trim();
            PropertyKey = txtPropertyKey.Text.Trim();
            PropertyValue = txtPropertyValue.Text.Trim();
            IsPropertyMasked = chkMasked.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}