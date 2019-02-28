//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public partial class Page : IDisposable
    {
        public bool IsCondition(string conditionName)
        {
            if (string.IsNullOrWhiteSpace(conditionName))
                throw new Exception("Condition name is not specified.");
            var c = pageCollection.ActiveTemplate.Conditions.FirstOrDefault(a => a.Name == conditionName);
            if (string.IsNullOrWhiteSpace(c.Value))
                throw new Exception("Condition '" + conditionName + "' is not set.");
            return BooleanEngine.Parse(c.Value, this);
        }
    }
}