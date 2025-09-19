using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SSOConfigManagerUI
{
    public partial class SetPropertiesForm : Form
    {
        public string AppName { get; private set; }
        public Dictionary<string, object> Properties { get; private set; }
        public List<string> PropertiesToDelete { get; private set; }
        private Dictionary<string, object> originalProperties;

        private TextBox txtAppName;
        private Label lblAppNotFound;
        private ListView listProperties;
        private TextBox txtPropertyKey;
        private TextBox txtPropertyValue;
        private Button btnAddProperty;
        private Button btnRemoveProperty;
        private Button btnOK;
        private Button btnCancel;

        public SetPropertiesForm()
        {
            Properties = new Dictionary<string, object>();
            PropertiesToDelete = new List<string>();
            originalProperties = new Dictionary<string, object>();
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtAppName = new TextBox();
            this.lblAppNotFound = new Label();
            this.listProperties = new ListView();
            this.txtPropertyKey = new TextBox();
            this.txtPropertyValue = new TextBox();
            this.btnAddProperty = new Button();
            this.btnRemoveProperty = new Button();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // Form
            this.Text = "Set Properties for SSO Application";
            this.Size = new System.Drawing.Size(500, 410);
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

            var lblProperties = new Label();
            lblProperties.Text = "Properties to Set:";
            lblProperties.Location = new System.Drawing.Point(20, 60);
            lblProperties.Size = new System.Drawing.Size(120, 23);
            this.Controls.Add(lblProperties);

            var lblPropertyKey = new Label();
            lblPropertyKey.Text = "Property Key:";
            lblPropertyKey.Location = new System.Drawing.Point(20, 235);
            lblPropertyKey.Size = new System.Drawing.Size(80, 23);
            this.Controls.Add(lblPropertyKey);

            var lblPropertyValue = new Label();
            lblPropertyValue.Text = "Property Value:";
            lblPropertyValue.Location = new System.Drawing.Point(20, 275);
            lblPropertyValue.Size = new System.Drawing.Size(80, 30);
            this.Controls.Add(lblPropertyValue);

            // TextBoxes
            this.txtAppName.Location = new System.Drawing.Point(150, 20);
            this.txtAppName.Size = new System.Drawing.Size(300, 23);
            this.txtAppName.Leave += TxtAppName_Leave;
            this.Controls.Add(this.txtAppName);

            // App Not Found Label
            this.lblAppNotFound.Text = "Application doesn't currently exist";
            this.lblAppNotFound.Location = new System.Drawing.Point(150, 47);
            this.lblAppNotFound.Size = new System.Drawing.Size(300, 23);
            this.lblAppNotFound.ForeColor = System.Drawing.Color.Red;
            this.lblAppNotFound.Visible = false;
            this.Controls.Add(this.lblAppNotFound);

            // Properties ListView
            this.listProperties.Location = new System.Drawing.Point(150, 75);
            this.listProperties.Size = new System.Drawing.Size(300, 140);
            this.listProperties.View = View.Details;
            this.listProperties.FullRowSelect = true;
            this.listProperties.GridLines = true;
            this.listProperties.Columns.Add("Key", 150);
            this.listProperties.Columns.Add("Value", 145);
            this.Controls.Add(this.listProperties);

            this.txtPropertyKey.Location = new System.Drawing.Point(110, 235);
            this.txtPropertyKey.Size = new System.Drawing.Size(150, 23);
            this.Controls.Add(this.txtPropertyKey);

            this.txtPropertyValue.Location = new System.Drawing.Point(110, 275);
            this.txtPropertyValue.Size = new System.Drawing.Size(150, 23);
            this.Controls.Add(this.txtPropertyValue);

            // Property Buttons
            this.btnAddProperty.Text = "Add Property";
            this.btnAddProperty.Location = new System.Drawing.Point(280, 245);
            this.btnAddProperty.Size = new System.Drawing.Size(90, 30);
            this.btnAddProperty.Click += BtnAddProperty_Click;
            this.Controls.Add(this.btnAddProperty);

            this.btnRemoveProperty.Text = "Remove";
            this.btnRemoveProperty.Location = new System.Drawing.Point(380, 245);
            this.btnRemoveProperty.Size = new System.Drawing.Size(70, 30);
            this.btnRemoveProperty.Click += BtnRemoveProperty_Click;
            this.Controls.Add(this.btnRemoveProperty);

            // Main Buttons
            this.btnOK.Text = "Set Properties";
            this.btnOK.Location = new System.Drawing.Point(290, 335);
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Click += BtnOK_Click;
            this.Controls.Add(this.btnOK);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(400, 335);
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
        }

        private void TxtAppName_Leave(object sender, EventArgs e)
        {
            ValidateApplicationName();
        }

        private void ValidateApplicationName()
        {
            string appName = txtAppName.Text.Trim();
            
            // Clear previous validation state
            lblAppNotFound.Visible = false;
            btnOK.Enabled = false;
            
            if (string.IsNullOrWhiteSpace(appName))
            {
                ClearPropertiesTable();
                return;
            }

            // Only do format validation first (fast)
            if (!IsValidApplicationNameFormat(appName))
            {
                lblAppNotFound.Text = "Invalid application name format";
                lblAppNotFound.ForeColor = System.Drawing.Color.Red;
                lblAppNotFound.Visible = true;
                ClearPropertiesTable();
                return;
            }

            // Only check existence for reasonable length names to avoid unnecessary SSO calls
            if (appName.Length < 2)
            {
                return; // Too short to be meaningful, don't check SSO yet
            }

            // Check if application exists (potentially slow operation)
            var existsResult = CheckIfApplicationExists(appName);
            
            if (existsResult.exists)
            {
                lblAppNotFound.Visible = false;
                btnOK.Enabled = true;
                LoadExistingProperties(appName);
            }
            else if (existsResult.error != null)
            {
                lblAppNotFound.Text = "Error checking application";
                lblAppNotFound.ForeColor = System.Drawing.Color.Orange;
                lblAppNotFound.Visible = true;
                ClearPropertiesTable();
            }
            else
            {
                lblAppNotFound.Text = "Application doesn't exist";
                lblAppNotFound.ForeColor = System.Drawing.Color.Red;
                lblAppNotFound.Visible = true;
                ClearPropertiesTable();
            }
        }

        private (bool exists, string error) CheckIfApplicationExists(string appName)
        {
            try
            {
                var applications = BizTalk.Tools.SSOApplicationConfiguration.SSOConfigManager.GetApplications();
                // Use case-insensitive comparison manually since Contains with StringComparer isn't available in older frameworks
                bool exists = applications.Any(app => string.Equals(app, appName, StringComparison.OrdinalIgnoreCase));
                return (exists, null);
            }
            catch (Exception ex)
            {
                // Return error details for better user feedback
                return (false, ex.Message);
            }
        }

        private bool IsValidApplicationNameFormat(string appName)
        {
            // Check for valid application name format
            if (string.IsNullOrWhiteSpace(appName))
                return false;
                
            // Check length (SSO applications typically have length limits)
            if (appName.Length > 260) // Common Windows path limit
                return false;
                
            // Check for invalid characters that might cause SSO issues
            char[] invalidChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            if (appName.IndexOfAny(invalidChars) >= 0)
                return false;
                
            // Check for leading/trailing spaces or dots (can cause issues)
            if (appName.StartsWith(" ") || appName.EndsWith(" ") || 
                appName.StartsWith(".") || appName.EndsWith("."))
                return false;
                
            return true;
        }

        private void LoadExistingProperties(string appName)
        {
            try
            {
                // Clear current properties and list
                ClearPropertiesTable();

                // Get existing properties from SSO
                string description, contactInfo, userAccount, adminAccount;
                var existingProperties = BizTalk.Tools.SSOApplicationConfiguration.SSOConfigManager.GetConfigProperties(
                    appName, out description, out contactInfo, out userAccount, out adminAccount);

                // Load existing properties into the form
                foreach (System.Collections.DictionaryEntry entry in existingProperties)
                {
                    if (entry.Key != null)
                    {
                        string key = entry.Key.ToString();
                        string value = entry.Value?.ToString() ?? "";
                        
                        // Add to internal dictionary
                        Properties[key] = value;
                        
                        // Store in original properties for deletion tracking
                        originalProperties[key] = value;
                        
                        // Add to ListView
                        var item = new ListViewItem(key);
                        item.SubItems.Add(value);
                        listProperties.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                // If we can't load properties, just continue with empty list
                // Could show a warning if needed
                System.Diagnostics.Debug.WriteLine($"Failed to load existing properties: {ex.Message}");
            }
        }

        private void ClearPropertiesTable()
        {
            Properties.Clear();
            originalProperties.Clear();
            PropertiesToDelete.Clear();
            listProperties.Items.Clear();
        }

        private void BtnAddProperty_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPropertyKey.Text))
            {
                MessageBox.Show("Property key is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string key = txtPropertyKey.Text.Trim();
            
            if (Properties.ContainsKey(key))
            {
                MessageBox.Show("Property key already exists. Use Remove to delete it first, or choose a different key.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string value = txtPropertyValue.Text.Trim();

            // If this property was marked for deletion, remove it from the deletion list
            if (PropertiesToDelete.Contains(key))
            {
                PropertiesToDelete.Remove(key);
            }

            Properties[key] = value;

            var item = new ListViewItem(key);
            item.SubItems.Add(value);
            listProperties.Items.Add(item);

            txtPropertyKey.Clear();
            txtPropertyValue.Clear();
            txtPropertyKey.Focus();
        }

        private void BtnRemoveProperty_Click(object sender, EventArgs e)
        {
            if (listProperties.SelectedItems.Count > 0)
            {
                var selectedItem = listProperties.SelectedItems[0];
                string key = selectedItem.Text;
                
                // If this was an original property, mark it for deletion
                if (originalProperties.ContainsKey(key))
                {
                    PropertiesToDelete.Add(key);
                }
                
                // Remove from current properties and ListView
                Properties.Remove(key);
                listProperties.Items.Remove(selectedItem);
            }
            else
            {
                MessageBox.Show("Please select a property to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string appName = txtAppName.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(appName))
            {
                MessageBox.Show("Application name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAppName.Focus();
                return;
            }

            if (!IsValidApplicationNameFormat(appName))
            {
                MessageBox.Show("Invalid application name format. Application names cannot contain special characters like \\ / : * ? \" < > | and cannot start or end with spaces or dots.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAppName.Focus();
                return;
            }

            var existsResult = CheckIfApplicationExists(appName);
            if (existsResult.error != null)
            {
                MessageBox.Show($"Error checking application existence: {existsResult.error}", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!existsResult.exists)
            {
                MessageBox.Show($"The specified application '{appName}' does not exist.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAppName.Focus();
                return;
            }

            // Allow operation if either properties to set OR properties to delete exist
            if (Properties.Count == 0 && PropertiesToDelete.Count == 0)
            {
                MessageBox.Show("At least one property must be added or removed.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppName = appName;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}