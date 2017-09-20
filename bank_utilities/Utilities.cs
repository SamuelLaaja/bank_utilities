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

            if (bankNumber.Length < 8)
            {
                Console.WriteLine("Bank number is too short!");
                return null;
            }

            // Check if user string's first 4 characters are [letter,letter,number,number]. If so, it is IBAN. Else it is BBAN.
            if (Char.IsLetter(bankNumber[0]) && Char.IsLetter(bankNumber[1]) && Char.IsNumber(bankNumber[2]) && Char.IsNumber(bankNumber[3]))
            {
                isIBAN = true;
            }

            string bankNumberCleanedUp = String.Empty;

            for (int i = 0; i < bankNumber.Length; i++)
            {
                // Remove spaces and punctuation marks from user string.
                if (Char.IsLetterOrDigit(bankNumber[i]))
                {
                    bankNumberCleanedUp += bankNumber[i];
                }
            }

            return new Tuple<bool, string>(isIBAN, bankNumberCleanedUp);
        }



        // Change a bank number into machine readable format
        private static string Machine(Tuple<bool, string> bankNumberTuple)
        {
            // Skips IBAN numbers
            if (bankNumberTuple.Item1)
            {
                return bankNumberTuple.Item2;
            }
            else
            {

                //14 numbers is the max length 
                int[] bankNumberWithoutZeros = new int[bankNumberTuple.Item2.Length];
                int[] bankNumber = new int[14];

                // cleanedLength is used to remember the length of cleaned up line
                int cleanedLength = 0;
                int i = 0;

                //What is this?
                // Ensure numbers are in proper encoding for the output <int> array
                int.TryParse(new string(bankNumberTuple.Item2[i], 1), out bankNumberWithoutZeros[cleanedLength++]);


                // Check which bank's standards to use
                int initialNumbers = 6;
                int counterForZerolessArray = 0;

                if (bankNumberWithoutZeros[0] == 4 || bankNumberWithoutZeros[0] == 5)
                {
                    initialNumbers = 7;
                }

                // First fill in initial numbers up to bank's standards.
                for (i = 0; i < initialNumbers; i++)
                {
                    bankNumber[i] = bankNumberWithoutZeros[counterForZerolessArray];
                    ++counterForZerolessArray;
                }

                // Then fill in zeros.
                for (; i <= (13 - (13 - cleanedLength)); i++)
                {
                    Console.WriteLine(i);
                    bankNumber[i] = 0;
                }

                // Last fill in rest of numbers.
                for (; i < 14; i++)
                {
                    if (counterForZerolessArray < bankNumberWithoutZeros.Length)
                    {
                        bankNumber[i] = bankNumberWithoutZeros[counterForZerolessArray];
                        ++counterForZerolessArray;
                    }
                    else
                    {
                        //Debug: does this ever happen?
                        bankNumber[i] = 9;
                    }
                }
                string bankNumberAsString = string.Join(String.Empty, bankNumber);
                // Output machine version of bank number
                Console.WriteLine("Machine version: " + string.Join(String.Empty, bankNumber));
                return bankNumberAsString;

            }

        }


        // Test for Basic Bank Account Number (BBAN) validity.
        public static Tuple<bool, string> Validate(Tuple<bool, string> bankNumberTuple)
        {

            if (bankNumberTuple != null)
            {
                // First change into machine readable format
                string bankNumber;
                bankNumber = Machine(bankNumberTuple);

                // Validate bank number

                // Test for International Bank Account Number (IBAN) validity.
                if (bankNumberTuple.Item1)
                {
                    return bankNumberTuple;
                }
                else
                {
                    // BBAN validation tests
                    int sum = 0;
                    if (bankNumber.Length > 14)
                    {
                        Console.WriteLine("Bank number is too long!");
                        return null;
                    }

                    for (int i = 0; i < 13; i++)
                    {

                        int weightedNumber = bankNumber[i];

                        // For every second number, multiply by 2 for correct weighting
                        if (i % 2 == 0)
                            weightedNumber *= 2;

                        // WeightedNumber can be between 0-18. 
                        // Calculate 1 and 8 separately by separating 18 into tenth division (int whole number) and remainder of tenth division (modulo)
                        // Then sum those together and add to total sum variable
                        sum += weightedNumber / 10 + weightedNumber % 10;

                        // Alternate way. Converts int to string, then to individual chars. Then Chars to strings for int.Tryparse() and sums the results.
                        // Works but is a lot slower. 
                        //foreach (char n in string.Join(String.Empty, weightedNumber))
                        //{
                        //    int result;
                        //    int.TryParse(string.Join(String.Empty, n), out result);
                        //    sum += result;
                        //}

                    }

                    // Calculate the next 10-divisible number from sum
                    int checksum = (int)Math.Ceiling(sum * 0.1) * 10;

                    // Check if checksum mathes with last number
                    if ((checksum - sum) == bankNumber[13])
                    {
                        Console.WriteLine("Number is valid.");
                        return new Tuple<bool, string>(false, bankNumber);
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

            return new Tuple<bool, string>(bankNumber.Item1, bankNumber.Item2);
        }

    }
}
