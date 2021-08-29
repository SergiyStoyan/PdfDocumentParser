//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Cliver.PdfDocumentParser;

namespace Cliver.SampleParser
{
    public partial class Settings
    {
        public static readonly Template2sSettings3 Template2s;
    }

    [SettingsAttributes.Config(Indented = false)]
    [SettingsAttributes.TypeVersion(210825)]
    public class Template2sSettings3 : Cliver.UserSettings
    {
        public List<Template2> Template2s = new List<Template2>();

        protected override void UnsupportedFormatHandler(Exception deserializingException)
        {
            try
            {
                //if (deserializingException != null)
                //    throw deserializingException;
                if (deserializingException?.Message.Contains("Could not create an instance of type Cliver.PdfDocumentParser.Template+Field.") == true || __TypeVersion < 210701)
                {
                    Newtonsoft.Json.Linq.JObject o = __Info.ReadStorageFileAsJObject();
                    for (int i = o["Template2s"].Count() - 1; i >= 0; i--)
                        for (int j = o["Template2s"][i]["Template"]["Fields"].Count() - 1; j >= 0; j--)
                        {
                            string s;
                            switch ((int)o["Template2s"][i]["Template"]["Fields"][j]["DefaultValueType"])
                            {
                                case 0:
                                    s = "Cliver.PdfDocumentParser.Template+Field+PdfText, PdfDocumentParser";
                                    break;
                                case 1:
                                    s = "Cliver.PdfDocumentParser.Template+Field+PdfTextLines, PdfDocumentParser";
                                    break;
                                case 2:
                                    s = "Cliver.PdfDocumentParser.Template+Field+PdfCharBoxs, PdfDocumentParser";
                                    break;
                                case 3:
                                    s = "Cliver.PdfDocumentParser.Template+Field+OcrText, PdfDocumentParser";
                                    break;
                                case 4:
                                    s = "Cliver.PdfDocumentParser.Template+Field+OcrTextLines, PdfDocumentParser";
                                    break;
                                case 5:
                                    s = "Cliver.PdfDocumentParser.Template+Field+OcrCharBoxs, PdfDocumentParser";
                                    break;
                                case 6:
                                    s = "Cliver.PdfDocumentParser.Template+Field+Image, PdfDocumentParser";
                                    break;
                                case 7:
                                    s = "Cliver.PdfDocumentParser.Template+Field+OcrTextLineImages, PdfDocumentParser";
                                    break;
                                default:
                                    throw new Exception("Unknown option: " + (int)o["Template2s"][i]["Template"]["Fields"][j]["DefaultValueType"]);
                            }
                            o["Template2s"][i]["Template"]["Fields"][j].First().AddBeforeSelf(new Newtonsoft.Json.Linq.JProperty("$type", s));
                        }
                    o["__TypeVersion"] = 210701;
                    __Info.WriteStorageFileAsJObject(o, false);
                    Reload();
                    return;
                }
                if (__TypeVersion < 210720)
                {
                    string s = __Info.ReadStorageFileAsString();
                    s = Regex.Replace(s, Regex.Escape("Cliver.HawkeyeQuoteParser.DocumentParsers"), "Cliver.HawkeyeQuoteParser.Pdf.DocumentParsers", RegexOptions.Singleline);
                    __Info.UpdateTypeVersionInStorageFileString(210720, ref s);
                    __Info.WriteStorageFileAsString(s);
                    Reload();
                    return;
                }
                if (__TypeVersion < 210820)//added new fields
                {
                    string s = __Info.ReadStorageFileAsString();
                    __Info.UpdateTypeVersionInStorageFileString(210820, ref s);
                    __Info.WriteStorageFileAsString(s);
                    Reload();
                    return;
                }
                if (__TypeVersion < 210824)//migration of fields
                {
                    if (!Message.YesNo("There is no complete conversion from the type version " + __TypeVersion + ". Some data may be lost. Would you like to convert to the current format?", null, Message.Icons.Exclamation))
                        throw new Exception("There is no default conversion from the given type version.");
                    Save();
                    Reload();
                    return;
                }
                if (__TypeVersion < 210825)//renamed fields
                {
                    Newtonsoft.Json.Linq.JObject o = __Info.ReadStorageFileAsJObject();
                    for (int i = o["Template2s"].Count() - 1; i >= 0; i--)
                        for (int j = o["Template2s"][i]["Template"]["Fields"].Count() - 1; j >= 0; j--)
                        {
                            o["Template2s"][i]["Template"]["Fields"][j].First().AddAfterSelf(new Newtonsoft.Json.Linq.JProperty("Ocr", o["Template2s"][i]["Template"]["Fields"][j]["OcrMode"]));
                        }
                    o["__TypeVersion"] = 210825;
                    __Info.WriteStorageFileAsJObject(o, false);
                    Reload();
                    return;
                }

                throw new Exception("Unsupported version of " + GetType().FullName + ": " + __TypeVersion + ". Accepted version: " + __Info.TypeVersion, deserializingException);
            }
            catch (Exception e)
            {
                Log.Error(e);
                string m = "Error while loading " + __Info.File
                    + "\r\nThe application will exit.\r\n\r\n(To run the application, remove the file or, to preserve it, move it to a safe location. Upon restart, the application will reset the file's data to their default state.)";
                Message.Error2(m, e);
                Log.Exit2(m);
            }
        }

