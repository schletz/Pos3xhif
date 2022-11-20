using System;
using System.Linq;
using Xunit;

namespace HolidayCalendar;
public class CalendarServiceTests
{
    [Fact]
    public void CalendarDaysLengthTest()
    {
        var service = new CalendarService(2000, 2400);
        // 2100, 2200 und 2300 sind keine Schaltjahre nach dem gregorianischem Kalender. Daher -3.
        Assert.True(service.CalendarDays.Count == 400 * 365 + 100 - 3);
    }
    [Fact]
    public void CalendarDaysAscendingTest()
    {
        var days = new CalendarService(2000, 2400).CalendarDays;
        var start = new DateTime(2000, 1, 1);
        for (int i = 0; i < days.Count; i++)
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
        var service = new CalendarService(2000, 2400);
        var dayOfYear = service.GetDaysOfYear(2100);
        Assert.True(dayOfYear.First().DateTime == new DateTime(2100, 1, 1) && dayOfYear.Last().DateTime == new DateTime(2100, 12, 31));
    }
    [Fact]
    public void GetDaysOfMonthTest()
    {
        var service = new CalendarService(2000, 2400);
        var dayOfYear = service.GetDaysOfMonth(2000, 2);
        Assert.True(dayOfYear.First().DateTime == new DateTime(2000, 2, 1) && dayOfYear.Last().DateTime == new DateTime(2000, 2, 29));
    }
    [Fact]
    public void CalendarDaysWorkingCounterTest()
    {
        var days = new CalendarService(2000, 2400).CalendarDays;
        int workingDayCounter = 0;
        foreach(var day in days)
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
}
