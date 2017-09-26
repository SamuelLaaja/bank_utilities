using System;
using System.Numerics;

namespace ReferenceNumbers
{
    public class National
    {
        public static string EnsureCorrectInput(string refNumber)
        {

            string refNumberCleanedUp = String.Empty;
            bool untilNonZero = false;
            // Remove letters from basic bank numbers
            for (int i = 0; i < refNumber.Length; i++)
            {
                // Remove letters, spaces and punctuation marks from user string.
                if (Char.IsNumber(refNumber[i]))
                {
                    // Remove zeros from the beginning
                    if (untilNonZero || refNumber[i] != '0')
                    {
                        refNumberCleanedUp += refNumber[i];
                        untilNonZero = true;
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

        public static string ValidifyReferenceNumber(string refNumber)
        {
            int w = 7;
            int sum = 0;
            // Calculate validation number
            for (int i = refNumber.Length-1; i >= 0; i--)
            {
                int weightedNumber;
                int.TryParse(new string(refNumber[i], 1), out weightedNumber);

                // Multiply by 7,3,1,7,3,1... for correct weighting
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

                // WeightedNumber result can be between 0-81. 
                // Calculate 1 and 8 separately by separating 18 into tenth division (int whole number) and remainder of tenth division (modulo)
                // Then sum those together and add to total sum variable
                sum += weightedNumber;
            }
            
            // Calculate the next 10-divisible number from sum
            int checksum = (int)Math.Ceiling(sum * 0.1) * 10;

            // Insert validation number to the end.
            refNumber = refNumber.Insert(refNumber.Length, String.Join(String.Empty, (checksum - sum)));
            refNumber = WhiteSpaces(refNumber);
            
            return refNumber;
        }

        public static string[] Generate(string basePart, int amount)
        {
            try
            {
                string[] generatedRefNumbers = new string[amount];
                if (amount == 1)
                    generatedRefNumbers[0] = ValidifyReferenceNumber(basePart);
                else
                    for (int i = 0; i < amount; i++)
                    {
                        generatedRefNumbers[i] = ValidifyReferenceNumber(basePart + String.Join(String.Empty, i+1));
                    }
                return generatedRefNumbers;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        // Add spaces every 5 numbers for clarity
        public static string WhiteSpaces(string refNumber) {

            int refLength = refNumber.Length;
            if (refLength > 5)
            {
                for (int i = refLength-5; i > 0; i = i - 5)
                {
                    refNumber = refNumber.Insert(i, " ");
                }
            }
            return refNumber;
        }

    }
}
