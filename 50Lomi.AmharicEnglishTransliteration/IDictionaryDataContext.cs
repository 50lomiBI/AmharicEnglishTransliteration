using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _50Lomi.AmharicEnglishTransliteration
{
    public interface IDictionaryDataContext
    {
        Task<string> GetAmharicWordUsingPhoneticTransliteration(string word);

        Task<string> GetAmharicWordUsingTranslation(string word);

    }
}
