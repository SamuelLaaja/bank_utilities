using System;

namespace ReferenceNumbers
{
    public class International
    {

        public static bool IsInternational(string refNumber)
        {
            // Check if user string's first 4 characters are [letter,letter,number,number]. If so, it is international. Else it is national.
            if (Char.IsLetter(refNumber[0]) && Char.IsLetter(refNumber[1]) && Char.IsNumber(refNumber[2]) && Char.IsNumber(refNumber[3]))
            {
                return true;
            }
            else
                return false;
        }

        public static string ReplaceLetters(string refNumber) {
            if (refNumber != null) {
                string[] replaced = new string[refNumber.Length];
                
                for (int i = 0; i < refNumber.Length; i++) {
                    if (Char.IsLetter(refNumber[i])) {
                        //replaced[i] = refNumber[i];
                        //to uppercase
                        Console.WriteLine((int)refNumber[i] -55);
                    }
                }
            }
            return refNumber;
        }

    }
}
