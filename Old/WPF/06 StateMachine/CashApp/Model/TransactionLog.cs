using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashApp.Model
{
    /// <summary>
    /// Speichert die Daten aller Transaktionen.
    /// </summary>
    public class TransactionLog
    {
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
    }
}
