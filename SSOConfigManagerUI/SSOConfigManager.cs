using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Microsoft.EnterpriseSingleSignOn.Interop;
using SSO_Program;

namespace BizTalk.Tools.SSOApplicationConfiguration
{
    public static class SSOConfigManager
    {
        private static string idenifierGUID = "ConfigProperties";

        public static void CreateConfigStoreApplication(string appName, string description, string contactInfo, string uAccountName, string adminAccountName, SSOPropBag propertiesBag, ArrayList maskArray)
        {
            int appFlags = 0;

            appFlags |= SSOFlag.SSO_FLAG_APP_CONFIG_STORE;
            appFlags |= SSOFlag.SSO_FLAG_SSO_WINDOWS_TO_EXTERNAL;
            appFlags |= SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;

            ISSOAdmin ssoAdmin = new ISSOAdmin();
            
            if (propertiesBag.PropertyCount > 0)
                ssoAdmin.CreateApplication(appName, description, contactInfo, uAccountName, adminAccountName, appFlags, propertiesBag.PropertyCount);
            else
            {
                ssoAdmin.CreateApplication(appName, description, contactInfo, uAccountName, adminAccountName, appFlags, 1);
            }
                
            int counter = 0;

            ssoAdmin.CreateFieldInfo(appName, "dummy", 0);

            foreach (DictionaryEntry de in propertiesBag.properties)
            {
                string propName = de.Key.ToString();
                int fieldFlags = 0;
                fieldFlags |= Convert.ToInt32(maskArray[counter]);

                ssoAdmin.CreateFieldInfo(appName, propName, fieldFlags);

                counter++;
            }

            ssoAdmin.UpdateApplication(appName, null, null, null, null, SSOFlag.SSO_FLAG_ENABLED, SSOFlag.SSO_FLAG_ENABLED);

            ISSOConfigStore configStore = new ISSOConfigStore();
            configStore.SetConfigInfo(appName, idenifierGUID, propertiesBag);
        }

