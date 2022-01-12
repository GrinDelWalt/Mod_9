﻿using EmptyFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
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
                        WorkingWithArchive(msg.Text, msg.Chat.Id, e);
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
        /// Работа с архивом файлов
        /// </summary>
        /// <param name="text"></param>
        /// <param name="id"></param>
        private async void WorkingWithArchive(string text, long id, Telegram.Bot.Types.Update e)
        {
            switch (text)
            {
                case "/start":
                    await _client.SendTextMessageAsync(id, "Привет", replyMarkup: GetButtonse());
                    break;
                case "Архив Фото":
                    WorkingWithFile(id, "*.jpeg");
                    break;
                case "Аудио сообщение":
                    WorkingWithFile(id, "*.audio");
                    break;
                case "Документы":
                    WorkingWithDocuments(id, e);
                    break;
                case "Видео":
                    WorkingWithFile(id, "*.video");
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
                    Console.WriteLine($"Название фото: {fileIdPhoto}");
                    DownLoadFile(fileIdPhoto, fileNamePhote);
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Фото загружено");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    string formatVideo = e.Message.Video.MimeType;
                    formatVideo = new string(formatVideo.TakeWhile(x => x != '/').ToArray());
                    string nameVideo = e.Message.Video.FileName + $".{formatVideo}";
                    string idVideo = e.Message.Video.FileId;
                    Console.WriteLine($"Название видео: {nameVideo}");
                    DownLoadFile(idVideo,nameVideo);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Voice:
                    string formatVoice = e.Message.Voice.MimeType;
                    formatVoice = new string(formatVoice.TakeWhile(x => x != '/').ToArray());
                    string nameVoice = Convert.ToString(e.Message.MessageId) + $".{formatVoice}";
                    string idVoice = e.Message.Voice.FileId;
                    Console.WriteLine($"Аудио сообщение номер: {nameVoice} длительность: {e.Message.Voice.Duration} сек");
                    DownLoadFile(idVoice, nameVoice);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    DownLoadFile(e.Message.Document.FileId, e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileSize);
                    break;
            }
        }
        /// <summary>
        /// поиск, отправка фото
        /// </summary>
        /// <param name="a">chat ID</param>
        private async void WorkingWithFile(long a, string type)
        {
            string[] fotoList = Directory.GetFiles(@"D:\File\", type);
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
        /// сортировка по расширениям
        /// </summary>
        private void WorkingWithDocuments(long id, Telegram.Bot.Types.Update e)
        {
            //string[] documents = Directory.GetFiles(@"D:\File\", );
            string path = @"D:\File\";
            int trimLength = path.Length;
            DirectoryInfo dir = new DirectoryInfo(path);
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
            var queryGroupByExt =
                from file in fileList
                group file by file.Extension.ToLower() into fileGroup
                orderby fileGroup.Key
                select fileGroup;

            PageOutput(trimLength, queryGroupByExt, id, e);

        }
        
        private void PageOutput(int trimLength, IEnumerable<IGrouping<string, FileInfo>> queryGroupByExt, long id, Telegram.Bot.Types.Update e)
        {
            foreach (var item in queryGroupByExt)
            {
                var r = _client.SendTextMessageAsync(id, $"расширение: {item.Key}",replyMarkup: GetInLineButtonFile(item.Key)).Result;
            }

            queryGroupByExt.
            bool goAgain = true;

            int numLines = Console.WindowHeight - 3;

            foreach (var fileGroup in queryGroupByExt)
            {
                int line = 0;
                do
                {
                    Console.Clear();
                    Console.WriteLine(fileGroup.Key == String.Empty ? "[none]" : fileGroup.Key);

                    var resultPage = fileGroup.Skip(line).Take(numLines);
                    Console.WriteLine(fileGroup.Count());

                    foreach (var e in resultPage)
                    {
                        Console.WriteLine("\t{0}", e.FullName.Substring(trimLength));
                    }
                    line += numLines;
                    Console.WriteLine("press any key to continue or the 'End' key to break...");
                    ConsoleKey key = Console.ReadKey().Key;
                    
                    if (key == ConsoleKey.End)
                    {
                        goAgain = false;
                        break;
                    }

                } while (line < fileGroup.Count());
                if (goAgain == false) 
                    break;
            }
        }

        private static IReplyMarkup GetInLineButtonFile(string type)
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "выбрать расширение", CallbackData = type });
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
            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Скачать", CallbackData = path});
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
