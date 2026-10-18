using System;

namespace BusStatistics.Application.Model;

/// <summary>
/// The weather of one day, imported from a weather service. There is no foreign key to the
/// trips or interventions: the only connection is the date.
/// </summary>
public class WeatherReading
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected WeatherReading() { }
#pragma warning restore CS8618

    public WeatherReading(DateOnly date, string condition, decimal temperatureC)
    {
        Date = date;
        Condition = condition;
        TemperatureC = temperatureC;
    }

    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Condition { get; set; }
    public decimal TemperatureC { get; set; }
}
