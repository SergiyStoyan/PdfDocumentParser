//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Cliver.PdfDocumentParser
{
    public partial class TemplateForm
    {
        public abstract class TemplateManager
        {
            /// <summary>
            /// this object is a buffer and can be changed unpredictably, so it should not be referenced from outside
            /// </summary>
            public Template Template;
            abstract public void Save();
            public string LastTestFile;
            virtual public void HelpRequest()
            {
                try
                {
                    System.Diagnostics.Process.Start(Settings.Constants.HelpFile);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            }
            virtual public Template.Anchor CreateDefaultAnchor()
            {
                return new Template.Anchor.PdfText { };
            }
            virtual public Template.Condition CreateDefaultCondition()
            {
                return new Template.Condition { };
            }
            virtual public Template.Field CreateDefaultField()
            {
                return new Template.Field.PdfText { };
            }
        }
    }
}