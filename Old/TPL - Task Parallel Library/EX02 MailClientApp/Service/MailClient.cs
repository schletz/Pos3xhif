using MailClientApp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MailClientApp.Service
{
    class MailClient
    {
        public readonly HttpClient client = new HttpClient();
        public event EventHandler NewMail;

        private HashSet<Mail> MailCache = new HashSet<Mail>();
        public IEnumerable<Mail> GetMails() => MailCache;

        /// <summary>
        /// Startet den Hintergrundtask, der alle n Millisekunden die Liste MailCache
        /// aktualisiert.
        /// </summary>
        /// <param name="interval">Zeitdauer in ms zwischen den Abfragen.</param>
        public void StartLoader(int interval)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bricht den Hintergrundvorgang ab und wartet, bis dieser beendet ist.
        /// </summary>
        /// <returns></returns>
        public async Task CancelAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lädt alle Mails über die URL http://schletz.org/getMails
        /// und schreibt sie in die Liste MailCache.
        /// Das Property CONTENT ist dabei nicht gesetzt (NULL)m denn der Content wird mit
        /// einem eigenen Aufruf über die Methode GetMailDetails bei Bedarf (beim Anklicken)
        /// geladen.
        /// Beispiel für eine Serverantwort:
        /// [{"ID":1183039879,"SENDER":"mail6@spengergasse.at","DATE_SENT":"2019-05-07 12:34:05","SUBJECT":"Betreff Nr. 3"}
        /// ,{"ID":47172859,"SENDER":"mail2@spengergasse.at","DATE_SENT":"2019-05-01 23:37:53","SUBJECT":"Betreff Nr. 2"}
        /// ,{"ID":1712933392,"SENDER":"mail3@spengergasse.at","DATE_SENT":"2019-02-05 05:39:06","SUBJECT":"Betreff Nr. 2"}
        /// ,{"ID":923970810,"SENDER":"mail2@spengergasse.at","DATE_SENT":"2019-01-21 04:48:44","SUBJECT":"Betreff Nr. 1"}
        /// ,{"ID":1714448846,"SENDER":"mail1@spengergasse.at","DATE_SENT":"2019-02-21 13:42:23","SUBJECT":"Betreff Nr. 3"}
        /// ,{"ID":1874855007,"SENDER":"mail1@spengergasse.at","DATE_SENT":"2019-02-14 16:23:45","SUBJECT":"Betreff Nr. 2"}
        /// ,{"ID":1213454164,"SENDER":"mail6@spengergasse.at","DATE_SENT":"2019-02-26 11:36:51","SUBJECT":"Betreff Nr. 2"}
        /// ,{"ID":1925895246,"SENDER":"mail6@spengergasse.at","DATE_SENT":"2019-01-15 22:40:42","SUBJECT":"Betreff Nr. 1"}
        /// ,{"ID":517961602,"SENDER":"mail2@spengergasse.at","DATE_SENT":"2019-01-01 22:07:40","SUBJECT":"Betreff Nr. 2"}
        /// ,{"ID":912573021,"SENDER":"mail5@spengergasse.at","DATE_SENT":"2019-02-01 02:28:52","SUBJECT":"Betreff Nr. 2"}
        /// ,{"ID":2006033986,"SENDER":"mail1@spengergasse.at","DATE_SENT":"2019-01-27 02:56:22","SUBJECT":"Betreff Nr. 3"}
        /// ,{"ID":2023767537,"SENDER":"mail5@spengergasse.at","DATE_SENT":"2019-02-21 11:02:43","SUBJECT":"Betreff Nr. 2"}]        /// </summary>
        /// <returns></returns>
        private async Task LoadFromServer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lädt den Content der übergebenen Mail vom Server.
        /// URL: http://schletz.org/getMailContent?mailid=xxx (xxx = mail.ID)
        /// Die Antwort ist - auch wenn nur 1 Datensatz geliefert wird - ein JSON Array.
        /// Daher ist mit JsonConvert.DeserializeObject<List<Mail>>(content) zu arbeiten.
        /// Beispiel für die Serverantwort:
        /// [{"ID":"12324","CONTENT":"Text der Nachricht 12324 10566944690"}]
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<Mail> GetMailDetails(Mail mail)
        {
            throw new NotImplementedException();
        }
    }
}
