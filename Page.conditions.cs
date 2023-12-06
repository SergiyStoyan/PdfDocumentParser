//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public partial class Page
    {
        public bool IsCondition(string conditionName, bool exceptionIfNotDefined = true)
        {
            if (string.IsNullOrWhiteSpace(conditionName))
                throw new Exception("Condition name is not specified.");
            var c = PageCollection.ActiveTemplate.Conditions.FirstOrDefault(a => a.Name == conditionName);
            if (c == null)
            {
                if (exceptionIfNotDefined)
                    throw new Exception("Condition '" + conditionName + "' does not exist.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(c.Value))
            {
                if (exceptionIfNotDefined)
                    throw new Exception("Condition '" + conditionName + "' is not set.");
                return false;
            }
            return BooleanEngine.Parse(c.Value, this);
        }

        /// <summary>
        /// To be used when condition is optional.
        /// </summary>
        /// <param name="conditionName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool? IsCondition2(string conditionName)
        {
            if (string.IsNullOrWhiteSpace(conditionName))
                throw new Exception("Condition name is not specified.");
            var c = PageCollection.ActiveTemplate.Conditions.FirstOrDefault(a => a.Name == conditionName);
            if (c == null)
                return null;
            if (string.IsNullOrWhiteSpace(c.Value))
                return null;
            return BooleanEngine.Parse(c.Value, this);
        }
    }
}