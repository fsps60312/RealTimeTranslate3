using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient client = new WebClient();
            client.DownloadFile("https://translate.google.com.tw/#view=home&op=translate&sl=auto&tl=zh-TW&text=lalala", "tmp.html");
        }
    }
}
