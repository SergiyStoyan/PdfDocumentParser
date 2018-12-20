//using System;
//using System.Collections.Generic;
//using System.Text;
//using Excel = Microsoft.Office.Interop.Excel;

//namespace Cliver
//{
//    /// <summary>
//    /// Safe interface to work with Microsoft.Office.Interop.Excel
//    /// providing correct cleanup of Excel processes
//    /// </summary>
//    public class ExcelWorkbook : IDisposable
//    {
//        const uint MAX_NUMBER_OF_OPEN_CELLS = 4;

//        public ExcelWorkbook(string file)
//        {
//            excel = new Microsoft.Office.Interop.Excel.Application();
//            if (excel == null) 
//                throw new Exception("Can't start Excel");
//            excel.DisplayAlerts = false;
//            workbooks = excel.Workbooks; 
//            workbook = workbooks.Open(file,
//                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
//                );
//            if (workbook == null)
//                throw new Exception("Can't open workbook '" + file + "'");
//            worksheets = workbook.Worksheets;
//        }

//        Excel.Application excel;
//        Excel.Workbooks workbooks;
//        Excel.Workbook workbook;
//        Excel.Sheets worksheets;
//        Excel.Worksheet worksheet;
//        //Excel.Range cell;

//        Dictionary<string, Excel.Range> cells = new Dictionary<string, Microsoft.Office.Interop.Excel.Range>();
//        List<string> cell_keys = new List<string>();

//        Excel.Range get_cell(string row, string column)
//        {
//            string key = get_cell_key(CurrentWorksheet, row, column);
//            if (!cells.ContainsKey(key))
//            {
//                if (cells.Count > MAX_NUMBER_OF_OPEN_CELLS)
//                    close_oldest_cell();
//                cells[key] = (Excel.Range)worksheet.Cells[row, column];
//                cell_keys.Insert(0, key);
//            }
//            else
//            {
//                int i = cell_keys.IndexOf(key);
//                if (i > 0)
//                {
//                    cell_keys.RemoveAt(i);
//                    cell_keys.Insert(0, key);
//                }
//            }
//            return cells[key];
//        }

//        string get_cell_key(string worksheet, string row, string column)
//        {
//            return worksheet + ";" + row + ";" + column;
//        }

//        void close_oldest_cell()
//        {
//            int c = cell_keys.Count;
//            if (c < 1)
//                return;
//            string key = cell_keys[c - 1];
//            cell_keys.RemoveAt(c - 1);
//            while (System.Runtime.InteropServices.Marshal.ReleaseComObject(cells[key]) > 0) ;
//            cells.Remove(key);
//        }
        
//        public void Dispose()
//        {
//            if (excel == null)
//                return;

//            if (workbook != null)
//            {
//                workbook.Close(Type.Missing, Type.Missing, Type.Missing);
//            }

//            excel.Quit();

//            for (int i = cell_keys.Count; i > 0; i--)
//                close_oldest_cell(); 

//            //if (cell != null)
//            //    while (System.Runtime.InteropServices.Marshal.ReleaseComObject(cell) > 0) ;
//            //cell = null;

//            if (worksheet != null)
//                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet) > 0) ;
//            worksheet = null;

//            if (worksheets != null)
//                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheets) > 0) ;
//            worksheets = null;

//            if (workbook != null)
//                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook) > 0) ;
//            workbook = null;

//            if (workbooks != null)
//                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks) > 0) ;
//            workbooks = null;

//            if (excel != null)
//                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(excel) > 0) ;
//            excel = null;

//            //GC.Collect();
//        }

//        ~ExcelWorkbook()
//        {
//            Dispose();
//        }

//        public void Save()
//        {
//            if (workbook != null)
//                workbook.Save();
//        }

//        public string[] Worksheets
//        {
//            get
//            {
//                List<string> wss = new List<string>();
//                foreach (Excel.Worksheet ws in worksheets)
//                    wss.Add(ws.Name);
//                return wss.ToArray();
//            }
//        }

//        public string CurrentWorksheet
//        {
//            get
//            {
//                return worksheet.Name;
//            }
//            set
//            {
//                if (worksheet != null)
//                    while (System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet) > 0) ;
//                worksheet = (Excel.Worksheet)worksheets[value];
//                if (worksheet == null)
//                    throw new Exception("Can't open worksheet '" + value + "'");
//            }
//        }

//        //Excel.Range get_cell(string row, string column)
//        //{
//        //    if (cell != null)
//        //        while (System.Runtime.InteropServices.Marshal.ReleaseComObject(cell) > 0) ;
//        //    cell = (Excel.Range)worksheet.Cells[row, column];
//        //    return cell;
//        //}

//        public object GetCellValue(object row, object column)
//        {
//            return get_cell(row.ToString(), column.ToString()).Value2;
//        }

//        public string GetCellText(object row, object column)
//        {
//            return get_cell(row.ToString(), column.ToString()).Text.ToString();
//        }

//        public void SetCellText(object row, object column, string text)
//        {
//            get_cell(row.ToString(), column.ToString()).Value2 = text;
//        }
//    }
//}
