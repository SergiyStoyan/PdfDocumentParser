//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
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
                    ProcessRoutines.Open(Settings.Constants.HelpFile);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    Message.Error(ex, TemplateForm);
                }
            }

            internal TemplateForm TemplateForm;

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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="template"></param>
            /// <param name="testFileDefaultFolder"></param>
            public TemplateManager(Template template0, string lastTestFile, string testFileDefaultFolder)
            {
                Template0 = template0;
                Template = template0.CreateCloneByJson();

                if (Template.Editor == null)
                    Template.Editor = new Template.EditorSettings { };

                Template = Template;
                LastTestFile = lastTestFile;
                TestFileDefaultFolder = testFileDefaultFolder;
            }
            /// <summary>
            /// The original template that is never altered.
            /// </summary>
            public readonly Template Template0;
            /// <summary>
            /// It is the resulting template. (!)It might be changed in unpredictable manner during editing so it must be used only if Save()
            /// </summary>
            public Template Template { get; internal set; }
            public string LastTestFile { get; internal set; }
            internal string TestFileDefaultFolder;
        }
    }
}