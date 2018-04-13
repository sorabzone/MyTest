using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLogger;
using NLog;

namespace PaySlipSimulator
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Pay Slip Simulator");
                Console.WriteLine("-------------------");
                Console.WriteLine("");
                Console.WriteLine("Press 1 to read commands from test.txt file or press any other key to enter manual commands. And press Enter");
                Console.WriteLine("");
                string command = Console.ReadLine();

                Simulator payslipSimulator = new Simulator();

                //If user selects "1" then simulator will commands from test.txt file in ToyPaySlip\ToyPaySlipSimulator\bin\Debug\TestData\ folder
                //Else user enter individual commands in console.
                if (command.Equals("1"))
                {
                    string[] lines = System.IO.File.ReadAllLines(@"TestData\test.txt");

                    payslipSimulator.FeedCommands(lines);

                    command = Console.ReadLine();
                }
                else
                {
                    System.Console.WriteLine("Please enter commands. ");
                    while (true)
                    {
                        Console.Write("command# : ");
                        command = Console.ReadLine();

                        //User can type 'EXIT' and press enter to exit program
                        if (command.ToUpper() == "EXIT")
                        {
                            Environment.Exit(0);
                        }

                        payslipSimulator.ExecuteSingleCommand(command);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonLogger.CommonLogger.LogError(ex);
                Console.ReadLine();
            }
        }
    }
}
