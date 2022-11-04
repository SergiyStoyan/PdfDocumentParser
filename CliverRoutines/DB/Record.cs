//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

//################################
// 
// (!!!) When using a certain database type include the respective implementation files into the project and add the required reference.
//
//################################
using System;
using System.Data;
using System.Collections.Generic;

namespace Cliver.Db
{
    public class Record : Dictionary<string, object>
    {
        public new object this[string name]
        {
            get
            {
                object o;
                if (this.TryGetValue(name, out o))
                    return o;
                return null;
            }
            set
            {
                this.Add(name, value);
            }
        }
    }
}