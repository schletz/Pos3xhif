using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCalculator
{

    class CalendarYear
    {
        private readonly int _year;

        public CalendarYear(int year)
        {
            _year = year;
        }
        public DateTime SchoolyearBegin => CalcDateOnNextWeekday(GetDate(9, 1), DayOfWeek.Monday);
        public DateTime SemesterHolidayBegin => CalcDateOnNextWeekday(GetDate(2, 1), DayOfWeek.Monday);
        public DateTime MainHolidayBegin => CalcDateOnNextWeekday(GetDate(6, 28), DayOfWeek.Saturday);
        public DateTime EasterSunday => CalcEasterSunday();
        public DateTime ChristiHimmelfahrt => EasterSunday.AddDays(39);
        public DateTime PfingstSunday => EasterSunday.AddDays(49);
        public DateTime Fronleichnam => EasterSunday.AddDays(60);

        /// <summary>
        /// Liefert ein Array aller Tage eines Jahres mit den entsprechenden Anmerkungen für
        /// Feiertage und Schulferien.
        /// </summary>
        public CalendarDay[] GetCalendarDays()
        {
            /* Arbeitsruhegesetz
             * https://www.ris.bka.gv.at/GeltendeFassung.wxe?Abfrage=Bundesnormen&Gesetzesnummer=10008541
             *
             * (2) Feiertage im Sinne dieses Bundesgesetzes sind:
             *     1. Jänner (Neujahr), 6. Jänner (Heilige Drei Könige), Ostermontag, 1. Mai (Staatsfeiertag), 
             *     Christi Himmelfahrt, Pfingstmontag, Fronleichnam, 15. August (Mariä Himmelfahrt),
             *     26. Oktober (Nationalfeiertag), 1. November (Allerheiligen),
             *     8. Dezember (Mariä Empfängnis), 25. Dezember (Weihnachten), 26. Dezember (Stephanstag).
            */
            var semesterHolidayBegin = SemesterHolidayBegin;
            var easterSunday = EasterSunday;
            var mainHolidayBegin = MainHolidayBegin;
            var schoolyearBegin = SchoolyearBegin;
            var xmaxHolidayBegin = GetDate(12, 23).DayOfWeek == DayOfWeek.Monday ? GetDate(12, 23) : GetDate(12, 24);

            var days = new CalendarDay[new DateTime(_year, 12, 31).DayOfYear];

            // Gesetzliche Feiertage
            AddDays(days, GetDate(1, 1), d => new CalendarDay(d, true, true, "Neujahr", "Weihnachtsferien"));
            AddDays(days, GetDate(1, 6), d => new CalendarDay(d, true, true, "Heilige 3 Könige", "Weihnachtsferien"));
            AddDays(days, easterSunday.AddDays(1), d => new CalendarDay(d, true, true, "Ostermontag", "Osterferien"));
            AddDays(days, GetDate(5, 1), d => new CalendarDay(d, true, true, "Staatsfeiertag", "Staatsfeiertag"));
            AddDays(days, easterSunday.AddDays(39), d => new CalendarDay(d, true, true, "Christi Himmelfahrt", "Christi Himmelfahrt"));
            AddDays(days, easterSunday.AddDays(50), d => new CalendarDay(d, true, true, "Pfingstmontag", "Pfingstferien"));
            AddDays(days, easterSunday.AddDays(60), d => new CalendarDay(d, true, true, "Fronleichnam", "Fronleichnam"));
            AddDays(days, GetDate(8, 15), d => new CalendarDay(d, true, true, "Mariä Himmelfahrt", "Sommerferien"));
            AddDays(days, GetDate(10, 26), d => new CalendarDay(d, true, true, "Nationalfeiertag", "Nationalfeiertag"));
            AddDays(days, GetDate(11, 1), d => new CalendarDay(d, true, true, "Allerheiligen", "Allerheiligen"));
            AddDays(days, GetDate(12, 8), d => new CalendarDay(d, true, true, "Mariä Empfängnis", "Mariä Empfängnis"));
            AddDays(days, GetDate(12, 25), d => new CalendarDay(d, true, true, "Weihnachten", "Weihnachtsferien"));
            AddDays(days, GetDate(12, 26), d => new CalendarDay(d, true, true, "Stephanstag", "Weihnachtsferien"));

            // Immer frei, aber mit einem Text versehen
            AddDays(days, easterSunday.AddDays(-7), d => new CalendarDay(d, false, true, "Palmsonntag", "Osterferien"));
            AddDays(days, easterSunday.AddDays(-3), d => new CalendarDay(d, false, true, "Gründonnerstag", "Osterferien"));
            AddDays(days, easterSunday.AddDays(-2), d => new CalendarDay(d, false, true, "Karfreitag", "Osterferien"));
            AddDays(days, easterSunday.AddDays(-1), d => new CalendarDay(d, false, true, "Karsamstag", "Osterferien"));
            AddDays(days, easterSunday, d => new CalendarDay(d, false, true, "Ostersonntag", "Osterferien"));
            AddDays(days, easterSunday.AddDays(49), d => new CalendarDay(d, false, true, "Pfingstsonntag", "Pfingstferien"));

            // Zu Tagen, die in die Ferien fallen können, Infos angeben. Da Einträge nicht mehr überschrieben werden,
            // müssen wir das vorher ermitteln.
            if (easterSunday.AddDays(-47) >= semesterHolidayBegin && easterSunday.AddDays(-47) < semesterHolidayBegin.AddDays(7))
                AddDays(days, easterSunday.AddDays(-47), d => new CalendarDay(d, false, true, "Faschingsdienstag", "Semesterferien"));
            else
                AddDays(days, easterSunday.AddDays(-47), d => new CalendarDay(d, false, false, "Faschingsdienstag", null));

            if (easterSunday.AddDays(-46) >= semesterHolidayBegin && easterSunday.AddDays(-46) < semesterHolidayBegin.AddDays(7))
                AddDays(days, easterSunday.AddDays(-46), d => new CalendarDay(d, false, true, "Aschermittwoch", "Semesterferien"));
            else
                AddDays(days, easterSunday.AddDays(-46), d => new CalendarDay(d, false, false, "Aschermittwoch", null));

            // Schulfreie Tage nach § 2 Schulzeitgesetz, die nicht frei nach dem Arbeitsruhegesetz sind
            // Vgl. SchZG: https://www.ris.bka.gv.at/GeltendeFassung.wxe?Abfrage=Bundesnormen&Gesetzesnummer=10009575
            // Tage, die schon angelegt wurden, werden im Array nicht überschrieben.
            AddDays(days, GetDate(1, 2), GetDate(1, 6), d => new CalendarDay(d, false, true, null, "Weihnachtsferien"));
            AddDays(days, semesterHolidayBegin, semesterHolidayBegin.AddDays(7), d => new CalendarDay(d, false, true, null, "Semesterferien"));
            // Novelle BGBl. I Nr. 49/2019: DI nach Ostern und Pfingsten ist ab 2020 nicht mehr frei.
            AddDays(days, easterSunday.AddDays(-8), easterSunday.AddDays(_year < 2020 ? 3 : 2), d => new CalendarDay(d, false, true, null, "Osterferien"));
            AddDays(days, easterSunday.AddDays(48), easterSunday.AddDays(_year < 2020 ? 52 : 51), d => new CalendarDay(d, false, true, null, "Pfingstferien"));
            AddDays(days, mainHolidayBegin, schoolyearBegin, d => new CalendarDay(d, false, true, null, "Sommerferien"));
            AddDays(days, GetDate(11, 2), d => new CalendarDay(d, false, true, "Allerseelen", "Allerseelen"));
            AddDays(days, GetDate(11, 15), d => new CalendarDay(d, false, true, "Heiliger Lepopld", "Heiliger Lepopld"));
            AddDays(days, xmaxHolidayBegin, GetDate(12, 31).AddDays(1), d => new CalendarDay(d, false, true, null, "Weihnachtsferien"));
            // Novelle BGBl. I Nr. 49/2019: Herbstferien vom 27.10. bis 31.10.
            if (_year >= 2020)
            {
                AddDays(days, GetDate(10, 27), GetDate(11, 1), d => new CalendarDay(d, false, true, null, "Herbstferien"));
            }

            // Zu bestimmten Tagen Infos angeben
            var firstAdvent = CalcDateOnNextWeekday(GetDate(11, 27), DayOfWeek.Sunday);
            AddDays(days, firstAdvent, d => new CalendarDay(d, false, false, "1. Advent", null));
            AddDays(days, firstAdvent.AddDays(7), d => new CalendarDay(d, false, false, "2. Advent", null));
            AddDays(days, firstAdvent.AddDays(14), d => new CalendarDay(d, false, false, "3. Advent", null));
            AddDays(days, firstAdvent.AddDays(21), d => new CalendarDay(d, false, false, "4. Advent", null));

            // Alle anderen Tage sind normale Arbeitstage
            AddDays(days, GetDate(1, 1), GetDate(12, 31).AddDays(1));
            return days;
        }

        /// <summary>
        /// Liefert ein Datum im Jahr des Objektes
        /// </summary>
        private DateTime GetDate(int month, int day) => new DateTime(_year, month, day);
        /// <summary>
        /// Fügt einen einzelnen Tag in ein Array von Jahrestagen ein, wenn noch kein Tag eingefügt wurde.
        /// </summary>
        private void AddDays(CalendarDay[] days, DateTime day, Func<DateTime, CalendarDay> converter)
        {
            int dayOfYear = day.DayOfYear - 1;
            if (days[dayOfYear] is null)
                days[dayOfYear] = converter(day);
        }
        /// <summary>
        /// Fügt eine Range von Tagen (begin bis exklusive end) in ein Array von Jahrestagen ein,
        /// wenn noch kein Tag eingefügt wurde.
        /// </summary>
        private void AddDays(CalendarDay[] days, DateTime begin, DateTime end)
            => AddDays(days, begin, end, d => new CalendarDay(d));
        /// <summary>
        /// Fügt eine Range von Tagen (begin bis exklusive end) in ein Array von Jahrestagen ein,
        /// wenn noch kein Tag eingefügt wurde.
        /// </summary>
        private void AddDays(CalendarDay[] days, DateTime begin, DateTime end, Func<DateTime, CalendarDay> converter)
        {
            for (; begin < end; begin = begin.AddDays(1))
            {
                int dayOfYear = begin.DayOfYear - 1;
                if (days[dayOfYear] is null)
                    days[dayOfYear] = converter(begin);
            }
        }
        /// <summary>
        /// Liefert den nächsten Tag, an dem der angegebene Wochentag eintritt. Wird für die
        /// Berechnung der Schulferien benötigt.
        /// DI, 15.11.2022, gesucht ist der nächste DI  --> 15.11.2022
        /// MI, 16.11.2022, gesucht ist der nächste DI  --> 22.11.2022
        /// </summary>
        private DateTime CalcDateOnNextWeekday(DateTime date, DayOfWeek dayOfWeek) =>
            date.AddDays((7 - (int)date.DayOfWeek + (int)dayOfWeek) % 7);

        /// <summary>
        /// Berechnet den Tag des Ostersonntags im Jahr des Objektes.
        /// Nach Spencer Jones, vgl. Meeus Astronomical Algorithms, 2nd Ed, S67
        /// </summary>
        private DateTime CalcEasterSunday()
        {
            int a = _year % 19;
            int b = _year / 100;
            int c = _year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int x = h + l - 7 * m + 114;
            int n = x / 31;
            int p = x % 31;
            return new DateTime(_year, n, p + 1);
        }
    }
}
