// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;

namespace WarehouseQ3
{
    // a) Marker Interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b) ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"[Electronic] ID: {Id}, Name: {Name}, Brand: {Brand}, Qty: {Quantity}, Warranty: {WarrantyMonths} months";
    }

    // c) GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() => $"[Grocery] ID: {Id}, Name: {Name}, Qty: {Quantity}, Expiry: {ExpiryDate:d}";
    }

    // e) Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d) Generic Inventory Repository
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
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items.Remove(id);
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException($"Quantity cannot be negative: {newQuantity}");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // f) WarehouseManager
    public class WarehouseManager
    {
        public InventoryRepository<ElectronicItem> Electronics { get; } = new();
        public InventoryRepository<GroceryItem> Groceries { get; } = new();

        public void SeedData()
        {
            // Electronics
            Electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            Electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));

            // Groceries
            Groceries.AddItem(new GroceryItem(101, "Milk", 50, DateTime.Today.AddDays(7)));
            Groceries.AddItem(new GroceryItem(102, "Bread", 30, DateTime.Today.AddDays(3)));
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
                Console.WriteLine($"Updated stock for {item.Name}. New quantity: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var manager = new WarehouseManager();

            // Seed data
            manager.SeedData();

            Console.WriteLine("=== Grocery Items ===");
            manager.PrintAllItems(manager.Groceries);

            Console.WriteLine("\n=== Electronic Items ===");
            manager.PrintAllItems(manager.Electronics);

            Console.WriteLine("\n--- Test Exceptions ---");

            // Duplicate item
            try
            {
                manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Lenovo", 12));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Duplicate test: {ex.Message}");
            }

            // Remove non-existent item
            manager.RemoveItemById(manager.Groceries, 999);

            // Invalid quantity update
            manager.IncreaseStock(manager.Electronics, 2, -50);
        }
    }
}

