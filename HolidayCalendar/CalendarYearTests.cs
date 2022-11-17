using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CalendarCalculator;

public class CalendarYearTests
{
    [Fact]
    public void DaysCountSuccessTest()
    {
        var year = new CalendarYear(2000);
        Assert.True(year.GetCalendarDays().Length == 366);
    }
    [Fact]
    public void SortSuccessTest()
    {
        var year = new CalendarYear(2000);
        var last = new DateTime(1999, 12, 31);
        bool sorted = true;
        foreach (var day in year.GetCalendarDays())
        {
            if (day.DateTime != last.AddDays(1))
            {
                sorted = false;
                break;
            }
            last = day.DateTime;
        }
        Assert.True(sorted);
    }
    [Fact]
    public void EasterSuccessTest()
    {
        var easterSundays = new Dictionary<int, DateTime>
        {
            { 2019, new DateTime(2019,4,21) },
            { 2020, new DateTime(2020,4,12) },
            { 2021, new DateTime(2021,4,4) },
            { 2022, new DateTime(2022,4,17) },
            { 2023, new DateTime(2023,4,9) },
            { 2035, new DateTime(2035,3,25) },
        };
        bool success = true;
        foreach (var easterSunday in easterSundays)
        {
            var year = new CalendarYear(easterSunday.Key);
            var easter = year.GetCalendarDays().First(d => d.HolidayName == "Ostersonntag");
            if (easter.DateTime != easterSunday.Value) { success = false; continue; }
        }
        Assert.True(success);
    }

    [Fact]
    public void WriteFileTest()
    {
        using var stream = new StreamWriter(File.Create("../../../calendar.txt"), new System.Text.UTF8Encoding(false));
        stream.WriteLine(
            "DATE\tDATE2000\tYEAR\tMONTH\tDAY\tDAYS_FROM_MARCH1\t" +
            "WEEKDAY_NR\tWEEKDAY_STR\tWORINGDAY_MO_FR\tWORKINGDAY_MO_SA\tSCHOOLDAY\t" +
            "PUBLIC_HOLIDAY\tSCHOOL_HOLIDAY\tHOLIDAY_NAME");
        for (int year = 2000; year < 2400; year++)
        {
            var calendarYear = new CalendarYear(year);
            foreach (var day in calendarYear.GetCalendarDays())
            {
                var dateTime = day.DateTime;
                stream.WriteLine(
                    $"{day.DateTime:yyyy-MM-dd}\t{day.Date2000:yyyy-MM-dd}\t{day.DateTime.Year}\t{day.DateTime.Month}\t{day.DateTime.Day}\t{day.DayOfYearSinceMarch}\t" +
                    $"{day.WeekdayNr}\t{day.WeekdayName}\t" +
                    $"{(day.IsWorkingDayMoFr ? 1 : 0)}\t{(day.IsWorkingDayMoSa ? 1 : 0)}\t{(day.IsSchoolDayMoFr ? 1 : 0)}\t" +
                    $"{(day.IsPublicHoliday ? 1 : 0)}\t{(day.IsSchoolHoliday ? 1 : 0)}\t{day.HolidayName}");
            }
        }
    }
}