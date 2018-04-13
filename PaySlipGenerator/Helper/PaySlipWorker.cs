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
using System;

namespace PaySlipGenerator.Helper
{
    public static class PaySlipWorker
    {
        public static ExcelPackage GeneratePaySlipsExcel(ExcelPackage package, string state)
        {
            try
            {
                int idxLastName = 1, idxFirstName = 0, idxAnnsualSalary = 2, idxSuperRate = 3, idxPayPeriod = 4;

                //Input
                bool ifAllEmpty = true;
                ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

                //Output
                ExcelPackage excelExport = new ExcelPackage();
                var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
                workSheetOutput.TabColor = System.Drawing.Color.Black;
                workSheetOutput.DefaultRowHeight = 12;
                int recordIndex = 1;

                //Header of output excel  
                workSheetOutput.Row(recordIndex).Height = 20;
                workSheetOutput.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheetOutput.Row(recordIndex).Style.Font.Bold = true;

                workSheetOutput.Cells[recordIndex, 1].Value = OutputExcelColumn.Name;
                workSheetOutput.Cells[recordIndex, 2].Value = OutputExcelColumn.GrossIncome;
                workSheetOutput.Cells[recordIndex, 3].Value = OutputExcelColumn.IncomeTax;
                workSheetOutput.Cells[recordIndex, 4].Value = OutputExcelColumn.NetIncome;
                workSheetOutput.Cells[recordIndex, 5].Value = OutputExcelColumn.Super;
                workSheetOutput.Cells[recordIndex, 6].Value = OutputExcelColumn.PayPeriod;

                // This loop will accept input excel even if order of columns is different
                for (int iCol = 1; iCol <= workSheet.Dimension.End.Column; iCol++)
                {
                    var colName = Convert.ToString(((object[,])workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column].Value)[0, iCol - 1]);
                    switch (colName)
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

                // Thread-safe collection to handle input data processing
                BlockingCollection<EngineInput> bag = new BlockingCollection<EngineInput>();

                // Blocking Consumer task - reaing records from collection and processing
                Task t2 = Task.Factory.StartNew(() =>
                {
                    PaySlipEngineFactory factory = null;
                    switch (state)
                    {
                        case States.NSW:
                            factory = new NSWFactory();
                            break;
                        case States.Victoria:
                            factory = new VictoriaFactory();
                            break;
                    }

                    EngineOutput paySlipOutput = null;
                    BasePaySlipEngine payEngine = factory.GetPaySlipEngine();
                    EngineInput input = null;

                    // Take() was called on a completed collection.
                    // Some other thread can call CompleteAdding after we pass the
                    // IsCompleted check but before we call Take. 
                    // In this example, we can simply catch the exception since the 
                    // loop will break on the next iteration.
                    while (!bag.IsCompleted)
                    {
                        recordIndex++; input = null;
                        try
                        {
                            input = bag.Take();
                        }
                        catch (InvalidOperationException) { break;  }

                        if (input != null)
                        {
                            paySlipOutput = payEngine.GeneratePaySlip(input);

                            workSheetOutput.Cells[recordIndex, 1].Value = paySlipOutput.Name;
                            workSheetOutput.Cells[recordIndex, 2].Value = paySlipOutput.GrossIncome;
                            workSheetOutput.Cells[recordIndex, 3].Value = paySlipOutput.IncomeTax;
                            workSheetOutput.Cells[recordIndex, 4].Value = paySlipOutput.NetIncome;
                            workSheetOutput.Cells[recordIndex, 5].Value = paySlipOutput.Super;
                            workSheetOutput.Cells[recordIndex, 6].Value = paySlipOutput.PayPeriod;
                        }
                    }
                });

                // Blocking Producer task - reading records from excel sheet
                Task t1 = Task.Factory.StartNew(() =>
                {
                    EngineInput inputObj = null;
                    for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        ifAllEmpty = true;
                        decimal dVal = 0;
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        inputObj = new EngineInput();

                        for (int iCol = 1; iCol <= workSheet.Dimension.End.Column; iCol++)
                        {
                            var value = Convert.ToString(((object[,])row.Value)[0, iCol - 1]);

                            if (!string.IsNullOrEmpty(value))
                                ifAllEmpty = false;

                            if (iCol == idxFirstName)
                            {
                                inputObj.FirstName = value;
                            }
                            else if (iCol == idxLastName)
                            {
                                inputObj.LastName = value;
                            }
                            else if (iCol == idxAnnsualSalary)
                            {
                                inputObj.AnnualSalary = decimal.TryParse(value, out dVal) ? dVal : throw new System.InvalidCastException($"Annual Salary is not in correct format. Value: {value}");
                            }
                            else if (iCol == idxSuperRate)
                            {
                                inputObj.SuperRate = decimal.TryParse(value, out dVal) ? dVal : throw new System.InvalidCastException($"Super Rate is not in correct format. Value: {value}");
                            }
                            else if (iCol == idxPayPeriod)
                            {
                                inputObj.PayPeriod = value;
                            }
                        }

                        if (ifAllEmpty)
                            break;

                        bag.Add(inputObj);
                    }

                    // Let consumer know we are done.
                    bag.CompleteAdding();
                });
                
                // Wait till both producer and consumer finish their job
                Task main = Task.WhenAll(t1, t2);
                main.Wait();
                return excelExport;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}