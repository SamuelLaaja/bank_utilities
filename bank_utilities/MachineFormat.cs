using System;

namespace Bank
{
    public class MachineFormat
    {

        // Change a bank number into machine readable format
        public static Tuple<bool, string> MachineReadable(Tuple<bool, string> bankNumberTuple)
        {
            // Skips IBAN numbers, they are already okay
            if (bankNumberTuple == null || bankNumberTuple.Item1)
            {
                return bankNumberTuple;
            }
            else
            {
                if (bankNumberTuple.Item2.Length > 14)
                {
                    Exception ex = new FormatException("Bank number is too long!");
                    throw ex;
                }
                else if (bankNumberTuple.Item2.Length < 8)
                {

                    Exception ex = new FormatException("Bank number is too short!");
                    throw ex;
                }


                // Check which bank's standards to use
                int initialNumbers = 6;

                if (bankNumberTuple.Item2[0] == '4' || bankNumberTuple.Item2[0] == '5')
                    initialNumbers = 7;

                string zeros = String.Empty;
                for (int i = initialNumbers; i < initialNumbers + (14 - bankNumberTuple.Item2.Length); i++)
                    zeros += "0";

                bankNumberTuple = new Tuple<bool, string>(bankNumberTuple.Item1, bankNumberTuple.Item2.Insert(initialNumbers, zeros));

                // Output machine version of bank number
                return bankNumberTuple;

            }

        }
    }
}
