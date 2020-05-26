using Artikelverwaltung.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Artikelverwaltung.ViewModel
{
    /// <summary>
    /// Basisklasse für alle ViewModels. Beinhaltet die Datenbankverbindung und eine Methode zum
    /// Speichern der Daten.
    /// </summary>
    public class BaseViewModel
    {
        protected readonly ArtikelContext _db = new ArtikelContext();

        /// <summary>
        /// Ruft SaveChanges auf. Wenn dies allerdings fehlschlägt, werden alle verwalteten
        /// Objekte des OR Mappers zurückgesetzt. Das verhindert, dass beim erneuten Aufruf von
        /// SaveChanges() der OR Mapper den Datensatz wiederholt schreiben will und dann wieder der
        /// Fehler auftritt.
        /// </summary>
        /// <returns></returns>
        protected bool TrySaveChanges()
        {
            try
            {
                _db.SaveChanges();
                return true;
            }
            // DbUpdateConcurrencyException tritt auf, wenn der Datensatz verwaltet, aber nicht
            // gefunden wurde. EF Core glaubt dann, ein anderer Prozess hat ihn gelöscht.
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                foreach (var entry in _db.ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        // Wurde ein Datensatz hinzugefügt und es gab einen Fehler? Dann den State
                        // wieder auf Detached setzen (INSERT ist also fehlgeschlagen)
                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;
                        // Wenn ein UPDATE fehlgeschlagen ist, werden die Originalen Werte wieder
                        // hergestellt.
                        case EntityState.Modified:
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        // Wenn ein DELETE fehlgeschlagen ist, wird der Datensatz wieder auf
                        // unverändert gesetzt.
                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged;
                            break;
                    }
                }
            }
            return false;
        }
    }
}
