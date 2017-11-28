using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Telegram.Bot.Types;

namespace DeltaCreditBot.Models
{
    public class BotUser
    {
        public long ChatId { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string MaritalStatus { get; set; }
        public string Status { get; set; }
        public string Statement { get; set; }
        public string AllDocuments { get; set; }
        public DateTime Birthday { get; set; }
        public List<string> PhotosId { get; set; } = new List<string>();
    }
}