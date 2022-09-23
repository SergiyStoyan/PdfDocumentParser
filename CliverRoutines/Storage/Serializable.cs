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
    public class Serializable
    {
        static public T __Load<T>(string file) where T : Serializable, new()
        {
            T t = (T)get(typeof(T), file, InitMode.LOAD);
            t.__Loaded();
            return t;
        }

        static public T __LoadOrCreate<T>(string file) where T : Serializable, new()
        {
            T t = (T)get(typeof(T), file, InitMode.LOAD_OR_CREATE);
            t.__Loaded();
            return t;
        }

        static public T __Create<T>(string file) where T : Serializable, new()
        {
            T t = (T)get(typeof(T), file, InitMode.CREATE);
            t.__Loaded();
            return t;
        }

        //static public Serializable __Load(Type serializableType, string file)
        //{
        //    Serializable t = get(serializableType, file, InitMode.LOAD);
        //    t.__Loaded();
        //    return t;
        //}

        //static public Serializable __LoadOrCreate(Type serializableType, string file)
        //{
        //    Serializable t = get(serializableType, file, InitMode.LOAD_OR_CREATE);
        //    t.__Loaded();
        //    return t;
        //}

        //static public Serializable __Create(Type serializableType, string file)
        //{
        //    Serializable t = get(serializableType, file, InitMode.CREATE);
        //    t.__Loaded();
        //    return t;
        //}

        static Serializable get(Type serializableType, string file, InitMode initMode)
        {
            if (!Path.IsPathRooted(file))
                file = Log.AppCompanyCommonDataDir + System.IO.Path.DirectorySeparatorChar + file;
            Serializable s;
            if (initMode == InitMode.CREATE || (initMode == InitMode.LOAD_OR_CREATE && !File.Exists(file)))
                s = (Serializable)Activator.CreateInstance(serializableType);
            else
                s = (Serializable)Cliver.Serialization.Json.Load(serializableType, file);
            s.__File = file;
            return s;
        }

        enum InitMode
        {
            LOAD,
            LOAD_OR_CREATE,
            CREATE
        }

        //[ScriptIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string __File { get; private set; }

        public void __Save(string file = null)
        {
            lock (this)
            {
                if (file != null)
                    __File = file;
                __Saving();
                Cliver.Serialization.Json.Save(__File, this, __Indented, true);
                __Saved();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public bool __Indented = true;

        virtual public void __Loaded()
        {

        }

        virtual public void __Saving()
        {

        }

        virtual public void __Saved()
        {

        }
    }
}