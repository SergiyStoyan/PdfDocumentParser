//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;

namespace Cliver.Win
{
    /// <summary>
    /// A property of this type is implicitly encrypted when it is a member of a Settings class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Encrypted<T> : Cliver.Encrypted<T> where T : class
    {
        static Encrypted()
        {
            InitializeDefault(new Endec2String(new Endec.ProtectedData()));
        }

        public Encrypted(T value = null) : base(value) { }
    }

    /// <summary>
    /// (!)Deprecated. Exists for backward compatibility. Only intended for use in Settings.
    /// </summary>
    public class StringEndec : Cliver.StringEndec
    {
        protected StringEndec(Endec endec) : base(endec)
        {
        }

        public StringEndec() : base(new Endec.ProtectedData())
        {
        }
    }
}