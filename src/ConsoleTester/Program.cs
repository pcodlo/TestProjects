using System;
using System.Globalization;

namespace ConsoleTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string doubleString = "7E-05";

            Console.WriteLine($"Converting {doubleString} to double with double.parse: {double.Parse(doubleString, new CultureInfo("no")):N10}");
            Console.WriteLine($"Converting {doubleString} to double with Convert.ToDouble: {Convert.ToDouble(doubleString):N10}");
        }
    }
}