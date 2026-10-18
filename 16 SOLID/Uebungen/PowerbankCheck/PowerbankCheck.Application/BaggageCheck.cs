using System.Collections.Generic;
using System.Globalization;

namespace PowerbankCheck.Application;

/// <summary>
/// Checks the baggage of passengers before boarding. The rules are described in the README of
/// this exercise.
/// </summary>
public class BaggageCheck
{
    /// <summary>
    /// Checks the carry-on bag of a passenger.
    /// </summary>
    public CheckResult CheckCarryOn(Passenger passenger)
    {
        foreach (string item in passenger.CarryOn.Items)
        {
            string name = item.ToLower();
            if (name == "fireworks" || name == "camping gas" || name == "lighter fluid")
            {
                return CheckResult.Forbidden;
            }
        }

        if (passenger.CarryOn.Powerbanks.Count > 2)
        {
            return CheckResult.Forbidden;
        }

        bool approvalRequired = false;
        foreach (Powerbank powerbank in passenger.CarryOn.Powerbanks)
        {
            decimal wattHours = powerbank.CapacityMilliampHours * powerbank.Voltage / 1000;
            if (wattHours > 160)
            {
                return CheckResult.Forbidden;
            }
            if (wattHours > 100)
            {
                approvalRequired = true;
            }
        }

        if (approvalRequired)
        {
            return CheckResult.ApprovalRequired;
        }
        return CheckResult.Allowed;
    }

    /// <summary>
    /// Checks the checked bag of a passenger.
    /// </summary>
    public CheckResult CheckCheckedBag(Passenger passenger)
    {
        // Powerbanks are never allowed in the cargo hold.
        if (passenger.CheckedBag.Powerbanks.Count > 0)
        {
            return CheckResult.Forbidden;
        }

        foreach (string item in passenger.CheckedBag.Items)
        {
            if (item.ToLower() == "fireworks" || item.ToLower() == "camping gas" || item.ToLower() == "lighter fluid")
            {
                return CheckResult.Forbidden;
            }
        }
        return CheckResult.Allowed;
    }

    /// <summary>
    /// Counts the powerbanks in the carry-on bags that need an approval of the airline.
    /// </summary>
    public int CountRequiredApprovals(IEnumerable<Passenger> passengers)
    {
        int count = 0;
        foreach (Passenger passenger in passengers)
        {
            foreach (Powerbank powerbank in passenger.CarryOn.Powerbanks)
            {
                if (powerbank.CapacityMilliampHours * powerbank.Voltage / 1000 > 100
                    && powerbank.CapacityMilliampHours * powerbank.Voltage / 1000 <= 160)
                {
                    count++;
                }
            }
        }
        return count;
    }

    /// <summary>
    /// Creates a report for the gate staff with one line for every powerbank and item.
    /// </summary>
    public List<string> CreateReport(Passenger passenger)
    {
        List<string> lines = new List<string>();
        lines.Add($"Passenger: {passenger.Name}");

        foreach (Powerbank powerbank in passenger.CarryOn.Powerbanks)
        {
            decimal wh = powerbank.CapacityMilliampHours * powerbank.Voltage / 1000;
            string status = "allowed";
            if (wh > 160)
            {
                status = "forbidden";
            }
            else if (wh > 100)
            {
                status = "approval required";
            }
            lines.Add($"Carry-on: powerbank \"{powerbank.Name}\", {wh.ToString("0.0", CultureInfo.InvariantCulture)} Wh: {status}");
        }
        if (passenger.CarryOn.Powerbanks.Count > 2)
        {
            lines.Add($"Carry-on: too many powerbanks ({passenger.CarryOn.Powerbanks.Count}, max. 2)");
        }
        foreach (string item in passenger.CarryOn.Items)
        {
            string name = item.ToLower();
            bool forbidden = name == "fireworks" || name == "camping gas" || name == "lighter fluid";
            lines.Add($"Carry-on: item \"{item}\": {(forbidden ? "forbidden" : "allowed")}");
        }

        foreach (Powerbank powerbank in passenger.CheckedBag.Powerbanks)
        {
            decimal wh = powerbank.CapacityMilliampHours * powerbank.Voltage / 1000;
            lines.Add($"Checked bag: powerbank \"{powerbank.Name}\", {wh.ToString("0.0", CultureInfo.InvariantCulture)} Wh: forbidden");
        }
        foreach (string item in passenger.CheckedBag.Items)
        {
            string name = item.ToLower();
            bool forbidden = name == "fireworks" || name == "camping gas" || name == "lighter fluid";
            lines.Add($"Checked bag: item \"{item}\": {(forbidden ? "forbidden" : "allowed")}");
        }

        lines.Add($"Result carry-on: {CheckCarryOn(passenger)}");
        lines.Add($"Result checked bag: {CheckCheckedBag(passenger)}");
        return lines;
    }
}
