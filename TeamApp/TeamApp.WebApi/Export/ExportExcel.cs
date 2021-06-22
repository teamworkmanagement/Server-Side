﻿using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamApp.WebApi.Export
{
    public class ExportExcel
    {
        public static async Task<byte[]> GenerateExcelFile(List<Student> students)
        {
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("TestExportfdsdg");
            // create title
            workSheet.Cells["A1:D1"].Merge = true;
            workSheet.Cells["A1"].Value = "Students Export";
            workSheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A1"].Style.Font.Bold = true;
            // fill header
            List<string> listHeader = new List<string>()
            {
                "A2","B2","C2","D2",
            };
            listHeader.ForEach(c =>
            {
                workSheet.Cells[c].Style.Font.Bold = true;
                workSheet.Cells[c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            });

            workSheet.Cells[listHeader[0]].Value = "STT";
            workSheet.Cells[listHeader[1]].Value = "Id";
            workSheet.Cells[listHeader[2]].Value = "Name";
            workSheet.Cells[listHeader[3]].Value = "Age";

            //fill data
            for (int i = 0; i < students.Count; i++)
            {
                workSheet.Cells[i + 3, 1].Value = (i + 1).ToString();
                workSheet.Cells[i + 3, 2].Value = students[i].Id;
                workSheet.Cells[i + 3, 3].Value = students[i].Name;
                workSheet.Cells[i + 3, 4].Value = students[i].Age;
            }
            // format column width
            for (int i = 1; i < 5; i++)
            {
                switch (i)
                {
                    case 2:
                        workSheet.Column(i).Width = 60;
                        break;
                    case 3:
                        workSheet.Column(i).Width = 50;
                        break;
                    default:
                        workSheet.Column(i).Width = 20;
                        break;
                }
            }

            // format cell border
            for (int i = 0; i < students.Count; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    workSheet.Cells[i + 3, j].Style.Font.Size = 10;
                    workSheet.Cells[i + 3, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i + 3, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
