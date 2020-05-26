using Artikelverwaltung.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Artikelverwaltung.ViewModel
{
    public class BaseViewModel
    {
        protected readonly ArtikelContext _db = new ArtikelContext();

        protected bool TrySaveChanges()
        {
            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                foreach (var entry in _db.ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;
                        case EntityState.Modified:
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
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
