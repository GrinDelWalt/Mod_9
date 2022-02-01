using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Mod_9
{
    class Button
    {
        /// <summary>
        /// кнопка в чате по выбору расширения
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IReplyMarkup GetInLineButtonFile(string type)
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "выбрать расширение", CallbackData = type });
        }

        /// <summary>
        /// кнопка старт
        /// </summary>
        /// <returns></returns>
        public IReplyMarkup GetButtonseStart()
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
        /// вывод кнопки с текущим временем и даты
        /// </summary>
        /// <returns></returns>
        public IReplyMarkup GetButtonseDate()
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
        public IReplyMarkup GetInLineButton(string path)
        {

            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Скачать", CallbackData = path });
        }
        /// <summary>
        /// кнопки для выбора типа поиска файлов
        /// </summary>
        /// <returns></returns>
        public IReplyMarkup GetButtonse()
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
    }
}
