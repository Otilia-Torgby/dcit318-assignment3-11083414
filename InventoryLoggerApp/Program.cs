using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


public interface IInventoryEntity { int Id { get; } }


public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;


public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath) { _filePath = filePath; }

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new(_log);

    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using var sw = new StreamWriter(_filePath);
            sw.Write(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Save error: " + ex.Message);
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath)) { Console.WriteLine("No saved file found."); return; }
            using var sr = new StreamReader(_filePath);
            var json = sr.ReadToEnd();
            var loaded = JsonSerializer.Deserialize<List<T>>(json);
            _log.Clear();
            if (loaded != null) _log.AddRange(loaded);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Load error: " + ex.Message);
        }
    }
}


public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger = new("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Stapler", 12, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Printer Paper (A4)", 500, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Markers", 40, DateTime.Now));
        _logger.Add(new InventoryItem(4, "USB Drives", 25, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Printer Toner", 6, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
            Console.WriteLine($"{item.Id}: {item.Name} | Qty={item.Quantity} | Added={item.DateAdded:g}");
    }

    public static void Main()
    {
        var app = new InventoryApp();

        app.SeedSampleData();
        app.SaveData();

        app = new InventoryApp();
        app.LoadData();
        app.PrintAllItems();
    }
}
