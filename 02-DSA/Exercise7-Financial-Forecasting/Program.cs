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
    static void Main()
    {
        var revenueHistory = new List<RevenuePoint>
        {
            new RevenuePoint(1,  12000m),
            new RevenuePoint(2,  13400m),
            new RevenuePoint(3,  12800m),
            new RevenuePoint(4,  14100m),
            new RevenuePoint(5,  15000m),
            new RevenuePoint(6,  16000m),
            new RevenuePoint(7,  17200m),
            new RevenuePoint(8,  16800m),
            new RevenuePoint(9,  17900m),
            new RevenuePoint(10, 19000m)
        };

        Console.WriteLine("=== Financial Forecasting Exercise ===\n");

        Console.WriteLine("Historical revenue data:");
        foreach (var point in revenueHistory)
        {
            Console.WriteLine($"Month {point.Month,2}: {point.Revenue:C0}");
        }

        Console.WriteLine();
        Console.WriteLine("Forecast methods:");
        Console.WriteLine("1. Simple Moving Average (SMA)");
        Console.WriteLine("2. Linear Regression\n");

        int forecastMonths = 3;
        int smaWindow = 4;
        Forecasting.PrintForecastComparison(revenueHistory, forecastMonths, smaWindow);

        var regressionModel = Forecasting.LinearRegression(revenueHistory);
        Console.WriteLine("Regression model:");
        Console.WriteLine($"  Revenue = {regressionModel.Slope:F2} * month + {regressionModel.Intercept:F2}\n");

        Console.WriteLine("Why this exercise matters:");
        Console.WriteLine("- SMA smooths short-term variation and is useful for baseline planning.");
        Console.WriteLine("- Regression captures the long-term trend so forecasts adapt to growth.");
        Console.WriteLine("- Real-world forecasting compares multiple methods before choosing a strategy.");
        Console.WriteLine("- This demo teaches how data-driven financial projections are built.");
    }
}
