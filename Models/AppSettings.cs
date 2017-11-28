using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeltaCreditBot.Models
{
    public class AppSettings
    {
        //Настройки бота
        public static string Url { get; set; } = "";

        public static string Name { get; set; } = "";

        public static string Key { get; set; } = "";

        //Настройки отправки сообщений
        public static string SendEmail { get; set; } = "";

        public static string SendEmailPassword { get; set; } = "";

        public static string SMTP_Address { get; set; } = "smtp.yandex.ru";

        public static int SMTP_Port { get; set; } = 25;

        public static bool SSL { get; set; } = true;


        //Настройки получения сообщений
        public static string GetEmail { get; set; } = "";

        //Пользователи
        public static List<BotUser> Users { get; set; } = new List<BotUser>();
    }
}