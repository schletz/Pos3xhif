using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace GeizhalsArtikelfinder.Model
{
    public class GeizhalsDb
    {
        public ICollection<Angebot> Angebote { get; set; }
        public ICollection<Artikel> Artikels { get; set; }
        public ICollection<Haendler> Haendlers { get; set; }
        private GeizhalsDb()
        { }

        public static GeizhalsDb FromXml(string filename)
        {
            GeizhalsDb db = new GeizhalsDb();
            try
            {
                XDocument doc = XDocument.Load(filename);
                db.Angebote = (from a in doc.Root.Element("Angebote").Elements("Angebot")
                            select new Angebot
                            {
                                Id = int.Parse(a.Attribute("Id").Value, CultureInfo.InvariantCulture),
                                Artikel = long.Parse(a.Attribute("Artikel").Value, CultureInfo.InvariantCulture),
                                Haendler = long.Parse(a.Attribute("Haendler").Value, CultureInfo.InvariantCulture),
                                Datum = DateTime.ParseExact(a.Attribute("Datum").Value, "yyyy-MM-dd", CultureInfo.InstalledUICulture),
                                Preis = decimal.Parse(a.Attribute("Preis").Value, CultureInfo.InvariantCulture),
                                AnzVerkaeufe = int.Parse(a.Attribute("AnzVerkaeufe").Value, CultureInfo.InvariantCulture),
                                Url = a.Attribute("Url").Value,
                            }).ToList();
                db.Artikels = (from a in doc.Root.Element("Artikelliste").Elements("Artikel")
                            let ean = long.Parse(a.Attribute("Ean").Value, CultureInfo.InvariantCulture)
                            select new Artikel
                            {
                                Ean = ean,
                                Name = a.Attribute("Name").Value,
                                Kategorie = a.Attribute("Kategorie").Value,
                                Angebote = db.Angebote.Where(an => an.Artikel == ean).ToList()
                            }).ToList();
                db.Haendlers = (from h in doc.Root.Element("Haendlerliste").Elements("Haendler")
                             let uid = long.Parse(h.Attribute("Uid").Value, CultureInfo.InvariantCulture)
                             select new Haendler
                             {
                                 Uid = uid,
                                 Name = h.Attribute("Name").Value,
                                 Land = h.Attribute("Land").Value,
                                 Url = h.Attribute("Url").Value,
                                 Angebote = db.Angebote.Where(an => an.Haendler == uid).ToList()
                             }).ToList();
                foreach (Angebot a in db.Angebote)
                {
                    a._Haendler = db.Haendlers.First(h => h.Uid == a.Haendler);
                    a._Artikel = db.Artikels.First(ar => ar.Ean == a.Artikel);
                }
            }
            catch { }
            return db;
        }
    }
}