        public static void SetConfigProperties(string appName, SSOPropBag propertyBag)
        {
            try 
            {
                ISSOConfigStore configStore = new ISSOConfigStore();
                ISSOAdmin ssoAdmin = new ISSOAdmin();

                // First, get existing configuration to preserve it
                SSOPropBag existingBag = new SSOPropBag();
                try
                {
                    configStore.GetConfigInfo(appName, "ConfigProperties", SSOFlag.SSO_FLAG_RUNTIME, existingBag);
                }
                catch
                {
                    // No existing config, that's fine
                }

                // Create a new merged bag with existing properties
                SSOPropBag mergedBag = new SSOPropBag();
                
                // First, copy all existing properties to the merged bag
                foreach (DictionaryEntry existingEntry in existingBag.properties)
                {
                    object value = existingEntry.Value;
                    mergedBag.Write(existingEntry.Key.ToString(), ref value);
                }
                
                // Then, add/overwrite with new properties
                foreach (DictionaryEntry newEntry in propertyBag.properties)
                {
                    object value = newEntry.Value;
                    mergedBag.Write(newEntry.Key.ToString(), ref value);
                }

                // Add field definitions for ALL properties (existing + new)
                foreach (DictionaryEntry de in mergedBag.properties)
                {
                    string propName = de.Key.ToString();
                    try
                    {
                        // Try to create the field (this will fail if it already exists, which is fine)
                        ssoAdmin.CreateFieldInfo(appName, propName, 0);
                    }
                    catch
                    {
                        // Field already exists, continue
                    }
                }

                // Update the application to ensure it's in the right state
                ssoAdmin.UpdateApplication(appName, null, null, null, null, SSOFlag.SSO_FLAG_ENABLED, SSOFlag.SSO_FLAG_ENABLED);

                // Set the merged configuration (preserving existing + adding new)
                configStore.SetConfigInfo(appName, "ConfigProperties", mergedBag);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to set properties for application '{appName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Replaces all configuration properties for an SSO application by recreating the application.
        /// This ensures complete property deletion without leaving orphaned properties in different storage locations.
        /// </summary>
        /// <param name="appName">Name of the SSO application</param>
        /// <param name="propertyBag">New properties to set (existing properties not in this bag will be deleted)</param>
        public static void ReplaceAllConfigProperties(string appName, SSOPropBag propertyBag)
        {
            try 
            {
                ISSOConfigStore configStore = new ISSOConfigStore();
                ISSOAdmin ssoAdmin = new ISSOAdmin();

                // Get application info before deletion
                string description, contactInfo, userAccount, adminAccount;
                int flags, count;
                ssoAdmin.GetApplicationInfo(appName, out description, out contactInfo, out userAccount, out adminAccount, out flags, out count);
                
                // Delete the application completely
                ssoAdmin.DeleteApplication(appName);
                
                // Recreate the application with only the new properties
                var maskArray = new ArrayList();
                for (int i = 0; i < propertyBag.PropertyCount; i++)
                {
                    maskArray.Add(0); // Not masked
                }
                
                CreateConfigStoreApplication(appName, description, contactInfo, userAccount, adminAccount, propertyBag, maskArray);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update properties for application '{appName}': {ex.Message}", ex);
            }
        }

        public static HybridDictionary GetConfigProperties(string appName, out string description, out string contactInfo, out string appUserAcct, out string appAdminAcct)
        {
            int flags;
            int count;

            ISSOAdmin ssoAdmin = new ISSOAdmin();
            ssoAdmin.GetApplicationInfo(appName, out description, out contactInfo, out appUserAcct, out appAdminAcct, out flags, out count);
            
            HybridDictionary result = new HybridDictionary();
            
            // Try different identifier and flag combinations to get all properties
            TryGetPropertiesFromLocation(appName, "ConfigProperties", SSOFlag.SSO_FLAG_RUNTIME, result);
            TryGetPropertiesFromLocation(appName, idenifierGUID, SSOFlag.SSO_FLAG_RUNTIME, result);
            
            // If we still have no properties, try with different flags
            if (result.Count == 0)
            {
                TryGetPropertiesFromLocation(appName, "ConfigProperties", SSOFlag.SSO_FLAG_APP_CONFIG_STORE, result);
                
                if (result.Count == 0)
                {
                    throw new Exception("Failed to get any config properties from any location.");
                }
            }

            return result;
        }

        private static void TryGetPropertiesFromLocation(string appName, string identifier, int flag, HybridDictionary result)
        {
            try
            {
                ISSOConfigStore configStore = new ISSOConfigStore();
                SSOPropBag propertiesBag = new SSOPropBag();
                configStore.GetConfigInfo(appName, identifier, flag, propertiesBag);
                
                // Add properties to result (merge with existing)
                foreach (DictionaryEntry entry in propertiesBag.properties)
                {
                    if (entry.Key != null)
                    {
                        result[entry.Key] = entry.Value ?? "";
                    }
                }
            }
            catch
            {
                // This identifier/flag combination might not exist, continue silently
            }
        }

        public static void DeleteApplication(string appName)
        {
            ISSOAdmin ssoAdmin = new ISSOAdmin();

            ssoAdmin.DeleteApplication(appName);
        }

        public static List<string> GetApplications()
        {
            string[] applications = null;
            string[] descs;
            string[] contacts = null;

            try
            {
                var mapper = new ISSOMapper();

                AffiliateApplicationType appTypes = AffiliateApplicationType.ConfigStore;

                IPropertyBag propBag = (IPropertyBag)mapper;

                uint appFilterFlagMask = SSOFlag.SSO_FLAG_APP_FILTER_BY_TYPE;
                uint appFilterFlags = (uint)appTypes;

                object appFilterFlagsObj = (object)appFilterFlags;
                object appFilterFlagMaskObj = (object)appFilterFlagMask;

                propBag.Write("AppFilterFlags", ref appFilterFlagsObj);
                propBag.Write("AppFilterFlagMask", ref appFilterFlagMaskObj);                
                
                mapper.GetApplications(out applications, out descs, out contacts);
            }
            catch (COMException comEx)
            {
                HandleCOMException(comEx, 0);
            }

            List<string> filteredApps = new List<string>();

            if (applications != null)
            {
                // Filter applications to only include those with "SSO Administrators" as admin account
                // and exclude those with "Contact Information" as contact info
                foreach (string appName in applications)
                {
                    try
                    {
                        ISSOAdmin ssoAdmin = new ISSOAdmin();
                        string description, contactInfo, userAccount, adminAccount;
                        int flags, count;
                        
                        ssoAdmin.GetApplicationInfo(appName, out description, out contactInfo, 
                            out userAccount, out adminAccount, out flags, out count);
                        
                        // Only include apps where admin account is "SSO Administrators"
                        // AND exclude apps where contact info is "Contact Information"
                        if (string.Equals(adminAccount, "SSO Administrators", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(contactInfo, "Contact Information", StringComparison.OrdinalIgnoreCase))
                        {
                            filteredApps.Add(appName);
                        }
                    }
                    catch
                    {
                        // If we can't get app info, skip this application
                        continue;
                    }
                }
            }

            return filteredApps;
        }

        private static void HandleCOMException(COMException comEx, int ignoreErrorCode)
        {
            if (comEx.ErrorCode != ignoreErrorCode)
            {
                throw new ApplicationException(string.Format("SSO error - code: {0}, message: {1}", comEx.ErrorCode, comEx.Message), comEx);

            }
        }

        public static bool SearchKeys(HybridDictionary dict, string key)
        {
            foreach (var k in dict.Keys)
            {
                if (k.ToString().ToUpper().Contains(key.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool SearchValues(HybridDictionary dict, string value)
        {
            foreach (var v in dict.Values)
            {
                if (v.ToString().ToUpper().Contains(value.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        // Helper method to generate a consistent identifier for an application
        private static string GenerateConsistentGuid(string appName)
        {
            // For SSO ConfigStore applications, use the application name itself as identifier
            return appName;
        }
    }


    public class SSOPropBag : IPropertyBag
    {
        internal HybridDictionary properties;

        public SSOPropBag()
        {
            properties = new HybridDictionary(true);
        }

        public SSOPropBag(HybridDictionary d)
        {
            properties = d;
        }

        #region IPropertyBag Members

        public void Read(string propName, out object ptrVar, int errorLog)
        {
            // For reading from SSO storage, we should return what's in our collection
            // If the property doesn't exist, return null
            if (properties.Contains(propName))
            {
                ptrVar = properties[propName];
            }
            else
            {
                ptrVar = null;
            }
        }

        public void Write(string propName, ref object ptrVar)
        {
            properties[propName] = ptrVar;  // Use indexer to overwrite if exists
        }

        public int PropertyCount
        {
            get
            {
                return properties.Count;
            }
        }

        #endregion
    }

}