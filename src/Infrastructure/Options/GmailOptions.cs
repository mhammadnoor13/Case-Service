using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Options
{
    public class GmailOptions
    {
        public const string GmailOptionskey = "GmailOptions";

        public string Host {  get; set; }
        public int Port {  get; set; }
        public string Email {  get; set; }
        public string Password {  get; set; }

    }
}
