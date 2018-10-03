
# Purpose

This application's main purpose is to allow your systems to be able to allow entry of text in both latin txt and geez text, but when it is displayed to the user to provide one text (either completly geez or completely latin/english).

* Note that even though the application allow translation it's main goal is transliteration

* There is an english amharic phonetic dictionary with over 130,000 word in it.  This data was generate from  [The CMU Pronouncing Dictionary](http://www.speech.cs.cmu.edu/cgi-bin/cmudict).

* The Translation is implemented by the usage of the dictionary obtained from [Yidnekachew](https://github.com/Yidnekachew/amharic-dictionary-database)'s github repo.

# Usage
The methods for transliteration, are in AmharicEnglishTransliteration class.

The AmharicEnglishTransliteration require an implementation IDictionaryDataContex, the default implementation is a CSVDictionaryDataContex.

```
    var dataContext = new CSVDictionaryDataContext();
    var transliterater = new AmharicEnglishTransliteraterdataContext);
``` 
### The three main methods provided  are:
1) Convert Geez/Amharic text to Latin/English

``` 
var latinText = transliterater.ConvertGeezToLatin(processedGeezUnTranslatedText);
```

 2) Convert English text to Geez text with translation

```
 var geezText = transliterater.ConvertEnglishToGeezUsingTranslation(englishText);
 ```

3) Convert English text to Geez text only using phonetic

```
 var geezText = transliterater.ConvertEnglishToGeezWithoutTranslation(englishText);
 ```

 # Customization
The system is made with extensibility in mind. 
* The source of the data can easily be changed from the csv files to database by implementing your own custom IDictionaryDataContex.

* By taking the data provided you can implement this in any other languge you desire.

# License

Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.