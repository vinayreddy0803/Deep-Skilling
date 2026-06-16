using System;

public class DatabaseConnection
{
    private static DatabaseConnection? instance = null;
    
    private DatabaseConnection()
    {
        Console.WriteLine("Database Connection Created!");
    }
    
    public static DatabaseConnection GetInstance()
    {
        if (instance == null)
        {
            instance = new DatabaseConnection();
        }
        return instance;
    }
    
    public void Connect()
    {
        Console.WriteLine("Connected to Database!");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Singleton Pattern Demo ===\n");
        
        DatabaseConnection db1 = DatabaseConnection.GetInstance();
        db1.Connect();
        
        DatabaseConnection db2 = DatabaseConnection.GetInstance();
        db2.Connect();
        
        Console.WriteLine("\nAre db1 and db2 the same object?");
        Console.WriteLine(db1 == db2);
    }
}