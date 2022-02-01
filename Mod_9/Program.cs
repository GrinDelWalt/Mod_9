using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace Mod_9
{
    class Program
    {
        
        static void Main(string[] args)
        {


            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                TelegraBotHelper hlp = new TelegraBotHelper(token: File.ReadAllText(@"C:\Users\Гоша\Desktop\Token_bot.txt"));
                hlp.GetUpdates();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

        }
    }
}
 