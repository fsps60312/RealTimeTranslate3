using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTranslate3
{
    public static class YahooDictionary
    {
        public static string TranslateUrl(string word) { return "https://tw.dictionary.search.yahoo.com/search?p=" + System.Net.WebUtility.UrlEncode(word); }
    }
}
