using EmptyFiles;
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
        Telegram.Bot.Types.Update e;
        IEnumerable<IGrouping<string, FileInfo>> queryGroupByExt;
        string fileType;
        string filePath;

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
                                this.e = e;
                                MessageReader();
                                offset = e.Id + 1;
                            }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }

                    Thread.Sleep(1000);
                }
            }
        }
        private void MessageReader()
        {
            var msg = e.Message;


            switch (e.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    if (msg.Text != null)
                    {
                        WorkingWithArchive(msg.Text);
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    string path = e.CallbackQuery.Data;
                    Callback(path);
                    break;
                default:
                    Console.WriteLine("необработанный тип данных");
                    break;
            }
            
            
            if (e.Message != null)
            {
                TypeFile();
            }
            ConsoleStatus();
        }


        private void ConsoleStatus()
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
        private async void WorkingWithArchive(string text)
        {
            long id = e.Message.Chat.Id;
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
                    WorkingWithDocuments(id);
                    break;
                case "Видео":
                    WorkingWithFile(id, "*.video");
                    break;
                default:
                    if (this.filePath == null)
                    {
                        await _client.SendTextMessageAsync(id, "Привет, я не понимаю тебя, возможно я еще не умею делать то чего ты хочешь, обратись к моему создателю и возможно он научит меня тому что тебе нужно:) для Запуска нажми: start",
                        replyMarkup: GetButtonseStart());
                    }
                    else
                    {
                        UploadingFile();
                    }
                    break;
            }
        }
        /// <summary>
        /// сохронение файлов из ТБ
        /// </summary>
        private async void TypeFile()
        {
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    string fileIdPhoto = e.Message.Photo[e.Message.Photo.Length - 1].FileId;
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите название фотографии или нажмите кнопку дата и фото будет присвоено названия текущего времени и даты по МСК", replyMarkup: GetButtonseDate());
                    this.fileType = ".jpeg";
                    this.filePath = fileIdPhoto;
                    //UploadingFile(fileNamePhoto, fileIdPhoto);
                    //await _client.SendTextMessageAsync(e.Message.Chat.Id, "Фото загружено");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    string formatVideo = e.Message.Video.MimeType;
                    formatVideo = new string(formatVideo.TakeWhile(x => x != '/').ToArray());
                    string nameVideo = e.Message.Video.FileName + $".{formatVideo}";
                    string idVideo = e.Message.Video.FileId;
                    Console.WriteLine($"Название видео: {nameVideo}");
                    //UploadingFile(idVideo,nameVideo);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Voice:
                    string formatVoice = e.Message.Voice.MimeType;
                    formatVoice = new string(formatVoice.TakeWhile(x => x != '/').ToArray());
                    string nameVoice = Convert.ToString(e.Message.MessageId) + $".{formatVoice}";
                    string idVoice = e.Message.Voice.FileId;
                    Console.WriteLine($"Аудио сообщение номер: {nameVoice} длительность: {e.Message.Voice.Duration} сек");
                    //UploadingFile(idVoice, nameVoice);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    //UploadingFile(e.Message.Document.FileId, e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileSize);
                    break;
            }
        }

        

        /// <summary>
        /// поиск, отправка файлов
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

                    FileInfo file = new FileInfo(item);
                    string data = file.Extension.ToLower();
                    data = string.Join(",", item, data);
                    var r = _client.SendTextMessageAsync(a, item, replyMarkup: GetInLineButton(data)).Result;
                }
            }
        }
        

        private void Callback(string path)
        {
            string[] data = path.Split(',');
            switch (data[1])
            {
                case ".jpeg":
                    DownloadPhoto(data[0]);
                    break;
                case ".video":
                    DownloadVideo(data[0]);
                    break;
                case ".audio":
                    DownloadAudio(data[0]);
                    break;

                default:
                    DownLoadFile(data[0]);
                    break;
            }
            
        }
        /// <summary>
        /// сортировка по расширениям
        /// </summary>
        private void WorkingWithDocuments(long id)
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

            this.queryGroupByExt = queryGroupByExt;
            KeyProcessing(trimLength, id);
        }

        private void KeyProcessing(int trimLength, long id)
        {

            foreach (var fileGroup in this.queryGroupByExt)
            {
                var r = _client.SendTextMessageAsync(id, $"расширение: {fileGroup.Key}", replyMarkup: GetInLineButtonFile(fileGroup.Key)).Result;
            }

            foreach (var fileGroup in this.queryGroupByExt)
            {
                for (int i = 0; i < this.queryGroupByExt.Count(); i++)
                {
                    if (e.CallbackQuery != null && fileGroup.Key == e.CallbackQuery.Data)
                    {
                        foreach (var item in fileGroup)
                        {
                            var dok = _client.SendTextMessageAsync(id, item.FullName.Substring(trimLength), replyMarkup: GetInLineButton(Convert.ToString(item))).Result;
                        }
                    }
                }
            }


            
            //int line = 0;
            //do
            //{
            //    Console.Clear();
            //    Console.WriteLine(fileGroup.Key == String.Empty ? "[none]" : fileGroup.Key);

            //    var resultPage = fileGroup;
            //    Console.WriteLine(fileGroup.Count());

            //    foreach (var item in fileGroup)
            //    {
            //        Console.WriteLine("\t{0}", item.FullName.Substring(trimLength));
            //    }
            //    line += numLines;
            //    Console.WriteLine("press any key to continue or the 'End' key to break...");
            //    ConsoleKey key = Console.ReadKey().Key;

            //    if (key == ConsoleKey.End)
            //    {
            //        goAgain = false;
            //        break;
            //    }

            //} while (line < fileGroup.Count());
            //if (goAgain == false) 
            //    break;
        }
        private async void DownLoadFile(string v)
        {
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendDocumentAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }

        private async void DownloadAudio(string v)
        {
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendAudioAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }

        private async void DownloadVideo(string v)
        {
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendVideoAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }

        private async void DownloadPhoto(string v)
        {
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendPhotoAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
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
        private static IReplyMarkup GetButtonseDate()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{new KeyboardButton { Text = DateTime.Now.ToString("dd'.'MM'.'yyyy' 'HH'.'mm'.'ss") } }
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
        async void UploadingFile()
        {

            var file = await _client.GetFileAsync(filePath);
            FileStream fs = new FileStream(@"D:\File\" + e.Message.Text + fileType, FileMode.Create);
            await _client.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();

            this.fileType = null;
            this.filePath = null;
        }
    }
}
