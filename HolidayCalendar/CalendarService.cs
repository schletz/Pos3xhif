using System;
using System.Collections.Generic;

namespace HolidayCalendar;
/// <summary>
/// Serviceklasse für einen zentralen Kalender. In ASP.NET Core als Singleton mit dem gewünschen
/// Bereich von Jahren registrieren:
///     services.AddSingleton(provider => new CalendarService(2000,2100));
/// </summary>
class CalendarService
{
    private readonly CalendarDay[] _calendarDays;
    private readonly DateTime _day0;
    /// <summary>
    /// Erzeugt ein durchgehendes Array mit allen Tagen der angegebenen Range.
    /// </summary>
    public CalendarService(int yearFrom, int yearToExclusive)
    {
        if (yearFrom >= yearToExclusive) { throw new ArgumentException("Invalid year range."); }

        _day0 = new DateTime(yearFrom, 1, 1);
        int daysCount = (int)((new DateTime(yearToExclusive, 1, 1).Ticks - _day0.Ticks) / TimeSpan.TicksPerDay);
        var days = new CalendarDay[daysCount];
        var daysSpan = days.AsSpan();
        int i = 0;
        for (int year = yearFrom; year < yearToExclusive; year++)
        {
            var calendarYear = new CalendarYear(year);
            i += calendarYear.GetCalendarDays(daysSpan.Slice(i, new DateTime(year, 12, 31).DayOfYear));
        }
        _calendarDays = days;
    }

    public IReadOnlyList<CalendarDay> CalendarDays => _calendarDays;
    /// <summary>
    /// Gibt die Position des Tages im internen Array zurück.
    /// </summary>
    public int CalcDayNumber(DateTime date) => (int)((date.Date.Ticks - _day0.Ticks) / TimeSpan.TicksPerDay);
    /// <summary>
    /// Bestimmt die Anzahl der Arbeitstage zwischen 2 Datumswerten.
    /// </summary>
    public int GetWorkingDaysBetween(DateTime start, DateTime end) =>
        _calendarDays[CalcDayNumber(end)].WorkingdayCounter - _calendarDays[CalcDayNumber(start)].WorkingdayCounter;
    /// <summary>
    /// Ermittelt das Datum, wann die übergebene Anzahl an Arbeitstagen vergangen sind.
    /// </summary>
    public DateTime AddWorkingDays(DateTime start, int workingDays)
    {
        var startDay = _calendarDays[CalcDayNumber(start)];
        var counter = startDay.WorkingdayCounter + workingDays;
        // Bei negativen Werten wird der Tag davor gesucht, da der Counter an freien Tagen
        // seinen Wert beibehät (also 34, 35, 35, 35, 36).
        if (workingDays < 0) counter--;
        int i = CalcDayNumber(start.AddDays(workingDays));
        while (_calendarDays[i].WorkingdayCounter != counter)
        {
            i += workingDays < 0 ? -1 : 1;
        }
        if (workingDays < 0) i++; // Der erste Tag der Serie mit gleichen Counter Werten.
        return _calendarDays[i].DateTime;
    }

}
