using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EnterpriseSingleSignOn.Interop;
using SSOConfigManagerUI.Models;
using SSOConfigManagerUI.DTOs;
using BizTalk.Tools.SSOApplicationConfiguration;

namespace SSOConfigManagerUI.Services
{
    /// <summary>
    /// Service implementation for SSO operations using the Microsoft Enterprise SSO API
    /// </summary>
    public class SSOService : ISSOService
    {
        private const string IdentifierGUID = "ConfigProperties";
        private readonly ApplicationSettings _settings;

        public SSOService() : this(new ApplicationSettings())
        {
        }

        public SSOService(ApplicationSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public OperationResult CreateApplication(CreateApplicationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Name))
                    return OperationResult.Failed("Application name is required");

                // Convert properties to SSO format
                var propBag = new SSOPropBag();
                var maskArray = new ArrayList();

                foreach (var property in request.Properties ?? new List<SSOProperty>())
                {
                    object value = property.Value;
                    propBag.Write(property.Key, ref value);
                    maskArray.Add(property.IsMasked ? 1 : 0);
                }

                // Use existing SSOConfigManager for actual creation
                SSOConfigManager.CreateConfigStoreApplication(
                    request.Name,
                    request.Description ?? string.Empty,
                    request.ContactInfo ?? _settings.DefaultContactInfo,
                    request.UserAccount ?? _settings.DefaultUserAccount,
                    request.AdminAccount ?? _settings.DefaultAdminAccount,
                    propBag,
                    maskArray);

                return OperationResult.Successful($"Application '{request.Name}' created successfully");
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Failed to create application: {ex.Message}", ex.ToString());
            }
        }

        public OperationResult SetApplicationProperties(SetPropertiesRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.ApplicationName))
                    return OperationResult.Failed("Application name is required");

                // Convert properties to SSO format
                var propBag = new SSOPropBag();
                if (request.Properties != null)
                {
                    foreach (var kvp in request.Properties)
                    {
                        object value = kvp.Value;
                        propBag.Write(kvp.Key, ref value);
                    }
                }

                // Use existing method for property replacement
                SSOConfigManager.ReplaceAllConfigProperties(
                    request.ApplicationName, 
                    propBag);

                return OperationResult.Successful("Properties updated successfully");
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Failed to set properties: {ex.Message}", ex.ToString());
            }
        }

        public OperationResult<Dictionary<string, object>> GetApplicationProperties(string applicationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationName))
                    return OperationResult<Dictionary<string, object>>.Failed("Application name is required");

                var configStore = new ISSOConfigStore();
                var propBag = new SSOPropBag();

                configStore.GetConfigInfo(applicationName, IdentifierGUID, Microsoft.EnterpriseSingleSignOn.Interop.SSOFlag.SSO_FLAG_RUNTIME, propBag);

                var properties = new Dictionary<string, object>();
                foreach (DictionaryEntry entry in propBag.properties)
                {
                    properties[entry.Key.ToString()] = entry.Value;
                }

                return OperationResult<Dictionary<string, object>>.Successful(properties);
            }
            catch (Exception ex)
            {
                return OperationResult<Dictionary<string, object>>.Failed(
                    $"Failed to get properties for '{applicationName}': {ex.Message}", 
                    ex.ToString());
            }
        }

        public OperationResult DeleteApplication(string applicationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationName))
                    return OperationResult.Failed("Application name is required");

                SSOConfigManager.DeleteApplication(applicationName);
                
                return OperationResult.Successful($"Application '{applicationName}' deleted successfully");
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Failed to delete application: {ex.Message}", ex.ToString());
            }
        }

        public OperationResult<List<string>> GetAllApplications()
        {
            try
            {
                var applications = SSOConfigManager.GetApplications();
                return OperationResult<List<string>>.Successful(applications ?? new List<string>());
            }
            catch (Exception ex)
            {
                return OperationResult<List<string>>.Failed(
                    $"Failed to retrieve applications: {ex.Message}", 
                    ex.ToString());
            }
        }

        public OperationResult<bool> ApplicationExists(string applicationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationName))
                    return OperationResult<bool>.Failed("Application name is required");

                var appsResult = GetAllApplications();
                if (!appsResult.Success)
                    return OperationResult<bool>.Failed("Failed to check application existence", appsResult.ErrorDetails);

                bool exists = appsResult.Data.Any(app => 
                    string.Equals(app, applicationName, StringComparison.OrdinalIgnoreCase));

                return OperationResult<bool>.Successful(exists);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failed(
                    $"Failed to check if application exists: {ex.Message}", 
                    ex.ToString());
            }
        }

        public OperationResult<SSOApplication> GetApplication(string applicationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationName))
                    return OperationResult<SSOApplication>.Failed("Application name is required");

                // Check if application exists first
                var existsResult = ApplicationExists(applicationName);
                if (!existsResult.Success)
                    return OperationResult<SSOApplication>.Failed("Failed to verify application existence", existsResult.ErrorDetails);

                if (!existsResult.Data)
                    return OperationResult<SSOApplication>.Failed($"Application '{applicationName}' does not exist");

                // Get properties
                var propertiesResult = GetApplicationProperties(applicationName);
                if (!propertiesResult.Success)
                    return OperationResult<SSOApplication>.Failed("Failed to retrieve application properties", propertiesResult.ErrorDetails);

                // Create application object
                var application = new SSOApplication(applicationName)
                {
                    ContactInfo = _settings.DefaultContactInfo,
                    UserAccount = _settings.DefaultUserAccount,
                    AdminAccount = _settings.DefaultAdminAccount
                };

                // Add properties
                foreach (var kvp in propertiesResult.Data)
                {
                    application.AddProperty(kvp.Key, kvp.Value);
                }

                return OperationResult<SSOApplication>.Successful(application);
            }
            catch (Exception ex)
            {
                return OperationResult<SSOApplication>.Failed(
                    $"Failed to get application '{applicationName}': {ex.Message}", 
                    ex.ToString());
            }
        }
    }
}