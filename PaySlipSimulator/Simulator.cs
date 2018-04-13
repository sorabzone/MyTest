using System;
using PaySlipEngine;
using PaySlipEngine.Common;
using PaySlipSimulator.Model;

namespace PaySlipSimulator
{
    public class Simulator
    {
        private PaySlipEngine.NSWPaySlipEngine _paySlip;

        public Simulator()
        {
            _paySlip = new PaySlipEngine.NSWPaySlipEngine();
        }

        /// <summary>
        /// This method reads commands from an array and sends to paySlip object.
        /// </summary>
        /// <param name="lines">array of paySlip commands</param>
        public void FeedCommands(string[] lines)
        {
            try
            {
                // Display the file contents by using a foreach loop.
                System.Console.WriteLine("Contents of text.txt = ");
                foreach (string line in lines)
                {
                    Console.WriteLine("command# : " + line);
                    ExecuteSingleCommand(line);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends incoming command to paySlip.
        /// </summary>
        /// <param name="userInput">command for paySlip</param>
        /// <returns></returns>
        public string ExecuteSingleCommand(string userInput)
        {
            try
            {
                string output = userInput;
                CommandArguments arguments = new CommandArguments();
                if (ParseCommand(userInput, arguments))
                {
                    switch (arguments.Instruction)
                    {
                        case Command.PLACE:
                            _paySlip.Place(arguments.X, arguments.Y, arguments.Face);
                            break;
                        case Command.MOVE:
                            _paySlip.Move();
                            break;
                        case Command.LEFT:
                            _paySlip.Left();
                            break;
                        case Command.RIGHT:
                            _paySlip.Right();
                            break;
                        case Command.REPORT:
                            output = _paySlip.Report();
                            Console.WriteLine(output);
                            break;
                        default:
                            break;
                    }
                }
                return output;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Parse the incoming command 
        /// </summary>
        /// <param name="command">command for paySlip</param>
        /// <param name="cmdArgs">returns command arguments</param>
        /// <returns></returns>
        private bool ParseCommand(string command, CommandArguments cmdArgs)
        {
            try
            {
                Command inputCommand;
                string paySlipCmd = string.Empty;
                string[] argDelimiter = command.Split(' ');

                //Check for valid command
                //Empty command or command with more than 2 string parts is invalid. It also checks spelling
                if (argDelimiter.Length > 0 && argDelimiter.Length < 3 && Enum.TryParse<Command>(argDelimiter[0], true, out inputCommand))
                {
                    cmdArgs.Instruction = inputCommand;
                }
                else
                    return false;

                //PLACE command without coordinates is invalid
                //non-PLACE command with extra characters/words is invalid
                if ((cmdArgs.Instruction.Equals(Command.PLACE) && argDelimiter.Length == 1) || (!cmdArgs.Instruction.Equals(Command.PLACE) && argDelimiter.Length == 2))
                {
                    return false;
                }
                else if (cmdArgs.Instruction.Equals(Command.PLACE) && argDelimiter.Length == 2)
                {
                    return ParseArguments(argDelimiter[1], cmdArgs);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Parse the arguments passed to PLACE command
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="cmdArgs"></param>
        /// <returns></returns>
        private bool ParseArguments(string arguments, CommandArguments cmdArgs)
        {
            try
            {
                var subArgs = arguments.Split(',');
                int x, y;
                Direction face;

                if (subArgs.Length == 3 &&
                    int.TryParse(subArgs[0], out x) &&
                    int.TryParse(subArgs[1], out y) &&
                    Enum.TryParse<Direction>(subArgs[2], true, out face))
                {
                    cmdArgs.X = x;
                    cmdArgs.Y = y;
                    cmdArgs.Face = face;
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
