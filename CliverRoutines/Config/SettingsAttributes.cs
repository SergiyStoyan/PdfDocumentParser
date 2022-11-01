//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;

namespace Cliver
{
    public class SettingsAttributes
    {
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
        public class ConfigAttribute : Attribute
        {
            /// <summary>
            /// Whether a Settings field/type be initiated by Config implicitly. Used to save the loading time/memory.
            /// An optional field, when needed, must be initiated explicitly by Config.Reload(string settingsFieldFullName)
            /// </summary>
            public bool Optional = false;

            /// <summary>
            /// Whether a Settings field/type be serialized with indention. Used to make the storage file smaller.
            /// </summary>
            public bool Indented = true;

            /// <summary>
            /// Whether the serializable fields/properties of a Settings field/type whose values are NULL, be serialized.
            /// </summary>
            public bool NullSerialized = false;

            //!!!it never must be used as it leads to losing changes
            //public bool IgnoreDefaultValues;
        }

        /// <summary>
        /// Check if the type version of the data stored in the storage file is supported.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public class TypeVersionAttribute : Attribute
        {
            /// <summary>
            /// The version of the Settings type to which this attribute is applied.
            /// </summary>
            public readonly uint Value;

            /// <summary>
            /// Settings type attribute. Used to check if the storage file format is supported.
            /// </summary>
            /// <param name="value">Version of the Settings type to which this attribute is applied.</param>
            public TypeVersionAttribute(uint value)
            {
                Value = value;
            }

            ///// <summary>
            ///// Settings type attribute. Used to check if the storage file format is supported.
            ///// </summary>
            ///// <param name="value">Version of the Settings type to which this attribute is applied.</param>
            //public TypeVersionAttribute(Version value)
            //{
            //    Value = value;
            //}

            //public TypeVersionAttribute(int major, int minor, int build, int revision = 0)
            //{
            //    Value = new Version(major, minor, build, revision);
            //}

            //public TypeVersionAttribute(string version)
            //{
            //    Value = new Version(version);
            //}
        }

        /// <summary>
        /// Provides a Settings field or a Settings type with encryption facility.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
        public class EncryptedAttribute : System.Attribute
        {
            /// <summary>
            /// Encryption/decryption engine.
            /// </summary>
            readonly public Endec2String Endec;

            /// <summary>
            /// Settings field attribute that is used for encrypting.
            /// </summary>
            /// <param name="endecGetterHostingType">Class that exposes the Endec2String getter.</param>
            /// <param name="endecGetterName">Name of the Endec2String getter. The getter must be static.</param>
            public EncryptedAttribute(Type endecGetterHostingType, string endecGetterName)
            {
                try
                {
                    if (endecGetterHostingType == null)
                        throw new Exception("endecGetterHostingType cannot be NULL.");
                    if (string.IsNullOrWhiteSpace(endecGetterName))
                        throw new Exception("endecGetterName cannot be empty.");
                    System.Reflection.PropertyInfo pi = endecGetterHostingType.GetProperty(endecGetterName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (pi != null)
                    {
                        //if (!pi.PropertyType.IsSubclassOf(typeof(Endec2String)))//!!!does not work
                        if (!typeof(Endec2String).IsAssignableFrom(pi.PropertyType))
                            throw new Exception("Type of the property " + endecGetterHostingType.FullName + "." + endecGetterName + " is not " + typeof(Endec2String).FullName);
                        Endec = pi.GetValue(null) as Endec2String;
                    }
                    else
                    {
                        System.Reflection.FieldInfo fi = endecGetterHostingType.GetField(endecGetterName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        if (fi == null)
                            throw new Exception(endecGetterHostingType.FullName + " class does not expose the property/field '" + endecGetterName + "'");
                        //if (!fi.FieldType.IsSubclassOf(typeof(Endec2String)))//!!!does not work
                        if (!typeof(Endec2String).IsAssignableFrom(fi.FieldType))
                            throw new Exception("Type of the field " + endecGetterHostingType.FullName + "." + endecGetterName + " is not " + typeof(Endec2String).FullName);
                        Endec = fi.GetValue(null) as Endec2String;
                    }
                    if (Endec == null)
                        throw new Exception("Property " + endecGetterHostingType.FullName + "." + endecGetterName + " is NULL.");
                }
                catch (Exception e)
                {
                    throw new Exception("Error in the attribute " + GetType().FullName, e);
                }
            }
        }
    }
}