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
            //Should fix Console ä ö å problems
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            International.ReplaceLetters(Console.ReadLine());
            try
            {
                // Tuple boolean is used to determine if bank number is BBAN or IBAN. null if invalid number.
                Tuple<bool, string> bankNumberTuple = null;
                // Get user to input a bank number.
                Console.WriteLine("Please write a bank account number (BBAN or IBAN) 8-14 numbers:");
                bankNumberTuple = Input.InputBankNumber(Console.ReadLine());
                
                // Change bank number into machine format
                bankNumberTuple = MachineFormat.MachineReadable(bankNumberTuple);
                Console.WriteLine("Machine readable version: " + bankNumberTuple.Item2);
                // And test for validity.
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
                    
                    // If bank number is in BBAN format, change into IBAN
                    if (!bankNumberTuple.Item1)
                    {
                        Console.WriteLine("Changing BBAN number to IBAN format: ");
                        bankNumberTuple = BBANtoIBAN.ChangeBBANtoIBAN(bankNumberTuple);
                        Console.WriteLine(bankNumberTuple.Item2);
                    }

                    // Find BIC code from IBAN
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
                Console.WriteLine("Please write a (national) reference number's base part (3-19 numbers) and optionally end with a validation number:");
                string refNumberBase = Console.ReadLine();
                bool isInternational = International.IsInternational(refNumberBase);
                refNumberBase = National.EnsureCorrectInput(refNumberBase, isInternational);

                Console.WriteLine("How many reference numbers you want to generate, if any:");
                string amount = Console.ReadLine();
                int amountInt;
                
                // If user is not generating any numbers, check the validity of given number.
                if (!int.TryParse(amount, out amountInt) || amountInt < 1)
                {
                    //Checks if the given last number matches with program-generated last number (whitespaces included)
                    string refValid = National.ValidifyReferenceNumber(refNumberBase.Substring(0, refNumberBase.Length-1));
                    if (refValid.Equals(refNumberBase))
                        Console.WriteLine(National.WhiteSpaces(refValid) + " - OK");
                    else
                    {
                        Console.WriteLine("Reference number is incorrect!");
                    }
                }
                // Otherwise generate given amount of reference numbers
                else
                {
                    string[] refGenerated = National.Generate(refNumberBase, amountInt);
                    for (int i = 0; i < refGenerated.Length; i++)
                    {                        
                        // Add white spaces and print all generated numbers
                        Console.WriteLine(National.WhiteSpaces(refGenerated[i]));
                    }
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

