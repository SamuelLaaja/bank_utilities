using System;

namespace Barcode
{
    public class BarCodes
    {
        public static string InputInvoice(string number)
        {
            char[] cleanedUp = "00000000".ToCharArray();
            string tempCleanedUp = String.Empty;
            int howManyDecimals = -1;
            int i = 0;

            for (; i < number.Length; i++)
            {
                // Resets decimal counter every time . or , is found
                if (number[i] == '.' || number[i] == ',')
                    howManyDecimals = 0;

                // Remove letters, spaces and punctuation marks from user string.
                if (Char.IsDigit(number[i]))
                {
                    if (howManyDecimals > -1)
                        howManyDecimals++;
                    tempCleanedUp += number[i];
                }
            }
            // Not very pretty fix for inputs thats are wholenumbers only
            if (howManyDecimals == -1)
                howManyDecimals = 0;

            int wholeNumberAmount = tempCleanedUp.Length - howManyDecimals;
            if (wholeNumberAmount > 6)
            {
                Console.WriteLine("Barcode does not support invoice amount that is a million or more. Zero invoice is used instead.");
                return "00000000";
            }

            //only first two decimals are accepted. No rounding, though.
            howManyDecimals = Math.Min(howManyDecimals, 2);

            for (i = 0; i < wholeNumberAmount + howManyDecimals; i++)
            {
                cleanedUp[6 - wholeNumberAmount + i] = tempCleanedUp[i];
            }

            tempCleanedUp = new string(cleanedUp);

            if (tempCleanedUp == "00000000")
            {
                Exception e = new FormatException("Thank you for using our services!");
                throw e;
            }

            // outputs 6 chars for euros, 2 chars for cents
            return tempCleanedUp;
        }


        public static string InputDate(string number)
        {
            // Input is expected in format DD MM YYYY with punctuation or whatever as field separators. Works without separators too.
            string cleanedUp = String.Empty;
            string[] fields = { "00", "00", "0000" };
            int nextField = 0;
            int o = 0; // i offset
            int inputLength = number.Length;

            for (int i = 0; (i + o < inputLength) && nextField <= 2; i++)
            {
                // Remove non-date garbage from user string.
                if ((i + o < inputLength) && Char.IsDigit(number[i + o]))
                {

                    //Console.WriteLine("{0} {1} {2}",i,o,inputLength);

                    if (nextField < 2)
                    {
                        //if next char is also a number:
                        if ((i + o + 1 < inputLength) && Char.IsDigit(number[i + o + 1]))
                        {
                            fields[nextField] = number.Substring(i + o, 2);
                            o++; // Skips the next char in for-loop
                        }
                        else
                        {
                            //if next char is not a number, add 0 in front
                            fields[nextField] = new string(new char[] { '0', number[i + o] });
                        }
                    }
                    // Year can be inputted in YYYY or YY format (also YYY even though barcode format does not support it)
                    else
                    {
                        if ((i + o + 3 < inputLength) &&
                            Char.IsDigit(number[i + o]) &&
                            Char.IsDigit(number[i + o + 1]) &&
                            Char.IsDigit(number[i + o + 2]) &&
                            Char.IsDigit(number[i + o + 3]))
                        {
                            fields[nextField] = number.Substring(i + o, 4);
                            o = o + 3; // Skips the 3 next chars in for-loop + (i++)
                        }
                        else if ((i + o + 2 < inputLength) &&
                            Char.IsDigit(number[i + o]) &&
                            Char.IsDigit(number[i + o + 1]) &&
                            Char.IsDigit(number[i + o + 2]))
                        {
                            fields[nextField] = "2" + number.Substring(i + o, 3);
                            o = o + 2; // Skips the 2 next chars in for-loop + (i++)
                        }
                        //if next char is also a number:
                        else if ((i + o + 1 < inputLength) && Char.IsDigit(number[i + o + 1]))
                        {
                            fields[nextField] = new string(new char[] { '2', '0', number[i + o], number[i + o + 1] });
                            o++; // Skips the next char in for-loop
                        }
                        else
                        {
                            //if next char is not a number, add 0 in front. 
                            //This case should not exist in principle since we are not creating invoices from the first decade of millennium
                            fields[nextField] = new string(new char[] { '2', '0', '0', number[1] });
                        }
                    }
                    nextField++;
                }
            }

            cleanedUp = $"{fields[2]}/{fields[1]}/{fields[0]}";
            // Tries to put user string into DateType format. If fails, it is not a valid date.
            // Error if one field is zero but not others. Allows completely zero input fields and completely non-zero input fields.
            if (cleanedUp != "00/00/0000")
            {
                if (!DateTime.TryParse(cleanedUp, out DateTime tempDate))
                //|| ((fields[0] == "00") != (fields[1] == "00") != (fields[2] == "0000")))
                {
                    Exception e = new FormatException("Incorrect date: " + cleanedUp);
                    cleanedUp = "00000000";
                    throw e;
                }
                else
                {
                    //When everything is correct. Output is in format YYMMDD. 
                    cleanedUp = fields[2].Substring(2, 2) + fields[1] + fields[0];
                }
            }
            else
            {
                //Zero string is okay. Removes / separators.
                cleanedUp = "00000000";
            }

            return cleanedUp;
        }
        /// <summary>
        /// Fills string beginning with 0's and rest with given characters
        /// </summary>
        /// <param name="input"></param>
        /// <param name="totalLength"></param>
        /// <returns></returns>
        public static string FillWithZeroPrefix(string input, int totalLength)
        {
            char[] newString = new String('0', totalLength).ToCharArray();
            int inputLength = input.Length;
            int lengthDiff = totalLength - inputLength;
            if (inputLength <= totalLength)
            {
                for (int i = input.Length - 1; i >= 0; i--)
                {
                    newString[i + lengthDiff] = input[i];
                }

            }
            return new String(newString);
        }


