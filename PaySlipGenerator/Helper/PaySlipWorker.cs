using OfficeOpenXml;
using OfficeOpenXml.Style;
using PaySlipEngine.BaseEngine;
using PaySlipEngine.Constant;
using PaySlipEngine.Model;
using PaySlipFactory;
using PaySlipFactory.StateFactories;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace PaySlipGenerator.Helper
{
    public static class PaySlipWorker
    {
        /// <summary>
        /// 1. This method takes excel as input and state code.
        /// 2. The PaySlip factory creates the State specific payslip generator/engine depending on the State selected by user in UI
        /// 3. Producer/Consumer multithread design pattern is used. 
        ///    Producer reads single record at a time and add records in thread-safe collection.
        ///    Consumer reads records from above collection and process them and adds to the excel worksheet object
        /// 4. Input excel can contain columns in any order.
        /// 5. Order/Format of columns in output excel sheet is fixed
        /// </summary>
        /// <param name="package"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static ExcelPackage GeneratePaySlipsExcel(ExcelPackage package, string state)
        {
            int idxFirstName = 0, idxLastName = 1, idxAnnsualSalary = 2, idxSuperRate = 3, idxPayPeriod = 4;

            //Input
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

            var maxColumnCount = workSheet.Dimension.End.Column;

            // This loop will accept input excel even if order of columns is different
            for (int iCol = 1; iCol <= maxColumnCount; iCol++)
            {
                var colName = Convert.ToString(((object[,])workSheet.Cells[1, 1, 1, maxColumnCount].Value)[0, iCol - 1]);
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
            };

            // Thread-safe collection to handle input data processing
            BlockingCollection<EngineInput> bag = new BlockingCollection<EngineInput>();

            // Blocking Consumer task - reaing records from collection and processing
            Task consumer = Task.Factory.StartNew(() =>
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
                    default:
                        throw new InvalidOperationException("Unknown State.");
                }

                EngineOutput paySlipOutput = null;
                BasePaySlipEngine payEngine = factory.GetPaySlipEngine();

                foreach (var input in bag.GetConsumingEnumerable())
                {
                    recordIndex++;
                    paySlipOutput = payEngine.GeneratePaySlip(input);

                    workSheetOutput.Cells[recordIndex, 1].Value = paySlipOutput.Name;
                    workSheetOutput.Cells[recordIndex, 2].Value = paySlipOutput.GrossIncome;
                    workSheetOutput.Cells[recordIndex, 3].Value = paySlipOutput.IncomeTax;
                    workSheetOutput.Cells[recordIndex, 4].Value = paySlipOutput.NetIncome;
                    workSheetOutput.Cells[recordIndex, 5].Value = paySlipOutput.Super;
                    workSheetOutput.Cells[recordIndex, 6].Value = paySlipOutput.PayPeriod;
                };
            });

            // Blocking Producer task - reading records from excel sheet
            Task producer = Task.Factory.StartNew(() =>
            {
                // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
                var exceptions = new ConcurrentQueue<Exception>();
                Parallel.For(2, workSheet.Dimension.End.Row + 1, (rowNumber, loopState) =>
                {
                    try
                    {
                        var ifAllEmpty = true;

                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        var inputObj = new EngineInput();

                        for (int iCol = 1; iCol <= workSheet.Dimension.End.Column; iCol++)
                        {
                            decimal dVal = 0;
                            var value = Convert.ToString(((object[,])row.Value)[0, iCol - 1]);

                            if (!string.IsNullOrEmpty(value))
                                ifAllEmpty = false;

                            if (iCol == idxFirstName)
                            {
                                inputObj.FirstName = value.Trim();
                            }
                            else if (iCol == idxLastName)
                            {
                                inputObj.LastName = value.Trim();
                            }
                            else if (iCol == idxAnnsualSalary)
                            {
                                inputObj.AnnualSalary = decimal.TryParse(value.Trim(), out dVal) ? dVal > 0 ? dVal : throw new System.InvalidCastException($"Annual Salary must be greater than 0. Value: {value}") : throw new System.InvalidCastException($"Annual Salary is not in correct format. Value: {value}");
                            }
                            else if (iCol == idxSuperRate)
                            {
                                inputObj.SuperRate = decimal.TryParse(value.Trim(), out dVal) ? (dVal >= 0 && dVal <= 50) ? dVal : throw new System.InvalidCastException($"Super rate should be in 0-50% range. Value: {value}") : throw new System.InvalidCastException($"Super Rate is not in correct format. Value: {value}");
                            }
                            else if (iCol == idxPayPeriod)
                            {
                                inputObj.PayPeriod = value.Trim();
                            }
                        }

                        if (ifAllEmpty)
                            loopState.Stop();

                        bag.Add(inputObj);
                    }
                    // Store the exception and continue with the loop.                    
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                        loopState.Stop();
                    }
                });

                // Throw the exceptions here after the loop completes.
                if (exceptions.Count > 0) throw new AggregateException(exceptions);
                // Let consumer know we are done.
                bag.CompleteAdding();
            });

            // Wait till both producer and consumer finish their job
            producer.Wait();
            consumer.Wait();

            return excelExport;
        }
    }
}