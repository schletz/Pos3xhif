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
    public void EasterSundayTest()
    {
        // Von ptb.de - https://www.ptb.de/cms/de/ptb/fachabteilungen/abt4/fb-44/ag-441/darstellung-der-gesetzlichen-zeit/wann-ist-ostern.html
        var easterSundays = new Dictionary<int, DateTime>
        {
            {2000,new DateTime(2000,4,23)}, {2001,new DateTime(2001,4,15)}, {2002,new DateTime(2002,3,31)}, {2003,new DateTime(2003,4,20)}, {2004,new DateTime(2004,4,11)}, {2005,new DateTime(2005,3,27)}, {2006,new DateTime(2006,4,16)}, {2007,new DateTime(2007,4,8)}, {2008,new DateTime(2008,3,23)}, {2009,new DateTime(2009,4,12)}, {2010,new DateTime(2010,4,4)}, {2011,new DateTime(2011,4,24)}, {2012,new DateTime(2012,4,8)}, {2013,new DateTime(2013,3,31)}, {2014,new DateTime(2014,4,20)}, {2015,new DateTime(2015,4,5)}, {2016,new DateTime(2016,3,27)}, {2017,new DateTime(2017,4,16)}, {2018,new DateTime(2018,4,1)}, {2019,new DateTime(2019,4,21)}, {2020,new DateTime(2020,4,12)}, {2021,new DateTime(2021,4,4)}, {2022,new DateTime(2022,4,17)}, {2023,new DateTime(2023,4,9)}, {2024,new DateTime(2024,3,31)}, {2025,new DateTime(2025,4,20)}, {2026,new DateTime(2026,4,5)}, {2027,new DateTime(2027,3,28)}, {2028,new DateTime(2028,4,16)}, {2029,new DateTime(2029,4,1)}, {2030,new DateTime(2030,4,21)}, {2031,new DateTime(2031,4,13)}
        };
        bool success = true;
        foreach (var easterSunday in easterSundays)
        {
            var easter = new CalendarYear(easterSunday.Key).EasterSunday;
            if (easter != easterSunday.Value) { success = false; continue; }
        }
        Assert.True(success);
    }

    [Fact]
    public void SchoolyearBeginTest()
    {
        var days = new Dictionary<int, int>
        {
            { 2022, 5 }, {2023, 4}, {2024, 2}, {2025, 1}, {2026, 7}, {2027, 6}, {2029, 3}
        };

        bool success = true;
        foreach (var day in days)
        {
            var schoolyearBegin = new CalendarYear(day.Key).SchoolyearBegin;
            if (schoolyearBegin != new DateTime(day.Key, 9, day.Value)) { success = false; continue; }
        }
        Assert.True(success);
    }
    [Fact]
    public void MainHolidayBeginTest()
    {
        var days = new Dictionary<int, (int month, int day)>
        {
            { 2022, (7,2) }, {2023, (7,1)}, {2024, (6,29)}, {2025, (6,28)}, {2026, (7,4)}, {2027, (7,3)}, {2029, (6,30)}
        };

        bool success = true;
        foreach (var day in days)
        {
            var mainHolidayBegin = new CalendarYear(day.Key).MainHolidayBegin;
            if (mainHolidayBegin != new DateTime(day.Key, day.Value.month, day.Value.day)) { success = false; continue; }
        }
        Assert.True(success);
    }

    [Fact]
    public void SemesterHolidayBeginTest()
    {
        var days = new Dictionary<int, int>
        {
            { 2022, 7 }, {2023, 6}, {2024, 5}, {2025, 3}, {2026, 2}, {2027, 1}, {2030, 4}
        };

        bool success = true;
        foreach (var day in days)
        {
            var schoolyearBegin = new CalendarYear(day.Key).SemesterHolidayBegin;
            if (schoolyearBegin != new DateTime(day.Key, 2, day.Value)) { success = false; continue; }
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
        // Für die Schuljahre 2000/2001 - 2399/2400 das Jahr 2400 auch generieren. Sonst wird
        // 2399/2400 am 1.1. abgeschnitten.
        for (int year = 2000; year <= 2400; year++)
        {
            var calendarYear = new CalendarYear(year);
            foreach (var day in calendarYear.GetCalendarDays())
            {
                var dateTime = day.DateTime;
                stream.WriteLine(
                    $"{day.DateTime:yyyy-MM-dd}\t{day.Date2000:yyyy-MM-dd}\t{day.DateTime.Year}\t{day.DateTime.Month}\t{day.DateTime.Day}\t{day.Schoolyear}\t" +
                    $"{day.WeekdayNr}\t{day.WeekdayName}\t" +
                    $"{(day.IsWorkingDayMoFr ? 1 : 0)}\t{day.WorkingdayCounter}\t{(day.IsSchoolDayMoFr ? 1 : 0)}\t{day.SchooldayCounter}\t" +
                    $"{(day.IsPublicHoliday ? 1 : 0)}\t{(day.IsSchoolHoliday ? 1 : 0)}\t{day.PublicHolidayName}\t{day.SchoolHolidayName}");
            }
        }
        Assert.True(true);
    }
}