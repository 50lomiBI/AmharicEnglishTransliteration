using System;
using System.Threading.Tasks;

namespace AmharicEnglishTransliteration.Test
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

            var englishText = "This main purpose is to allow your systems to be able to allow entry of text in both latin txt and geez text.";
            var amharicText = "ልጆችን አማርኛ የማስተማርያ ቀላልና አስደሳች ዌብሳይት:: ይሞክሩት አሁን!";
            
            var processedGeezTranslatedText = await transliterater.ConvertEnglishToGeezUsingTranslation(englishText);
            var processedGeezUnTranslatedText = await transliterater.ConvertEnglishToGeezWithoutTranslation(englishText);
            var processedEnglishText = transliterater.ConvertGeezToLatin(amharicText);

            Console.WriteLine(" English to Amharic conversion");
            Console.WriteLine($"Root Text : '({englishText})'");
            Console.WriteLine($"Transliterated Text : '({processedGeezUnTranslatedText})'");
            Console.WriteLine($"Translated Text : '({processedGeezTranslatedText})'");


            Console.WriteLine();
            Console.WriteLine("########################################################################"); 
            Console.WriteLine();
            Console.WriteLine(" Amharic to English conversion");
            Console.WriteLine($"Root Text : '({amharicText})'");
            Console.WriteLine($"Transliterated Text : '({processedEnglishText})'");

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
