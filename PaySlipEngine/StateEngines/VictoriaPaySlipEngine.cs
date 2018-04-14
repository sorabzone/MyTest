using PaySlipEngine.BaseEngine;
using System;

namespace PaySlipEngine.StateEngines
{
    /// <summary>
    /// This class calculates income tax for NSW state 
    /// </summary>
    public class VictoriaPaySlipEngine : BasePaySlipEngine
    {
        /// <summary>
        /// This method calculates income tax
        /// </summary>
        /// <returns>income tax amount</returns>
        public override decimal CalculateIncomeTax(decimal annualSalary)
        {
            decimal incomeTax= 0;

            if (annualSalary <= 18200)
            {
                incomeTax = 0;
            }
            else if (annualSalary > 18200 && annualSalary <= 37000)
            {
                incomeTax = Math.Round(((annualSalary - 18200) * 15 / 100)/12);
            }
            else if (annualSalary > 37000 && annualSalary <= 87000)
            {
                incomeTax = Math.Round((3572 + ((annualSalary - 37000) * 20 / 100)) / 12);
            }
            else if (annualSalary > 87000 && annualSalary <= 180000)
            {
                incomeTax = Math.Round((19822 + ((annualSalary - 87000) * 25 / 100)) / 12);
            }
            else if (annualSalary > 180000)
            {
                incomeTax = Math.Round((54232 + ((annualSalary - 180000) * 30 / 100)) / 12);
            }

            return incomeTax;
        }
    }
}
