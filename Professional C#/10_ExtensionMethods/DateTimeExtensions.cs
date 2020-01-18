using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionDemo
{
    static class DateTimeExtensions
    {
        public static bool IsWeekend(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday) { return true; }
            if (date.DayOfWeek == DayOfWeek.Sunday) { return true; }
            return false;
        }
    }

    static class DateTimeHelpers
    {
        public static bool IsWeekend(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday) { return true; }
            if (date.DayOfWeek == DayOfWeek.Sunday) { return true; }
            return false;
        }
    }
}
