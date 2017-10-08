using System;

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

namespace Barcode
{
    public class BarCodes
    {
        public static string InputInvoice(string number)
        {
            string cleanedUp = String.Empty;

            for (int i = 0; i < number.Length; i++)
            {
                // Remove lettera, spaces and punctuation marks excluding ,. from user string.
                if (Char.IsDigit(number[i]) || number[i] == '.' || number[i] == ',')
                {
                    cleanedUp += number[i];
                }
            }
            // Currently allows 0,0 euro payments to be done...
            return cleanedUp;
        }


        public static string InputDate(string number)
        {
            // Input is expected in format DD MM YYYY with punctuation or whatever as field separators. Works without separators too.
            string cleanedUp = String.Empty;
            string[] fields = { "00", "00", "0000" };
            int nextField = 0;
            int o = 0; // i offset
            int inputLength = number.Length;


            for (int i = 0; i + o < inputLength && nextField <= 2; i++)
            {
                // Remove non-date garbage from user string.
                if (Char.IsDigit(number[i + o]))
                {
                    // Days and Months. Currently does not limit max values (31 & 12)
                    if (nextField < 2)
                    {
                        //if next char is also a number:
                        if (inputLength >= i + o + 1 && Char.IsDigit(number[i + o + 1]))
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
                    // Year can be inputted in YYYY or YY format
                    else
                    {
                        if (inputLength >= i + 3 &&
                            Char.IsDigit(number[i + o]) &&
                            Char.IsDigit(number[i + o + 1]) &&
                            Char.IsDigit(number[i + o + 2]) &&
                            Char.IsDigit(number[i + o + 3]))
                        {
                            fields[nextField] = number.Substring(i + o, 4);
                            o = o + 3; // Skips the 3 next chars in for-loop
                        }
                        //if next char is also a number:
                        else if (inputLength >= i + o + 1 && Char.IsDigit(number[i + o + 1]))
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

            //output is in format YYMMDD. Zero string is okay.
            return fields[2].Substring(2, 2) + fields[1] + fields[0];
        }
    }
}
