using System;

namespace HolidayCalendar;
public class CalendarDay : IComparable<CalendarDay>, IEquatable<CalendarDay>
{
    private static readonly string[] _weekdayNames
        = new string[] { "SO", "MO", "DI", "MI", "DO", "FR", "SA" };
    private static readonly long _jsEpoch = new DateTime(1970, 1, 1).Ticks;
    public CalendarDay(DateTime dateTime) : this(dateTime, false, false, null, null) { }

    public CalendarDay(DateTime dateTime, bool isPublicHoliday, string holidayName) : this(dateTime, isPublicHoliday, true, holidayName, null)
    { }

    public CalendarDay(DateTime dateTime, bool isPublicHoliday, bool isSchoolHoliday, string? publicHolidayName, string? schoolHolidayName)
    {
        DateTime = dateTime;
        IsPublicHoliday = isPublicHoliday;
        IsSchoolHoliday = isSchoolHoliday;
        PublicHolidayName = publicHolidayName;
        SchoolHolidayName = schoolHolidayName;
        DateTime schoolyearBegin = new DateTime(DateTime.Year, 9, 1);
        // Ersten MO im September ermitteln.
        schoolyearBegin = schoolyearBegin.AddDays((7 - (int)schoolyearBegin.DayOfWeek + 1) % 7);
        Schoolyear = DateTime < schoolyearBegin ? DateTime.Year - 1 : DateTime.Year;
        Date2000 = new DateTime(2000, DateTime.Month, DateTime.Day);
        JsTimestamp = (DateTime.Ticks - _jsEpoch) / TimeSpan.TicksPerMillisecond;
        WeekdayNr = DateTime.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)DateTime.DayOfWeek;
        WeekdayName = _weekdayNames[(int)DateTime.DayOfWeek];
        IsWorkingDayMoFr = DateTime.DayOfWeek != DayOfWeek.Saturday && DateTime.DayOfWeek != DayOfWeek.Sunday && !IsPublicHoliday;
        IsWorkingDayMoSa = DateTime.DayOfWeek != DayOfWeek.Sunday && !IsPublicHoliday;
        IsSchoolDayMoFr = DateTime.DayOfWeek != DayOfWeek.Saturday && DateTime.DayOfWeek != DayOfWeek.Sunday && !IsSchoolHoliday;
    }

    public long JsTimestamp { get; }
    public DateTime DateTime { get; }
    public DateTime Date2000 { get; }
    public int Schoolyear { get; }
    public int WeekdayNr { get; }
    public string WeekdayName { get; }
    public bool IsWorkingDayMoFr { get; }
    public bool IsWorkingDayMoSa { get; }
    public bool IsSchoolDayMoFr { get; }
    public bool IsPublicHoliday { get; }
    public bool IsSchoolHoliday { get; }
    public string? PublicHolidayName { get; set; }
    public string? SchoolHolidayName { get; set; }
    public int WorkingdayCounter { get; set; }
    public int SchooldayCounter { get; set; }

    public int CompareTo(CalendarDay? other) => DateTime.CompareTo(other?.DateTime);
    public bool Equals(CalendarDay? other) => DateTime.Equals(other?.DateTime);
}
