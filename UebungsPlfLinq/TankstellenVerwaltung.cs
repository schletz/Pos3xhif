using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace TankstellenVw
{
    public class TankstellenVerwaltung
    {
        /// <summary>
        /// Setzt die Formatierung der JSON Ausgabe.
        /// Indented = Eingerückt, None = string ohne Leerstellen.
        /// </summary>
        private readonly Formatting JsonFormat = Formatting.Indented;
        /// <summary>
        /// Hält die Liste aller Tankstellen.
        /// </summary>
        public List<Tankstelle> Tankstellen { get; set; }
        /// <summary>
        /// Hält die Liste aller Tagespreise.
        /// </summary>
        public ICollection<Tagespreis> Tagespreise { get; set; }
        /// <summary>
        /// Hält die Liste aller Verkäufe.
        /// </summary>
        public ICollection<Verkauf> Verkaeufe { get; set; }

        /// <summary>
        /// Lädt den XML Export der Datenbank in die Modelklasse.
        /// Vorgegebene Methode, hier ist nichts zu verändern.
        /// </summary>
        /// <param name="filename">Quelldatei</param>
        public void LoadXml(string filename)
        {
            XDocument doc;
            XElement root;

            try { doc = XDocument.Load(filename); root = doc.Root; }
            catch { return; }

            Tankstellen =
                (from t in root.Elements("Tankstelle")
                 select new Tankstelle
                 {
                     Id = int.Parse(t.Attribute("Id").Value),
                     Bundesland = t.Attribute("Bundesland").Value,
                     PLZ = int.Parse(t.Attribute("PLZ").Value),
                     Betreiber = t.Attribute("Betreiber").Value,
                     Tagespreise =
                        (from tp in t.Elements("Tagespreis")
                         select new Tagespreis
                         {
                             Tankstelle = int.Parse(tp.Attribute("Tankstelle").Value),
                             Tag = DateTime.ParseExact(tp.Attribute("Tag").Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                             PreisBenzin = decimal.Parse(tp.Attribute("PreisBenzin").Value, CultureInfo.InvariantCulture),
                             PreisDiesel = decimal.Parse(tp.Attribute("PreisDiesel").Value, CultureInfo.InvariantCulture),
                             Verkaeufe =
                                 (from v in tp.Elements("Verkauf")
                                  select new Verkauf
                                  {
                                      Id = int.Parse(v.Attribute("Id").Value),
                                      Tankstelle = int.Parse(v.Attribute("Tankstelle").Value),
                                      Tag = DateTime.ParseExact(v.Attribute("Tag").Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                      VerkaufBenzinLiter = v.Attribute("VerkaufBenzinLiter") == null ? (decimal?)null : decimal.Parse(v.Attribute("VerkaufBenzinLiter").Value, CultureInfo.InvariantCulture),
                                      VerkaufDieselLiter = v.Attribute("VerkaufDieselLiter") == null ? (decimal?)null : decimal.Parse(v.Attribute("VerkaufDieselLiter").Value, CultureInfo.InvariantCulture),
                                      VerkaufShopEuro = v.Attribute("VerkaufShopEuro") == null ? (decimal?)null : decimal.Parse(v.Attribute("VerkaufShopEuro").Value, CultureInfo.InvariantCulture),
                                  }).ToList()
                         }).ToList()
                 }).ToList();

            // Referenzen auf alle Tagespreise und Verkäufe setzen.
            Tagespreise = Tankstellen.SelectMany(t => t.Tagespreise).ToList();
            Verkaeufe = Tagespreise.SelectMany(tp => tp.Verkaeufe).ToList();

            // Rücknavigationen schreiben.
            foreach (Tagespreis tp in Tagespreise)
            {
                tp._Tankstelle = Tankstellen.FirstOrDefault(t => t.Id == tp.Tankstelle);
            }
            foreach (Verkauf v in Verkaeufe)
            {
                v._Tagespreis = Tagespreise.FirstOrDefault(tp => tp.Tankstelle == v.Tankstelle && tp.Tag == v.Tag);
            }
        }
        /// <summary>
        /// Musteraufgabe: Erstelle eine Abfrage, die alle Tankstellen eines Bundeslandes liefert.
        /// Erzeuge ein Objekt mit den Properties Id (Tankstellen-ID) und Betreiber.
        /// </summary>
        /// <param name="bundesland">B, N oder W</param>
        /// <returns></returns>
        public string ListeTankstellen(string bundesland)
        {
            var result = from t in Tankstellen
                         where t.Bundesland == bundesland
                         select new
                         {
                             Id = t.Id,
                             Betreiber = t.Betreiber
                         };
            return JsonConvert.SerializeObject(result, JsonFormat);
        }

        /// <summary>
        /// Erstelle eine Abfrage, die für die übergebene Tankstelle pro Tag die Anzahl der Verkaufsvorgänge
        /// liefert. Dabei soll nach der übergebenen Tankstellen-ID gefiltert werden.
        /// Erzeuge ein neues Objekt mit den Properties Tankstelle für die Tankstellen-ID,
        /// Tag für den Tag und AnzVerkaeufe für die Anzahl der Verkäufe.
        /// Korrekte Ausgabe für die ID 1001:
        /// [
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-03T00:00:00",
        ///     "AnzVerkaeufe": 15
        ///   },
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-04T00:00:00",
        ///     "AnzVerkaeufe": 11
        ///   },
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-05T00:00:00",
        ///     "AnzVerkaeufe": 8
        ///   },
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-06T00:00:00",
        ///     "AnzVerkaeufe": 6
        ///   },
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-07T00:00:00",
        ///     "AnzVerkaeufe": 14
        ///   },
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-08T00:00:00",
        ///     "AnzVerkaeufe": 12
        ///   },
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-09T00:00:00",
        ///     "AnzVerkaeufe": 14
        ///   }
        /// ]
        /// </summary>
        /// <param name="tankstellenId"></param>
        /// <returns></returns>
        public string GetAnzahlVerkaeufe(int tankstellenId)
        {
            var result = "";                                   // Füge hier die korrekte Lösung ein.
            return JsonConvert.SerializeObject(result, JsonFormat);
        }

        /// <summary>
        /// Erstelle eine Liste von Objekten, die pro Tankstelle den Gesamtumsatz für Benzin,
        /// Diesel und den Shop berechnet. Dabei ist nach dem übergebenenen Bundesland zu filtern.
        /// Die Objekte haben die Properties Tankstelle für die Tankstellen-ID sowie UmsatzBenzin,
        /// UmsatzDiesel und UmsatzShop für die einzelnen Sparten.
        /// Der Umsatz errechnet sich für einen Tag mit Preis x Verkaufte Liter an diesem Tag.
        /// Dieser Umsatz muss dann für alle Tage aufsummiert werden.
        /// Korrekte Ausgabe (für das Bundesland N)
        /// [
        ///   {
        ///     "Tankstelle": 1003,
        ///     "UmsatzBenzin": 2120.63499000,
        ///     "UmsatzDiesel": 2303.52452000,
        ///     "UmsatzShop": 346.9300
        ///   },
        ///   {
        ///     "Tankstelle": 1004,
        ///     "UmsatzBenzin": 1355.85216000,
        ///     "UmsatzDiesel": 1712.24042000,
        ///     "UmsatzShop": 138.3300
        ///   },
        ///   {
        ///     "Tankstelle": 1005,
        ///     "UmsatzBenzin": 1912.65605000,
        ///     "UmsatzDiesel": 1619.27809000,
        ///     "UmsatzShop": 129.6000
        ///   },
        ///   {
        ///     "Tankstelle": 1006,
        ///     "UmsatzBenzin": 1660.23519000,
        ///     "UmsatzDiesel": 1592.83231000,
        ///     "UmsatzShop": 118.9200
        ///   }
        /// ]        /// </summary>
        /// <param name="bundesland"></param>
        /// <returns></returns>
        public string GetUmsaetze(string bundesland)
        {
            var result = "";                                   // Füge hier die korrekte Lösung ein.
            return JsonConvert.SerializeObject(result, JsonFormat);
        }

        /// <summary>
        /// Erstelle eine Abfrage, die pro Tankstelle die Tage herausfindet, an denen überhaupt
        /// nichts an dieser Tankstelle im Shop verkauft wurde. Über den Parameter bundesland
        /// soll vorher nur nach Tankstellen in diesem Bundesland gefiltert werden.
        /// Das neu zu erstellende Objekt soll die Properties Tankstelle für die Tankstellen-ID und
        /// Tag für das Datum des betreffenden Tages enthalten.
        /// Korrekte Ausgabe für das Bundesland B:
        /// [
        ///   {
        ///     "Tankstelle": 1001,
        ///     "Tag": "2018-12-08T00:00:00"
        ///   },
        ///   {
        ///     "Tankstelle": 1002,
        ///     "Tag": "2018-12-05T00:00:00"
        ///   },
        ///   {
        ///     "Tankstelle": 1002,
        ///     "Tag": "2018-12-08T00:00:00"
        ///   },
        ///   {
        ///     "Tankstelle": 1002,
        ///     "Tag": "2018-12-09T00:00:00"
        ///   }
        /// ]        
        /// </summary>
        /// <param name="bundesland">B, N oder W</param>
        /// <returns></returns>
        public string GetTageOhneShopverkaeufe(string bundesland)
        {
            var result = "";                                   // Füge hier die korrekte Lösung ein.
            return JsonConvert.SerializeObject(result, JsonFormat);
        }

        /// <summary>
        /// Erstelle eine Abfrage, die pro Tag die maximalen Preise für Benzin oder Diesel
        /// herausfindet. Dabei ist ein Objekt mit den Properties Tag, MaxPreisBenzin und 
        /// MaxPreisDiesel zu erstellen.
        /// Korrekte Ausgabe:
        /// [
        ///   {
        ///     "Tag": "2018-12-03T00:00:00",
        ///     "MaxPreisBenzin": 1.3500,
        ///     "MaxPreisDiesel": 1.2825
        ///   },
        ///   {
        ///     "Tag": "2018-12-04T00:00:00",
        ///     "MaxPreisBenzin": 1.3729,
        ///     "MaxPreisDiesel": 1.2965
        ///   },
        ///   {
        ///     "Tag": "2018-12-05T00:00:00",
        ///     "MaxPreisBenzin": 1.3837,
        ///     "MaxPreisDiesel": 1.3225
        ///   },
        ///   {
        ///     "Tag": "2018-12-06T00:00:00",
        ///     "MaxPreisBenzin": 1.3976,
        ///     "MaxPreisDiesel": 1.3092
        ///   },
        ///   {
        ///     "Tag": "2018-12-07T00:00:00",
        ///     "MaxPreisBenzin": 1.4540,
        ///     "MaxPreisDiesel": 1.3298
        ///   },
        ///   {
        ///     "Tag": "2018-12-08T00:00:00",
        ///     "MaxPreisBenzin": 1.4540,
        ///     "MaxPreisDiesel": 1.3431
        ///   },
        ///   {
        ///     "Tag": "2018-12-09T00:00:00",
        ///     "MaxPreisBenzin": 1.4540,
        ///     "MaxPreisDiesel": 1.3272
        ///   }
        /// ]
        /// </summary>
        /// <returns></returns>
        public string GetMaxPreiseProTag()
        {
            var result = "";                                   // Füge hier die korrekte Lösung ein.
            return JsonConvert.SerializeObject(result, JsonFormat);
        }

        /// <summary>
        /// Erstelle eine Abfrage, die pro Bundesland und Betreiber den durchschnittlichen
        /// Verkaufspreis für Benzin und Diesel ermittelt.
        /// Das neu zu erstellende Objekt hat die Properties Bundesland, Betreiber, 
        /// PreismittelBenzin und PreismittelDiesel.
        /// Korrekte Ausgabe:
        /// [
        ///   {
        ///     "Bundesland": "B",
        ///     "Betreiber": "BP",
        ///     "PreismittelBenzin": 1.3192071428571428571428571429,
        ///     "PreismittelDiesel": 1.2457714285714285714285714286
        ///   },
        ///   {
        ///     "Bundesland": "N",
        ///     "Betreiber": "OMV",
        ///     "PreismittelBenzin": 1.3475928571428571428571428571,
        ///     "PreismittelDiesel": 1.2429678571428571428571428571
        ///   },
        ///   {
        ///     "Bundesland": "W",
        ///     "Betreiber": "BP",
        ///     "PreismittelBenzin": 1.317425,
        ///     "PreismittelDiesel": 1.2594357142857142857142857143
        ///   },
        ///   {
        ///     "Bundesland": "W",
        ///     "Betreiber": "OMV",
        ///     "PreismittelBenzin": 1.3239571428571428571428571429,
        ///     "PreismittelDiesel": 1.2381714285714285714285714286
        ///   }
        /// ]
        /// </summary>
        /// <returns></returns>
        public string GetPreismittelProBetreiberUndBundesland()
        {
            var result = "";                                   // Füge hier die korrekte Lösung ein.
            return JsonConvert.SerializeObject(result, JsonFormat);
        }

        /// <summary>
        /// Wo wurde der höchste Preis für Benzin verlangt? Ermittle zuerst den Höchstpreis über 
        /// den gesamten Datenbestand hinweg und finde dann die Tagespreise heraus, die diesen
        /// Höchstpreis für Benzin gespeichert haben.
        /// Erstelle jeweils ein neues Objekt mit den Properties Tankstelle (Tankstellen-ID),
        /// Betreiber, Tag und Preis.
        /// Korrekte Ausgabe:
        /// [
        ///   {
        ///     "Tankstelle": 1004,
        ///     "Betreiber": "OMV",
        ///     "Tag": "2018-12-07T00:00:00",
        ///     "Preis": 1.4540
        ///   },
        ///   {
        ///     "Tankstelle": 1004,
        ///     "Betreiber": "OMV",
        ///     "Tag": "2018-12-09T00:00:00",
        ///     "Preis": 1.4540
        ///   },
        ///   {
        ///     "Tankstelle": 1010,
        ///     "Betreiber": "BP",
        ///     "Tag": "2018-12-08T00:00:00",
        ///     "Preis": 1.4540
        ///   }
        /// ]
        /// </summary>
        /// <returns></returns>
        public string GetDatumDesMaxBenzinpreises()
        {
            var result = "";                                   // Füge hier die korrekte Lösung ein.
            return JsonConvert.SerializeObject(result, JsonFormat);
        }
    }
}
