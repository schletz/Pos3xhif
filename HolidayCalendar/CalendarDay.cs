using System;

namespace CalendarCalculator
{
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
        }

        public DateTime DateTime { get; }

        public bool IsPublicHoliday { get; }
        public string? PublicHolidayName { get; }
        public string? SchoolHolidayName { get; }
        public bool IsSchoolHoliday { get; }
        public int Schoolyear
        {
            get
            {
                DateTime schoolyearBegin = new DateTime(DateTime.Year, 9, 1);
                // Ersten MO im September ermitteln.
                schoolyearBegin = schoolyearBegin.AddDays((7 - (int)schoolyearBegin.DayOfWeek + 1) % 7);
                return DateTime < schoolyearBegin ? DateTime.Year - 1 : DateTime.Year;
            }
        }
        public DateTime Date2000 => new DateTime(2000, DateTime.Month, DateTime.Day);
        public long JsTimestamp => (DateTime.Ticks - _jsEpoch) / TimeSpan.TicksPerMillisecond;
        public int WeekdayNr => DateTime.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)DateTime.DayOfWeek;
        public string WeekdayName => _weekdayNames[(int)DateTime.DayOfWeek];
        public bool IsWorkingDayMoFr =>
            DateTime.DayOfWeek != DayOfWeek.Saturday && DateTime.DayOfWeek != DayOfWeek.Sunday && !IsPublicHoliday;
        public bool IsWorkingDayMoSa =>
            DateTime.DayOfWeek != DayOfWeek.Sunday && !IsPublicHoliday;
        public bool IsSchoolDayMoFr =>
            DateTime.DayOfWeek != DayOfWeek.Saturday && DateTime.DayOfWeek != DayOfWeek.Sunday && !IsSchoolHoliday;

        public int CompareTo(CalendarDay? other) => DateTime.CompareTo(other?.DateTime);
        public bool Equals(CalendarDay? other) => DateTime.Equals(other?.DateTime);
    }
}
