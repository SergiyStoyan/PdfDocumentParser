//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Cliver
{
    public static class XmlRoutines
    {
        public static string Move2Element(this XmlTextReader xtr, string element_name, bool ignore_case = false)
        {
            try
            {
                while (xtr.Read())
                {
                    if (xtr.NodeType == XmlNodeType.Element && xtr.Name == element_name)
                        return xtr.ReadString();

                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static string Move2Attribute(this XmlTextReader xtr, string attribute_name, bool ignore_case = false)
        {
            //try
            //{
                xtr.MoveToFirstAttribute();
                do
                {
                    if (string.Compare(xtr.Name, attribute_name, ignore_case) == 0)
                        return xtr.Value;
                }
                while (xtr.MoveToNextAttribute());
            //}
            //catch (Exception e)
            //{
            //    LogMessage.Error(e);
            //}
            return null;
        }

        public static Dictionary<string, string> GetAttributeName2AttributeValues(this XmlTextReader xtr, Regex attribute_name_filter)
        {
            //try
            //{
                Dictionary<string, string> n2vs = new Dictionary<string, string>();
                xtr.MoveToFirstAttribute();
                do
                {
                    if (attribute_name_filter.IsMatch(xtr.Name))
                        n2vs[xtr.Name] = xtr.Value;
                }
                while (xtr.MoveToNextAttribute());
                return n2vs;
            //}
            //catch (Exception e)
            //{
            //    LogMessage.Error(e);
            //}
            //return null;
        }

        public static void WriteElement(this XmlTextWriter xtw, string element_name, params string[] attribute_value_pairs)
        {
            xtw.WriteStartElement(element_name);
            for (int i = 0; i < attribute_value_pairs.Length; i += 2)
                xtw.WriteAttributeString(attribute_value_pairs[i], attribute_value_pairs[i + 1]);
            xtw.WriteEndElement();
            xtw.Flush();
        }
    }
}
