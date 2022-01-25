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
        public TelegraBotHelper tel;
        
        public Download()
        {
            tel = new TelegraBotHelper();
        }

        /// <summary>
        /// выгрузка документа
        /// </summary>
        /// <param name="v"></param>
        public async void DownLoadFile(string v)
        {
            await tel._client.SendTextMessageAsync(tel.e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await tel._client.SendDocumentAsync(tel.e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// выгрузка аудио записи
        /// </summary>
        /// <param name="v"></param>
        public async void DownloadAudio(string v)
        {
            await tel._client.SendTextMessageAsync(tel.e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await tel._client.SendAudioAsync(tel.e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// выгрузка видео
        /// </summary>
        /// <param name="v"></param>
        public async void DownloadVideo(string v)
        {
            await tel._client.SendTextMessageAsync(tel.e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await tel._client.SendVideoAsync(tel.e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// выгрузка фото
        /// </summary>
        /// <param name="v"></param>
        public async void DownloadPhoto(string v)
        {
            await tel._client.SendTextMessageAsync(tel.e.CallbackQuery.Message.Chat.Id, "Загрузка...");
            using (FileStream downlode = File.OpenRead(v))
            {
                await tel._client.SendPhotoAsync(tel.e.CallbackQuery.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(downlode));
            }
        }
        /// <summary>
        /// метод загрузки данных из ТГ бота на ПК
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="name"></param>
        public async void UploadingFile()
        {
            long id = tel.e.Message.Chat.Id;
            await tel._client.SendTextMessageAsync(id, "Загрузка");
            string fileExtension = tel.fileExtension;
            string filePath = tel.filePath;
            string fileMessage = tel.fileMessage;
            tel.fileExtension = null;
            tel.filePath = null;
            tel.fileMessage = null;
            var file = await tel._client.GetFileAsync(filePath);
            FileStream fs = new FileStream(@"D:\File\" + tel.e.Message.Text + fileExtension, FileMode.Create);
            await tel._client.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();

            await tel._client.SendTextMessageAsync(id, $"{fileMessage}");
        }
    }
}
