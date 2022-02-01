using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RegularExpressionsAndMachines
{
    class Program
    {
        public const string INPUT_FILE = "../../../input.txt";
        public const string OUTPUT_FILE = "../../../output.txt";

        static void Main(string[] args)
        {
            List<string> strings = new List<string>();

            using (StreamReader inputStream = new StreamReader(INPUT_FILE))
            {
                String line;
                while ((line = inputStream.ReadLine()) != null)
                {
                    strings.Add(line);
                }
            }

            StreamWriter output = new StreamWriter(OUTPUT_FILE);

            switch (strings.First())
            {
                case ExpressionType.LEFT_GRAMMAR:
                    ExpressionConverter rightConverter = new LeftGrammarExpressionConverter();
                    strings.Remove(ExpressionType.LEFT_GRAMMAR);
                    Dictionary<string, Dictionary<string, string>> rightStates = rightConverter.ConvertExpressions(strings);
                    rightConverter.PrintMachineMinimizationFormat(rightStates, output);
                    break;
                case ExpressionType.RIGHT_GRAMMAR:
                    ExpressionConverter leftConverter = new RightGrammarExpressionConverter();
                    strings.Remove(ExpressionType.RIGHT_GRAMMAR);
                    Dictionary<string, Dictionary<string, string>> leftStates = leftConverter.ConvertExpressions(strings);
                    leftConverter.PrintMachineMinimizationFormat(leftStates, output);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid grammar type");
            }

            output.Close();
            
        }
    }
}
