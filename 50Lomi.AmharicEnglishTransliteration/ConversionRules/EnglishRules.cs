using System.Collections.Generic;

namespace _50Lomi.AmharicEnglishTransliteration
{
    public static class EnglishRules
    {
        public static List<string> EnglishVowels = new List<string>
        {
            "a","e","i","o","u"
        };

        public static List<string> EnglishConsonants = new List<string>
        {
            "b",
            "c",
            "d",
            "f",
            "g",
            "h",
            "j",
            "k",
            "l",
            "m",
            "n",
            "p",
            "q",
            "r",
            "s",
            "t",
            "v",
            "w",
            "x",
            "y",
            "z",
        };

        public static Dictionary<string, string> EnglishVowelPhonetics = new Dictionary<string, string>
        {
            { "a", "a" },
            { "e", "e" },
            { "i", "i" },
            { "o", "o" },
            { "u", "" },
        };

        public static List<string> EnglishDiphthongs = new List<string>
        {
            //"oi","oy","ou","ow","au","aw"
        };

        public static List<string> EnglishDiagraphs = new List<string>
        {
            "ch",
            "sh",
            "ph",
            "th"
        };

    }
}