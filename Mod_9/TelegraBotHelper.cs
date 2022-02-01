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

        public Telegram.Bot.Types.Update e;
        IEnumerable<IGrouping<string, FileInfo>> queryGroupByExt;

        public string fileExtension;
        public string filePath;
        public string fileMessage;
        private bool callback;

        public Button button;

        string path;

        public Telegram.Bot.TelegramBotClient _client;
        private readonly string _token;
        //private Dictionary<long, UserState> _clientStates = new Dictionary<long, UserState>();
        /// <summary>
        /// токен
        /// </summary>
        /// <param name="token"></param>
        public TelegraBotHelper(string token)
        {
            button = new Button();
            
            this._token = token;
        }

        /// <summary>
        /// проверка на наличие новых данных + Timeout
        /// </summary>
        internal void GetUpdates()
        {
            if (path == null)
            {
                path = Environment.CurrentDirectory;
            }
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
        /// <summary>
        /// определения типа входных данных
        /// </summary>
        private async void MessageReader()
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
                    if (this.callback)
                    {
                        await _client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"выбранно: {e.CallbackQuery.Data}");
                        DocProcessing();
                    }
                    else
                    {
                        Callback(e.CallbackQuery.Data);
                    }
                   
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

        /// <summary>
        /// вывод данных в консоль
        /// </summary>
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
            Download dow = new Download(e, _client, fileExtension, fileMessage, filePath);
            long id = e.Message.Chat.Id;
            switch (text)
            {
                case "/start":
                    await _client.SendTextMessageAsync(id, "Привет", replyMarkup: button.GetButtonse());
                    break;
                case "Архив Фото":
                    WorkingWithFile("*.jpeg");
                    break;
                case "Аудио сообщение":
                    WorkingWithFile("*.ogg");
                    break;
                case "Документы":
                    WorkingWithDocuments();
                    break;
                case "Видео":
                    WorkingWithFile("*.mp4");
                    break;
                default:
                    if (this.filePath == null)
                    {
                        await _client.SendTextMessageAsync(id, "Привет, я не понимаю тебя, возможно я еще не умею делать то чего ты хочешь, обратись к моему создателю и возможно он научит меня тому что тебе нужно:) для Запуска нажми: start",
                        replyMarkup: button.GetButtonseStart());
                    }
                    else
                    {
                        dow.UploadingFile();
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
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите название фотографии или нажмите кнопку дата и фото будет присвоено названия текущего времени и даты по МСК", replyMarkup: button.GetButtonseDate());
                    this.fileExtension = ".jpeg";
                    this.filePath = fileIdPhoto;
                    this.fileMessage = "Фото загружено";
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    string formatVideo = e.Message.Video.MimeType;
                    formatVideo = Expansion(formatVideo);
                    this.fileExtension = $".{formatVideo}";
                    this.filePath = e.Message.Video.FileId;
                    this.fileMessage = "Видео загружено";
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите название видео или нажмите кнопку дата и фото будет присвоено названия текущего времени и даты по МСК", replyMarkup: button.GetButtonseDate());
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Voice:
                    string formatVoice = e.Message.Voice.MimeType;
                    formatVoice = Expansion(formatVoice);
                    this.fileExtension = $".{formatVoice}";
                    this.filePath = e.Message.Voice.FileId;
                    this.fileMessage = "Аудио запись загружена";
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите название аудио записи или нажмите кнопку дата и фото будет присвоено названия текущего времени и даты по МСК", replyMarkup: button.GetButtonseDate());
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    string formatDoc = e.Message.Document.MimeType;
                    formatDoc = Expansion(formatDoc);
                    this.fileExtension = $".{formatDoc}";
                    this.filePath = e.Message.Document.FileId;
                    this.fileMessage = "Документ загружен";
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите название Файла или нажмите кнопку дата и фото будет присвоено названия текущего времени и даты по МСК", replyMarkup: button.GetButtonseDate());
                    break;
            }
        }
        /// <summary>
        /// возврат формата файла
        /// </summary>
        /// <param name="expansion"></param>
        /// <returns></returns>
        private string Expansion(string expansion)
        {
            return expansion = expansion.Substring(expansion.IndexOf('/') + 1);
        }
        /// <summary>
        /// поиск, отправка файлов
        /// </summary>
        /// <param name="a">chat ID</param>
        private async void WorkingWithFile(string type)
        {
            
            long id = e.Message.Chat.Id;
            string[] fotoList = Directory.GetFiles(this.path + "\\File", type);
            Dictionary<int, string> path = new Dictionary<int, string>();
            int inckrement = 0;
            if (fotoList.Length == 0)
            {
                await _client.SendTextMessageAsync(id, "Упс, похоже архив пуст");
            }
            else
            {
                foreach (var item in fotoList)
                {
                    inckrement++;
                    path.Add(inckrement, item);

                    FileInfo file = new FileInfo(item);
                    string data = file.Extension.ToLower();
                    data = string.Join(",", file.Name, data);
                    var r = _client.SendTextMessageAsync(id, file.Name, replyMarkup: button.GetInLineButton(data)).Result;
                }
            }
        }
        /// <summary>
        /// опеределения формата файла дял загрузки
        /// </summary>
        /// <param name="path"></param>
        private void Callback(string path)
        {
            Download dow = new Download(e, _client, fileExtension, fileMessage, filePath);
            string[] data = path.Split(',');
            if (data.Length == 2)
            {
                switch (data[1])
                {
                    case ".jpeg":
                        dow.DownloadPhoto(this.path + "\\File\\" + data[0]);
                        break;
                    case ".mp4":
                        dow.DownloadVideo(this.path + "\\File\\" + data[0]);
                        break;
                    case ".ogg":
                        dow.DownloadAudio(this.path + "\\File\\" + data[0]);
                        break;
                    default:
                        dow.DownLoadFile(this.path + "\\File\\" + data[0]);
                        break;
                }
            }
        }
        /// <summary>
        /// сортировка по расширениям
        /// </summary>
        private void WorkingWithDocuments()
        {
            
            DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory + "\\File");
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
            var queryGroupByExt =
                from file in fileList
                group file by file.Extension.ToLower() into fileGroup
                orderby fileGroup.Key
                select fileGroup;

            this.queryGroupByExt = queryGroupByExt;
            KeyProcessing();
        }
        /// <summary>
        /// вывод скиска формата файлов
        /// </summary>
        /// <param name="trimLength"></param>
        private void KeyProcessing()
        {
            long id = e.Message.Chat.Id;
            foreach (var fileGroup in this.queryGroupByExt)
            {
                var r = _client.SendTextMessageAsync(id, $"расширение: {fileGroup.Key}", replyMarkup: button.GetInLineButtonFile(fileGroup.Key)).Result;
            }
            this.callback = true;
        }
        /// <summary>
        /// вывод файлов по ключу
        /// </summary>
        private void DocProcessing()
        {
            
            foreach (var fileGroup in this.queryGroupByExt)
            {
                if (e.CallbackQuery != null && fileGroup.Key == e.CallbackQuery.Data)
                {
                    foreach (var item in fileGroup)
                    {
                        FileInfo type = new FileInfo(Convert.ToString(item));
                        string dataDoc = type.Extension.ToLower();
                        dataDoc = string.Join(",", Convert.ToString(item.Name), dataDoc);
                        var dok = _client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, Convert.ToString(item), replyMarkup: button.GetInLineButton(dataDoc)).Result;
                    }
                }
            }
            this.callback = false;
        }
    }
}
