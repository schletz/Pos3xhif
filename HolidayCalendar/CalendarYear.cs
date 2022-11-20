using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HolidayCalendar;
class CalendarYear
{
    // Kumulierte Arbeits- und Schultage ab dem 1.1.2000 zu jedem Jahresbeginn bis 2400. Ermöglicht einen
    // lückenlosen Zähler über alle Jahre.
    private static int[] _workingdayCounterStarts = new int[] { 0, 248, 498, 749, 999, 1252, 1502, 1750, 2000, 2253, 2504, 2756, 3006, 3256, 3507, 3757, 4008, 4258, 4506, 4756, 5007, 5259, 5511, 5761, 6009, 6261, 6511, 6762, 7014, 7262, 7512, 7763, 8013, 8266, 8516, 8764, 9014, 9266, 9517, 9769, 10019, 10269, 10520, 10770, 11021, 11271, 11519, 11769, 12020, 12272, 12524, 12774, 13022, 13274, 13524, 13775, 14027, 14275, 14525, 14776, 15026, 15279, 15529, 15777, 16027, 16279, 16530, 16782, 17032, 17282, 17533, 17783, 18034, 18284, 18532, 18782, 19033, 19285, 19537, 19787, 20035, 20287, 20537, 20788, 21040, 21288, 21538, 21789, 22039, 22292, 22542, 22790, 23040, 23292, 23543, 23795, 24045, 24295, 24546, 24796, 25047, 25299, 25549, 25797, 26047, 26299, 26550, 26802, 27052, 27302, 27553, 27803, 28054, 28304, 28552, 28802, 29053, 29305, 29557, 29807, 30055, 30307, 30557, 30808, 31060, 31308, 31558, 31809, 32059, 32312, 32562, 32810, 33060, 33312, 33563, 33815, 34065, 34315, 34566, 34816, 35067, 35317, 35565, 35815, 36066, 36318, 36570, 36820, 37068, 37320, 37570, 37821, 38073, 38321, 38571, 38822, 39072, 39325, 39575, 39823, 40073, 40326, 40577, 40829, 41079, 41329, 41580, 41830, 42081, 42331, 42579, 42829, 43080, 43332, 43584, 43834, 44082, 44334, 44584, 44835, 45087, 45335, 45585, 45836, 46086, 46339, 46589, 46837, 47087, 47339, 47590, 47842, 48092, 48342, 48593, 48843, 49094, 49344, 49592, 49842, 50093, 50343, 50594, 50846, 51096, 51346, 51597, 51847, 52098, 52348, 52596, 52846, 53097, 53349, 53601, 53851, 54099, 54351, 54601, 54852, 55104, 55352, 55602, 55853, 56103, 56356, 56606, 56854, 57104, 57357, 57608, 57860, 58110, 58360, 58611, 58861, 59112, 59362, 59610, 59860, 60111, 60363, 60615, 60865, 61113, 61365, 61615, 61866, 62118, 62366, 62616, 62867, 63117, 63370, 63620, 63868, 64118, 64370, 64621, 64873, 65123, 65373, 65624, 65874, 66125, 66375, 66623, 66873, 67124, 67376, 67628, 67878, 68126, 68378, 68628, 68879, 69131, 69379, 69629, 69880, 70130, 70383, 70633, 70881, 71131, 71383, 71634, 71886, 72136, 72386, 72637, 72887, 73138, 73388, 73636, 73886, 74137, 74389, 74641, 74891, 75139, 75389, 75640, 75890, 76141, 76391, 76639, 76889, 77140, 77392, 77644, 77894, 78142, 78394, 78644, 78895, 79147, 79395, 79645, 79896, 80146, 80399, 80649, 80897, 81147, 81399, 81650, 81902, 82152, 82402, 82653, 82903, 83154, 83404, 83652, 83902, 84153, 84405, 84657, 84907, 85155, 85407, 85657, 85908, 86160, 86408, 86658, 86909, 87159, 87412, 87662, 87910, 88160, 88412, 88663, 88915, 89165, 89415, 89666, 89916, 90167, 90417, 90665, 90915, 91166, 91418, 91670, 91920, 92168, 92420, 92670, 92921, 93173, 93421, 93671, 93922, 94172, 94425, 94675, 94923, 95173, 95426, 95677, 95929, 96179, 96429, 96680, 96930, 97181, 97431, 97679, 97929, 98180, 98432, 98684, 98934, 99182, 99434, 99684, 99935, 100187 };
    private static int[] _schooldayCounterStarts = new int[] { 0, 184, 368, 554, 742, 928, 1113, 1296, 1480, 1670, 1856, 2041, 2226, 2410, 2596, 2784, 2970, 3156, 3339, 3523, 3709, 3894, 4078, 4262, 4444, 4628, 4813, 4997, 5181, 5364, 5547, 5731, 5916, 6101, 6285, 6467, 6650, 6836, 7020, 7204, 7388, 7571, 7755, 7940, 8124, 8309, 8491, 8674, 8858, 9043, 9227, 9411, 9593, 9777, 9962, 10146, 10330, 10513, 10696, 10880, 11065, 11250, 11434, 11616, 11799, 11985, 12169, 12353, 12537, 12720, 12904, 13089, 13273, 13458, 13640, 13823, 14007, 14192, 14376, 14560, 14742, 14926, 15111, 15295, 15479, 15662, 15845, 16029, 16214, 16399, 16583, 16765, 16948, 17134, 17318, 17502, 17686, 17869, 18053, 18238, 18422, 18606, 18790, 18972, 19155, 19341, 19525, 19709, 19893, 20076, 20260, 20445, 20629, 20814, 20996, 21179, 21363, 21548, 21732, 21916, 22098, 22282, 22467, 22651, 22835, 23018, 23201, 23385, 23570, 23755, 23939, 24121, 24304, 24490, 24674, 24858, 25042, 25225, 25409, 25594, 25778, 25963, 26145, 26328, 26512, 26697, 26881, 27065, 27247, 27431, 27616, 27800, 27984, 28167, 28350, 28534, 28719, 28904, 29088, 29270, 29453, 29640, 29824, 30008, 30192, 30375, 30559, 30744, 30928, 31113, 31295, 31478, 31662, 31847, 32031, 32215, 32397, 32581, 32766, 32950, 33134, 33317, 33500, 33684, 33869, 34054, 34238, 34420, 34603, 34789, 34973, 35157, 35341, 35524, 35708, 35893, 36077, 36262, 36444, 36627, 36811, 36996, 37180, 37364, 37548, 37731, 37915, 38100, 38284, 38469, 38651, 38834, 39018, 39203, 39387, 39571, 39753, 39937, 40122, 40306, 40490, 40673, 40856, 41040, 41225, 41410, 41594, 41776, 41959, 42146, 42330, 42514, 42698, 42881, 43065, 43250, 43434, 43619, 43801, 43984, 44168, 44353, 44537, 44721, 44903, 45087, 45272, 45456, 45640, 45823, 46006, 46190, 46375, 46560, 46744, 46926, 47109, 47295, 47479, 47663, 47847, 48030, 48214, 48399, 48583, 48768, 48950, 49133, 49317, 49502, 49686, 49870, 50052, 50236, 50421, 50605, 50789, 50972, 51155, 51339, 51524, 51709, 51893, 52075, 52258, 52444, 52628, 52812, 52996, 53179, 53363, 53548, 53732, 53917, 54099, 54282, 54466, 54651, 54835, 55019, 55201, 55384, 55568, 55753, 55937, 56122, 56304, 56487, 56671, 56856, 57040, 57224, 57406, 57590, 57775, 57959, 58143, 58326, 58509, 58693, 58878, 59063, 59247, 59429, 59612, 59798, 59982, 60166, 60350, 60533, 60717, 60902, 61086, 61271, 61453, 61636, 61820, 62005, 62189, 62373, 62555, 62739, 62924, 63108, 63292, 63475, 63658, 63842, 64027, 64212, 64396, 64578, 64761, 64947, 65131, 65315, 65499, 65682, 65866, 66051, 66235, 66420, 66602, 66785, 66969, 67154, 67338, 67522, 67704, 67888, 68073, 68257, 68441, 68624, 68807, 68991, 69176, 69361, 69545, 69727, 69910, 70097, 70281, 70465, 70649, 70832, 71016, 71201, 71385, 71570, 71752, 71935, 72119, 72304, 72488, 72672, 72854, 73038, 73223, 73407, 73591 };
    private readonly int _year;

