using System.Collections.Generic;
using SSOConfigManagerUI.Models;

namespace SSOConfigManagerUI.DTOs
{
    /// <summary>
    /// Request object for creating new SSO applications
    /// </summary>
    public class CreateApplicationRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public string UserAccount { get; set; }
        public string AdminAccount { get; set; }
        public List<SSOProperty> Properties { get; set; }

        public CreateApplicationRequest()
        {
            Properties = new List<SSOProperty>();
        }

        public CreateApplicationRequest(string name, string description = "") : this()
        {
            Name = name;
            Description = description;
        }
    }

    /// <summary>
    /// Request object for setting/updating application properties
    /// </summary>
    public class SetPropertiesRequest
    {
        public string ApplicationName { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public List<string> PropertiesToDelete { get; set; }

        public SetPropertiesRequest()
        {
            Properties = new Dictionary<string, object>();
            PropertiesToDelete = new List<string>();
        }

        public SetPropertiesRequest(string applicationName) : this()
        {
            ApplicationName = applicationName;
        }
    }

    /// <summary>
    /// Response object for operations that return results
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorDetails { get; set; }

        public static OperationResult Successful(string message = "Operation completed successfully")
        {
            return new OperationResult { Success = true, Message = message };
        }

        public static OperationResult Failed(string message, string errorDetails = null)
        {
            return new OperationResult 
            { 
                Success = false, 
                Message = message, 
                ErrorDetails = errorDetails 
            };
        }
    }

    /// <summary>
    /// Response object for operations that return data
    /// </summary>
    public class OperationResult<T> : OperationResult
    {
        public T Data { get; set; }

        public static OperationResult<T> Successful(T data, string message = "Operation completed successfully")
        {
            return new OperationResult<T> 
            { 
                Success = true, 
                Message = message, 
                Data = data 
            };
        }

        public new static OperationResult<T> Failed(string message, string errorDetails = null)
        {
            return new OperationResult<T> 
            { 
                Success = false, 
                Message = message, 
                ErrorDetails = errorDetails 
            };
        }
    }
}