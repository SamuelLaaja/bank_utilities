using System;
using Ekoodi.Utilities.Test;

// Program for testing Task1 from Ekoodi.Utilities
namespace bban_validator
{
    class Program
    {
        static void Main(string[] args)
        {
            string userBankNumber = Console.ReadLine();
            // Test for bank number validity. Returns a boolean.
            Utilities.BBAN_Validate(userBankNumber);
            Console.ReadKey();
        }
    }
}
