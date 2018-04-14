using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using PaySlipEngine.BaseEngine;
using PaySlipEngine.Constant;
using PaySlipEngine.Model;
using PaySlipFactory.StateFactories;
using PaySlipGenerator.Helper;
using System;
using System.Linq;

namespace PaySlipTest
{
    [TestClass]
    public class UnitTestPaySlip
    {
        /// <summary>
        /// Test data provided in MYOB test
        /// </summary>
        [TestMethod]
        public void PaySlip_NSWMYOB1()
        {
            var factory = new NSWFactory();
            BasePaySlipEngine payEngine = factory.GetPaySlipEngine();
            var input = new EngineInput()
            {
                FirstName = "David",
                LastName = "Rudd",
                AnnualSalary = 60050,
                SuperRate = 9,
                PayPeriod = "01 March - 31 March"
            };

            var paySlipOutput = payEngine.GeneratePaySlip(input);

            Assert.IsTrue(paySlipOutput.Name.Equals("David Rudd")
                && paySlipOutput.GrossIncome == 5004
                && paySlipOutput.IncomeTax == 922
                && paySlipOutput.NetIncome == 4082
                && paySlipOutput.Super == 450);
        }

        /// <summary>
        /// Test data provided in MYOB test
        /// </summary>
        [TestMethod]
        public void PaySlip_NSW_MYOB2()
        {
            var factory = new NSWFactory();
            BasePaySlipEngine payEngine = factory.GetPaySlipEngine();
            var input = new EngineInput()
            {
                FirstName = "Ryan",
                LastName = "Chen",
                AnnualSalary = 120000,
                SuperRate = 10,
                PayPeriod = "01 March - 31 March"
            };

            var paySlipOutput = payEngine.GeneratePaySlip(input);

            Assert.IsTrue(paySlipOutput.Name.Equals("Ryan Chen")
                && paySlipOutput.GrossIncome == 10000
                && paySlipOutput.IncomeTax == 2669
                && paySlipOutput.NetIncome == 7331
                && paySlipOutput.Super == 1000);
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// </summary>
        [TestMethod]
        public void PaySlip_NSW_MYOB1_Excel()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = "David";
            workSheetOutput.Cells[2, 2].Value = "Rudd";
            workSheetOutput.Cells[2, 3].Value = 60050;
            workSheetOutput.Cells[2, 4].Value = 9;
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };

            Assert.IsTrue(paySlipOutput.Name.Equals("David Rudd")
                && paySlipOutput.GrossIncome == 5004
                && paySlipOutput.IncomeTax == 922
                && paySlipOutput.NetIncome == 4082
                && paySlipOutput.Super == 450);
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// </summary>
        [TestMethod]
        public void PaySlip_NSW_MYOB2_Excel()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = "Ryan";
            workSheetOutput.Cells[2, 2].Value = "Chen";
            workSheetOutput.Cells[2, 3].Value = 120000;
            workSheetOutput.Cells[2, 4].Value = 10;
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };

            Assert.IsTrue(paySlipOutput.Name.Equals("Ryan Chen")
                && paySlipOutput.GrossIncome == 10000
                && paySlipOutput.IncomeTax == 2669
                && paySlipOutput.NetIncome == 7331
                && paySlipOutput.Super == 1000);
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// super greater than 50 throws error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void PaySlip_NSW_MYOB1_Excel_InvalidSuper()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = "David";
            workSheetOutput.Cells[2, 2].Value = "Rudd";
            workSheetOutput.Cells[2, 3].Value = 60050;
            workSheetOutput.Cells[2, 4].Value = 51;
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// 0 salary throws error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void PaySlip_NSW_MYOB1_Excel_InvalidSalary()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = "David";
            workSheetOutput.Cells[2, 2].Value = "Rudd";
            workSheetOutput.Cells[2, 3].Value = 0;
            workSheetOutput.Cells[2, 4].Value = 9;
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// super less than 0 throws error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void PaySlip_NSW_MYOB1_Excel_NegativeSuper()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = "David";
            workSheetOutput.Cells[2, 2].Value = "Rudd";
            workSheetOutput.Cells[2, 3].Value = 60050;
            workSheetOutput.Cells[2, 4].Value = -1;
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// non decimal super rate throws error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void PaySlip_NSW_MYOB1_Excel_SuperNaN()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = "David";
            workSheetOutput.Cells[2, 2].Value = "Rudd";
            workSheetOutput.Cells[2, 3].Value = 60050;
            workSheetOutput.Cells[2, 4].Value = "9%";
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// non decimal salary throws error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void PaySlip_NSW_MYOB1_Excel_SalaryNaN()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = "David";
            workSheetOutput.Cells[2, 2].Value = "Rudd";
            workSheetOutput.Cells[2, 3].Value = "Sixty Thousand";
            workSheetOutput.Cells[2, 4].Value = 9;
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };
        }

        /// <summary>
        /// Test data provided in MYOB test as excel input
        /// Blank places are trimmed while reading from excel
        /// </summary>
        [TestMethod]
        public void PaySlip_NSW_MYOB1_Excel_BlankSpace()
        {
            ExcelPackage excelExport = new ExcelPackage();
            var workSheetOutput = excelExport.Workbook.Worksheets.Add("PaySlips");
            workSheetOutput.Cells[1, 1].Value = InputExcelColumn.FirstName;
            workSheetOutput.Cells[1, 2].Value = InputExcelColumn.LastName;
            workSheetOutput.Cells[1, 3].Value = InputExcelColumn.AnnualSalary;
            workSheetOutput.Cells[1, 4].Value = InputExcelColumn.SuperRate;
            workSheetOutput.Cells[1, 5].Value = InputExcelColumn.PayPeriod;

            workSheetOutput.Cells[2, 1].Value = " David ";
            workSheetOutput.Cells[2, 2].Value = " Rudd ";
            workSheetOutput.Cells[2, 3].Value = 60050;
            workSheetOutput.Cells[2, 4].Value = 9;
            workSheetOutput.Cells[2, 5].Value = "01 March - 31 March";

            var output = PaySlipWorker.GeneratePaySlipsExcel(excelExport, States.NSW);

            ExcelWorksheet workSheet = output.Workbook.Worksheets.First();
            var maxColumnCount = workSheet.Dimension.End.Column;
            var row = workSheet.Cells[2, 1, 2, maxColumnCount];

            var paySlipOutput = new EngineOutput()
            {
                Name = Convert.ToString(((object[,])row.Value)[0, 0]),
                GrossIncome = Convert.ToDecimal(((object[,])row.Value)[0, 1]),
                IncomeTax = Convert.ToDecimal(((object[,])row.Value)[0, 2]),
                NetIncome = Convert.ToDecimal(((object[,])row.Value)[0, 3]),
                Super = Convert.ToDecimal(((object[,])row.Value)[0, 4]),
                PayPeriod = Convert.ToString(((object[,])row.Value)[0, 5])
            };

            Assert.IsTrue(paySlipOutput.Name.Equals("David Rudd")
                && paySlipOutput.GrossIncome == 5004
                && paySlipOutput.IncomeTax == 922
                && paySlipOutput.NetIncome == 4082
                && paySlipOutput.Super == 450);
        }
    }
}
