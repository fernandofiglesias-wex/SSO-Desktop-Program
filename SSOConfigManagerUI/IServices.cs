using System;
using System.Collections.Generic;
using SSOConfigManagerUI.Models;
using SSOConfigManagerUI.DTOs;

namespace SSOConfigManagerUI.Services
{
    /// <summary>
    /// Interface for SSO configuration management operations
    /// </summary>
    public interface ISSOService
    {
        /// <summary>
        /// Creates a new SSO application with the specified properties
        /// </summary>
        OperationResult CreateApplication(CreateApplicationRequest request);

        /// <summary>
        /// Updates properties for an existing SSO application
        /// </summary>
        OperationResult SetApplicationProperties(SetPropertiesRequest request);

        /// <summary>
        /// Retrieves properties for a specific SSO application
        /// </summary>
        OperationResult<Dictionary<string, object>> GetApplicationProperties(string applicationName);

        /// <summary>
        /// Deletes an SSO application completely
        /// </summary>
        OperationResult DeleteApplication(string applicationName);

        /// <summary>
        /// Gets a list of all SSO applications
        /// </summary>
        OperationResult<List<string>> GetAllApplications();

        /// <summary>
        /// Checks if an SSO application exists
        /// </summary>
        OperationResult<bool> ApplicationExists(string applicationName);

        /// <summary>
        /// Retrieves complete application information including metadata
        /// </summary>
        OperationResult<SSOApplication> GetApplication(string applicationName);
    }

    /// <summary>
    /// Interface for application management and business logic
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Creates a new application with validation and business rules
        /// </summary>
        OperationResult CreateApplication(string name, string description, Dictionary<string, (object value, bool masked)> properties);

        /// <summary>
        /// Updates application properties with proper validation
        /// </summary>
        OperationResult UpdateApplicationProperties(string applicationName, Dictionary<string, object> properties, List<string> propertiesToDelete = null);

        /// <summary>
        /// Validates application name format and availability
        /// </summary>
        OperationResult<bool> ValidateApplicationName(string applicationName);

        /// <summary>
        /// Gets application configuration settings
        /// </summary>
        ApplicationSettings GetApplicationSettings();

        /// <summary>
        /// Converts between different property formats
        /// </summary>
        SSOApplication ConvertToSSOApplication(string name, Dictionary<string, object> properties);
    }
}