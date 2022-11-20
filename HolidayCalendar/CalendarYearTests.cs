using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var easter = year.GetCalendarDays().First(d => d.PublicHolidayName == "Ostersonntag");
            if (easter.DateTime != easterSunday.Value) { success = false; continue; }
        }
        Assert.True(success);
    }

    [Fact]
    public void WriteFileTest()
    {
        using var stream = new StreamWriter(File.Create("../../../calendar.txt"), System.Text.Encoding.Unicode);
        stream.WriteLine(
            "DATE\tDATE2000\tYEAR\tMONTH\tDAY\tSCHOOLYEAR\t" +
            "WEEKDAY_NR\tWEEKDAY_STR\t" +
            "WORKINGDAY\tWORKINGDAY_COUNTER\tSCHOOLDAY\tSCHOOLDAY_COUNTER\t" +
            "PUBLIC_HOLIDAY\tSCHOOL_HOLIDAY\tPUBLIC_HOLIDAY_NAME\tSCHOOL_HOLIDAY_NAME");
        int workingdayCounter = 0;
        int schooldayCounter = 0;
        // Für die Schuljahre 2000 - 2399 die Jahre 1999 und 2400 auch generieren. Sonst werden
        // sie am 1.1. abgeschnitten.
        for (int year = 1999; year <= 2400; year++)
        {
            var calendarYear = new CalendarYear(year);
            var schoolyearBegin = calendarYear.SchoolyearBegin;
            foreach (var day in calendarYear.GetCalendarDays())
            {
                if (day.IsWorkingDayMoFr) { workingdayCounter++; }
                if (day.IsSchoolDayMoFr) { schooldayCounter++; }
                var dateTime = day.DateTime;
                int schoolyear = dateTime >= schoolyearBegin ? dateTime.Year : dateTime.Year - 1;
                stream.WriteLine(
                    $"{day.DateTime:yyyy-MM-dd}\t{day.Date2000:yyyy-MM-dd}\t{day.DateTime.Year}\t{day.DateTime.Month}\t{day.DateTime.Day}\t{schoolyear}\t" +
                    $"{day.WeekdayNr}\t{day.WeekdayName}\t" +
                    $"{(day.IsWorkingDayMoFr ? 1 : 0)}\t{workingdayCounter}\t{(day.IsSchoolDayMoFr ? 1 : 0)}\t{schooldayCounter}\t" +
                    $"{(day.IsPublicHoliday ? 1 : 0)}\t{(day.IsSchoolHoliday ? 1 : 0)}\t{day.PublicHolidayName}\t{day.SchoolHolidayName}");
            }
        }
    }
}