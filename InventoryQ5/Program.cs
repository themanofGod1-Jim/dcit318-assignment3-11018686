// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// b) Marker Interface
public interface IInventoryEntity
{
    int Id { get; }
}

// a) Immutable Inventory Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// c) Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly string _filePath;
    private List<T> _log = new();

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
        Console.WriteLine($"Added: {item}");
    }

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
            Console.WriteLine($"Data saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File {_filePath} not found. No data loaded.");
                return;
            }

            string json = File.ReadAllText(_filePath);
            _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            Console.WriteLine($"Data loaded from {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file: {ex.Message}");
        }
    }
}

// f) Integration Layer – InventoryApp
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger = new("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Smartphone", 25, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Milk", 50, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Bread", 30, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Tablet", 15, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        Console.WriteLine("=== Inventory Items ===");
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine(item);
        }
    }
}

// g) Main Application Flow
class Program
{
    static void Main()
    {
        var app = new InventoryApp();

        // Seed sample data
        app.SeedSampleData();

        // Save to file
        app.SaveData();

        // Clear memory and simulate new session
        Console.WriteLine("\nClearing memory and reloading...\n");

        var newAppSession = new InventoryApp();
        newAppSession.LoadData();

        // Print all items
        newAppSession.PrintAllItems();
    }
}

