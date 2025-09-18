using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BizTalk.Tools.SSOApplicationConfiguration;

namespace SSO_Program
{
    public static class ApplicationManager
    {
        public static string ContactInfo = "BizTalk@WexInc.com";

        public static string CreateApplication(string appName, string description, Dictionary<string, (object value, bool masked)> properties)
        {
            var propBag = new SSOPropBag();
            var maskArray = new ArrayList();
            foreach (var kvp in properties)
            {
                object value = kvp.Value.value;
                propBag.Write(kvp.Key, ref value);
                maskArray.Add(kvp.Value.masked ? 1 : 0);
            }
            try
            {
                SSOConfigManager.CreateConfigStoreApplication(appName, description, ContactInfo,
                    "SSO Affiliate Administrators", "SSO Administrators", propBag, maskArray);
                return "Application created successfully!";
            }
            catch (Exception ex)
            {
                return $"Error creating application: {ex.Message}";
            }
        }

        public static string SetConfigProperties(string appName, Dictionary<string, object> properties, List<string> propertiesToDelete = null)
        {
            try
            {
                // Create final property bag with ONLY the properties from the UI (which already excludes deleted ones)
                var finalPropBag = new SSOPropBag();
                if (properties != null)
                {
                    foreach (var kvp in properties)
                    {
                        object value = kvp.Value;
                        finalPropBag.Write(kvp.Key, ref value);
                    }
                }

                // Replace the configuration completely - this should remove any properties not in the bag
                SSOConfigManager.ReplaceAllConfigProperties(appName, finalPropBag);

                return "Properties updated successfully!";
            }
            catch (Exception ex)
            {
                return $"Error setting properties: {ex.Message}";
            }
        }

        public static string GetConfigProperties(string appName)
        {
            try
            {
                string description, contactInfo, userAccount, adminAccount;
                HybridDictionary properties = SSOConfigManager.GetConfigProperties(appName, out description,
                    out contactInfo, out userAccount, out adminAccount);
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Description: {description}");
                sb.AppendLine($"Contact Info: {contactInfo}");
                sb.AppendLine($"User Account: {userAccount}");
                sb.AppendLine($"Admin Account: {adminAccount}");
                sb.AppendLine("Properties:");
                foreach (DictionaryEntry prop in properties)
                {
                    sb.AppendLine($"{prop.Key}: {prop.Value}");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error getting properties: {ex.Message}";
            }
        }

        public static string DeleteApplication(string appName)
        {
            try
            {
                SSOConfigManager.DeleteApplication(appName);
                return "Application deleted successfully!";
            }
            catch (Exception ex)
            {
                return $"Error deleting application: {ex.Message}";
            }
        }

        public static List<string> GetAllApplications()
        {
            try
            {
                var apps = SSOConfigManager.GetApplications();
                return apps;
            }
            catch (Exception ex)
            {
                return new List<string> { $"Error retrieving applications: {ex.Message}" };
            }
        }
    }
}
