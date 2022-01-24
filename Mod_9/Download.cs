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
        Telegram.Bot.Types.Update e;
        Telegram.Bot.TelegramBotClient _client;

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
    }
}
