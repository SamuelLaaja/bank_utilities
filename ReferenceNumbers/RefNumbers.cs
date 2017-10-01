using System;

namespace ReferenceNumbers
{
    public class RefNumbers
    {
        public static string EnsureCorrectInput(string refNumber, bool isInternational = false)
        {
            string refNumberCleanedUp = String.Empty;
            bool untilNonZero = false;
            if (refNumber != String.Empty)
            {
                for (int i = 0; i < refNumber.Length; i++)
                {
                    // Remove letters, spaces and punctuation marks from user string. Only numbers are allowed for national ref number.
                    if (isInternational ? Char.IsLetterOrDigit(refNumber[i]) : Char.IsNumber(refNumber[i]))
                    {
                        // Remove zeros from the beginning
                        if (untilNonZero || refNumber[i] != '0')
                        {
                            refNumberCleanedUp += refNumber[i];
                            untilNonZero = true;
                        }
                    }
                }
            }

            if (refNumberCleanedUp.Length < 3)
            {
                Exception ex = new FormatException("Reference number is too short!");
                throw ex;
            }
            else if (refNumberCleanedUp.Length > 19)
            {
                Exception ex = new FormatException("Reference number is too long!");
                throw ex;
            }

            return refNumberCleanedUp;
        }

        public static bool ValidifyReferenceNumber(string refNumber, bool isInternational = false)
        {
            // Test for International Reference Number validity.
            if (isInternational)
            {
                // Replaces letters with numbers
                refNumber = Bank.International.ReplaceLetters(refNumber);
                string country = refNumber.Substring(0, 6);
                // Adds country code to the end of string
                string newOrder = refNumber.Substring(6) + country;
                string tempString = String.Empty;
                int tempInt = 0;

                // Checks for IBAN validness. Calculates modulo in partitions of 7 integers, 
                // because otherwise bank number is often too long even for ulong ( ulong < 18446744073709551616)
                // Follows the example of: http://tarkistusmerkit.teppovuori.fi/tarkmerk.htm#jakojaannos2 
                for (int i = 0; i < newOrder.Length; i += 7)
                {
                    if (newOrder.Length - i < 7)
                        int.TryParse(tempString + newOrder.Substring(i), out tempInt);
                    else
                        int.TryParse(tempString + newOrder.Substring(i, 7), out tempInt);

                    tempString = (tempInt % 97).ToString();
                }

                int modulo97;
                if (int.TryParse(tempString, out modulo97))
                {
                    // If modulo resulted to 1, ref number is valid
                    if (modulo97 == 1)
                    {
                        return true;
                    }
                }
            }
            //National validity check
            else {
                //Calculate last number again and test if it matches with user given last number.
                string refCalculated = CalculateReferenceNumber(refNumber.Substring(0, refNumber.Length - 1));
                if (refCalculated.Equals(refNumber))
                    return true;
            }
            return false;
        }

        public static string CalculateReferenceNumber(string refNumber)
        {
            
        // Calculate national validation number (last number)
            int w = 7;
            int sum = 0;
            // Calculate validation number, starting from end
            for (int i = refNumber.Length - 1; i >= 0; i--)
            {
                int weightedNumber;
                int.TryParse(new string(refNumber[i], 1), out weightedNumber);

                // Multiply with 7,3,1,7,3,1... for correct weighting
                // WeightedNumber result can be between 0-81. 
                weightedNumber *= w;

                switch (w)
                {
                    case 7:
                        w = 3;
                        break;
                    case 3:
                        w = 1;
                        break;
                    case 1:
                        w = 7;
                        break;
                }
                // Add to total sum
                sum += weightedNumber;
            }

            // Calculate the next 10-divisible number from sum
            int checksum = (int)Math.Ceiling(sum * 0.1) * 10;

            // Insert (checksum - sum) validation number to the end.
            refNumber = refNumber.Insert(refNumber.Length, String.Join(String.Empty, (checksum - sum)));

            return refNumber;
        }

        // Generates a string array full of valid ref numbers using basePart as foundation
        public static string[] Generate(string basePart, int amount)
        {
            try
            {
                string[] generatedRefNumbers = new string[amount];
                if (amount == 1)
                    generatedRefNumbers[0] = CalculateReferenceNumber(basePart);
                else
                    for (int i = 0; i < amount; i++)
                    {
                        generatedRefNumbers[i] = CalculateReferenceNumber(basePart + String.Join(String.Empty, i+1));
                    }
                return generatedRefNumbers;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        // Add spaces for clarity
        public static string WhiteSpaces(string refNumber) {

            bool isInternational = Bank.International.IsInternational(refNumber);
            // Add spaces every 4 numbers, starting from left side
            if (isInternational)
            {
                int refLength = refNumber.Length;
                int offset = 0;
                if (refLength > 4)
                {
                    for (int i = 4; i < refLength+offset; i = i + 5)
                    {
                        refNumber = refNumber.Insert(i, " ");
                        offset++;
                    }
                }
            }
            // Add spaces every 5 numbers, starting from right side
            else
            {
                int refLength = refNumber.Length;
                if (refLength > 5)
                {
                    for (int i = refLength - 5; i > 0; i = i - 5)
                    {
                        refNumber = refNumber.Insert(i, " ");
                    }
                }
            }
            return refNumber;
        }

    }
}
