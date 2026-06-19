using System;
using System.Collections.Generic;
using System.Linq;

public class Product
{
    public int Id { get; }
    public string Name { get; }

    public Product(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class SearchResult
{
    public int Index { get; }
    public int Comparisons { get; }
    public Product? FoundProduct { get; }

    public SearchResult(int index, int comparisons, Product? foundProduct)
    {
        Index = index;
        Comparisons = comparisons;
        FoundProduct = foundProduct;
    }
}

public static class SearchEngine
{
    public static SearchResult LinearSearch(Product[] catalog, int targetId)
    {
        int comparisons = 0;
        for (int i = 0; i < catalog.Length; i++)
        {
            comparisons++;
            if (catalog[i].Id == targetId)
            {
                return new SearchResult(i, comparisons, catalog[i]);
            }
        }
        return new SearchResult(-1, comparisons, null);
    }

    public static SearchResult BinarySearch(Product[] catalog, int targetId)
    {
        int left = 0;
        int right = catalog.Length - 1;
        int comparisons = 0;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            comparisons++;

            if (catalog[mid].Id == targetId)
            {
                return new SearchResult(mid, comparisons, catalog[mid]);
            }
            if (catalog[mid].Id < targetId)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return new SearchResult(-1, comparisons, null);
    }
}

class Program
{
    static void Main()
    {
        var catalog = new Product[]
        {
            new Product(101, "Wireless Mouse"),
            new Product(122, "Gaming Keyboard"),
            new Product(134, "Noise-Cancelling Headphones"),
            new Product(145, "Bluetooth Speaker"),
            new Product(157, "Smartwatch"),
            new Product(169, "Laptop Stand"),
            new Product(180, "Portable Charger"),
            new Product(192, "Webcam"),
            new Product(205, "USB-C Hub"),
            new Product(217, "External SSD")
        };

        Console.WriteLine("=== DSA Exercise 2: E-commerce Search Demo ===\n");
        PrintCatalog(catalog);

        var testIds = new int[] { 157, 101, 217 };
        PrintHeader("Existing products tests");
        var linearResults = new List<SearchResult>();
        var binaryResults = new List<SearchResult>();

        foreach (int targetId in testIds)
        {
            Console.WriteLine($"Searching for ID {targetId}...");
            var linearResult = SearchEngine.LinearSearch(catalog, targetId);
            var binaryResult = SearchEngine.BinarySearch(catalog, targetId);

            PrintResult("Linear Search", targetId, linearResult);
            PrintResult("Binary Search", targetId, binaryResult);
            Console.WriteLine();

            linearResults.Add(linearResult);
            binaryResults.Add(binaryResult);
        }

        PrintHeader("Missing product test");
        int missingId = 999;
        var linearMissing = SearchEngine.LinearSearch(catalog, missingId);
        var binaryMissing = SearchEngine.BinarySearch(catalog, missingId);
        PrintResult("Linear Search", missingId, linearMissing);
        PrintResult("Binary Search", missingId, binaryMissing);

        Console.WriteLine();
        PrintComparisonTable(testIds, linearResults, binaryResults, missingId, linearMissing, binaryMissing);
        PrintConclusion();
    }

    static void PrintCatalog(Product[] catalog)
    {
        Console.WriteLine("Product catalog:");
        foreach (var product in catalog)
        {
            Console.WriteLine($"  {product.Id} - {product.Name}");
        }
        Console.WriteLine();
    }

    static void PrintHeader(string title)
    {
        Console.WriteLine($"--- {title} ---\n");
    }

    static void PrintResult(string searchType, int targetId, SearchResult result)
    {
        string foundText = result.Index >= 0
            ? $"found {result.FoundProduct!.Name} at index {result.Index}"
            : "not found";
        Console.WriteLine($"{searchType} for {targetId}: {foundText} | comparisons = {result.Comparisons}");
    }

    static void PrintComparisonTable(int[] ids, List<SearchResult> linearResults, List<SearchResult> binaryResults, int missingId, SearchResult linearMissing, SearchResult binaryMissing)
    {
        Console.WriteLine("=== Algorithm Comparison ===");
        Console.WriteLine("ID      | Linear Comparisons | Binary Comparisons");
        Console.WriteLine("--------+--------------------+--------------------");

        for (int i = 0; i < ids.Length; i++)
        {
            Console.WriteLine($"{ids[i],-7} | {linearResults[i].Comparisons,-18} | {binaryResults[i].Comparisons,-18}");
        }

        Console.WriteLine($"{missingId,-7} | {linearMissing.Comparisons,-18} | {binaryMissing.Comparisons,-18}");
        Console.WriteLine();
    }

    static void PrintConclusion()
    {
        Console.WriteLine("Why Binary Search is usually better for sorted data:");
        Console.WriteLine("- Linear Search checks each item one by one (O(n)).");
        Console.WriteLine("- Binary Search halves the search range each time (O(log n)).");
        Console.WriteLine("- For this 10-item catalog, linear search may take up to 10 comparisons.");
        Console.WriteLine("- Binary search takes only 4 comparisons or fewer for the same catalog.");
        Console.WriteLine("- As the catalog grows, Binary Search becomes much faster than Linear Search.");
        Console.WriteLine();
        Console.WriteLine("✅ Summary:");
        Console.WriteLine("  - Linear Search: O(n)");
        Console.WriteLine("  - Binary Search: O(log n)");
        Console.WriteLine("  - Use Binary Search when data is sorted and random-access is available.");
        Console.WriteLine("  - Use Linear Search when the data is small, unsorted, or when items are added frequently.");
    }
}

