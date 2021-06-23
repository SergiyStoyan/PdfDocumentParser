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
    /// <summary>
    /// Atribute which can be applied to a Settings field.
    /// </summary>
    public class SettingsFieldAttribute
    {
        /// <summary>
        /// Whether the Settings field be initiated by Config implicitly.
        /// An optional field, when needed, must be initiated explicitly by Config.Reload(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class OptionalAttribute : Attribute
        {
            public readonly bool Value;

            public OptionalAttribute(bool value = false)
            {
                Value = value;
            }
        }

        /// <summary>
        /// Whether the Settings field to be serialized with indention.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class IndentedAttribute : Attribute
        {
            public readonly bool Value;

            public IndentedAttribute(bool value = true)
            {
                Value = value;
            }
        }

        /// <summary>
        /// Whether those serializable fields/properties of the Settings field whose values are NULL, be serialized.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class NullSerializedAttribute : Attribute
        {
            public readonly bool Value;

            public NullSerializedAttribute(bool value = false)
            {
                Value = value;
            }
        }

        //!!!it never must be used as brings to losing changes
        //[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        //public class IgnoreDefaultValuesAttribute : Attribute
        //{
        //}
    }


    /// <summary>
    /// Atribute which can be applied to a Settings type.
    /// </summary>
    public class SettingsTypeAttribute
    {
        /// <summary>
        /// Check if the type version of the data stored in the storage file is supported.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public class TypeVersionAttribute : Attribute
        {
            public readonly uint MinSupportedTypeVersion;

            /// <summary>
            /// The version of the Settings type to which this attribute is applied.
            /// </summary>
            public readonly uint Value;

            public bool IsTypeVersionSupported(Settings settings)
            {
                return settings.__TypeVersion >= MinSupportedTypeVersion && settings.__TypeVersion <= Value;
            }

            /// <summary>
            /// Settings type attribute. Used to check if the storage file format is supported.
            /// </summary>
            /// <param name="value">Version of the Settings type to which this attribute is applied.</param>
            /// <param name="minSupportedTypeVersion">It must be less than or equal to the value.</param>
            public TypeVersionAttribute(uint value, uint minSupportedTypeVersion)
            {
                try
                {
                    if (value < minSupportedTypeVersion)
                        throw new Exception("Value (" + value + ") cannot be less than minSupportedTypeVersion (" + minSupportedTypeVersion + ")");
                    Value = value;
                    MinSupportedTypeVersion = minSupportedTypeVersion;
                }
                catch (Exception e)
                {
                    throw new Exception("Wrong parameters of the attribute " + GetType().FullName, e);
                }
            }

            /// <summary>
            /// Settings type attribute. Used to check if the storage file format is supported.
            /// </summary>
            /// <param name="value">Version of the Settings type to which this attribute is applied.</param>
            public TypeVersionAttribute(uint value) : this(value, value) { }
        }
    }

    /// <summary>
    /// Atribute which can be applied either to a Settings field or a Settings type.
    /// </summary>
    public class SettingsAttribute
    {
        /// <summary>
        /// Provides the Settings field or the Settings type with encryption facility.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
        public class EncryptedAttribute : System.Attribute
        {
            /// <summary>
            /// Eencrypt/decrypt engine.
            /// </summary>
            readonly public StringEndec Endec;

            /// <summary>
            /// Settings field attribute that is used for encrypting.
            /// </summary>
            /// <param name="stringEndecGetterHostingType">Class that exposes the StringEndec getter.</param>
            /// <param name="stringEndecGetterName">Name of the StringEndec getter. The getter must be public static.</param>
            public EncryptedAttribute(Type stringEndecGetterHostingType, string stringEndecGetterName)
            {
                try
                {
                    if (stringEndecGetterHostingType == null)
                        throw new Exception("stringEndecGetterHostingType cannot be NULL.");
                    if (string.IsNullOrWhiteSpace(stringEndecGetterName))
                        throw new Exception("stringEndecGetterName cannot be empty.");
                    System.Reflection.PropertyInfo pi = stringEndecGetterHostingType.GetProperty(stringEndecGetterName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
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