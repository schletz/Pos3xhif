using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailClientApp.Model;
using MailClientApp.Service;

namespace MailClientApp.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private readonly MailClient client = new MailClient();
        public IEnumerable<Mail> Mails => client.GetMails();
        public Mail SelectedMail { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
