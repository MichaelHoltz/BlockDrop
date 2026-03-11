using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace BlockDrop
{
    /// <summary>
    /// SharedBase Class
    /// </summary>
    public class SettingsBase
    {

        /// <summary>
        /// UI Language (e.g., "en-US" or "it-IT")
        /// </summary>
        [Category("Configuration")]
        [Description("UI Language for the application (e.g., en-US or it-IT)")]
        [DefaultValue("en-US")]
        [TypeConverter(typeof(UILanguageTypeConverter))]
        public virtual string UILanguage { get; set; } = "en-US";
        /// <summary>
        /// FilePath For Settings. - Don't need to save it in JSON File though.
        /// 
        /// This variable is intentionally a Field vs Property
        /// </summary>
        [JsonIgnore]
        public string FilePath = @"Invalid File Path - set a valid one";

        /// <summary>
        /// Constructor which allows the file Path to be passed
        /// </summary>
        /// <param name="filePath"></param>
        public SettingsBase(string filePath)
        {
            FilePath = filePath;
        }
        /// <summary>
        /// Function to get the absolute path to this file with no file name
        /// </summary>
        /// <returns></returns>
        public string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(FilePath);
        }

      

        /// <summary>
        /// Save all Properties of the object to currently set FilePath Field
        /// </summary>
        public bool Save()
        {
            try
            {
                if (FilePath.Length > 0)
                {

                    string path = Path.GetDirectoryName(FilePath);
                    if (path.Length > 3 && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string temp = JsonConvert.SerializeObject(this, Formatting.Indented);
                    File.WriteAllText(FilePath, temp);
                }
            }
            catch 
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Load Settings from Default Location
        /// Stores them in this object and also returns them. (Except for FilePath which is ignored primarily so the Path can be changed / File Moved later.)
        /// </summary>
        /// <param name="autoSave">if true, will save the settings immediately if a new object is created due to the file path not existing</param>
        public T Load<T>(bool autoSave = false)
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    //return JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
                    T values = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath)); //Works but calls constructor with Null FilePath need to understand what is going on.
                                                                                             //Iterate through all Properties of an Object programmatically - for Sub Custom Object Types
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.CanWrite) // Found this for when property is JsonIgnore
                        {
                            property.SetValue(this, property.GetValue(values, null), null);
                        }
                        //PropertyInfo[] level2Properties = 

                    }
                    return values;
                }
                catch (System.IO.IOException) //if file is being written to or is locked 
                {
                    //Create the Directory if it doesn't exist and return a new Type of Object
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                    return (T)Activator.CreateInstance(typeof(T), new object[] { FilePath }); //Create a new Default Object

                }
            }
            else
            {
                //Create the Directory if it doesn't exist and return a new Type of Object
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                T newObj = (T)Activator.CreateInstance(typeof(T), new object[] { FilePath }); //Create a new Default Object
                //Automatically Save it if requested.
                if (autoSave && newObj is SettingsBase sb)
                {
                    sb.Save();
                }
                return newObj;
            }
        }
        /// <summary>
        /// Load Settings from specific Location
        /// then returns them. 
        /// </summary>
        public static T Load<T>(string filePath, bool autoSave = false)
        {

            if (File.Exists(filePath))
            {
                try
                {
                    //return JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
                    T values = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath)); //Works but calls constructor with Null FilePath need to understand what is going on.
                    //Iterate through all Properties of an Object programmatically - for Sub Custom Object Types
                    //PropertyInfo[] properties = typeof(T).GetProperties();
                    //foreach (PropertyInfo property in properties)
                    //{
                    //    if (property.CanWrite) // Found this for when property is JsonIgnore
                    //    {
                    //        property.SetValue(values, property.GetValue(values, null), null);
                    //    }
                    //    //PropertyInfo[] level2Properties = 

                    //}
                    (values as SettingsBase).FilePath = filePath;
                    return values;
                }
                catch (System.IO.IOException) //if file is being written to or is locked 
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    return (T)Activator.CreateInstance(typeof(T), new object[] { filePath }); //Create a new Default Object

                }

            }
            else
            {
                //Create the Directory if it doesn't exist and return a new Type of Object
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
               // return (T)Activator.CreateInstance(typeof(T), new object[] { filePath }); //Create a new Default Object
                T newObj = (T)Activator.CreateInstance(typeof(T), new object[] { filePath }); //Create a new Default Object
                //Automatically Save it if requested.
                if (autoSave && newObj is SettingsBase sb)
                {
                    sb.Save();
                }
                return newObj;
            }
        }

        /// <summary>
        /// Creates a shallow clone of the current settings object.
        /// </summary>
        public T Clone<T>() where T : SettingsBase, new()
        {
            var clone = new T();
            foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(clone, prop.GetValue(this));
                }
            }
            clone.FilePath = this.FilePath;
            return clone;
        }
        /// <summary>
        /// Returns a list of property names and new values that differ from another SettingsBase instance.
        /// </summary>
        /// <example > 
        /// // Clone the settings
        //      _originalServerSettings = _serverSettings.Clone<ServerSettings>();
        //  // When saving, get changed properties
        //      var changes = _serverSettings.GetChangedProperties(_originalServerSettings);
        //      foreach (var change in changes)
        //      {
        //          _log.Info($"Property changed: {change.Property} from '{change.OldValue}' to '{change.NewValue}'");
        //      }
        //      _originalServerSettings = _serverSettings.Clone<ServerSettings>();
        //      </example>
        public List<(string Property, object OldValue, object NewValue)> GetChangedProperties(SettingsBase other)
        {
            var changes = new List<(string, object, object)>();
            var type = this.GetType();
            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanRead)
                {
                    var oldValue = prop.GetValue(other);
                    var newValue = prop.GetValue(this);
                    if ((oldValue == null && newValue != null) ||
                        (oldValue != null && !oldValue.Equals(newValue)))
                    {
                        changes.Add((prop.Name, oldValue, newValue));
                    }
                }
            }
            return changes;
        }
    }
    /// <summary>
    /// Langeuage Type Converter for Property Grid
    /// </summary>
    public class UILanguageTypeConverter : StringConverter
    {
        /// <summary>
        /// GetStandardValuesSupported
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        /// <summary>
        /// GetStandardValuesExclusive
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // Only allow selection from the list
        }
        /// <summary>
        /// GetStandardValues - only English or Italian for now.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new[] { "en-US", "it-IT" });
        }
    }

       /// <summary>
    /// Class to help display Double numbers in the Property Grid reasonably.
    /// </summary>
    public class CustomDoubleTypeConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string s = (string)value;
                return double.Parse(s, NumberStyles.Any, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }
        /// <summary>
        /// ConvertTo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((double)value).ToString("N6", culture);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
