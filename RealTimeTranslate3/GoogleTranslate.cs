using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTranslate3
{
    static class GoogleTranslate
    {
        public static string TranslateUrlEC(string word) { return "https://translate.google.com.tw/#view=home&op=translate&sl=auto&tl=zh-TW&text=" + System.Net.WebUtility.UrlEncode(word); }
        public static string TranslateUrlCE(string word) { return "https://translate.google.com.tw/#view=home&op=translate&sl=auto&tl=en&text=" + System.Net.WebUtility.UrlEncode(word); }
        private static bool IsChinese(char c) { return '\u4e00' <= c && c <= '\u9fff'; }
        static bool IsEnglish(string word) { return word.All(c => !IsChinese(c)); }
        public static string TranslateUrlAuto(string word) { return IsEnglish(word) ?  TranslateUrlEC(word): TranslateUrlCE(word); }
    }
}
