using System;
using System.Numerics;

namespace ReferenceNumbers
{
    public class National
    {
        public static Tuple<bool, string> InputReferenceNumber(string refNumber)
        {
            if (refNumber.Length < 3)
            {
                Exception ex = new FormatException("Reference number is too short!");
                throw ex;
            } else if (refNumber.Length > 19) {
                Exception ex = new FormatException("Reference number is too long!");
                throw ex;
            }


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
            
            int w = 7;
            int sum = 0;
            // Calculate validation number
            for (int i = refNumberCleanedUp.Length-1; i >= 0; i--)
            {
                int weightedNumber;
                int.TryParse(new string(refNumberCleanedUp[i], 1), out weightedNumber);

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

                Console.Write(weightedNumber + " ");
            }
            
            // Calculate the next 10-divisible number from sum
            int checksum = (int)Math.Ceiling(sum * 0.1) * 10;

            Console.WriteLine("\n{0} - {1}", checksum, sum);

            //Insert validation number as last number if not already so.
            if (refNumberCleanedUp[refNumberCleanedUp.Length-1] != (checksum - sum).ToString()[0])
            refNumber = refNumberCleanedUp.Insert(refNumberCleanedUp.Length, String.Join(String.Empty, (checksum - sum)));
                        

            // Add spaces every 5 numbers for clarity
            

            return new Tuple <bool, string> (false, refNumber);
        }
    }
}
