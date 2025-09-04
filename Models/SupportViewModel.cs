using RMS.Data.Entities;
using System.Collections.Generic;

namespace RMS.Models
{
    public class SupportViewModel
    {
        public SupportMessage NewMessage { get; set; }
        public List<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
    }
}
