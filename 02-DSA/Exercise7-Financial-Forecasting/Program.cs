using System;
using System.Collections.Generic;
using System.Linq;

public class RevenuePoint
{
    public int Month { get; }
    public decimal Revenue { get; }

    public RevenuePoint(int month, decimal revenue)
    {
        Month = month;
        Revenue = revenue;
    }
}

public static class Forecasting
{
    public static decimal SimpleMovingAverage(IEnumerable<RevenuePoint> history, int windowSize)
    {
        var values = history.Select(point => point.Revenue).TakeLast(windowSize);
        return values.Average();
    }

    public static (decimal Slope, decimal Intercept) LinearRegression(IEnumerable<RevenuePoint> history)
    {
        var points = history.ToList();
        int n = points.Count;
        decimal sumX = points.Sum(p => p.Month);
        decimal sumY = points.Sum(p => p.Revenue);
        decimal sumXY = points.Sum(p => p.Month * p.Revenue);
        decimal sumX2 = points.Sum(p => p.Month * p.Month);

        decimal slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        decimal intercept = (sumY - slope * sumX) / n;
        return (slope, intercept);
    }

    public static decimal PredictUsingRegression((decimal Slope, decimal Intercept) model, int month)
    {
        return model.Slope * month + model.Intercept;
    }

    public static void PrintForecastComparison(IEnumerable<RevenuePoint> history, int forecastMonths, int smaWindow)
    {
        var historyList = history.ToList();
        var regressionModel = LinearRegression(historyList);
        var lastMonth = historyList.Max(p => p.Month);

        Console.WriteLine("Forecast comparison for the next {0} months:\n", forecastMonths);
        Console.WriteLine("Month | SMA Forecast | Regression Forecast");
        Console.WriteLine("------+--------------+---------------------");

        for (int i = 1; i <= forecastMonths; i++)
        {
            int futureMonth = lastMonth + i;
            decimal smaForecast = SimpleMovingAverage(historyList, smaWindow);
            decimal regressionForecast = PredictUsingRegression(regressionModel, futureMonth);
            Console.WriteLine($"{futureMonth,5} | {smaForecast,12:C0} | {regressionForecast,19:C0}");

            historyList.Add(new RevenuePoint(futureMonth, smaForecast));
        }

        Console.WriteLine();
    }
}

class Program
{
    // Recursive future value calculation:
    // FV(present, rate, years) = (1 + rate) * FV(present, rate, years - 1)
    // Base case: years == 0 => return present
    static decimal FutureValueRecursive(decimal presentValue, decimal annualRate, int years)
    {
        if (years <= 0)
            return presentValue;

        decimal next = presentValue * (1 + annualRate);
        return FutureValueRecursive(next, annualRate, years - 1);
    }

    // Iterative version for verification
    static decimal FutureValueIterative(decimal presentValue, decimal annualRate, int years)
    {
        decimal fv = presentValue;
        for (int i = 0; i < years; i++)
        {
            fv *= (1 + annualRate);
        }
        return fv;
    }

    static void Main()
    {
        Console.WriteLine("=== Exercise7: Financial Forecasting (Recursive Future Value) ===\n");

        // Sample scenarios to demonstrate recursion
        var scenarios = new[]
        {
            (present: 1000m, rate: 0.05m, years: 5),
            (present: 5000m, rate: 0.07m, years: 10),
            (present: 15000m, rate: 0.06m, years: 20)
        };

        foreach (var s in scenarios)
        {
            decimal fvRec = FutureValueRecursive(s.present, s.rate, s.years);
            decimal fvItr = FutureValueIterative(s.present, s.rate, s.years);
            Console.WriteLine($"Present: {s.present:C0}, Rate: {s.rate:P0}, Years: {s.years}");
            Console.WriteLine($"  Future Value (Recursive): {fvRec:C2}");
            Console.WriteLine($"  Future Value (Iterative): {fvItr:C2}");
            Console.WriteLine("  Difference: " + (fvRec - fvItr).ToString("C2") + "\n");
        }

        // Interactive example: accept user input and compute
        Console.WriteLine("Interactive mode: enter present value, annual rate (%) and years.");
        Console.Write("Present value (e.g. 10000): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal pv)) pv = 10000m;

        Console.Write("Annual rate percent (e.g. 7 for 7%): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal ratePercent)) ratePercent = 7m;
        decimal rateDec = ratePercent / 100m;

        Console.Write("Years (integer): ");
        if (!int.TryParse(Console.ReadLine(), out int yrs)) yrs = 10;

        decimal forecastRec = FutureValueRecursive(pv, rateDec, yrs);
        Console.WriteLine($"\nForecast after {yrs} years (recursive): {forecastRec:C2}");

        Console.WriteLine("\n✅ Recursive forecasting completed.");
    }
}
