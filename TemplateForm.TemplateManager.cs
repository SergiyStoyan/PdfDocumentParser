//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;

namespace Cliver.PdfDocumentParser
{
    public partial class TemplateForm
    {
        /// <summary>
        /// Defines custom routines while editing a template.
        /// </summary>
        public abstract class TemplateManager
        {
            abstract public void Save();
            
            virtual public void HelpRequest()
            {
                try
                {
                    System.Diagnostics.Process.Start(Settings.Constants.HelpFile);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    Message.Error(ex);
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
                return new Template.Field { DefaultValueType = Template.Field.ValueTypes.PdfText };
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="template">!!!Attention: it can be changed unpredictably, so it should not be a reference to an outside object that will be used later.</param>
            /// <param name="testFileDefaultFolder"></param>
            public TemplateManager(Template template, string lastTestFile, string testFileDefaultFolder)
            {
                Template = template;
                LastTestFile = lastTestFile;
                TestFileDefaultFolder = testFileDefaultFolder;
            }
            public Template Template { get; internal set; }
            public string LastTestFile { get; internal set; }
            internal string TestFileDefaultFolder;
        }
    }
}