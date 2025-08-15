using System;
using System.Collections.Generic;


public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}


public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
    }

    public override string ToString() => $"Electronics #{Id}: {Name} ({Brand}), Qty={Quantity}, Warranty={WarrantyMonths}m";
}


public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiry)
    {
        Id = id; Name = name; Quantity = quantity; ExpiryDate = expiry;
    }

    public override string ToString() => $"Grocery #{Id}: {Name}, Qty={Quantity}, Exp={ExpiryDate:d}";
}


public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }


public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var v))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return v;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems() => new(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Phone", 10, "Samsung", 12));
        _groceries.AddItem(new GroceryItem(100, "Rice 5kg", 20, DateTime.Today.AddMonths(12)));
        _groceries.AddItem(new GroceryItem(101, "Milk 1L", 50, DateTime.Today.AddMonths(2)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
            Console.WriteLine(item);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased. New qty = {item.Quantity}");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine("Item removed.");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    public static void Main()
    {
        var mgr = new WareHouseManager();
        mgr.SeedData();

        Console.WriteLine("Groceries:");
        mgr.PrintAllItems(mgr._groceries);
        Console.WriteLine("\nElectronics:");
        mgr.PrintAllItems(mgr._electronics);

     
        try { mgr._electronics.AddItem(new ElectronicItem(1, "Tablet", 3, "Apple", 12)); }
        catch (Exception ex) { Console.WriteLine($"Duplicate add: {ex.Message}"); }

        mgr.RemoveItemById(mgr._groceries, 999);   // non-existent
        try { mgr._electronics.UpdateQuantity(2, -5); }
        catch (Exception ex) { Console.WriteLine($"Invalid qty: {ex.Message}"); }
    }
}
