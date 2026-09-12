using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryRecordsSystem
{
    // b. Marker interface for logging
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // a. Immutable inventory record (implements IInventoryEntity)
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c. Generic inventory logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void SaveToFile()
        {
            try
            {
                using (var writer = new StreamWriter(_filePath))
                {
                    string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                    writer.Write(json);
                }

                Console.WriteLine($"Saved {_log.Count} item(s) to {_filePath}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied while saving file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    string json = reader.ReadToEnd();
                    var items = JsonSerializer.Deserialize<List<T>>(json);

                    _log.Clear();
                    if (items != null)
                    {
                        _log.AddRange(items);
                    }
                }

                Console.WriteLine($"Loaded {_log.Count} item(s) from {_filePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing file contents: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
    }

    // f. Integration layer
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Office Chair", 25, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Printer Paper (Ream)", 100, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Wireless Mouse", 40, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Desk Lamp", 15, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            Console.WriteLine("\n--- Inventory Items ---");
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:g}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Inventory Records System ===\n");

            string filePath = "inventory.json";

            // Session 1: create and persist data
            var app = new InventoryApp(filePath);
            app.SeedSampleData();
            app.SaveData();

            // Simulate a new session by creating a fresh InventoryApp instance
            Console.WriteLine("\nSimulating a new session (memory cleared)...\n");
            var newSessionApp = new InventoryApp(filePath);
            newSessionApp.LoadData();
            newSessionApp.PrintAllItems();
        }
    }
}
