using System;
using Ekoodi.Utilities.Test;

// Program for testing Task1 from Ekoodi.Utilities
namespace bban_validator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get user to input a bank number. Repeat if necessary.
            Tuple<bool, string> bankNumber = null;
            while (bankNumber == null)
            {
                bankNumber = Utilities.InputBankNumber();
            }
            // Change bank number into machine format and test for validity.
            bankNumber = Utilities.Validate(bankNumber);

            // Change from BBAN to IBAN
            bankNumber = Utilities.BBANtoIBAN(bankNumber);
            
            Console.ReadKey();
        }
    }
}
