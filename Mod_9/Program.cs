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

namespace Mod_9
{
    class Program
    {
        
        static void Main(string[] args)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                TelegraBotHelper hlp = new TelegraBotHelper(token: File.ReadAllText(@"C:\Users\Гоша\Desktop\Token_bot.txt"));
                hlp.GetUpdates();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
           
        }
        
       
        

    }
}
 