        public static string Modulo103(string barCode)
        {
            foreach (char c in barCode)
            {
                if (!Char.IsDigit(c))
                {
                    Exception e = new FormatException("Error: Input values should only contain numbers.");
                    throw e;
                }
            }
            int sum = 105;
            int w = 1;
            // Calculate validation number, starting from position 5 (first weight that is not 1)
            for (int i = 0; i < barCode.Length; i = i + 2)
            {
                int.TryParse(barCode.Substring(i, 2), out int weightedNumber);
                weightedNumber *= w;
                //Console.Write(barCode.Substring(i, 2) + $"[{w}] ");
                //if (w <= 27)
                w++;
                //else
                //    w = 1;
                //w -= 27;
                // Add to total sum
                sum += weightedNumber;
            }
            return (sum % 103).ToString();
        }


        // Add spaces for clarity
        public static string WhiteSpaces(string refNumber)
        {
            // Add spaces every 2 numbers, starting from left side
            int refLength = refNumber.Length;
            int offset = 0;
            if (refLength > 2)
            {
                for (int i = 2; i <= refLength + offset; i = i + 3)
                {
                    refNumber = refNumber.Insert(i, " ");
                    offset++;
                }
            }
            return refNumber;
        }
    }
}

//LASKU 1:
//Tilinro: Sp. FI79 4405 2020 0360 82 Summa: 4 883,15 Viite: 86851 62596 19897 Eräpäivä: 12.6.2010
//[105] 47 94 40 52 02 00 36 08 20 04 88 31 50 00 00 00 08 68 51 62 59 61 98 97 10 06 12 [40] [stop] 

//LASKU 2:
//Tilinro: Nordea FI58 1017 1000 0001 22 Summa: 482,99 Viite: 55958 22432 94671 Eräpäivä: 31.1.2012
//[105] 45 81 01 71 00 00 00 12 20 00 48 29 90 00 00 00 05 59 58 22 43 29 46 71 12 01 31 [55] [stop]

//LASKU 3:
//Tilinro: Op.FI02 5000 4640 0013 02 Summa: 693,80 Viite: 69 87567 20834 35364 Eräpäivä: 24.7.2011
//[105] 40 25 00 04 64 00 01 30 20 00 69 38 00 00 0 0 06 98 75 67 20 83 43 53 64 11 07 24 [14] [stop]

//LASKU 7:
//Tilinro: SEB FI83 3301 0001 1007 75 Summa: 150000,20 Viite: RF71 9212 5374 2525 3989 7737 Eräpäivä: 25.5.2016
//[105] 58 33 30 10 00 11 00 77 51 50 00 02 07 10 92 12 53 74 25 25 39 89 77 37 16 05 25 [ 80] [stop] 
