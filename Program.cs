using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using VäderAppProd.Models;
using VäderAppProd.DataAccess;

namespace VäderAppProd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var filePath = @"C:\Users\Malek Mustafa\source\repos\VäderAppProd\bin\Debug\net8.0\TempFuktData.csv";

            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Filen {filePath} hittades inte. Kontrollera sökvägen och försök igen.");
                }

                var weatherData = ReadCsv(filePath);

                ExecuteWithDbContext(dbContext =>
                {
                    dbContext.Database.EnsureDeleted(); // Tar bort befintlig databas om den finns
                    dbContext.Database.EnsureCreated(); // Skapar databasen med rätt schema

                    if (!dbContext.WeatherData.Any())
                    {
                        dbContext.WeatherData.AddRange(weatherData);
                        dbContext.SaveChanges();
                        Console.WriteLine("Databasen har fyllts med data.");
                    }

                    Console.WriteLine($"Totalt antal rader: {dbContext.WeatherData.Count()}");
                });

                ShowMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade: {ex.Message}");
            }
        }

        private static void PauseBeforeReturning()
        {
            Console.WriteLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey();
        }

        public static void ShowMenu()
        {
            while (true)
            {
                Console.Clear(); // Rensar terminalen innan menyn visas
                Console.WriteLine("\nVälj en funktion:");
                Console.WriteLine("1. Visa medeltemperatur för ett datum (Ute)");
                Console.WriteLine("2. Sortera varmaste dagar (Ute)");
                Console.WriteLine("3. Sortera torraste dagar (Ute)");
                Console.WriteLine("4. Visa meteorologisk höst och vinter");
                Console.WriteLine("5. Visa dagar med störst mögelrisk (Ute)");
                Console.WriteLine("6. Visa medeltemperatur för ett datum (Inne)");
                Console.WriteLine("7. Sortera varmaste dagar inomhus");
                Console.WriteLine("8. Sortera torraste dagar inomhus");
                Console.WriteLine("9. Visa dagar med störst mögelrisk (Inne)");
                Console.WriteLine("10. Avsluta");

                var choice = GetValidatedInput("Välj ett alternativ (1-10): ", 1, 10);

                Console.Clear(); // Rensar konsolen direkt efter valet

                switch (choice)
                {
                    case 1:
                        ShowAverageTemperature("Ute");
                        break;
                    case 2:
                        ShowSortedTemperatures("Ute");
                        break;
                    case 3:
                        ShowSortedHumidity("Ute");
                        break;
                    case 4:
                        ShowMeteorologicalSeasons();
                        break;
                    case 5:
                        ShowMoldRisk("Ute");
                        break;
                    case 6:
                        ShowAverageTemperature("Inne");
                        break;
                    case 7:
                        ShowSortedTemperatures("Inne");
                        break;
                    case 8:
                        ShowSortedHumidity("Inne");
                        break;
                    case 9:
                        ShowMoldRisk("Inne");
                        break;
                    case 10:
                        Console.WriteLine("Avslutar programmet...");
                        Environment.Exit(0);
                        break;
                }

                PauseBeforeReturning(); // Vänta på att användaren trycker på en tangent innan menyn åter visas
            }
        }

        private static int GetValidatedInput(string prompt, int min, int max)
        {
            int result;
            do
            {
                Console.Write(prompt);
            } while (!int.TryParse(Console.ReadLine(), out result) || result < min || result > max);
            return result;
        }

        private static DateTime GetValidatedDate(string prompt)
        {
            DateTime date;
            do
            {
                Console.Write(prompt);
            } while (!DateTime.TryParse(Console.ReadLine(), out date));
            return date;
        }

        private static List<WeatherRecord> ReadCsv(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            });

            csv.Context.RegisterClassMap<VäderDataMap>();
            return csv.GetRecords<WeatherRecord>().ToList();
        }

        private static void ExecuteWithDbContext(Action<VäderDBContext> action)
        {
            using var dbContext = new VäderDBContext();
            action(dbContext);
        }

        public static void ShowAverageTemperature(string location)
        {
            var dateInput = GetValidatedDate("Ange ett datum (yyyy-MM-dd): ");

            ExecuteWithDbContext(dbContext =>
            {
                var avgTemp = dbContext.WeatherData
                    .Where(w => w.Date.Date == dateInput.Date && w.Location == location)
                    .Average(w => w.Temperature);

                Console.WriteLine($"Medeltemperatur för {location}: {avgTemp:F2}°C");
            });
        }

        public static void ShowSortedTemperatures(string location)
        {
            var count = GetValidatedInput("Hur många dagar vill du visa? ", 1, int.MaxValue);

            ExecuteWithDbContext(dbContext =>
            {
                var sortedDays = dbContext.WeatherData
                    .Where(w => w.Location == location)
                    .GroupBy(w => w.Date.Date)
                    .Select(g => new { Date = g.Key, AvgTemp = g.Average(w => w.Temperature) })
                    .OrderByDescending(d => d.AvgTemp)
                    .Take(count);

                foreach (var day in sortedDays)
                    Console.WriteLine($"{day.Date:yyyy-MM-dd}: {day.AvgTemp:F2}°C");
            });
        }

        public static void ShowSortedHumidity(string location)
        {
            var count = GetValidatedInput("Hur många dagar vill du visa? ", 1, int.MaxValue);

            ExecuteWithDbContext(dbContext =>
            {
                var sortedDays = dbContext.WeatherData
                    .Where(w => w.Location == location)
                    .GroupBy(w => w.Date.Date)
                    .Select(g => new { Date = g.Key, AvgHumidity = g.Average(w => w.Humidity) })
                    .OrderBy(d => d.AvgHumidity)
                    .Take(count);

                foreach (var day in sortedDays)
                    Console.WriteLine($"{day.Date:yyyy-MM-dd}: {day.AvgHumidity:F2}%");
            });
        }

        public static void ShowMeteorologicalSeasons()
        {
            ExecuteWithDbContext(dbContext =>
            {
                var autumnStart = dbContext.WeatherData
                    .Where(w => w.Location == "Ute")
                    .GroupBy(w => w.Date.Date)
                    .Where(g => g.Average(w => w.Temperature) < 10)
                    .Select(g => g.Key)
                    .OrderBy(date => date)
                    .FirstOrDefault();

                var winterStart = dbContext.WeatherData
                    .Where(w => w.Location == "Ute")
                    .GroupBy(w => w.Date.Date)
                    .Where(g => g.Average(w => w.Temperature) < 0)
                    .Select(g => g.Key)
                    .OrderBy(date => date)
                    .FirstOrDefault();

                Console.WriteLine($"Meteorologisk höst börjar: {autumnStart:yyyy-MM-dd}");
                Console.WriteLine($"Meteorologisk vinter börjar: {winterStart:yyyy-MM-dd}");
            });
        }

        public static void ShowMoldRisk(string location)
        {
            var count = GetValidatedInput("Hur många dagar vill du visa? ", 1, int.MaxValue);

            ExecuteWithDbContext(dbContext =>
            {
                // Hämta alla data till minnet
                var data = dbContext.WeatherData
                    .Where(w => w.Location == location)
                    .AsEnumerable() // Flytta vidare beräkningar till klienten
                    .GroupBy(w => w.Date.Date)
                    .Select(g => new { Date = g.Key, AvgMoldRisk = g.Average(w => w.MoldRisk) })
                    .OrderByDescending(d => d.AvgMoldRisk)
                    .Take(count);

                Console.WriteLine($"Dagar med störst mögelrisk ({location}):");
                foreach (var day in data)
                {
                    Console.WriteLine($"{day.Date:yyyy-MM-dd}: {day.AvgMoldRisk:F2}");
                }
            });
        }
    }
}