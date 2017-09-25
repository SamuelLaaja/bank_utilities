using System;

namespace Bank
{
    public class BBANtoIBAN
    {

        // Transform BBAN number into IBAN number
        public static Tuple<bool, string> ChangeBBANtoIBAN(Tuple<bool, string> bankNumberTuple)
        {
            if (bankNumberTuple != null && bankNumberTuple.Item1 == false)
            {
                // 'FI' area code as number + 00
                string country = "151800";
                //adds country code to the end of string and tries to change it into ulong number
                string IBANnewOrder = bankNumberTuple.Item2 + country;
                string tempString = String.Empty;
                int tempInt = 0;

                // Checks for IBAN validness. Calculates modulo in partitions of 7 integers, because otherwise bank number is often too long even for ulong ( ulong < 18446744073709551616)
                for (int i = 0; i < IBANnewOrder.Length; i += 7)
                {
                    if (IBANnewOrder.Length - i < 7)
                        int.TryParse(tempString + IBANnewOrder.Substring(i), out tempInt);
                    else
                        int.TryParse(tempString + IBANnewOrder.Substring(i, 7), out tempInt);

                    tempString = (tempInt % 97).ToString();
                }

                int modulo97;
                int.TryParse(tempString, out modulo97);

                modulo97 = 98 - modulo97;

                bankNumberTuple = new Tuple<bool, string>(true, "FI" + String.Join(String.Empty, modulo97) + bankNumberTuple.Item2);

            }
            return bankNumberTuple;
        }
    }
}
