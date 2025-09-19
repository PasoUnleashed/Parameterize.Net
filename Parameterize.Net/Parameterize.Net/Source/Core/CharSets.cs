using System;

namespace Parameterize2.Net
{
    public enum Charset
    {
        Numeric,
        AlphaLower,
        AlphaUpper,
        AlphaAll,
        AlphaNum
    }
    public static class CharSets
    {
        public static char[] GetSet(Charset set)
        {
            switch (set)
            {
                case Charset.Numeric:
                    return NumericChars;
                    break;
                case Charset.AlphaLower:
                    return AlphabeticLowercaseChars;
                    break;
                case Charset.AlphaUpper:
                    return AlphabeticUppercaseChars;
                    break;
                case Charset.AlphaAll:
                    return AlphabeticChars;
                    break;
                case Charset.AlphaNum:
                    return AlphanumericChars;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(set), set, null);
            }
        }
        // Static array of numeric characters (0-9)
        public static readonly char[] NumericChars = new[]
            { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        // Static array of lowercase alphabetic characters (a-z)
        public static readonly char[] AlphabeticLowercaseChars = 
        { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        // Static array of uppercase alphabetic characters (A-Z)
        public static readonly char[] AlphabeticUppercaseChars = 
        { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        // Static array of all alphabetic characters (a-z, A-Z)
        public static readonly char[] AlphabeticChars = 
        { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        // Static array of alphanumeric characters (a-z, A-Z, 0-9)
        public static readonly char[] AlphanumericChars = 
        { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    }
}