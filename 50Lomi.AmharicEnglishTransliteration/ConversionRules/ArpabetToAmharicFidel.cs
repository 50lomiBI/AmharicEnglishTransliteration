using System;
using System.Collections.Generic;
using System.Text;

namespace _50Lomi.AmharicEnglishTransliteration
{
    public class ArpabetToAmharicFidel
    {
        public static Dictionary<string, string> arpabetFidelTranslation = new Dictionary<string, string>
        {
            //vowels
            {"AA","o" },
            {"AE","a" },
            {"AH","ä" },
            {"AO","o" },
            {"AW","awo" },
            {"AY","ayə" },
            {"EH","e" },
            {"ER","ärə" },
            {"EY","e" },
            {"IH","i" },
            {"IY","iyə" },
            {"OW","owə" },
            {"OY","oyə" },
            {"UH","u" },
            {"UW","uwo" },

            //consonants
            {"B","bə" },
            {"CH","čə" },
            {"D","də" },
            {"DH","žə" },
            {"F","fə" },
            {"G","gə" },
            {"HH","hə" },
            {"JH","ǧə" },
            {"K","kə" },
            {"L","lə" },
            {"M","mə" },
            {"N","nə" },
            {"NG","nəgə" },
            {"P","pə" },
            {"R","rə" },
            {"S","sə" },
            {"SH","šə" },
            {"T","tə" },
            {"TH","zə" },
            {"V","və" },
            {"W","wə" },
            {"Y","yə" },
            {"Z","zə" },
            {"ZH","žə" },

        };

        public static IEnumerable<string> arpabetConsonants = new List<string>
        {
            "B",
            "CH",
            "D",
            "DH",
            "EL",
            "EM",
            "EN",
            "F",
            "G",
            "HH",
            "JH",
            "K",
            "L",
            "M",
            "N",
            "NG",
            "P",
            "Q",
            "R",
            "S",
            "SH",
            "T",
            "TH",
            "V",
            "W",
            "Y",
            "Z",
            "ZH",
        };
        public static IEnumerable<string> arpabetVowels = new List<string>
        {
            "AA",
            "AE",
            "AH",
            "AO",
            "AW",
            "AY",
            "EH",
            "ER",
            "EY",
            "IH",
            "IY",
            "OW",
            "OY",
            "UH",
            "UW",
        };

    }
}
