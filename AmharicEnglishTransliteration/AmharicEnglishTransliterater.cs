using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmharicEnglishTransliteration
{
    public class AmharicEnglishTransliterater
    {
        IDictionaryDataContext DataContext;

        public AmharicEnglishTransliterater(IDictionaryDataContext dictionaryDbContext)
        {
            DataContext = dictionaryDbContext;
        }

        public string ConvertGeezToLatin(string word)
        {
            var latinText = new StringBuilder();
            foreach (var l in word)
            {
                if (GeezPhoneticTranslations.GeezToPhonetic.ContainsKey(l.ToString()))
                    latinText.Append(GeezPhoneticTranslations.GeezToPhonetic[l.ToString()]);
                else
                    latinText.Append(l);
            }

            //sanitize the roman word
            latinText = latinText.Replace("ʾä", "a");
            latinText = latinText.Replace("ʾu", "u");
            latinText = latinText.Replace("ʾi", "i");
            latinText = latinText.Replace("ʾə", "ae");
            latinText = latinText.Replace("ʾo", "o");
            latinText = latinText.Replace("ä", "a");
            latinText = latinText.Replace("ə", "");
            latinText = latinText.Replace("ś", "s");
            latinText = latinText.Replace("š", "sh");
            latinText = latinText.Replace("č", "ch");
            latinText = latinText.Replace("ñ", "ngh");
            latinText = latinText.Replace("ž", "x");
            latinText = latinText.Replace("ǧ", "j");

            return latinText.ToString();
        }

        public async Task<string> ConvertEnglishToGeezUsingTranslation(string englishText)
        {
            return await ConvertEnglishToGeez(englishText, true);
        }
        public async Task<string> ConvertEnglishToGeezWithoutTranslation(string englishText)
        {
            return await ConvertEnglishToGeez(englishText, false);
        }

        internal async Task<string> ConvertEnglishToGeez(string englishText, bool useTranslation = true)
        {
            //convert the text to word list
            var textWordList = englishText.Split(' ');

            var amharicText = new StringBuilder();
            foreach (var word in textWordList)
            {
                if (amharicText.Length > 0)
                    amharicText.Append(" ");

                var amharicWord = await ConvertEnglishWordToGeez(useTranslation, word);
                amharicText.Append(amharicWord);
            }

            return amharicText.ToString();
        }
        internal async Task<string> ConvertEnglishWordToGeez(bool useTranslation, string text)
        {
            var amharicWord = string.Empty;
            var words = new List<string>();

            var specialCharactersList = new List<char>
            {
                '-',',', '-',' ','.','_','!','@','#','%','^','&','*',
                '(',')','_','-','+','=','{','}','[',']',';',':',
                '\"','\'','\\','|',',','<','>','.','/','?','\n'
            };

            foreach (var specialCharacter in specialCharactersList)
            {
                if (text.Contains(specialCharacter))
                {
                    return await TransliterateEnglishToGeezWithSpecialCharacter(useTranslation, text, specialCharacter);
                }
            }

            amharicWord = await TransliterateEnglishWordToGeez(useTranslation, text);

            return amharicWord;
        }

        internal async Task<string> TransliterateEnglishToGeezWithSpecialCharacter(bool useTranslation, string text, char specialCharacter)
        {
            var amharicWord = string.Empty;
            var words = text.Split(specialCharacter).ToList();
            for (int i = 0; i < words.Count; i++)
            {
                string word = words[i];
                if (i > 0)
                    amharicWord += specialCharacter;
                amharicWord += await ConvertEnglishWordToGeez(useTranslation, word);
            }
            return amharicWord;
        }
        internal async Task<string> TransliterateEnglishWordToGeez(bool useTranslation, string word)
        {
            var amharicWord = string.Empty;

            //try using dtranslationictionary translation implementation
            if (useTranslation)
            {
                amharicWord = await DataContext.GetAmharicWordUsingTranslation(word);
            }

            //try phonetic for english text that has failed to be converted using translationn dictionary
            if (string.IsNullOrEmpty(amharicWord))
            {
                amharicWord = await DataContext.GetAmharicWordUsingPhoneticTransliteration(word);
            }

            //as a last resort generate the geez text based on english rules
            if (string.IsNullOrEmpty(amharicWord))
            {
                amharicWord = EstimateEnglishToGeezTransliteration(word);
            }

            return amharicWord;
        }

        public virtual string EstimateEnglishToGeezTransliteration(string word)
        {
            var geezPhonetic = BreakDownEnglishWordToGeezPhonetic(word);
            var geezText = ConvertGeezePhoneticToGeezText(geezPhonetic);
            return geezText;
        }
        internal string BreakDownEnglishWordToGeezPhonetic(string word)
        {
            var geezPhonetic = new StringBuilder();
            var previousLetter = string.Empty;
            var previousLetterIsVowel = false;
            var previousLetterIsConsonant = false;
            var numberofVowels = 0;
            var isStandalonePhonetic = false;
            var letterIsVowel = false;
            var lastPhoneticText = string.Empty;
            for (int i = 0; i < word.Length; i++)
            {
                string letter = word[i].ToString();

                var letterIsConsonants = EnglishRules.EnglishConsonants.Contains(letter, StringComparer.InvariantCultureIgnoreCase);
                letterIsVowel = EnglishRules.EnglishVowels.Contains(letter, StringComparer.InvariantCultureIgnoreCase);

                isStandalonePhonetic = (!(geezPhonetic.Length>0) && previousLetterIsVowel)
                    || (previousLetterIsVowel && EnglishRules.EnglishVowels.Contains(geezPhonetic.ToString().LastOrDefault().ToString()))
                    || (previousLetterIsConsonant && !letterIsVowel)
                    || (geezPhonetic.Length>0 && !(GeezPhoneticTranslations.AmharicPhoneticSpecialCharacter.Contains(geezPhonetic.ToString().LastOrDefault().ToString()) || EnglishRules.EnglishVowels.Contains(geezPhonetic.ToString().LastOrDefault().ToString()) || EnglishRules.EnglishConsonants.Contains(geezPhonetic.ToString().LastOrDefault().ToString())));

                if (letterIsVowel)
                {
                    if (previousLetterIsVowel)
                    {
                        if (EnglishRules.EnglishDiphthongs.Contains(previousLetter + letter))
                            previousLetter += letter;
                    }
                    else
                    {
                        numberofVowels++;

                        if (!string.IsNullOrEmpty(previousLetter))
                            if (previousLetter == "c" &&
                                (letter == "e" || letter == "i" || letter == "y"))
                                geezPhonetic.Append( ConvertEnglishLetterToGeezPhonetic("s", isStandalonePhonetic));
                            else if (previousLetter == "g" &&
                                    (letter == "e" || letter == "i" || letter == "y"))
                                geezPhonetic.Append(ConvertEnglishLetterToGeezPhonetic("j", isStandalonePhonetic));
                            else
                                geezPhonetic.Append(ConvertEnglishLetterToGeezPhonetic(previousLetter, isStandalonePhonetic));
                        previousLetter = letter;
                    }
                }
                else
                {
                    if (previousLetterIsVowel)
                    {
                        if (!string.IsNullOrEmpty(previousLetter))
                            geezPhonetic.Append(ConvertEnglishLetterToGeezPhonetic(previousLetter, isStandalonePhonetic));
                        previousLetter = letter;
                    }
                    else
                    {
                        if (!string.Equals(previousLetter, letter))
                        {
                            if (letterIsConsonants && EnglishRules.EnglishDiagraphs.Contains(previousLetter + letter))
                                previousLetter += letter;
                            else
                            {
                                geezPhonetic.Append(ConvertEnglishLetterToGeezPhonetic(previousLetter, isStandalonePhonetic));
                                previousLetter = letter;
                            }
                        }
                    }
                }

                if (i < word.Length - 1)
                {
                    previousLetterIsConsonant = letterIsConsonants;
                    previousLetterIsVowel = letterIsVowel;
                }
            }
            isStandalonePhonetic = (!(geezPhonetic.Length > 0) && previousLetterIsVowel)
                || !(!previousLetterIsVowel && letterIsVowel)
                || (geezPhonetic.Length>0 && !(GeezPhoneticTranslations.AmharicPhoneticSpecialCharacter.Contains(geezPhonetic.ToString().LastOrDefault().ToString()) || EnglishRules.EnglishVowels.Contains(geezPhonetic.ToString().LastOrDefault().ToString()) || EnglishRules.EnglishConsonants.Contains(geezPhonetic.ToString().LastOrDefault().ToString())));

            if (previousLetter != "e" || numberofVowels != 2)
                geezPhonetic.Append(ConvertEnglishLetterToGeezPhonetic(previousLetter, isStandalonePhonetic));

            return geezPhonetic.ToString();
        }
        internal string ConvertEnglishLetterToGeezPhonetic(string englishLetter, bool shouldBeCompletePhonetic)
        {
            var geezPhonetic = string.Empty;

            //vowel handling
            if (string.Equals(englishLetter, "a", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "ʾa" : "a";
            else if (string.Equals(englishLetter, "e", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "ʾe" : "e";
            else if (string.Equals(englishLetter, "i", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "ʾi" : "i";
            else if (string.Equals(englishLetter, "o", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "ʾo" : "o";
            else if (string.Equals(englishLetter, "u", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "ʾä" : "ä";

            //consonants handling
            else if (string.Equals(englishLetter, "b", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "bə" : "b";
            else if (string.Equals(englishLetter, "c", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "kə" : "k";
            else if (string.Equals(englishLetter, "d", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "də" : "d";
            else if (string.Equals(englishLetter, "f", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "fə" : "f";
            else if (string.Equals(englishLetter, "g", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "gə" : "g";
            else if (string.Equals(englishLetter, "h", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "hə" : "h";
            else if (string.Equals(englishLetter, "j", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "jə" : "j";
            else if (string.Equals(englishLetter, "k", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "kə" : "k";
            else if (string.Equals(englishLetter, "l", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "lə" : "l";
            else if (string.Equals(englishLetter, "m", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "mə" : "m";
            else if (string.Equals(englishLetter, "n", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "nə" : "n";
            else if (string.Equals(englishLetter, "p", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "pə" : "p";
            else if (string.Equals(englishLetter, "q", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "qə" : "q";
            else if (string.Equals(englishLetter, "r", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "rə" : "r";
            else if (string.Equals(englishLetter, "s", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "sə" : "s";
            else if (string.Equals(englishLetter, "t", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "tə" : "t";
            else if (string.Equals(englishLetter, "v", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "və" : "v";
            else if (string.Equals(englishLetter, "w", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "wə" : "w";
            else if (string.Equals(englishLetter, "x", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "zə" : "z";
            else if (string.Equals(englishLetter, "y", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "yə" : "y";
            else if (string.Equals(englishLetter, "z", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "zə" : "z";

            //consonant diagraphs
            else if (string.Equals(englishLetter, "ch", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "čə" : "č";
            else if (string.Equals(englishLetter, "sh", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "šə" : "š";
            else if (string.Equals(englishLetter, "ph", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "fə" : "f";
            else if (string.Equals(englishLetter, "th", StringComparison.CurrentCultureIgnoreCase))
                geezPhonetic = shouldBeCompletePhonetic ? "zə" : "z";

            //vowel Diphthongs
            //TODO add proper translation for diphthongs
            //else if (string.Equals(englishLetter, "oi", StringComparison.CurrentCultureIgnoreCase))
            //    geezPhonetic = shouldBeCompletePhonetic ? "čə" : "č";
            //else if (string.Equals(englishLetter, "oy", StringComparison.CurrentCultureIgnoreCase))
            //    geezPhonetic = shouldBeCompletePhonetic ? "šə" : "š";
            //else if (string.Equals(englishLetter, "ou", StringComparison.CurrentCultureIgnoreCase))
            //    geezPhonetic = shouldBeCompletePhonetic ? "fə" : "f";
            //else if (string.Equals(englishLetter, "au", StringComparison.CurrentCultureIgnoreCase))
            //    geezPhonetic = shouldBeCompletePhonetic ? "zə" : "z";
            //else if (string.Equals(englishLetter, "aw", StringComparison.CurrentCultureIgnoreCase))
            //    geezPhonetic = shouldBeCompletePhonetic ? "zə" : "z";

            else
                geezPhonetic = englishLetter;

            return geezPhonetic;
        }
        public string ConvertGeezePhoneticToGeezText(string word)
        {
            var geezeText = new StringBuilder();
            var CurrentPhonnetic = string.Empty;
            foreach (var l in word)
            {
                if (!(EnglishRules.EnglishVowels.Contains(l.ToString())
                    || EnglishRules.EnglishConsonants.Contains(l.ToString())
                    || GeezPhoneticTranslations.AmharicPhoneticSpecialCharacter.Contains(l.ToString())))
                {
                    geezeText.Append( l);
                    continue;
                }

                CurrentPhonnetic += l;
                if (GeezPhoneticTranslations.PhoneticToGeez.ContainsKey(CurrentPhonnetic))
                {
                    geezeText.Append(GeezPhoneticTranslations.PhoneticToGeez[CurrentPhonnetic]);
                    CurrentPhonnetic = string.Empty;
                }
            }
            return geezeText.ToString();
        }
        
    }
}
