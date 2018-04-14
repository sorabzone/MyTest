using PaySlipEngine.Model;
using System;

namespace PaySlipEngine.BaseEngine
{
    public abstract class BasePaySlipEngine
    {
        /// <summary>
        /// This method generates payslip of the employee
        /// </summary>
        /// <param name="">EngineInput object having details of employee salary</param>
        /// <returns>EngineOutput object having details of generated payslip</returns>
        public virtual EngineOutput GeneratePaySlip(EngineInput employeeSalaryDetail)
        {
            EngineOutput payslip = new EngineOutput();

            payslip.Name = employeeSalaryDetail.FirstName + " " + employeeSalaryDetail.LastName;
            payslip.PayPeriod = employeeSalaryDetail.PayPeriod;
            payslip.GrossIncome = CalculateGrossIncome(employeeSalaryDetail.AnnualSalary);
            payslip.IncomeTax = CalculateIncomeTax(employeeSalaryDetail.AnnualSalary);
            payslip.NetIncome = CalculateNetIncome(payslip.GrossIncome, payslip.IncomeTax);
            payslip.Super = CalculateSuper(payslip.GrossIncome, employeeSalaryDetail.SuperRate);

            return payslip;
        }

        /// <summary>
        /// This method calculates gross income
        /// </summary>
        /// <returns>gross income amount</returns>
        public virtual decimal CalculateGrossIncome(decimal annualSalary)
        {
            try
            {
                return Math.Round(annualSalary / 12);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method calculates income tax
        /// </summary>
        /// <returns>income tax amount</returns>
        public abstract decimal CalculateIncomeTax(decimal annualSalary);

        /// <summary>
        /// This methos calculates net income
        /// </summary>
        /// <returns>Net income amount</returns>
        public virtual decimal CalculateNetIncome(decimal grossIncome, decimal incomeTax)
        {
            try
            {
                return Math.Round(grossIncome - incomeTax);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// /// This method calculates super    
        /// </summary>
        /// <returns>super amount</returns>
        public virtual decimal CalculateSuper(decimal grossIncome, decimal rate)
        {
            try
            {
                return Math.Round(grossIncome * rate / 100);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    };
}
