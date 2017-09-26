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
            string replaced = String.Empty;
            if (refNumber != null) {
                refNumber = refNumber.ToUpper();
                for (int i = 0; i < refNumber.Length; i++) {
                    if (Char.IsNumber(refNumber[i]))
                    {
                        replaced += new String(refNumber[i],1);
                    } else if (Char.IsLetter(refNumber[i]))
                    {
                        replaced += ((int)refNumber[i] -55).ToString();
                    }
                }
            }
            return replaced;
        }

    }
}
