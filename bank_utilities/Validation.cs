using System;

namespace Bank_Tools
{
    public class Validation
    {

        // Validate bank number (BBAN & IBAN)
        public static Tuple<bool, string> Validate(Tuple<bool, string> bankNumberTuple)
        {
            bankNumberTuple = MachineFormat.MachineReadable(bankNumberTuple);
            if (bankNumberTuple != null)
            {
                // Test for International Bank Account Number (IBAN) validity.
                if (bankNumberTuple.Item1)
                {
                    // 'XX' area code as number + 00
                    string country = International.ReplaceLetters(bankNumberTuple.Item2.Substring(0, 4));
                    // Adds country code to the end of string
                    string IBANnewOrder = bankNumberTuple.Item2.Substring(4) + country;
                    string tempString = String.Empty;
                    int tempInt = 0;

                    // Checks for IBAN validness. Calculates modulo in partitions of 7 integers, 
                    // because otherwise bank number is often too long even for ulong ( ulong < 18446744073709551616)
                    // Follows the example of: http://tarkistusmerkit.teppovuori.fi/tarkmerk.htm#jakojaannos2 
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
                    // If modulo resulted to 1, bank number is valid
                    if (modulo97 == 1)
                    {
                        return bankNumberTuple;
                    }
                    else
                    {
                        return null;
                    }
                }

                // Test for Basic Bank Account Number (BBAN) validity.
                else
                {
                    // BBAN validation tests
                    int sum = 0;
                    string bankNumber;
                    bankNumber = bankNumberTuple.Item2;
                    
                    for (int i = 0; i < 13; i++)
                    {
                        int weightedNumber;
                        int.TryParse(new string(bankNumber[i], 1), out weightedNumber);

                        // For every second number, multiply by 2 for correct weighting
                        if (i % 2 == 0)
                            weightedNumber *= 2;

                        // WeightedNumber can be between 0-18. 
                        // Calculate 1 and 8 separately by separating 18 into tenth division (int whole number) and remainder of tenth division (modulo)
                        // Then sum those together and add to total sum variable
                        sum += weightedNumber / 10 + weightedNumber % 10;
                    }
                    
                    int lastNumber;
                    int.TryParse(new string(bankNumber[13], 1), out lastNumber);

                    // Calculate the next 10-divisible number from sum
                    int checksum = (int)Math.Ceiling(sum * 0.1) * 10;
                    
                    // Check if checksum mathes with last number
                    if ((checksum - sum) == lastNumber)
                    {
                        return new Tuple<bool, string>(bankNumberTuple.Item1, bankNumber);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
                return null;
        }
    }
}
