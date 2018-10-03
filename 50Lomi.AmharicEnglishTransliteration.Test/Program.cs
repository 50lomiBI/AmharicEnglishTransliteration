using System;
using System.Threading.Tasks;

namespace _50Lomi.AmharicEnglishTransliteration.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Program p = new Program();
            p.Run().Wait();
        }

        public async Task Run()
        {

            //dictionary data context
            var dataContext = new CSVDictionaryDataContext();
            var transliterater = new AmharicEnglishTransliterater(dataContext);

            var englishText = "This application's main purpose is to allow your systems to be able to allow entry of text in both latin txt and geez text, but when it is displayed to the user to provide one text (either completly geez or completely latin/english).";
            var processedGeezTranslatedText = await transliterater.ConvertEnglishToGeezUsingTranslation(englishText);
            var processedGeezUnTranslatedText = await transliterater.ConvertEnglishToGeezWithoutTranslation(englishText);
            var processedEnglishText = transliterater.ConvertGeezToLatin(processedGeezUnTranslatedText);
        }

        static void GenerateProcessedData()
        {
            var processor = new RawDataProccessor();

            //create a simple key value amharic english dictionary
            var amharicEnglishDictionaryRoute = processor.ProcessAmharicDictionaryCSV();

            //create a simple key value amharic english dictionary
            var englishAmharicTransliterationDictionary = processor.ProcessEnglishPhoneticData();
        }
    }
}
