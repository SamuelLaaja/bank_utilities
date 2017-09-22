using System;

namespace Ekoodi.Utilities.Test
{
    public class Utilities
    {

        // Get user to input a bank number. Define if it is BBAN or IBAN and clean up spaces and special characters.
        public static Tuple<bool, string> InputBankNumber()
        {
            Console.WriteLine("Please write a bank account number (BBAN or IBAN + 8-14 numbers):");
            string bankNumber = Console.ReadLine();
            bankNumber.ToUpper();
            bool isIBAN = false;


            // Check if user string's first 4 characters are [letter,letter,number,number]. If so, it is IBAN. Else it is BBAN.
            if (Char.IsLetter(bankNumber[0]) && Char.IsLetter(bankNumber[1]) && Char.IsNumber(bankNumber[2]) && Char.IsNumber(bankNumber[3]))
            {
                isIBAN = true;
            }

            string bankNumberCleanedUp = String.Empty;

            //Allow letters in international bank numbers
            if (isIBAN)
            {
                for (int i = 0; i < bankNumber.Length; i++)
                {
                    // Remove spaces and punctuation marks from user string.
                    if (Char.IsLetterOrDigit(bankNumber[i]))
                    {
                        bankNumberCleanedUp += bankNumber[i];
                    }
                }
            }
            // Remove letters from basic bank numbers
            else
            {
                for (int i = 0; i < bankNumber.Length; i++)
                {
                    // Remove letters, spaces and punctuation marks from user string.
                    if (Char.IsNumber(bankNumber[i]))
                    {
                        bankNumberCleanedUp += bankNumber[i];
                    }
                }
            }

            return new Tuple<bool, string>(isIBAN, bankNumberCleanedUp);
        }



        // Change a bank number into machine readable format
        private static Tuple<bool, string> Machine(Tuple<bool, string> bankNumberTuple)
        {
            // Skips IBAN numbers
            if (bankNumberTuple == null || bankNumberTuple.Item1)
            {
                return bankNumberTuple;
            }
            else
            {
                if (bankNumberTuple.Item2.Length > 14)
                {
                    Console.WriteLine("Bank number is too long!");
                    return null;
                }
                else if (bankNumberTuple.Item2.Length < 8)
                {
                    Console.WriteLine("Bank number is too short!");
                    return null;
                }


                // Check which bank's standards to use
                int initialNumbers = 6;

                if (bankNumberTuple.Item2[0] == '4' || bankNumberTuple.Item2[0] == '5')
                    initialNumbers = 7;

                string zeros = String.Empty;
                for (int i = initialNumbers; i < initialNumbers + (14 - bankNumberTuple.Item2.Length); i++)
                    zeros += "0";

                bankNumberTuple = new Tuple<bool, string>(bankNumberTuple.Item1, bankNumberTuple.Item2.Insert(initialNumbers, zeros));

                // Output machine version of bank number
                Console.WriteLine("Machine readable version: " + bankNumberTuple.Item2);
                return bankNumberTuple;

            }

        }


        // Validate bank number
        public static Tuple<bool, string> Validate(Tuple<bool, string> bankNumberTuple)
        {
            bankNumberTuple = Machine(bankNumberTuple);
            if (bankNumberTuple != null)
            {

                // Test for International Bank Account Number (IBAN) validity.
                if (bankNumberTuple.Item1)
                {
                    return bankNumberTuple;
                }
                // Test for Basic Bank Account Number (BBAN) validity.
                else
                {
                    // BBAN validation tests
                    Console.Write("(BBAN) Weighted numbers: ");

                    int sum = 0;
                    string bankNumber;
                    bankNumber = bankNumberTuple.Item2;


                    for (int i = 0; i < 13; i++)
                    {

                        int weightedNumber;
                        int.TryParse(new string (bankNumber[i],1), out weightedNumber);

                        // For every second number, multiply by 2 for correct weighting
                        if (i % 2 == 0)
                            weightedNumber *= 2;

                        // WeightedNumber can be between 0-18. 
                        // Calculate 1 and 8 separately by separating 18 into tenth division (int whole number) and remainder of tenth division (modulo)
                        // Then sum those together and add to total sum variable
                        sum += weightedNumber / 10 + weightedNumber % 10;
                        Console.Write(weightedNumber + " ");

                    }

                    int lastNumber;
                    int.TryParse(new string(bankNumber[13], 1), out lastNumber);
                    
                    // Calculate the next 10-divisible number from sum
                    int checksum = (int)Math.Ceiling(sum * 0.1) * 10;
                    Console.WriteLine("\nChecksum number (" + checksum + ") minus sum of individual numbers (" + sum + ") is supposed to match with last number: " + lastNumber );

                    // Check if checksum mathes with last number
                    if ((checksum - sum) == lastNumber)
                    {
                        Console.WriteLine("Number is valid.");
                        return new Tuple<bool, string>(bankNumberTuple.Item1, bankNumber);
                    }
                    else
                    {
                        Console.WriteLine("Number is not valid.");
                        return null;
                    }
                }
            }
            else
                return null;
        }



        // Transform BBAN number into IBAN number
        public static Tuple<bool, string> BBANtoIBAN(Tuple<bool, string> bankNumber)
        {
            if (bankNumber != null && bankNumber.Item1 == false)
            {
                Console.WriteLine("Changing BBAN number to IBAN format: ");
                // 'FI' area code as number + 00
                string country = "151800";
                ulong bankNumberAsNumber;
                ulong.TryParse(bankNumber.Item2 + country, out bankNumberAsNumber);
                for (ulong i = 0; i < 100; i=i+10) {
                    for (ulong j = 0; j < 10; j++)
                    {
                        if ((bankNumberAsNumber + i + j) % 97 == 1) {
                            // Valid IBAN number found!
                            //Console.Write(i/10 + " " + j + "\n");
                            bankNumber = new Tuple<bool, string>(bankNumber.Item1, "FI" + String.Join(String.Empty, i/10, j) + bankNumber.Item2);
                            // Skips out of loop;
                            i = 100;
                            j = 10;
                        }
                    }

                }
                Console.WriteLine(bankNumber.Item2);
            }
            return bankNumber;
        }

    }
}
