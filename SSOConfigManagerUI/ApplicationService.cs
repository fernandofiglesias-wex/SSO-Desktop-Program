using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SSOConfigManagerUI.Models;
using SSOConfigManagerUI.DTOs;

namespace SSOConfigManagerUI.Services
{
    /// <summary>
    /// Service implementation for application management and business logic
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private readonly ISSOService _ssoService;
        private readonly ApplicationSettings _settings;

        public ApplicationService() : this(new SSOService(), new ApplicationSettings())
        {
        }

        public ApplicationService(ISSOService ssoService, ApplicationSettings settings)
        {
            _ssoService = ssoService ?? throw new ArgumentNullException(nameof(ssoService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public OperationResult CreateApplication(string name, string description, Dictionary<string, (object value, bool masked)> properties)
        {
            try
            {
                // Validate application name
                var nameValidation = ValidateApplicationName(name);
                if (!nameValidation.Success)
                    return nameValidation;

                // Check if application already exists
                var existsResult = _ssoService.ApplicationExists(name);
                if (!existsResult.Success)
                    return OperationResult.Failed("Failed to check if application exists", existsResult.ErrorDetails);

                if (existsResult.Data)
                    return OperationResult.Failed($"Application '{name}' already exists");

                // Convert properties to domain objects
                var request = new CreateApplicationRequest(name, description)
                {
                    ContactInfo = _settings.DefaultContactInfo,
                    UserAccount = _settings.DefaultUserAccount,
                    AdminAccount = _settings.DefaultAdminAccount
                };

                foreach (var kvp in properties ?? new Dictionary<string, (object, bool)>())
                {
                    request.Properties.Add(new SSOProperty(kvp.Key, kvp.Value.value, kvp.Value.masked));
                }

                return _ssoService.CreateApplication(request);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Failed to create application: {ex.Message}", ex.ToString());
            }
        }

        public OperationResult UpdateApplicationProperties(string applicationName, Dictionary<string, object> properties, List<string> propertiesToDelete = null)
        {
            try
            {
                // Validate application name
                var nameValidation = ValidateApplicationName(applicationName);
                if (!nameValidation.Success)
                    return nameValidation;

                // Check if application exists
                var existsResult = _ssoService.ApplicationExists(applicationName);
                if (!existsResult.Success)
                    return OperationResult.Failed("Failed to check if application exists", existsResult.ErrorDetails);

                if (!existsResult.Data)
                    return OperationResult.Failed($"Application '{applicationName}' does not exist");

                // Create request
                var request = new SetPropertiesRequest(applicationName)
                {
                    Properties = properties ?? new Dictionary<string, object>(),
                    PropertiesToDelete = propertiesToDelete ?? new List<string>()
                };

                return _ssoService.SetApplicationProperties(request);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Failed to update properties: {ex.Message}", ex.ToString());
            }
        }

        public OperationResult<bool> ValidateApplicationName(string applicationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationName))
                    return OperationResult<bool>.Failed("Application name cannot be empty");

                // Check length
                if (applicationName.Length < 1 || applicationName.Length > 260)
                    return OperationResult<bool>.Failed("Application name must be between 1 and 260 characters");

                // Check for invalid characters (basic validation)
                if (applicationName.Contains("\\") || applicationName.Contains("/") || 
                    applicationName.Contains(":") || applicationName.Contains("*") ||
                    applicationName.Contains("?") || applicationName.Contains("\"") ||
                    applicationName.Contains("<") || applicationName.Contains(">") ||
                    applicationName.Contains("|"))
                {
                    return OperationResult<bool>.Failed("Application name contains invalid characters");
                }

                // Check for reserved names
                var reservedNames = new[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", 
                                          "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", 
                                          "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

                if (reservedNames.Contains(applicationName.ToUpperInvariant()))
                    return OperationResult<bool>.Failed("Application name is a reserved system name");

                return OperationResult<bool>.Successful(true, "Application name is valid");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failed($"Failed to validate application name: {ex.Message}", ex.ToString());
            }
        }

        public ApplicationSettings GetApplicationSettings()
        {
            return _settings;
        }

        public SSOApplication ConvertToSSOApplication(string name, Dictionary<string, object> properties)
        {
            var application = new SSOApplication(name);

            foreach (var kvp in properties ?? new Dictionary<string, object>())
            {
                application.AddProperty(kvp.Key, kvp.Value);
            }

            return application;
        }
    }
}