using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Hrms.Helper.StaticClasses
{
    public static class ExcelHelper
    {
        //        public static Microsoft.Office.Interop.Excel.Application excel;
        //        public static Microsoft.Office.Interop.Excel.Workbook excelworkBook;
        //        public static Microsoft.Office.Interop.Excel.Worksheet excelSheet;
        //        public static Microsoft.Office.Interop.Excel.Range excelCellrange;

        //        public static void GenerateExcel(string reportName, DataTable dataTable, string filePath)
        //        {
        //            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        //            {
        //                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        //            }

        //            excel = new Microsoft.Office.Interop.Excel.Application();
        //            excel.Visible = false;
        //            excel.DisplayAlerts = false;
        //            excelworkBook = excel.Workbooks.Add(Type.Missing);
        //            excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
        //            excelSheet.Name = reportName;

        //            var colCount = 1;
        //            foreach (DataColumn column in dataTable.Columns)
        //            {
        //                excelSheet.Cells[1, colCount] = column.ColumnName;
        //                colCount++;
        //            }

        //            var rowCount = 2;
        //            foreach (DataRow rows in dataTable.Rows)
        //            {
        //                colCount = 1;
        //                foreach (DataColumn column in dataTable.Columns)
        //                {
        //                    excelSheet.Cells[rowCount, colCount] = rows[0].ToString();
        //                    colCount++;
        //                }
        //            }

        //            excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1],
        //                excelSheet.Cells[dataTable.Rows.Count + 1, dataTable.Columns.Count]];
        //            excelCellrange.EntireColumn.AutoFit();
        //            Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;

        //            excelworkBook.SaveAs(filePath);
        //            //excel.Save();
        //            excelworkBook.Close(true);
        //            excel.Quit();

        //            Marshal.ReleaseComObject(excel);
        //        }
    }
}
