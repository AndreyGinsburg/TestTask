using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace WebApplication31.Models
{
    public class Excel
    {
        public string path = "";
        public _Application excel = new _Excel.Application();
        public Workbook wb;
        public Worksheet ws;
        public Excel(string path,int sheets)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[1];
        }
    }
}