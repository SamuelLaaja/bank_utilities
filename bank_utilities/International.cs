using System;

namespace Bank
{
    public class International
    {
        // Check if user string's first 4 characters are [letter,letter,number,number]. If so, it is international. Else it is national.
        public static bool IsInternational(string userString)
        {
            if (Char.IsLetter(userString[0]) && Char.IsLetter(userString[1]) && Char.IsNumber(userString[2]) && Char.IsNumber(userString[3]))
                return true;
            else
                return false;
        }

        //Replaces letters from user string with numeric values, A = 10, B = 11, etc.
        public static string ReplaceLetters(string userString)
        {
            string replaced = String.Empty;
            if (userString != null)
            {
                userString = userString.ToUpper();
                for (int i = 0; i < userString.Length; i++)
                {
                    if (Char.IsNumber(userString[i]))
                    {
                        replaced += new String(userString[i], 1);
                    }
                    else if (Char.IsLetter(userString[i]))
                    {
                        // Transforms a character into numeric value and offsets with -55
                        replaced += ((int)userString[i] - 55).ToString();
                    }
                }
            }
            return replaced;
        }

        // Transform national number into international number (both bank numbers and reference numbers)
        public static string ChangeToInternational(string number, string countryCode)
        {
            if (number != String.Empty)
            {
                // Adds country code to the end of string and +00
                string newOrder = number + ReplaceLetters(countryCode) + "00";
                string tempString = String.Empty;
                int tempInt = 0;

                // Checks for IBAN validness. Calculates modulo in partitions of 7 integers, because otherwise bank number is often too long even for ulong ( ulong < 18446744073709551616)
                // Calculates modulo in partitions of 7 integers, 
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
                int.TryParse(tempString, out modulo97);

                modulo97 = 98 - modulo97;

                number = countryCode + (modulo97 < 10 ? "0": "") + String.Join(String.Empty, modulo97) + number;
            }
            return number;
        }
                
    }
}
