using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailClientApp.Model
{
    public class Mail : IEquatable<Mail>
    {
        public int ID { get; set; }
        public string SENDER { get; set; }
        public DateTime? DATE_SENT { get; set; }
        public DateTime? DATE_RECEIVED { get; set; }
        public string SUBJECT { get; set; }
        public string CONTENT { get; set; }
        public override bool Equals(object obj) => Equals(obj as Mail);
        public bool Equals(Mail other) => other == null ? false : ID == other.ID;
        public override int GetHashCode() => ID.GetHashCode();
    }
}
