using System;
using System.Drawing;
using System.Windows.Forms;

namespace SSOConfigManagerUI.Base
{
    /// <summary>
    /// Base class for all dialog forms with common styling and behavior
    /// </summary>
    public abstract class BaseDialog : Form
    {
        protected Button btnOK;
        protected Button btnCancel;

        protected BaseDialog()
        {
            InitializeBaseDialog();
        }

        private void InitializeBaseDialog()
        {
            // Common form properties
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;

            // Create common buttons
            CreateButtons();
            
            // Call virtual method for derived classes to set up their UI
            this.SuspendLayout();
            InitializeControls();
            SetupLayout();
            this.ResumeLayout(false);
        }

        private void CreateButtons()
        {
            // OK Button
            btnOK = new Button
            {
                Text = "OK",
                Size = new Size(75, 23),
                DialogResult = DialogResult.OK,
                UseVisualStyleBackColor = true
            };
            btnOK.Click += BtnOK_Click;

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(75, 23),
                DialogResult = DialogResult.Cancel,
                UseVisualStyleBackColor = true
            };

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        protected virtual void BtnOK_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                OnFormAccepted();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Override this method to create and configure controls specific to the derived form
        /// </summary>
        protected abstract void InitializeControls();

        /// <summary>
        /// Override this method to position controls and set up the layout
        /// </summary>
        protected virtual void SetupLayout()
        {
            // Position OK and Cancel buttons at the bottom right
            var formWidth = this.ClientSize.Width;
            var formHeight = this.ClientSize.Height;

            btnCancel.Location = new Point(formWidth - 85, formHeight - 35);
            btnOK.Location = new Point(formWidth - 170, formHeight - 35);

            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        /// <summary>
        /// Override this method to validate user input before accepting the form
        /// </summary>
        protected virtual bool ValidateInput()
        {
            return true;
        }

        /// <summary>
        /// Override this method to perform actions when the form is accepted
        /// </summary>
        protected virtual void OnFormAccepted()
        {
        }

        /// <summary>
        /// Helper method to create and position a label
        /// </summary>
        protected Label CreateLabel(string text, Point location, Size? size = null)
        {
            var label = new Label
            {
                Text = text,
                Location = location,
                Size = size ?? new Size(120, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(label);
            return label;
        }

        /// <summary>
        /// Helper method to create and position a textbox
        /// </summary>
        protected TextBox CreateTextBox(Point location, Size? size = null, bool multiline = false)
        {
            var textBox = new TextBox
            {
                Location = location,
                Size = size ?? new Size(200, multiline ? 60 : 23),
                Multiline = multiline
            };
            if (multiline)
            {
                textBox.ScrollBars = ScrollBars.Vertical;
            }
            this.Controls.Add(textBox);
            return textBox;
        }

        /// <summary>
        /// Helper method to create and position a checkbox
        /// </summary>
        protected CheckBox CreateCheckBox(string text, Point location, Size? size = null)
        {
            var checkBox = new CheckBox
            {
                Text = text,
                Location = location,
                Size = size ?? new Size(150, 23),
                UseVisualStyleBackColor = true
            };
            this.Controls.Add(checkBox);
            return checkBox;
        }
    }

    /// <summary>
    /// Base class for forms that validate application names and show validation feedback
    /// </summary>
    public abstract class BaseValidatedForm : BaseDialog
    {
        protected TextBox txtAppName;
        protected Label lblAppNotFound;
        protected bool isValidating = false;

        protected BaseValidatedForm()
        {
        }

        protected override void InitializeControls()
        {
            // Create application name input
            CreateLabel("Application Name:", new Point(20, 20));
            txtAppName = CreateTextBox(new Point(150, 20), new Size(250, 23));
            txtAppName.Leave += TxtAppName_Leave;

            // Create validation feedback label
            lblAppNotFound = new Label
            {
                Text = "Application doesn't exist",
                Location = new Point(150, 47),
                Size = new Size(250, 23),
                ForeColor = Color.Red,
                Visible = false
            };
            this.Controls.Add(lblAppNotFound);
        }

        private void TxtAppName_Leave(object sender, EventArgs e)
        {
            if (!isValidating)
            {
                ValidateApplicationName();
            }
        }

        protected virtual void ValidateApplicationName()
        {
            isValidating = true;
            try
            {
                string appName = txtAppName.Text.Trim();
                
                // Clear previous validation state
                lblAppNotFound.Visible = false;
                btnOK.Enabled = false;
                
                if (string.IsNullOrWhiteSpace(appName))
                {
                    OnValidationCleared();
                    return;
                }

                // Perform validation specific to derived form
                var validationResult = PerformApplicationValidation(appName);
                
                if (validationResult.IsValid)
                {
                    lblAppNotFound.Visible = false;
                    btnOK.Enabled = true;
                    OnValidationSucceeded(appName);
                }
                else
                {
                    lblAppNotFound.Text = validationResult.ErrorMessage;
                    lblAppNotFound.ForeColor = validationResult.IsWarning ? Color.Orange : Color.Red;
                    lblAppNotFound.Visible = true;
                    btnOK.Enabled = validationResult.AllowProceed;
                    OnValidationFailed(validationResult.ErrorMessage);
                }
            }
            finally
            {
                isValidating = false;
            }
        }

        /// <summary>
        /// Override this method to perform application-specific validation
        /// </summary>
        protected abstract ValidationResult PerformApplicationValidation(string applicationName);

        /// <summary>
        /// Called when validation is cleared (empty input)
        /// </summary>
        protected virtual void OnValidationCleared()
        {
        }

        /// <summary>
        /// Called when validation succeeds
        /// </summary>
        protected virtual void OnValidationSucceeded(string applicationName)
        {
        }

        /// <summary>
        /// Called when validation fails
        /// </summary>
        protected virtual void OnValidationFailed(string errorMessage)
        {
        }

        public string ApplicationName => txtAppName?.Text?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Result of application name validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsWarning { get; set; }
        public bool AllowProceed { get; set; }

        public static ValidationResult Valid()
        {
            return new ValidationResult { IsValid = true, AllowProceed = true };
        }

        public static ValidationResult Invalid(string message)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = message, AllowProceed = false };
        }

        public static ValidationResult Warning(string message, bool allowProceed = true)
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = message, 
                IsWarning = true, 
                AllowProceed = allowProceed 
            };
        }
    }
}