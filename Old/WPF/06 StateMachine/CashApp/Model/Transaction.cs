using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashApp.Model
{
    /// <summary>
    /// Speichert die Informationen zu einer Transaktion.
    /// </summary>
    public class Transaction
    {
        public DateTime Date { get; private set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
    }
}
