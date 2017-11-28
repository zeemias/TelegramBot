using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Telegram.Bot.Types;
using DeltaCreditBot.Models;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net.Mail;
using System.IO;
using Telegram.Bot.Types.InlineKeyboardButtons;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Globalization;

namespace DeltaCreditBot.Controllers
{
    public class MessageController : ApiController
    {
        private Object thisLock = new Object();
        private Object LockSend = new Object();

        [Route(@"api/message/update")]
        public async Task<OkResult> Update([FromBody]Update update)
        {
            var message = update.Message;
            var client = await Bot.Get();
            var chatId = message.Chat.Id;
            try
            {
                if (AppSettings.Users.Any(t => t.ChatId == chatId) == false)
                {
                    lock (thisLock)
                    {
                        AppSettings.Users.Add(new BotUser { ChatId = chatId, State = "Диалог" });
                    }
                }
                BotUser user = AppSettings.Users.FirstOrDefault(t => t.ChatId == chatId);

                if(message.Text == null)
                {
                    user.PhotosId.Add(message.Photo.Where(t => t.FileId != null).LastOrDefault().FileId);
                    message.Text = "Фото";
                }

                if (message.Text.StartsWith("/start") || message.Text.StartsWith("В главное меню"))
                {
                    await client.SendTextMessageAsync(chatId, "Вас приветствует справочный бот Банка ДельтаКредит. \nВы являетесь клиентом Банка?",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Да, я клиент Банка!"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Нет, очень хочу им стать!"),
                                                        }
                                                    }));
                    user.State = "Диалог";
                    user.PhotosId.Clear();
                }
                else if (message.Text.StartsWith("Да, я клиент Банка!"))
                {
                    await client.SendTextMessageAsync(chatId, "Для получения консультации по любым интересующим Вас вопросам рекомендуем обратиться по телефону горячей линии: 8-800-200-0707.",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Оставить обратную связь"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Нет, очень хочу им стать!"))
                {
                    await client.SendTextMessageAsync(chatId, "Выберете действие",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Подать заявку на кредит"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Заявка на консультацию"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("ДельтаЭкспресс – онлайн ипотека"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("ДельтаЭкспресс – онлайн ипотека"))
                {
                    await client.SendTextMessageAsync(chatId, "ДельтаЭкспресс - инновационный сервис, позволяющий получить предварительное одобрение кредита онлайн, узнать сумму кредита, на которую вы можете рассчитывать, загрузить фото и копии необходимых документов. И все это без визита в офис!",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Перейти на сайт"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Я хочу дослать документы"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Забыл пароль от личного кабинета"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Оставить обратную связь"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Забыл пароль от личного кабинета"))
                {
                    await client.SendTextMessageAsync(chatId, "Восстановить пароль Вы можете на сайте ДельтаЭкспресс, выбрав пункт «забыли пароль»",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Перейти на сайт"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Перейти на сайт"))
                {
                    await client.SendTextMessageAsync(chatId, "Нажмите на кнопку, чтобы перейти на сайт",
                                                    replyMarkup: new InlineKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new InlineKeyboardUrlButton("ДельтаКредит", "https://de.deltacredit.ru"),
                                                        }
                                                    }));
                    await client.SendTextMessageAsync(chatId, "Или введите URL в адресной строке браузера: de.deltacredit.ru",
                                                   replyMarkup: new ReplyKeyboardMarkup(new[]
                                                   {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                   }));

                }
                else if (message.Text.StartsWith("Оставить обратную связь"))
                {
                    await client.SendTextMessageAsync(chatId, "Введите текст сообщения",
                                                   replyMarkup: new ReplyKeyboardMarkup(new[]
                                                   {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                   }));
                    user.State = "Обратная связь";
                }
                else if (message.Text.StartsWith("Подать заявку на кредит"))
                {
                    await client.SendTextMessageAsync(chatId, "Укажите пожалуйста Ваши ФИО",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Заявка-ФИО";
                }
                else if (message.Text.StartsWith("Заявка на консультацию"))
                {
                    await client.SendTextMessageAsync(chatId, "Укажите Ваш номер телефона",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Консультация-номер телефона";
                }
                else if (message.Text.StartsWith("Я хочу дослать документы"))
                {
                    await client.SendTextMessageAsync(chatId, "Укажите Ваш номер телефона при регистрации",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Документы-номер телефона";

                }
                else if (user.State == "Обратная связь")
                {
                    if (SendEmail("Клиент оставил обратную связь", message.Text))
                    {
                        await client.SendTextMessageAsync(chatId, "Сообщение отправлено!",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Сообщение не отправлено. Попробуйте позже.",
                                                   replyMarkup: new ReplyKeyboardMarkup(new[]
                                                   {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                   }));
                    }
                    user.State = "Диалог";
                }
                else if (user.State == "Консультация-номер телефона")
                {
                    if (RegexPhone(message.Text))
                    {
                        user.PhoneNumber = message.Text;
                        await client.SendTextMessageAsync(chatId, "Как к Вам можно обращаться?",
                                                        replyMarkup: new ReplyKeyboardMarkup(new[]
                                                        {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                        }));
                        user.State = "Консультация-имя";
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Неправильный номер телефона. Попробуйте еще раз",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }

                }
                else if (user.State == "Консультация-имя")
                {
                    user.Name = message.Text;
                    string mailBody = "Имя клиента: " + user.Name + "<br>Телефон: " + user.PhoneNumber;
                    if (SendEmail("Клиент заказал звонок в чат-боте", mailBody))
                    {
                        await client.SendTextMessageAsync(chatId, "Наш сотрудник в ближайшее время свяжется с Вами!",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Заявка не отправлена. Попробуйте позже.",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    user.State = "Диалог";
                }
                else if (user.State == "Документы-номер телефона")
                {
                    if (RegexPhone(message.Text))
                    {
                        user.PhoneNumber = message.Text;
                        await client.SendTextMessageAsync(chatId, "Загрузите Ваши документы",
                                                        replyMarkup: new ReplyKeyboardMarkup(new[]
                                                        {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                        }));
                        user.State = "Документы";
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Неправильный номер телефона. Попробуйте еще раз",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                }
                else if (user.State == "Документы")
                {
                    await client.SendTextMessageAsync(chatId, "Все фото загружены, для отправки, нажмите отправить",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Отправить"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));

                    user.State = "Диалог";
                }
                else if (user.State == "Заявка-ФИО")
                {
                    user.Name = message.Text;
                    await client.SendTextMessageAsync(chatId, "Укажите дату Вашего рождения (в формате ДД.ММ.ГГГГ)",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));

                    user.State = "Заявка-Дата";
                }
                else if (user.State == "Заявка-Дата")
                {
                    if (RegexDate(message.Text))
                    {
                        user.Birthday = DateTime.ParseExact(message.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        await client.SendTextMessageAsync(chatId, "Укажите Ваш номер телефона",
                                                        replyMarkup: new ReplyKeyboardMarkup(new[]
                                                        {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                        }));
                        user.State = "Заявка-номер телефона";
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Неправильная дата рождения. Попробуйте еще раз",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                }
                else if (user.State == "Заявка-номер телефона")
                {
                    if (RegexPhone(message.Text))
                    {
                        user.PhoneNumber = message.Text;
                        await client.SendTextMessageAsync(chatId, "Укажите Ваш адрес электронной почты",
                                                        replyMarkup: new ReplyKeyboardMarkup(new[]
                                                        {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                        }));
                        user.State = "Заявка-почта";
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Неправильный номер телефона. Попробуйте еще раз",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                }
                else if (user.State == "Заявка-почта")
                {
                    if (RegexEmail(message.Text))
                    {
                        user.Email = message.Text;
                        user.State = "Диалог";
                        await client.SendTextMessageAsync(chatId, "Выберете Ваше семейное положение",
                                                        replyMarkup: new ReplyKeyboardMarkup(new[]
                                                        {
                                                            new[]
                                                            {
                                                                new KeyboardButton("Замужем/Женат"),
                                                            },
                                                            new[]
                                                            {
                                                                new KeyboardButton("Холост"),
                                                            },
                                                            new[]
                                                            {
                                                                new KeyboardButton("В разводе"),
                                                            },
                                                            new[]
                                                            {
                                                                new KeyboardButton("Вдова/вдовец"),
                                                            },
                                                            new[]
                                                            {
                                                                new KeyboardButton("Гражданский брак"),
                                                            },
                                                            new[]
                                                            {
                                                                new KeyboardButton("В главное меню"),
                                                            }

                                                        }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Неправильный номер телефона. Попробуйте еще раз",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                }
                else if (message.Text.StartsWith("Замужем/Женат"))
                {
                    user.MaritalStatus = "Замужем/Женат";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Работа по найму"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Собственник бизнеса"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Пенсионер"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Адвокат"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Диалог";
                }
                else if (message.Text.StartsWith("Холост"))
                {
                    user.MaritalStatus = "Холост";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Работа по найму"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Собственник бизнеса"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Пенсионер"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Адвокат"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Диалог";
                }
                else if (message.Text.StartsWith("В разводе"))
                {
                    user.MaritalStatus = "В разводе";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Работа по найму"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Собственник бизнеса"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Пенсионер"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Адвокат"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Диалог";
                }
                else if (message.Text.StartsWith("Вдова/вдовец"))
                {
                    user.MaritalStatus = "Вдова/вдовец";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Работа по найму"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Собственник бизнеса"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Пенсионер"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Адвокат"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Диалог";
                }
                else if (message.Text.StartsWith("Гражданский брак"))
                {
                    user.MaritalStatus = "Гражданский брак";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Работа по найму"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Собственник бизнеса"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Пенсионер"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Адвокат"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Диалог";
                }
                else if (message.Text.StartsWith("Работа по найму"))
                {
                    user.Status = "Работа по найму";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("2-НДФЛ"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Форма Банка"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Собственник бизнеса"))
                {
                    user.Status = "Собственник бизнеса";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("2-НДФЛ"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Форма Банка"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Пенсионер"))
                {
                    user.Status = "Пенсионер";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("2-НДФЛ"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Форма Банка"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Адвокат"))
                {
                    user.Status = "Адвокат";
                    await client.SendTextMessageAsync(chatId, "Выберете вашу форму занятости",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("2-НДФЛ"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Форма Банка"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("2-НДФЛ"))
                {
                    user.Statement = "2-НДФЛ";
                    await client.SendTextMessageAsync(chatId, "У Вас документы уже на руках?",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Да"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Нет"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Форма Банка"))
                {
                    user.Statement = "Форма Банка";
                    await client.SendTextMessageAsync(chatId, "У Вас документы уже на руках?",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("Да"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("Нет"),
                                                        },
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                }
                else if (message.Text.StartsWith("Да"))
                {
                    user.AllDocuments = "Да";
                    await client.SendTextMessageAsync(chatId, "Приложите фотографии Вашего паспорта",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    user.State = "Заявка-паспорт";
                }
                else if (user.State == "Заявка-паспорт")
                {
                    await client.SendTextMessageAsync(chatId, "Приложите фотографии трудовой книжки",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Заявка-трудовая";
                }
                else if (user.State == "Заявка-трудовая")
                {
                    await client.SendTextMessageAsync(chatId, "Приложите фотографию документа подтверждающего доход",
                                                    replyMarkup: new ReplyKeyboardMarkup(new[]
                                                    {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                    }));
                    user.State = "Заявка-отправка";
                }
                else if (user.State == "Заявка-отправка")
                {
                    string mailBody = SendMailBody();
                    List<Stream> stream = new List<Stream>();
                    for (int i = 0; i < user.PhotosId.Count; i++)
                    {
                        var files = await client.GetFileAsync(user.PhotosId[i]);
                        stream.Add(files.FileStream);
                    }
                    if (SendEmail("Клиент оставил заявку в чат-боте", mailBody, stream))
                    {
                        await client.SendTextMessageAsync(chatId, "Ваша заявка принята. В ближайшее время наш сотрудник свяжется с Вами!",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Заявка не отправлена. Попробуйте позже.",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    user.State = "Диалог";
                }
                else if (message.Text.StartsWith("Нет"))
                {
                    user.AllDocuments = "Нет";
                    string mailBody = SendMailBody();
                    if (SendEmail("Клиент оставил заявку в чат-боте", mailBody))
                    {
                        await client.SendTextMessageAsync(chatId, "Документы получены Вашим персональным менеджером. Ожидайте звонка специалиста!",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Документы не отправлены. Попробуйте позже.",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    user.State = "Диалог";
                }
                else if (message.Text.StartsWith("Отправить"))
                {
                    List<Stream> stream = new List<Stream>();
                    for (int i = 0; i < user.PhotosId.Count; i++)
                    {
                        var files = await client.GetFileAsync(user.PhotosId[i]);
                        stream.Add(files.FileStream);
                    }
                    if (SendEmail("Клиент дослал документы " + user.PhoneNumber, "Документы прикреплены к письму", stream))
                    {
                        await client.SendTextMessageAsync(chatId, "Документы получены Вашим персональным менеджером. Ожидайте звонка специалиста!",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Документы не отправлены. Попробуйте позже.",
                                                       replyMarkup: new ReplyKeyboardMarkup(new[]
                                                       {
                                                        new[]
                                                        {
                                                            new KeyboardButton("В главное меню"),
                                                        }

                                                       }));
                    }
                    user.State = "Диалог";
                }

                string SendMailBody()
                {
                    string mailBody = "ФИО клиента: " + user.Name + "<br>Дата рождения: " + user.Birthday.ToString("d") + "<br>Номер телефона: " + user.PhoneNumber +
                        "<br>E-mail: " + user.Email + "<br>Семейное положение: " + user.MaritalStatus + "<br>Форма занятости: " + user.Status +
                        "<br>Справка о доходе: " + user.Statement + " <br>Наличие документов на руках: " + user.AllDocuments;
                    return mailBody;
                }

                bool RegexDate(string date)
                {
                    Regex regex = new Regex(@"\d\d\.\d\d\.\d\d\d\d");
                    Match match = regex.Match(date);
                    if (match.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                bool RegexEmail(string mail)
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(mail);
                    if (match.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                bool RegexPhone (string number)
                {
                    Regex regex = new Regex(@"^((\+7|8)+([0-9]){10})$");
                    Match match = regex.Match(number);
                    if (match.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                bool SendEmail(string mailSubject, string mailBody, List<Stream> photoStream = null)
                {
                    try
                    {
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(AppSettings.SendEmail);
                        mail.To.Add(AppSettings.GetEmail);
                        mail.Subject = mailSubject;
                        mail.Body = mailBody;
                        mail.IsBodyHtml = true;

                        if (photoStream != null)
                        {
                            for(int i = 0; i < photoStream.Count; i++)
                            {
                                mail.Attachments.Add(new Attachment(photoStream[i], "Attached photo", "image/jpeg"));
                            }
                            user.PhotosId.Clear();
                        }
                        using (SmtpClient smtp = new SmtpClient(AppSettings.SMTP_Address, AppSettings.SMTP_Port))
                        {
                            smtp.Credentials = new NetworkCredential(AppSettings.SendEmail, AppSettings.SendEmailPassword);
                            smtp.EnableSsl = AppSettings.SSL;
                            smtp.Send(mail);
                        }
                        return true;
                    }
                    catch 
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                await client.SendTextMessageAsync(chatId, e.Message);
            }
            

            return Ok();
        }
    }
}
