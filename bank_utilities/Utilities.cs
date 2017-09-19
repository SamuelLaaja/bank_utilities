using System;

namespace Ekoodi.Utilities.Test
{
    public static class Utilities
    {

        public static void PrintGreetings()
        {
            Console.WriteLine("Hello Ekoodi!");
        }
        
        //Task1
        public static void BBAN_Validate(string userBankNumber)
        {
            //14 numbers is the max length 
            int[] bankNumberWithoutZeros = new int[userBankNumber.Length] ;
            int[] bankNumber = new int[14] ;

            // cleanedLength is used to remember the length of cleaned up line
            int cleanedLength = 0;
            int i = 0;
            bool isValid = false;

            

            for (; i < userBankNumber.Length; i++)
            {
                // Remove spaces and punctuation marks from user line. Also letters.
                if (Char.IsNumber(userBankNumber[i]))
                {
                    // Ensure numbers are in proper encoding for the output <int> array
                    int.TryParse(new string(userBankNumber[i],1), out bankNumberWithoutZeros[cleanedLength++]);
                }
            }


            // BBAN validation tests
            if (cleanedLength > 14)
            {
                isValid = false;
                Console.WriteLine("Bank number is too long!");
            }
            else if (cleanedLength < 8)
            {
                isValid = false;
                Console.WriteLine("Bank number is too short!");
            }
            else
            {
                // Check which bank's standards to use
                int initialNumbers = 6;
                int counterForZerolessArray = 0;
                isValid = true;

                if (bankNumberWithoutZeros[0] == 4 || bankNumberWithoutZeros[0] == 5)
                {
                    initialNumbers = 7;
                }

                // First fill in initial numbers up to bank's standards.
                for (i = 0; i < initialNumbers; i++) {
                    bankNumber[i] = bankNumberWithoutZeros[counterForZerolessArray];
                    ++counterForZerolessArray;
                }

                // Then fill in zeros.
                for (; i <= (initialNumbers+13-cleanedLength); i++)
                {
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
                    else {
                        bankNumber[i] = 9;
                    }
                }

            }
            
            
            if (isValid)
            {
            Console.WriteLine(string.Join( String.Empty, bankNumber ) );
            Console.WriteLine("Number is valid.");
            } else
            Console.WriteLine("Number is not valid.");

        }

    }
}
