//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
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
            InitializeDefault(new StringEndec.ProtectedData());
        }

        /// <summary>
        /// (!)The default constructor is used by the deserializer.
        /// </summary>
        public Encrypted() : base() { }

        public Encrypted(T value) : base(value) { }
    }
}