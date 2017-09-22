using System;

namespace Ekoodi.Utilities.Test
{
    public class Utilities
    {

        // Get user to input a bank number. Define if it is BBAN or IBAN and clean up spaces and special characters.
        public static Tuple<bool, string> InputBankNumber()
        {
            Console.WriteLine("Please write a bank account number (BBAN or IBAN + 8-14 numbers):");
            string bankNumber = Console.ReadLine();
            bankNumber.ToUpper();
            bool isIBAN = false;


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



        // Change a bank number into machine readable format
        private static Tuple<bool, string> Machine(Tuple<bool, string> bankNumberTuple)
        {
            // Skips IBAN numbers
            if (bankNumberTuple == null || bankNumberTuple.Item1)
            {
                return bankNumberTuple;
            }
            else
            {
                if (bankNumberTuple.Item2.Length > 14)
                {
                    Console.WriteLine("Bank number is too long!");
                    return null;
                }
                else if (bankNumberTuple.Item2.Length < 8)
                {
                    Console.WriteLine("Bank number is too short!");
                    return null;
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
                Console.WriteLine("Machine readable version: " + bankNumberTuple.Item2);
                return bankNumberTuple;

            }

        }


        // Validate bank number
        public static Tuple<bool, string> Validate(Tuple<bool, string> bankNumberTuple)
        {
            bankNumberTuple = Machine(bankNumberTuple);
            if (bankNumberTuple != null)
            {

                // Test for International Bank Account Number (IBAN) validity.
                if (bankNumberTuple.Item1)
                {
                    Console.WriteLine("Testing for IBAN validity...");
                    // 'FI' area code as number + 00
                    //Cheating for now...
                    string country = "1518" + bankNumberTuple.Item2.Substring(2,2);
                    string IBANnewOrder = bankNumberTuple.Item2.Substring(4) + country;
                    Console.WriteLine(IBANnewOrder);
                    //adds country code to the end of string and tries to change it into ulong number


                    //ulong limit is 18446744073709551615 (20 numbers)
                    // FI4250001510000023 fails, is above ulong limit
                    // FI1447201020025440 also fails
                    // http://tarkistusmerkit.teppovuori.fi/tarkmerk.htm#jakojaannos2
                    string tempString = String.Empty;
                    int tempInt = 0;
                    //// Tarkistus. Lasketaan jakojäännös seitsemän pätkissä, koska muuten jaettava on useimmiten liian iso.
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
                    
                    //ulong bankNumberAsNumber;
                    //Console.WriteLine( ulong.TryParse(IBANnewOrder, out bankNumberAsNumber));
                    //// 47201020025440151800 should result in 84
                    //Console.WriteLine(bankNumberAsNumber);
                    //ulong modulo97 = ((bankNumberAsNumber) % 97);
                    //Console.WriteLine(modulo97);
                    if (modulo97 == 1)
                    {
                        Console.WriteLine("Is valid IBAN.");
                        return bankNumberTuple;
                    }
                    else
                    {
                        Console.WriteLine("Is NOT valid IBAN.");
                        return null;
                    }
                }

                // Test for Basic Bank Account Number (BBAN) validity.
                else
                {
                    // BBAN validation tests
                    Console.Write("(BBAN) Weighted numbers: ");

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
                        Console.Write(weightedNumber + " ");

                    }

                    int lastNumber;
                    int.TryParse(new string(bankNumber[13], 1), out lastNumber);

                    // Calculate the next 10-divisible number from sum
                    int checksum = (int)Math.Ceiling(sum * 0.1) * 10;
                    Console.WriteLine("\nChecksum number (" + checksum + ") minus sum of individual numbers (" + sum + ") is supposed to match with last number: " + lastNumber);

                    // Check if checksum mathes with last number
                    if ((checksum - sum) == lastNumber)
                    {
                        Console.WriteLine("Number is valid.");
                        return new Tuple<bool, string>(bankNumberTuple.Item1, bankNumber);
                    }
                    else
                    {
                        Console.WriteLine("Number is not valid.");
                        return null;
                    }
                }
            }
            else
                return null;
        }



        // Transform BBAN number into IBAN number
        public static Tuple<bool, string> BBANtoIBAN(Tuple<bool, string> bankNumberTuple)
        {
            if (bankNumberTuple != null && bankNumberTuple.Item1 == false)
            {
                Console.WriteLine("Changing BBAN number to IBAN format: ");
               
                // 'FI' area code as number + 00
                string country = "151800";
                //adds country code to the end of string and tries to change it into ulong number
                string IBANnewOrder = bankNumberTuple.Item2 + country;
                Console.WriteLine(IBANnewOrder);

                string tempString = String.Empty;
                int tempInt = 0;
                //// Tarkistus. Lasketaan jakojäännös seitsemän pätkissä, koska muuten jaettava on useimmiten liian iso.
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



                // 47201020025440151800 should result in 84
                //ulong modulo97 = 98 - ((bankNumberAsNumber) % 97);
                modulo97 = 98 - modulo97;
                Console.WriteLine(modulo97);

                bankNumberTuple = new Tuple<bool, string>(true, "FI" + String.Join(String.Empty, modulo97) + bankNumberTuple.Item2);
                
                Console.WriteLine(bankNumberTuple.Item2);
            }
            return bankNumberTuple;
        }

        public static void BIC(Tuple<bool, string> bankNumber)
        {
            if (bankNumber != null)
            {
                //Requires IBAN number. Attempts automatic format change
                if (bankNumber.Item1 == false)
                {
                    bankNumber = BBANtoIBAN(bankNumber);
                }

                if (bankNumber.Item1 == true)
                {
                    string bankGroup = bankNumber.Item2.Substring(4, 3);
                    //Console.WriteLine(bankGroup);
                    // Bank group numeric length conditions. Determined by first value.
                    if (bankGroup[0] == '3')
                    {
                        //Length is two numbers
                        bankGroup = bankGroup.Substring(0, 2);
                    }
                    else if (bankGroup[0] == '4' || bankGroup[0] == '7')
                    {
                        //Length is three numbers
                        bankGroup = bankGroup.Substring(0, 3);
                    }
                    else
                    {
                        // Otherwise it is one number
                        bankGroup = bankGroup.Substring(0, 1);
                    }


                    string[,] bicCodeArray = new string[,]
                    {
                    {"1", "NDEAFIHH" },
                    {"2", "NDEAFIHH" },
                    {"31", "HANDFIHH" },
                    {"33", "ESSEFIHX" },
                    {"34", "DABAFIHX" },
                    {"36", "SBANFIHH" },
                    {"37", "DNBAFIHX" },
                    {"38", "SWEDFIHH" },
                    {"39", "SBANFIHH" },
                    {"400", "ITELFIHH" },
                    {"402", "ITELFIHH" },
                    {"403", "ITELFIHH" },
                    {"405", "HELSFIHH" },
                    {"406", "ITELFIHH" },
                    {"407", "ITELFIHH" },
                    {"408", "ITELFIHH" },
                    {"410", "ITELFIHH" },
                    {"411", "ITELFIHH" },
                    {"412", "ITELFIHH" },
                    {"414", "ITELFIHH" },
                    {"415", "ITELFIHH" },
                    {"416", "ITELFIHH" },
                    {"417", "ITELFIHH" },
                    {"418", "ITELFIHH" },
                    {"419", "ITELFIHH" },
                    {"420", "ITELFIHH" },
                    {"421", "ITELFIHH" },
                    {"423", "ITELFIHH" },
                    {"424", "ITELFIHH" },
                    {"425", "ITELFIHH" },
                    {"426", "ITELFIHH" },
                    {"427", "ITELFIHH" },
                    {"428", "ITELFIHH" },
                    {"429", "ITELFIHH" },
                    {"430", "ITELFIHH" },
                    {"431", "ITELFIHH" },
                    {"432", "ITELFIHH" },
                    {"435", "ITELFIHH" },
                    {"436", "ITELFIHH" },
                    {"437", "ITELFIHH" },
                    {"438", "ITELFIHH" },
                    {"439", "ITELFIHH" },
                    {"440", "ITELFIHH" },
                    {"441", "ITELFIHH" },
                    {"442", "ITELFIHH" },
                    {"443", "ITELFIHH" },
                    {"444", "ITELFIHH" },
                    {"445", "ITELFIHH" },
                    {"446", "ITELFIHH" },
                    {"447", "ITELFIHH" },
                    {"448", "ITELFIHH" },
                    {"449", "ITELFIHH" },
                    {"450", "ITELFIHH" },
                    {"451", "ITELFIHH" },
                    {"452", "ITELFIHH" },
                    {"454", "ITELFIHH" },
                    {"455", "ITELFIHH" },
                    {"456", "ITELFIHH" },
                    {"457", "ITELFIHH" },
                    {"458", "ITELFIHH" },
                    {"459", "ITELFIHH" },
                    {"460", "ITELFIHH" },
                    {"461", "ITELFIHH" },
                    {"462", "ITELFIHH" },
                    {"463", "ITELFIHH" },
                    {"464", "ITELFIHH" },
                    {"470", "POPFFI22" },
                    {"471", "POPFFI22" },
                    {"472", "POPFFI22" },
                    {"473", "POPFFI22" },
                    {"474", "POPFFI22" },
                    {"475", "POPFFI22" },
                    {"476", "POPFFI22" },
                    {"477", "POPFFI22" },
                    {"478", "POPFFI22" },
                    {"479", "POPFFI22" },
                    {"483", "ITELFIHH" },
                    {"484", "ITELFIHH" },
                    {"485", "ITELFIHH" },
                    {"486", "ITELFIHH" },
                    {"487", "ITELFIHH" },
                    {"488", "ITELFIHH" },
                    {"489", "ITELFIHH" },
                    {"490", "ITELFIHH" },
                    {"491", "ITELFIHH" },
                    {"492", "ITELFIHH" },
                    {"493", "ITELFIHH" },
                    {"495", "ITELFIHH" },
                    {"496", "ITELFIHH" },
                    {"497", "HELSFIHH" },
                    {"5", "OKOYFIHH" },
                    {"6", "AABAFI22" },
                    {"713", "CITIFIHX" },
                    {"715", "ITELFIHH" },
                    {"717", "BIGKFIH1" },
                    {"799", "HOLVFIHH" },
                    {"8", "DABAFIHH" }
                    };
                    int i = 0;
                    string bic = "BIC not found.";
                    while (i < bicCodeArray.Length)
                    {
                        if (bankGroup == bicCodeArray[i, 0])
                        {
                            bic = bicCodeArray[i, 1];
                            i = bicCodeArray.Length;
                        }
                        else
                            ++i;
                    }

                    Console.WriteLine("BIC: " + bic);
                }
            }
        }
    }
}