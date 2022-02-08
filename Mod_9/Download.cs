using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_9
{
    class Download
    {
        private Telegram.Bot.Types.Update e;
        private Telegram.Bot.TelegramBotClient _client;

        private string fileExtension;
        private string filePath;
        private string fileMessage;

        public Download(Telegram.Bot.Types.Update e, Telegram.Bot.TelegramBotClient _client, string fileExtension, string fileMessage, string filePath)
        {
            this.e = e;
            this._client = _client;
            this.fileExtension = fileExtension;
            this.fileMessage = fileMessage;
            this.filePath = filePath;
            
        }

        /// <summary>
        /// выгрузка документа
        /// </summary>
        /// <param name="v"></param>
        public async void DownLoadFile(string v)
        {
            await _client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendDocumentAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// выгрузка аудио записи
        /// </summary>
        /// <param name="v"></param>
        public async void DownloadAudio(string v)
        {
            await _client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendAudioAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// выгрузка видео
        /// </summary>
        /// <param name="v"></param>
        public async void DownloadVideo(string v)
        {
            await _client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendVideoAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// выгрузка фото
        /// </summary>
        /// <param name="v"></param>
        public async void DownloadPhoto(string v)
        {
            await _client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await _client.SendPhotoAsync(e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// метод загрузки данных из ТГ бота на ПК
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="name"></param>
        public async void UploadingFile()
        {
            long id = e.Message.Chat.Id;
            await _client.SendTextMessageAsync(id, "Загрузка");
          
            var file = await _client.GetFileAsync(filePath);
            try
            {
                using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\File\\" + e.Message.Text + fileExtension, FileMode.CreateNew))
                await _client.DownloadFileAsync(file.FilePath, fs);
            }
            catch (IOException)
            {
                for (int i = 1; i < 1000; i++)
                {
                    bool fileAvailability = File.Exists(Environment.CurrentDirectory + "\\File\\" + e.Message.Text + $"({i})" + fileExtension);
                    if (fileAvailability == false)
                    {
                        FileInfo fl = new FileInfo(Environment.CurrentDirectory + "\\File\\" + e.Message.Text + fileExtension);
                        string[] name = fl.Name.Split('.');
                        await _client.SendTextMessageAsync(id, $"такое имя уже существует создан файл с именем: {name[0] + $"({i})" + fileExtension}");
                        using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\File\\" + name[0] + $"({i})" + fileExtension, FileMode.CreateNew))
                        await _client.DownloadFileAsync(file.FilePath, fs);
                        break;
                    }
                }
            }
            await _client.SendTextMessageAsync(id, $"{fileMessage}");
        }
    }
}
