﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace HolidayCalendar;
public class CalendarServiceTests
{
    [Fact]
    public void CalendarDaysLengthTest()
    {
        var service = new CalendarService(2000, 2400);
        // 2100, 2200 und 2300 sind keine Schaltjahre nach dem gregorianischem Kalender. Daher -3.
        Assert.True(service.CalendarDays.Length == 400 * 365 + 100 - 3);
    }
    [Fact]
    public void CalendarDaysAscendingTest()
    {
        var days = new CalendarService(2000, 2400).CalendarDays;
        var start = new DateTime(2000, 1, 1);
        for (int i = 0; i < days.Length; i++)
        {
            if (days[i].DateTime != start.AddDays(i))
            {
                Assert.True(false);
                return;
            }
        }
        Assert.True(true);
    }
    [Fact]
    public void GetDaysOfYearTest()
    {
        var service = new CalendarService(2000, 2101);
        var dayOfYear = service.GetDaysOfYear(2100);
        Assert.True(dayOfYear.Length == 365 && dayOfYear[0].DateTime == new DateTime(2100, 1, 1) && dayOfYear[364].DateTime == new DateTime(2100, 12, 31));
    }
    [Fact]
    public void GetDaysOfMonthTest()
    {
        var service = new CalendarService(2022, 2026);
        var days1 = service.GetDaysOfMonth(2025, 12);
        var days2 = service.GetDaysOfMonth(2024, 2);
        Assert.True(days1.Length == 31 && days1[0].DateTime == new DateTime(2025, 12, 1) && days1[30].DateTime == new DateTime(2025, 12, 31));
        Assert.True(days2.Length == 29 && days2[0].DateTime == new DateTime(2024, 2, 1) && days2[28].DateTime == new DateTime(2024, 2, 29));
        Assert.Throws<ArgumentException>(() => service.GetDaysOfMonth(2021, 12));
        Assert.Throws<ArgumentException>(() => service.GetDaysOfMonth(2026, 1));

    }
    [Fact]
    public void CalendarDaysWorkingCounterTest()
    {
        var days = new CalendarService(2000, 2400).CalendarDays;
        int workingDayCounter = 0;
        foreach (var day in days)
        {
            if (day.IsWorkingDayMoFr) { workingDayCounter++; }
            if (day.WorkingdayCounter != workingDayCounter)
            {
                Assert.True(false);
                return;
            }
        }
        Assert.True(true);
    }
    [Fact]
    public void CalendarDaysSchooldayCounterTest()
    {
        var days = new CalendarService(2000, 2400).CalendarDays;
        int schoolDayCounter = 0;
        foreach (var day in days)
        {
            if (day.IsSchoolDayMoFr) { schoolDayCounter++; }
            if (day.SchooldayCounter != schoolDayCounter)
            {
                Assert.True(false);
                return;
            }
        }
        Assert.True(true);
    }

    [Fact]
    public void GetWorkingDaysBetweenTest()
    {
        var service = new CalendarService(2000, 2400);
        Assert.True(service.GetWorkingDaysBetween(new DateTime(2000, 1, 4), new DateTime(2000, 1, 5)) == 1);
        Assert.True(service.GetWorkingDaysBetween(new DateTime(2000, 1, 5), new DateTime(2000, 1, 7)) == 1);
        Assert.True(service.GetWorkingDaysBetween(new DateTime(2000, 1, 6), new DateTime(2000, 1, 7)) == 1);
        Assert.True(service.GetWorkingDaysBetween(new DateTime(2000, 1, 5), new DateTime(2000, 1, 10)) == 2);
    }

    [Fact]
    public void AddWorkingDaysTest()
    {
        var service = new CalendarService(2000, 2400);
        Assert.True(service.AddWorkingDays(new DateTime(2000, 1, 4), 1) == new DateTime(2000, 1, 5));
        Assert.True(service.AddWorkingDays(new DateTime(2000, 1, 5), 1) == new DateTime(2000, 1, 7));
        Assert.True(service.AddWorkingDays(new DateTime(2000, 1, 6), 1) == new DateTime(2000, 1, 7));
        Assert.True(service.AddWorkingDays(new DateTime(2000, 1, 5), 2) == new DateTime(2000, 1, 10));

        Assert.True(service.AddWorkingDays(new DateTime(2000, 1, 5), -1) == new DateTime(2000, 1, 4));
        Assert.True(service.AddWorkingDays(new DateTime(2000, 1, 7), -1) == new DateTime(2000, 1, 5));
        Assert.True(service.AddWorkingDays(new DateTime(2000, 1, 10), -2) == new DateTime(2000, 1, 5));

    }

    [Fact]
    public void WriteFileTest()
    {
        using var stream = new StreamWriter(File.Create("../../../calendar.txt"), Encoding.Unicode);
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
