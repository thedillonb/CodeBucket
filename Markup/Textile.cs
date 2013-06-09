using System;
using Textile;

namespace Markup
{
    public class Textile
    {
        public static string Transform(string input)
        {
            return TextileFormatter.FormatString(input);
        }
    }
}

