using OfficeOpenXml;
using System.Data;
using System.Linq;
using PaySlipEngine.Constant;
using System.Collections.Concurrent;
using PaySlipEngine.Model;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml.Style;
using PaySlipFactory;
using PaySlipFactory.StateFactories;
using PaySlipEngine.BaseEngine;

namespace PaySlipGenerator
{
    public static class ExcelPackageExtensions
    {
        public static ExcelPackage ToDataTable(this ExcelPackage package)
        {
            int idxLastName = 1, idxFirstName = 0, idxAnnsualSalary = 2, idxSuperRate = 3, idxPayPeriod = 4;

            //Input
            bool ifAllEmpty = true;
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            //Output
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("Transation");
            workSheetOutput.TabColor = System.Drawing.Color.Black;
            workSheetOutput.DefaultRowHeight = 12;
            //Header of table  
            workSheetOutput.Row(1).Height = 20;
            workSheetOutput.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetOutput.Row(1).Style.Font.Bold = true;

            workSheetOutput.Cells[1, 1].Value = "Name";
            workSheetOutput.Cells[1, 2].Value = "GrossIncome";
            workSheetOutput.Cells[1, 3].Value = "IncomeTax";
            workSheetOutput.Cells[1, 4].Value = "NetIncome";
            workSheetOutput.Cells[1, 5].Value = "Super";
            workSheetOutput.Cells[1, 6].Value = "PayPeriod";

            for (int iCol = 0; iCol < workSheet.Dimension.End.Column; iCol++)
            {
                switch (workSheet.Cells[1, 1, 1, iCol].Text)
                {
                    case InputExcelColumn.FirstName:
                        idxFirstName = iCol;
                        break;
                    case InputExcelColumn.LastName:
                        idxLastName = iCol;
                        break;
                    case InputExcelColumn.AnnualSalary:
                        idxAnnsualSalary = iCol;
                        break;
                    case InputExcelColumn.SuperRate:
                        idxSuperRate = iCol;
                        break;
                    case InputExcelColumn.PayPeriod:
                        idxPayPeriod = iCol;
                        break;
                }
            }

            ConcurrentBag<EngineInput> bag = new ConcurrentBag<EngineInput>();

            Task t1 = Task.Factory.StartNew(() =>
            {
                EngineInput inputObj = null;
                for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                {
                    ifAllEmpty = true;
                    decimal dVal = 0;
                    var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                    inputObj = new EngineInput();

                    for (int iCol = 0; iCol < workSheet.Dimension.End.Column; iCol++)
                    {
                        if (!string.IsNullOrEmpty(row[rowNumber, 1, rowNumber, iCol].Text))
                            ifAllEmpty = false;

                        if (iCol == idxFirstName)
                        {
                            inputObj.FirstName = row[rowNumber, 1, rowNumber, iCol].Text;
                        }
                        else if (iCol == idxLastName)
                        {
                            inputObj.LastName = row[rowNumber, 1, rowNumber, iCol].Text;
                        }
                        else if (iCol == idxAnnsualSalary)
                        {
                            inputObj.AnnualSalary = decimal.TryParse(row[rowNumber, 1, rowNumber, iCol].Text, out dVal) ? dVal : throw new System.InvalidCastException($"Annual Salary is not in correct format. Value: {row[rowNumber, 1, rowNumber, iCol].Text}");
                        }
                        else if (iCol == idxSuperRate)
                        {
                            inputObj.SuperRate = decimal.TryParse(row[rowNumber, 1, rowNumber, iCol].Text, out dVal) ? dVal : throw new System.InvalidCastException($"Super Rate is not in correct format. Value: {row[rowNumber, 1, rowNumber, iCol].Text}");
                        }
                        else if (iCol == idxPayPeriod)
                        {
                            inputObj.PayPeriod = row[rowNumber, 1, rowNumber, iCol].Text;
                        }
                    }

                    if (ifAllEmpty)
                        break;

                    bag.Add(inputObj);
                }
            });

            int recordIndex = 1;
            Task t2 = Task.Factory.StartNew(() =>
            {
                string state = "NSW";
                PaySlipEngineFactory factory = null;
                switch(state)
                {
                    case "NSW":
                        factory = new NSWFactory();
                        break;
                    case "Victoria":
                        factory = new VictoriaFactory();
                        break;
                }

                EngineOutput paySlipOutput = null;
                BasePaySlipEngine payEngine = factory.GetPaySlipEngine();

                foreach (EngineInput input in bag)
                {
                    paySlipOutput = payEngine.GeneratePaySlip(input);

                    workSheetOutput.Cells[recordIndex, 1].Value = paySlipOutput.Name;
                    workSheetOutput.Cells[recordIndex, 2].Value = paySlipOutput.GrossIncome;
                    workSheetOutput.Cells[recordIndex, 3].Value = paySlipOutput.IncomeTax;
                    workSheetOutput.Cells[recordIndex, 4].Value = paySlipOutput.NetIncome;
                    workSheetOutput.Cells[recordIndex, 5].Value = paySlipOutput.Super;
                    workSheetOutput.Cells[recordIndex, 6].Value = paySlipOutput.PayPeriod;
                    recordIndex++;
                }
                
            });
                return excelExport;
        }
    }
}