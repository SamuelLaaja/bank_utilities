using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Bank
{
    public class BicCode
    {

        public string Id { get; set; }
        public string Name { get; set; }

        public BicCode(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class BIC
    {

        // Prints out BIC
        public static string BIC_Define(Tuple<bool, string> bankNumber)
        {
            string bicPath = "bic_codes.json";

            if (bankNumber != null)
            {
                //Requires IBAN number. Attempts automatic format change
                if (bankNumber.Item1 == false)
                {
                    bankNumber = BBANtoIBAN.ChangeBBANtoIBAN(bankNumber);
                }

                if (bankNumber.Item1 == true)
                {
                    // Collect numbers that define bank group
                    string bankGroup = bankNumber.Item2.Substring(4, 3);

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

                    List<BicCode> bicCodes = new List<BicCode>();
                    using (FileStream fs = new FileStream(bicPath, FileMode.Open))
                    {
                        using (StreamReader r = new StreamReader(fs))
                        {
                            string json = r.ReadToEnd();
                            bicCodes = JsonConvert.DeserializeObject<List<BicCode>>(json);
                        }
                    }

                    var bicCode = bicCodes.Find(c => c.Id == bankGroup);

                    if(bicCode != null)
                    {
                        return bicCode.Name;
                    }
                                        
                }
                    return "BIC code not found.";
            }
            else
                return null;
        }
    }
}
