//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;

namespace Cliver.InvoiceParser
{
    class Excel : IDisposable
    {
        public Excel(string xls, string worksheetName)
        {
            package = new ExcelPackage(new FileInfo(xls));

            worksheet = package.Workbook.Worksheets.Where(x => x.Name == worksheetName).FirstOrDefault();
            if (worksheet == null)
                worksheet = package.Workbook.Worksheets.Add(worksheetName);
            worksheet.Cells.Style.Numberformat.Format = "@";
            //package.Workbook..ErrorCheckingOptions.BackgroundChecking = false;
        }
        ExcelPackage package;

        ~Excel()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (package != null)
                {
                    try
                    {
                        package.Dispose();
                    }
                    catch (Exception e)//unclear error here
                    {
                    }
                    package = null;
                }
            }
        }

        //public void OpenWorksheet(string name)
        //{
        //    worksheet = package.Workbook.Worksheets.Where(x => x.Name == name).FirstOrDefault();
        //    if (worksheet == null)
        //        worksheet = package.Workbook.Worksheets.Add(name);
        //}
        ExcelWorksheet worksheet;

        public void Save()
        {
            package.Save();
        }

        public int WriteLine(IEnumerable<object> values)
        {
            int i = 1;
            List<string> ss = new List<string>();
            foreach (object v in values)
            {
                string s;
                if (v is string)
                    s = (string)v;
                else if (v != null)
                    s = v.ToString();
                else
                    s = null;

                worksheet.Cells[j, i++].Value = s;
                //worksheet.Cells[j, i++].Style.Numberformat.Format = "@";
            }
            return j++;
        }
        int j = 1;

        public void SetLink(int j, int i, Uri uri)
        {
            worksheet.Cells[j, i].Hyperlink = uri;
        }
    }
}