        protected override void Loaded()
        {
        }

        protected override void Saving()
        {
            Template2s.RemoveAll(x => string.IsNullOrWhiteSpace(x.Template.Name));
        }

        //public void SaveIfTouched()
        //{
        //    if (!touched)
        //        return;
        //    Save();
        //}

        public void Touch()
        {
            touched = true;
            TouchedChanged?.BeginInvoke(null, null);
        }
        bool touched = false;
        public bool IsTouched()
        {
            return touched;
        }
        public delegate void OnTouchedChanged();
        public event OnTouchedChanged TouchedChanged;

        protected override void Saved()
        {
            touched = false;
            TouchedChanged?.BeginInvoke(null, null);
        }

        public Template2 CreateInitialTemplate()
        {
            return new Template2
            {
                FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                Template = new Template
                {
                    Name = "",
                    PageRotation = PdfDocumentParser.Template.PageRotations.NONE,
                    Deskew = null,
                    Anchors = new List<Template.Anchor>(),
                    Conditions = new List<Template.Condition> {
                            new Template.Condition { Name = Template2.ConditionNames.DocumentFirstPage },
                            new Template.Condition { Name = Template2.ConditionNames.DocumentLastPage }
                        },
                    Fields = new List<Template.Field>
                        {
                            new Template.Field.PdfText {  Name = Template2.FieldNames.InvoiceId , Rectangle=new Template.RectangleF(0,0,10,10)},
                            new Template.Field.PdfText {  Name = Template2.FieldNames.TotalAmount, Rectangle=new Template.RectangleF(0,0,10,10)},
                            new Template.Field.PdfText {  Name = Template2.FieldNames.ProductTable, Rectangle=new Template.RectangleF(0,0,10,10)},
                            new Template.Field.PdfTextLines { Name = Template2.FieldNames.ProductNames, Rectangle=new Template.RectangleF(0,0,10,10), ColumnOfTable=Template2.FieldNames.ProductTable},
                            new Template.Field.PdfTextLines { Name = Template2.FieldNames.ProductCosts, Rectangle=new Template.RectangleF(0,0,10,10), ColumnOfTable=Template2.FieldNames.ProductTable},
                        },
                    Editor = new Template.EditorSettings
                    {
                        TestPictureScale = 1.2m,
                        CheckConditionsAutomaticallyWhenPageChanged = true,
                        ExtractFieldsAutomaticallyWhenPageChanged = true,
                    },
                },
            };
        }
    }
}