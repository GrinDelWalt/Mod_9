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


namespace Mod_9
{
    class Program
    {
        static TelegramBotClient bot;

        static void Main(string[] args)
        {
            string token = File.ReadAllText(@"C:\Users\Гоша\Desktop\Token_bot.txt");

            string startUrl = $@"https://api.telegram.org/bot/";

            bot = new TelegramBotClient(token);

            bot.OnMessage += MessageReader;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void MessageReader(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string token = File.ReadAllText(@"C:\Users\Гоша\Desktop\Token_bot.txt");

            string text = $"{DateTime.Now.ToLongTimeString()}:  {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            string json = e.ToString();
            File.WriteAllText("Telegram.Json", json);

            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    string fileNamePhote = e.Message.Photo[e.Message.Photo.Length - 1].FileUniqueId + ".jpeg";
                    string fileIdPhoto = e.Message.Photo[e.Message.Photo.Length - 1].FileId;
                    Console.WriteLine(fileIdPhoto);
                    DownLoadFile(fileIdPhoto, fileNamePhote);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Audio:
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Voice:
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    DownLoadFile(e.Message.Document.FileId, e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileSize);
                    break;
               
            }

            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");
            //string path = @"D:\File";
            //if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            //{
            //    Console.WriteLine(e.Message.Document.FileName);
            //    Console.WriteLine(e.Message.Document.FileSize);

            //    Console.WriteLine(e.Message.Document.FileId);

               
            //}
            //else if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            //{
               
            //    var test = bot.GetFileAsync(e.Message.Photo[e.Message.Photo.Count() - 3].FileUniqueId);
            //    var download_url = @"https://api.telegram.org/file/bot<token>/" + test.Result.FilePath;
            //    Console.WriteLine(Convert.ToString(test));
            //        DownLoadPhoto(download_url, "photo.png");


            //    //DownLoadPhoto(e.Message.Photo[e.Message.Photo.Count() - 1].FileId, );
            //    //List<MyPhoto> myPhotoList;
            //    //MyPhoto photo = myPhotoList.FirstOrDefault(x => x.Number == 5);
            //}
            //else if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Voice)
            //{

            //}
            
        }
        static async void DownLoadFile(string fileId, string name)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(@"D:\File\" + name, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
        }
       
        

    }
}
 