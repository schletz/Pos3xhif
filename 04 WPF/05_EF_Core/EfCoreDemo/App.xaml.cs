using AutoMapper;
using ListDemo.Dto;
using ListDemo.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ListDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Registriert die in der Klasse DtoMappingProfile angeleten
        // Automapper Profile. Zugriff über App.Mapper.
        public static readonly IMapper Mapper = 
            new MapperConfiguration(cfg => cfg.AddMaps(typeof(DtoMappingProfile)))
            .CreateMapper();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new SchoolDb())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Seed();
            }
        }
    }
}
