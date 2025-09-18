using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSOConfigManagerUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnShowApps_Click(object sender, EventArgs e)
        {
            var apps = SSO_Program.ApplicationManager.GetAllApplications();
            txtOutput.Text = (apps != null && apps.Count > 0)
                ? "SSO Applications:\r\n" + string.Join("\r\n", apps)
                : "No applications found.";
        }
        
        private void btnCreateApp_Click(object sender, EventArgs e)
        {
            using (var createForm = new CreateAppForm())
            {
                if (createForm.ShowDialog() == DialogResult.OK)
                {
                    var properties = new Dictionary<string, (object value, bool masked)>();
                    
                    // Only add property if both key and value are provided
                    if (!string.IsNullOrWhiteSpace(createForm.PropertyKey) && !string.IsNullOrWhiteSpace(createForm.PropertyValue))
                    {
                        properties[createForm.PropertyKey] = (createForm.PropertyValue, createForm.IsPropertyMasked);
                    }
                    
                    txtOutput.Text = SSO_Program.ApplicationManager.CreateApplication(
                        createForm.AppName, 
                        createForm.Description, 
                        properties);
                }
            }
        }
        
        private void btnSetProps_Click(object sender, EventArgs e)
        {
            using (var setPropsForm = new SetPropertiesForm())
            {
                if (setPropsForm.ShowDialog() == DialogResult.OK)
                {
                    txtOutput.Text = SSO_Program.ApplicationManager.SetConfigProperties(
                        setPropsForm.AppName, 
                        setPropsForm.Properties,
                        setPropsForm.PropertiesToDelete);
                }
            }
        }
        
        private void btnGetProps_Click(object sender, EventArgs e)
        {
            var appName = Prompt("Enter application name:");
            txtOutput.Text = SSO_Program.ApplicationManager.GetConfigProperties(appName);
        }
        
        private void btnDeleteApp_Click(object sender, EventArgs e)
        {
            var appName = Prompt("Enter application name to delete:");
            if (string.IsNullOrWhiteSpace(appName))
            {
                txtOutput.Text = "Delete operation cancelled - no application name provided.";
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the SSO application '{appName}'?\n\nThis action cannot be undone.", 
                "Confirm Delete Application", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2); // Default to "No"
                
            if (result == DialogResult.Yes)
            {
                txtOutput.Text = SSO_Program.ApplicationManager.DeleteApplication(appName);
            }
            else
            {
                txtOutput.Text = "Delete operation cancelled.";
            }
        }
        
        // Helper methods for input dialogs
        private string Prompt(string text)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(text, "Input", "");
        }
        
        private int PromptInt(string text)
        {
            var input = Prompt(text);
            int val;
            return int.TryParse(input, out val) ? val : 0;
        }
    }
}