using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Mod_9
{
    internal class TelegraBotHelper
    {
        

        Telegram.Bot.TelegramBotClient _client;
        private readonly string _token;
        private Dictionary<long, UserState> _clientStates = new Dictionary<long, UserState>();
        /// <summary>
        /// токен
        /// </summary>
        /// <param name="token"></param>
        public TelegraBotHelper(string token)
        {
            this._token = token;
        }
        /// <summary>
        /// проверка на наличие новых данных + Timeout
        /// </summary>
        internal void GetUpdates()
        {
            _client = new Telegram.Bot.TelegramBotClient(_token);
            var me = _client.GetMeAsync().Result;
            if (me != null && !string.IsNullOrEmpty(me.Username))
            {
                int offset = 0;
                while (true)
                {
                    try
                    {
                        var updates = _client.GetUpdatesAsync(offset).Result;
                        if (updates != null && updates.Count() > 0)
                        {
                            foreach (var e in updates)
                            {
                                MessageReader(e);
                                offset = e.Id + 1;
                            }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }

                    Thread.Sleep(1000);
                }
            }
        }
        private async void MessageReader(Telegram.Bot.Types.Update e)
        {
            var msg = e.Message;


            switch (e.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    if (msg.Text != null)
                    {
                        WorkingWithArchive(msg.Text, msg.Chat.Id);
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    using (FileStream downlode = File.OpenRead(e.CallbackQuery.Data))
                    {
                        await _client.SendPhotoAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
                    }
                    break;
                default:
                    Console.WriteLine("необработанный тип данных");
                    break;
            }
            
            
            if (e.Message != null)
            {
                TypeFile(e);
            }
            ConsoleStatus(e);
        }
        private void ConsoleStatus(Telegram.Bot.Types.Update e)
        {
            if (e.Message != null)
            {
                string text = $"{DateTime.Now.ToLongTimeString()}:  {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
                Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");
            }
        }
        /// <summary>
        /// поиск, отправка фото
        /// </summary>
        /// <param name="a">chat ID</param>
        private async void WorkingWithPhotos(long a)
        {
            string[] fotoList = Directory.GetFiles(@"D:\File\", "*.Jpeg");
            Dictionary<int, string> path = new Dictionary<int, string>();
            int inckrement = 0;
            if (fotoList.Length == 0)
            {
                await _client.SendTextMessageAsync(a, "Упс, похоже архив пуст");
            }
            else
            {
                foreach (var item in fotoList)
                {
                    inckrement++;
                    path.Add(inckrement, item);
                    using (FileStream stream = File.OpenRead(item))
                    {
                        var r = _client.SendTextMessageAsync(a, item, replyMarkup: GetInLineButton(item)).Result;
                    }
                }
            }
        }
        /// <summary>
        /// Работа с архивом файлов
        /// </summary>
        /// <param name="text"></param>
        /// <param name="id"></param>
        private async void WorkingWithArchive(string text, long id)
        {
            switch (text)
            {
                case "/start":
                    await _client.SendTextMessageAsync(id, "Привет", replyMarkup: GetButtonse());
                    break;
                case "Архив Фото":
                    WorkingWithPhotos(id);
                    break;
                case "Аудио сообщение":
                    break;
                case "Документы":
                    break;
                case "Видео":
                    break;
                default:
                    await _client.SendTextMessageAsync(id, "Привет, я не понимаю тебя, возможно я еще не умею делать то чего ты хочешь, обратись к моему создателю и возможно он научит меня тому что тебе нужно:) для Запуска нажми: start",
                        replyMarkup: GetButtonseStart());
                    break;
            }
        }
        private async void TypeFile(Telegram.Bot.Types.Update e)
        {
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    string fileNamePhote = e.Message.Photo[e.Message.Photo.Length - 1].FileUniqueId + ".jpeg";
                    string fileIdPhoto = e.Message.Photo[e.Message.Photo.Length - 1].FileId;
                    Console.WriteLine(fileIdPhoto);
                    DownLoadFile(fileIdPhoto, fileNamePhote);
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Фото загружено");
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
        }

        /// <summary>
        /// кнопка старт
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup GetButtonseStart()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{new KeyboardButton { Text = "/start" } }
                }
            };
        }
        /// <summary>
        /// кнопка в чате "скачать"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IReplyMarkup GetInLineButton(string path)
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Скачать", CallbackData = path.ToString()});
        }
        /// <summary>
        /// кнопки для выбора типа поиска файлов
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup GetButtonse()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{new KeyboardButton { Text = "Архив Фото"}, new KeyboardButton { Text = "Аудио сообщение"} },
                    new List<KeyboardButton>{new KeyboardButton { Text = "Документы"}, new KeyboardButton { Text = "Видео"} }
                }
            };
        }
        /// <summary>
        /// метод загрузки данных из ТГ бота на ПК
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="name"></param>
        async void DownLoadFile(string fileId, string name)
        {
            var file = await _client.GetFileAsync(fileId);
            FileStream fs = new FileStream(@"D:\File\" + name, FileMode.Create);
            await _client.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
        }
    }
}
