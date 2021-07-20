//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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
            /// Encrypt/decrypt engine.
            /// </summary>
            readonly public StringEndec Endec;

            /// <summary>
            /// Settings field attribute that is used for encrypting.
            /// </summary>
            /// <param name="stringEndecGetterHostingType">Class that exposes the StringEndec getter.</param>
            /// <param name="stringEndecGetterName">Name of the StringEndec getter. The getter must be static.</param>
            public EncryptedAttribute(Type stringEndecGetterHostingType, string stringEndecGetterName)
            {
                try
                {
                    if (stringEndecGetterHostingType == null)
                        throw new Exception("stringEndecGetterHostingType cannot be NULL.");
                    if (string.IsNullOrWhiteSpace(stringEndecGetterName))
                        throw new Exception("stringEndecGetterName cannot be empty.");
                    System.Reflection.PropertyInfo pi = stringEndecGetterHostingType.GetProperty(stringEndecGetterName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (pi == null)
                        throw new Exception(stringEndecGetterHostingType.FullName + " class does not expose property '" + stringEndecGetterName + "'");
                    Endec = (StringEndec)pi.GetValue(null);
                }
                catch (Exception e)
                {
                    throw new Exception("Wrong parameters of the attribute " + GetType().FullName, e);
                }
            }
        }
    }
}