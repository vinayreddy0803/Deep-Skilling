using System;

public abstract class Document
{
    public abstract void Open();
    public abstract void Save();
}

public class PDFDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening PDF Document...");
    }
    
    public override void Save()
    {
        Console.WriteLine("Saving as PDF...");
    }
}

public class WordDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening Word Document...");
    }
    
    public override void Save()
    {
        Console.WriteLine("Saving as Word (.docx)...");
    }
}

public class ExcelDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening Excel Document...");
    }
    
    public override void Save()
    {
        Console.WriteLine("Saving as Excel (.xlsx)...");
    }
}

public class DocumentFactory
{
    public static Document CreateDocument(string documentType)
    {
        switch (documentType.ToLower())
        {
            case "pdf":
                return new PDFDocument();
            case "word":
                return new WordDocument();
            case "excel":
                return new ExcelDocument();
            default:
                throw new ArgumentException("Unknown document type");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Factory Method Pattern Demo ===\n");
        
        Console.WriteLine("Creating PDF Document:");
        Document pdfDoc = DocumentFactory.CreateDocument("pdf");
        pdfDoc.Open();
        pdfDoc.Save();
        
        Console.WriteLine();
        
        Console.WriteLine("Creating Word Document:");
        Document wordDoc = DocumentFactory.CreateDocument("word");
        wordDoc.Open();
        wordDoc.Save();
        
        Console.WriteLine();
        
        Console.WriteLine("Creating Excel Document:");
        Document excelDoc = DocumentFactory.CreateDocument("excel");
        excelDoc.Open();
        excelDoc.Save();
        
        Console.WriteLine("\n✅ All documents created using Factory!");
    }
}