    public CalendarYear(int year)
    {
        if (year < 2000) { throw new ArgumentOutOfRangeException("Only years between 2000 and 2400 are supported."); }
        if (year > 2400) { throw new ArgumentOutOfRangeException("Only years between 2000 and 2400 are supported."); }
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
        var days = new CalendarDay[new DateTime(_year, 12, 31).DayOfYear];
        GetCalendarDays(days);
        return days;
    }

    public int GetCalendarDays(Span<CalendarDay> days)
    {
        int daysOfYear = new DateTime(_year, 12, 31).DayOfYear;
        if (days.Length != daysOfYear) { throw new ArgumentException($"Invalid length of days Array. Expected: {daysOfYear}."); }
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

        // Schulfreie Tage nach § 2 Schulzeitgesetz, die nicht frei nach dem Arbeitsruhegesetz sind
        // Vgl. SchZG: https://www.ris.bka.gv.at/GeltendeFassung.wxe?Abfrage=Bundesnormen&Gesetzesnummer=10009575
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
        AddDays(days, GetDate(12, 24), d => new CalendarDay(d, true, true, "Heiliger Abend", "Weihnachtsferien"));
        AddDays(days, easterSunday.AddDays(-47), d => new CalendarDay(d, false, false, "Faschingsdienstag", null));
        AddDays(days, easterSunday.AddDays(-46), d => new CalendarDay(d, false, false, "Aschermittwoch", null));
        var firstAdvent = CalcDateOnNextWeekday(GetDate(11, 27), DayOfWeek.Sunday);
        AddDays(days, firstAdvent, d => new CalendarDay(d, false, false, "1. Advent", null));
        AddDays(days, firstAdvent.AddDays(7), d => new CalendarDay(d, false, false, "2. Advent", null));
        AddDays(days, firstAdvent.AddDays(14), d => new CalendarDay(d, false, false, "3. Advent", null));
        AddDays(days, firstAdvent.AddDays(21), d => new CalendarDay(d, false, false, "4. Advent", null));

        // Alle anderen Tage sind normale Arbeitstage
        int workingdayCounter = _workingdayCounterStarts[_year - 2000];
        int schooldayCounter = _schooldayCounterStarts[_year - 2000];
        int i = 0;
        for (; i < daysOfYear; i++)
        {
            var day = days[i];
            if (day is null) { days[i] = day = new CalendarDay(new DateTime(_year, 1, 1).AddDays(i)); }
            if (day.IsWorkingDayMoFr) { workingdayCounter++; }
            if (day.IsSchoolDayMoFr) { schooldayCounter++; }
            day.WorkingdayCounter = workingdayCounter;
            day.SchooldayCounter = schooldayCounter;
        }
        return i;
    }

