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
        /// Settings field attribute that indicates that the Settings field should not be initiated by Config by default.
        /// Such a field should be initiated explicitly when needed by Config.Reload(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class OptionalAttribute : Attribute
        {
        }

        /// <summary>
        /// Settings field attribute that set storage features for the Settings field.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class StorageAttribute : Attribute
        {
            /// <summary>
            /// Indicates whether the Settings field will be stored with indention or not.
            /// /// </summary>
            public bool Indented = true;
            /// <summary>
            /// Indicates whether null values in the Settings field are to be stored explicitly or not.
            /// </summary>
            public bool IgnoreNullValues = true;
            ///// <summary>!!!it never must be used as brings to losing changes
            ///// Indicates whether default values in the Settings field are to be stored explicitly or not.
            ///// </summary>
            //public bool IgnoreDefaultValues = false;

            /// <summary>
            /// Settings field attribute that set storage features for the Settings field.
            /// </summary>
            /// <param name="indented">Indicates that the Settings field be stored with indention</param>
            public StorageAttribute(bool indented = true, bool ignoreNullValues = true/*, bool ignoreDefaultValues = true*/)
            {
                Indented = indented;
                IgnoreNullValues = ignoreNullValues;
                //IgnoreDefaultValues = ignoreDefaultValues;
            }
        }

        /// <summary>
        /// Settings field attribute that is used for encrypting.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class CryptoAttribute : System.Attribute
        {
            /// <summary>
            /// Optional encrypt/decrypt facility for the Settings field.
            /// </summary>
            readonly public IStringCrypto Crypto;

            /// <summary>
            /// Settings field attribute that is used for encrypting.
            /// </summary>
            /// <param name="iStringCryptoGetterHostingType">Class that exposes the IStringCrypto getter.</param>
            /// <param name="iStringCryptoGetterName">Name of the IStringCrypto getter. The getter must be public static.</param>
            public CryptoAttribute(Type iStringCryptoGetterHostingType, string iStringCryptoGetterName)
            {
                System.Reflection.PropertyInfo pi = iStringCryptoGetterHostingType.GetProperty(iStringCryptoGetterName);
                if (pi == null)
                    throw new Exception(iStringCryptoGetterHostingType.FullName + " does not expose property " + iStringCryptoGetterName + "\r\nMake sure that " + GetType().Name + " is set properly.");
                Crypto = (IStringCrypto)pi.GetValue(null);
            }
        }
    }

    public class SettingsTypeAttribute
    {
        /// <summary>
        /// Settings type attribute. Used to check if the storage file format is supported.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public class TypeVersionAttribute : System.Attribute
        {
            public readonly uint MinSupportedTypeVersion;
            public readonly uint Value;

            public bool IsTypeVersionSupported(Settings settings)
            {
                return settings.__TypeVersion >= MinSupportedTypeVersion && settings.__TypeVersion <= Value;
            }

            /// <summary>
            /// Settings type attribute. Used to check if the storage file format is supported.
            /// </summary>
            /// <param name="value">Version of the Settings type to which this attribute is applied</param>
            /// <param name="minSupportedTypeVersion">Can be less or equal to the value.</param>
            public TypeVersionAttribute(uint value, uint minSupportedTypeVersion)
            {
                if (value < minSupportedTypeVersion)
                    throw new Exception("Value (" + value + ") cannot be less than minSupportedTypeVersion (" + minSupportedTypeVersion + ")");
                Value = value;
                MinSupportedTypeVersion = minSupportedTypeVersion;
            }

            /// <summary>
            /// Settings type attribute. Used to check if the storage file format is supported.
            /// </summary>
            /// <param name="value">Version of the Settings type to which this attribute is applied</param>
            public TypeVersionAttribute(uint value) : this(value, value) { }
        }
    }
}