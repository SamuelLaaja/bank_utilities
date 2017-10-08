using System;
using Bank;
using ReferenceNumbers;
using Barcode;

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
            // Tuple boolean is used to determine if bank number is BBAN or IBAN. null if invalid number.
            Tuple<bool, string> bankNumberTuple = null;
            string refNumber = String.Empty;
            bool refIsInternational;
            bool refValid = false;

            try
            {
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
                        bankNumberTuple = new Tuple<bool, string>(true, International.ChangeToInternational(bankNumberTuple.Item2, "FI"));
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
                Console.WriteLine("Please write a reference number (national or international).");
                Console.WriteLine("Base part is 3-19 numbers and optionally end with one (1) validation number:");
                refNumber = Console.ReadLine();
                refIsInternational = International.IsInternational(refNumber);
                string amount = String.Empty;
                refNumber = RefNumbers.EnsureCorrectInput(refNumber, refIsInternational);
                Console.WriteLine(refIsInternational ? "International reference number detected." : "National reference number detected.");
                if (!refIsInternational)
                {
                    Console.WriteLine("How many valid reference numbers you want to generate, if any:");
                    amount = Console.ReadLine();
                }

                // If user is not generating any numbers (amount = 0 or null), check the validity of given number.
                if (!int.TryParse(amount, out int amountInt) || amountInt < 1)
                {
                    //Checks if given validation number is correct
                    Console.WriteLine("Checking reference number validity...");
                    refValid = RefNumbers.ValidifyReferenceNumber(refNumber, refIsInternational);
                    if (refValid)
                    {
                        Console.WriteLine(RefNumbers.WhiteSpaces(refNumber) + " - OK");
                        if (!refIsInternational)
                            Console.WriteLine("International version: " + RefNumbers.WhiteSpaces(International.ChangeToInternational(refNumber, "RF")));
                    }
                    else
                    {
                        Console.WriteLine("Reference number is incorrect!");
                    }
                }
                // Otherwise generate given amount of reference numbers (with validation numbers). Only national numbers can be generated.
                else
                {
                    string[] refGenerated = RefNumbers.Generate(refNumber, amountInt);
                    for (int i = 0; i < refGenerated.Length; i++)
                    {
                        // Add white spaces and print all generated numbers
                        // Also change into international format and print them out
                        Console.WriteLine(RefNumbers.WhiteSpaces(refGenerated[i]) + "\t  International version:  " + RefNumbers.WhiteSpaces(International.ChangeToInternational(refGenerated[i], "RF")));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Bank Barcode function calls. Uses previous inputs as bank number and reference number
            try
            {
                if (bankNumberTuple != null && refNumber != null && refValid)
                {
                    // Input invoice amount
                    Console.WriteLine("If you are making an invoice, please insert invoice amount in euros: ");
                    string invoice = Console.ReadLine();
                    invoice = BarCodes.InputInvoice(invoice);
                    // Input due date
                    if (invoice != String.Empty)
                    {
                        Console.WriteLine("Please insert due date for payment (dd mm yyyy): ");
                        string dueDate = Console.ReadLine();
                        dueDate = BarCodes.InputDate(dueDate);

                        Console.WriteLine("Invoice: {0}  Due date: {1}", invoice, dueDate);
                        // variables:
                        // bankNumberTuple <isInternational (bool), bankNumber (string)>
                        // refNumber (string)
                        // refIsInternational (bool)
                        // invoice (string)
                        // dueDate (string)
                    }                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}

