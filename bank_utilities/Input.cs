using System;

namespace Bank
{
    public class Input
    {
        // Get user to input a bank number. Define if it is BBAN or IBAN and clean up spaces and special characters.
        public static Tuple<bool, string> InputBankNumber(string bankNumber)
        {
            bankNumber = bankNumber.ToUpper();
            bool isIBAN = false;

            if (bankNumber == String.Empty)
            {
                Exception ex = new FormatException("Skipping to next part...");
                throw ex;
            }

            // Cannot do IBAN check if string is too short for it.
            if ( bankNumber.Length < 4)
            {
                Exception ex = new FormatException("Bank number is too short!");
                throw ex;
            }

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

    }
}
