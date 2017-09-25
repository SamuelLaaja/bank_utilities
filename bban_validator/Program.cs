using System;
using Bank;
using ReferenceNumbers;

// Example bank numbers
// FI4250001510000023
// 472010225440
// 123456785
// 159030776

namespace Bank_Program
{
    // Program for testing Ekoodi.Utilities class methods
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Tuple boolean is used to determine if bank number is BBAN or IBAN. null if invalid number.
                Tuple<bool, string> bankNumberTuple = null;
                // Get user to input a bank number.
                Console.WriteLine("Please write a bank account number (BBAN or IBAN) 8-14 numbers:");
                bankNumberTuple = Input.InputBankNumber(Console.ReadLine());
                

                bankNumberTuple = MachineFormat.MachineReadable(bankNumberTuple);
                Console.WriteLine("Machine readable version: " + bankNumberTuple.Item2);

                // Change bank number into machine format and test for validity.
                Console.WriteLine("Testing for BBAN/IBAN validity...");
                bankNumberTuple = Validation.Validate(bankNumberTuple);

                if (bankNumberTuple == null)
                {
                    Console.WriteLine("Bank number is NOT valid.");
                }
                else
                {
                    String bankFormat = bankNumberTuple.Item1 ? "IBAN" : "BBAN";
                    Console.WriteLine("{0} is valid {1}.", bankNumberTuple.Item2, bankFormat);

                    if (!bankNumberTuple.Item1)
                    {
                        Console.WriteLine("Changing BBAN number to IBAN format: ");
                        bankNumberTuple = BBANtoIBAN.ChangeBBANtoIBAN(bankNumberTuple);
                        Console.WriteLine(bankNumberTuple.Item2);
                    }

                    Console.WriteLine("Trying to find matching BIC code:");
                    Console.WriteLine(BIC.BIC_Define(bankNumberTuple));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            // Reference number method calls
            try
            {
                Tuple<bool, string> referenceNumberTuple = null;
                Console.WriteLine("Please write a reference number (national) 4-20 numbers:");
                referenceNumberTuple = ReferenceNumbers.National.InputReferenceNumber(Console.ReadLine());

                if (referenceNumberTuple == null)
                {
                    Console.WriteLine("Reference number is NOT valid.");
                }
                else
                {
                    Console.WriteLine(referenceNumberTuple.Item2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}

