using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _50Lomi.AmharicEnglishTransliteration
{
    public class RawDataProccessor
    {
        static string amharicDictionaryAddress = "Data/EnglishAmharicDictionary.csv";
        static string processedAmharicDictionaryAddress = "ProcessedAmharicEnglishTranslation.csv";
        static string englishPhoneticDictionaryAddress = "Data/EnglishPhoneticDictionary.txt";
        static string processedEnglishToGeezeTransliterationAddress = "ProcessedEnglishToGeezeTransliteration.csv";

        public string ProcessEnglishPhoneticData()
        {
            var transliterater = new AmharicEnglishTransliterater(null);
            var englishPhonetic = File.ReadAllText(englishPhoneticDictionaryAddress);
            var phoneticList = englishPhonetic.Split('\n').Where(item => !(item.StartsWith(";;;") || string.IsNullOrEmpty(item))).Select(item => item.Split(' '));
            var englishToAmharicPhonetic = new StringBuilder();
            foreach (var word in phoneticList)
            {
                if (word.FirstOrDefault().Contains("(") && word.FirstOrDefault().Contains(")"))
                    continue;

                englishToAmharicPhonetic.Append(word.FirstOrDefault());
                
                var phoneticWord = string.Empty;
                for (int i = 1; i < word.Length; i++)
                {
                    phoneticWord += word[i];
                }
                var phoneticRepreseantation = phoneticWord.Replace("\r", "");
                phoneticWord = phoneticWord.Replace(" ", "").Replace("\r", "").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "");
                var geezePhonetic = ConvertEnglishPhoneticToGeezPhonetic(phoneticWord);
                var geezeScript = transliterater.ConvertGeezePhoneticToGeezText(geezePhonetic);
                englishToAmharicPhonetic.Append(",");
                englishToAmharicPhonetic.Append(phoneticRepreseantation);
                englishToAmharicPhonetic.Append(",");
                englishToAmharicPhonetic.Append(geezeScript);

                englishToAmharicPhonetic.Append("\n");
            }
            File.WriteAllText(processedEnglishToGeezeTransliterationAddress, englishToAmharicPhonetic.ToString());
            return processedEnglishToGeezeTransliterationAddress;
        }
        public string ConvertEnglishPhoneticToGeezPhonetic(string word)
        {
            var geezePhonetic = new StringBuilder();
            var CurrentPhonnetic = string.Empty;
            var previousConsonant = string.Empty;
            for (int i = 0; i < word.Length; i++)
            {
                char l = word[i];
                char nextCharacter = ' ';
                if (i + 1 < word.Length)
                    nextCharacter = word[i + 1];

                var letterIsDiagraph = EnglishRules.EnglishDiagraphs.Contains(l.ToString() + nextCharacter, StringComparer.InvariantCultureIgnoreCase);

                CurrentPhonnetic += l;
                if (ArpabetToAmharicFidel.arpabetFidelTranslation.ContainsKey(CurrentPhonnetic)
                    && !letterIsDiagraph)
                {
                    if (ArpabetToAmharicFidel.arpabetVowels.Contains(CurrentPhonnetic))
                    {
                        if (string.IsNullOrEmpty(previousConsonant))
                        {
                            if (ArpabetToAmharicFidel.arpabetFidelTranslation[CurrentPhonnetic].Length == 3)
                                if (string.IsNullOrEmpty(geezePhonetic.ToString()))
                                {
                                    geezePhonetic.Append("ʾ");
                                }
                                else
                                { geezePhonetic = geezePhonetic.Remove(geezePhonetic.Length - 1, 1); }
                            else
                                geezePhonetic.Append("ʾ");
                            geezePhonetic.Append(ArpabetToAmharicFidel.arpabetFidelTranslation[CurrentPhonnetic]);

                        }
                        else
                        {
                            geezePhonetic.Append(ArpabetToAmharicFidel.arpabetFidelTranslation[previousConsonant]);
                            geezePhonetic.Remove(geezePhonetic.Length - 1, 1);
                            geezePhonetic.Append(ArpabetToAmharicFidel.arpabetFidelTranslation[CurrentPhonnetic]);
                            previousConsonant = string.Empty;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(previousConsonant))
                        {
                            previousConsonant = CurrentPhonnetic;
                        }
                        else
                        {
                            geezePhonetic.Append(ArpabetToAmharicFidel.arpabetFidelTranslation[previousConsonant]);
                            previousConsonant = CurrentPhonnetic;
                        }
                    }
                }
                else
                    continue;
                CurrentPhonnetic = String.Empty;
            }
            if (!string.IsNullOrEmpty(previousConsonant) || !string.IsNullOrEmpty(CurrentPhonnetic))
            {
                if (ArpabetToAmharicFidel.arpabetFidelTranslation.ContainsKey(previousConsonant + CurrentPhonnetic))
                    geezePhonetic.Append(ArpabetToAmharicFidel.arpabetFidelTranslation[previousConsonant + CurrentPhonnetic]);
                else
                {
                    if (ArpabetToAmharicFidel.arpabetFidelTranslation.ContainsKey(previousConsonant))
                        geezePhonetic.Append(ArpabetToAmharicFidel.arpabetFidelTranslation[previousConsonant]);

                    if (ArpabetToAmharicFidel.arpabetFidelTranslation.ContainsKey(CurrentPhonnetic))
                        geezePhonetic.Append(ArpabetToAmharicFidel.arpabetFidelTranslation[CurrentPhonnetic]);
                }
            }
            return geezePhonetic.ToString();
        }

        public string ProcessAmharicDictionaryCSV()
        {
            var amharicDictionary = File.ReadAllText(amharicDictionaryAddress);
            var amharicTranslationList = amharicDictionary.Split('\n').Select(item => item.Split(',')).Select(item => new KeyValuePair<string, string>(item.FirstOrDefault().Replace("\"", ""), item.LastOrDefault().Split('2').FirstOrDefault().Replace("1.", "").Replace("1 ", "").Replace("1", "").Replace("\"", "").Split('፣').FirstOrDefault().Split('፤').FirstOrDefault()));
            var amharicSimpleTranslation = new StringBuilder();
            foreach (var amharicTranslation in amharicTranslationList)
            {
                amharicSimpleTranslation.Append(amharicTranslation.Key + "," + amharicTranslation.Value + "\n");
            }
            File.WriteAllText(processedAmharicDictionaryAddress, amharicSimpleTranslation.ToString());
            return processedAmharicDictionaryAddress;
        }

    }
}