    /// <summary>
    /// Liefert ein Datum im Jahr des Objektes
    /// </summary>
    private DateTime GetDate(int month, int day) => new DateTime(_year, month, day);
    /// <summary>
    /// Fügt einen einzelnen Tag in ein Array von Jahrestagen ein, wenn noch kein Tag eingefügt wurde.
    /// </summary>
    private void AddDays(Span<CalendarDay> days, DateTime date, Func<DateTime, CalendarDay> converter) =>
        AddDays(days, date, date.AddDays(1), converter);
    /// <summary>
    /// Fügt eine Range von Tagen (begin bis exklusive end) in ein Array von Jahrestagen ein,
    /// wenn noch kein Tag eingefügt wurde.
    /// </summary>
    private void AddDays(Span<CalendarDay> days, DateTime begin, DateTime end, Func<DateTime, CalendarDay> converter)
    {
        for (; begin < end; begin = begin.AddDays(1))
        {
            int dayOfYear = begin.DayOfYear - 1;
            var newDay = converter(begin);
            var day = days[dayOfYear];
            if (day is null)
                days[dayOfYear] = newDay;
            else
            {
                day.PublicHolidayName = day.PublicHolidayName ?? newDay.PublicHolidayName;
                day.SchoolHolidayName = day.SchoolHolidayName ?? newDay.SchoolHolidayName;
            }
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
