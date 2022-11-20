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
    private readonly int _yearFrom;
    private readonly int _yearToExclusive;
    private readonly DateTime _minDate;
    private readonly DateTime _maxDate;
    private readonly ArgumentException _outOfRangeException;
    /// <summary>
    /// Erzeugt ein durchgehendes Array mit allen Tagen der angegebenen Range.
    /// </summary>
    public CalendarService(int yearFrom, int yearToExclusive)
    {
        if (yearFrom >= yearToExclusive) { throw new ArgumentException("Invalid year range."); }

        _minDate = new DateTime(yearFrom, 1, 1);
        _maxDate = new DateTime(yearToExclusive, 1, 1);
        int daysCount = (int)((new DateTime(yearToExclusive, 1, 1).Ticks - _minDate.Ticks) / TimeSpan.TicksPerDay);
        var days = new CalendarDay[daysCount];
        var daysSpan = days.AsSpan();
        int i = 0;
        for (int year = yearFrom; year < yearToExclusive; year++)
        {
            var calendarYear = new CalendarYear(year);
            i += calendarYear.GetCalendarDays(daysSpan.Slice(i, DaysOfYear(year)));
        }
        _yearFrom = yearFrom;
        _yearToExclusive = yearToExclusive;
        _calendarDays = days;
        _outOfRangeException = new ArgumentException($"Date is not in Range. Only years {_yearFrom} to {_yearToExclusive - 1} are allowed.");
    }

    public DateTime MinDate => _minDate;
    public DateTime MaxDate => _maxDate;
    public int YearFrom => _yearFrom;
    public int YearToExclusive => _yearToExclusive;
    public CalendarDay[] CalendarDays => _calendarDays;

    public int DaysOfYear(int year) => 365 + (year % 400 == 0 ? 1 : year % 100 == 0 ? 0 : year % 4 == 0 ? 1 : 0);
    /// <summary>
    /// Liefert die Kalendertage eines Jahres.
    /// </summary>
    public CalendarDay[] GetDaysOfYear(int year)
    {
        var start = new DateTime(year, 1, 1);
        var first = CalcDayNumber(start);
        var last = CalcDayNumber(start.AddYears(1).AddDays(-1));
        return _calendarDays[first..(last + 1)];
    }
    /// <summary>
    /// Liefert die Kalendertage eines Monats.
    /// </summary>
    public CalendarDay[] GetDaysOfMonth(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var first = CalcDayNumber(start);
        var last = CalcDayNumber(start.AddMonths(1).AddDays(-1));
        return _calendarDays[first..(last + 1)];
    }
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
            if (i < 0 || i >= _calendarDays.Length) { throw _outOfRangeException; }
        }
        if (workingDays < 0) i++; // Der erste Tag der Serie mit gleichen Counter Werten.
        return _calendarDays[i].DateTime;
    }
    /// <summary>
    /// Gibt die Position des Tages im internen Array zurück.
    /// </summary>
    private int CalcDayNumber(DateTime date)
    {
        int i = (int)((date.Date.Ticks - _minDate.Ticks) / TimeSpan.TicksPerDay);
        if (i < 0 || i >= _calendarDays.Length) { throw _outOfRangeException; }
        return i;
    }

}
