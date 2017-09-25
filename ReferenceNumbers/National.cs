using System;
using System.Numerics;

namespace ReferenceNumbers
{
    public class National
    {
        public static BigInteger InputReferenceNumber()
        {
            BigInteger referenceNumber;
            BigInteger.TryParse(Console.ReadLine(), out referenceNumber);
            return referenceNumber;
        }
    }
}
