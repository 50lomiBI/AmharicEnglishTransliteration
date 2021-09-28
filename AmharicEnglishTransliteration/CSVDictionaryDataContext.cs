using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmharicEnglishTransliteration
{
    public class CSVDictionaryDataContext : IDictionaryDataContext
    {

        private IEnumerable<KeyValuePair<string, string>> englishAmharicTranslationDictionary;
        public IEnumerable<KeyValuePair<string, string>> EnglishAmharicTranslationDictionary
        {
            get { return GetEnglishAmharicTranslationDictionary(); }
            set { englishAmharicTranslationDictionary = value; }
        }

        private IEnumerable<KeyValuePair<string, string>> englishAmharicPhoneticDictionary;
        public IEnumerable<KeyValuePair<string, string>> EnglishAmharicPhoneticDictionary
        {
            get { return GetEnglishAmharicPhoneticDictionary(); }
            set { englishAmharicPhoneticDictionary = value; }
        }


        public virtual IEnumerable<KeyValuePair<string, string>> GetEnglishAmharicTranslationDictionary()
        {

            string processedAmharicDictionaryAddress = "Data/ProcessedAmharicEnglishTranslation.csv";

            if (englishAmharicTranslationDictionary != null)
                return englishAmharicTranslationDictionary;

            var amharicDictionaryCSV = File.ReadAllText(processedAmharicDictionaryAddress);
            englishAmharicTranslationDictionary = amharicDictionaryCSV.Split('\n').Select(item => item.Split(',')).Select(item => new KeyValuePair<string, string>(item.FirstOrDefault(), item.LastOrDefault().Replace("\r", "")));
            return englishAmharicTranslationDictionary;
        }
        public virtual IEnumerable<KeyValuePair<string, string>> GetEnglishAmharicPhoneticDictionary()
        {
            string processedEnglishToGeezeTransliterationAddress = "Data/ProcessedEnglishToGeezeTransliteration.csv";

            if (englishAmharicPhoneticDictionary != null)
                return englishAmharicPhoneticDictionary;

            var amharicPhoneticCSV = File.ReadAllText(processedEnglishToGeezeTransliterationAddress);
            englishAmharicPhoneticDictionary = amharicPhoneticCSV
                .Split('\n')
                .Where(item => !(string.IsNullOrEmpty(item) || item.StartsWith(";;;")))
                .Select(item => item.Split(','))
                .Select(item => new KeyValuePair<string, string>(item.FirstOrDefault(), item.LastOrDefault().Replace("\r", "")));
            return englishAmharicPhoneticDictionary;
        }


        public virtual async Task<string> GetAmharicWordUsingPhoneticTransliteration(string word)
        {
            return EnglishAmharicPhoneticDictionary.Where(item => string.Equals(item.Key, word, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value;
        }
        public virtual async Task<string> GetAmharicWordUsingTranslation(string word)
        {
            return EnglishAmharicTranslationDictionary.Where(item => string.Equals(item.Key, word, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value;
        }

    }

}
