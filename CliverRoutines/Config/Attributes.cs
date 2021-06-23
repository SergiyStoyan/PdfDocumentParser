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
    public class SettingsFieldAttribute
    {
        /// <summary>
        /// It makes the Settings field not be initiated by Config by default.
        /// Such a field, when needed, must be initiated explicitly by Config.Reload(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class OptionalAttribute : Attribute
        {
        }

        /// <summary>
        /// It makes the Settings field be serialized without indention.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class NotIndentedAttribute : Attribute
        {
        }

        /// <summary>
        /// It makes those serializable fields/properties of the Settings field whose values are NULL, be serialized.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class SerializeNullValuesAttribute : Attribute
        {
        }

        //!!!it never must be used as brings to losing changes
        //[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        //public class IgnoreDefaultValuesAttribute : Attribute
        //{
        //}

        /// <summary>
        /// It provides the Settings field with encryption facility.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class CryptoAttribute : System.Attribute
        {
            /// <summary>
            /// Eencrypt/decrypt engine.
            /// </summary>
            readonly public StringCrypto Crypto;

            /// <summary>
            /// Settings field attribute that is used for encrypting.
            /// </summary>
            /// <param name="stringCryptoGetterHostingType">Class that exposes the StringCrypto getter.</param>
            /// <param name="stringCryptoGetterName">Name of the StringCrypto getter. The getter must be public static.</param>
            public CryptoAttribute(Type stringCryptoGetterHostingType, string stringCryptoGetterName)
            {
                try
                {
                    if (stringCryptoGetterHostingType == null)
                        throw new Exception("stringCryptoGetterHostingType cannot be NULL.");
                    if (string.IsNullOrWhiteSpace(stringCryptoGetterName))
                        throw new Exception("stringCryptoGetterName cannot be empty.");
                    System.Reflection.PropertyInfo pi = stringCryptoGetterHostingType.GetProperty(stringCryptoGetterName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (pi == null)
                        throw new Exception(stringCryptoGetterHostingType.FullName + " class does not expose property '" + stringCryptoGetterName + "'");
                    Crypto = (StringCrypto)pi.GetValue(null);
                }
                catch (Exception e)
                {
                    throw new Exception("Wrong parameters of the attribute " + GetType().FullName, e);
                }
            }
        }
    }

    public class SettingsTypeAttribute
    {
        /// <summary>
        /// It checks if the storage file format is supported.
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
}