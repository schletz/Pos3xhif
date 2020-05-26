using Artikelverwaltung.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.ComponentModel;

namespace Artikelverwaltung.ViewModel
{
    public class MainViewModel
    {
        public ICommand ShowWindow { get; }

        public MainViewModel()
        {
            ShowWindow = new RelayCommand((windowName) =>
            {
                Window newWindow = null;
                switch (windowName as string)
                {
                    case "Kunde":
                        newWindow = new KundeWindow();
                        break;
                    case "Artikel":
                        newWindow = new ArtikelWindow();
                        break;
                }
                newWindow?.ShowDialog();
            });
        }
    }
}
