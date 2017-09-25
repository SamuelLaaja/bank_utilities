using System;
using System.Numerics;

namespace Bank
{
    public class ReferenceUtilities
    {
        public static BigInteger InputReferenceNumber(){
            BigInteger referenceNumber;
                BigInteger.TryParse(Console.ReadLine(), out referenceNumber);
            return referenceNumber;
        }
    }
}
