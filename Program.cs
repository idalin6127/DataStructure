using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Lab1
{

    class Medalist
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Medals { get; set; }
    }

    class Program
    {
        private static List<Medalist> medalists = new List<Medalist>();
        static void Main(string[] args)
        {
            LoadMedalistsFromCSV();

            while (true)
            {
                Console.WriteLine("Choose an option: ");
                Console.WriteLine("1. Add new medalist information");
                Console.WriteLine("2. Delete a specific medalist");
                Console.WriteLine("3. Search for a medalist");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddMedalist();
                            break;
                        case 2:
                            DeleteMedalist();
                            break;
                        case 3:
                            SearchMedalist();
                            break;
                        case 4:
                            SaveMedalistsToCSV();
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }

        static void LoadMedalistsFromCSV()
        {
            try
            {
                using (StreamReader reader = new StreamReader("Medals.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        string[] data = reader.ReadLine().Split(',');
                        if (data.Length == 5)
                        {
                            string name = data[0].Trim();
                            string country = data[1];
                            if (int.TryParse(data[2], out int medals))
                            {
                                medalists.Add(new Medalist
                                {
                                    Name = name,
                                    Country = country,
                                    Medals = medals
                                });
                            }
                            else
                            {
                                Console.WriteLine($"Error parsing medals for {name}. Skipping this entry.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid data format. Skipping this entry.");
                        }
                    }
                }

                // Display loaded data for debugging
                Console.WriteLine("Loaded Medalists:");
                foreach (var medalist in medalists)
                {
                    Console.WriteLine($"Name: {medalist.Name}, Country: {medalist.Country}, Medals: {medalist.Medals}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Medals.csv not found. Please create the file with data.");
            }
        }

        static void SaveMedalistsToCSV()
        {
            using (StreamWriter writer = new StreamWriter("Medals.csv"))
            {
                foreach (var medalist in medalists)
                {
                    Console.WriteLine($"{medalist.Name},{medalist.Country},{medalist.Medals}");
                }
            }
        }
        static void AddMedalist()
        {
            Console.Write("Enter medalist's name: ");
            string name = Console.ReadLine();
            Console.Write("Enter medalist's country: ");
            string country = Console.ReadLine();

            Console.Write("Enter number of medals: ");
            if (int.TryParse(Console.ReadLine(), out int medals))
            {
                medalists.Add(new Medalist
                {
                    Name = name,
                    Country = country,
                    Medals = medals
                });
                Console.WriteLine("Medalist added successfully.");
            }
            else
            {
                Console.WriteLine("Invalid input for medals. Please enter a number.");
            }
        }

        static void DeleteMedalist()
        {
            Console.WriteLine("Enter the name of the medalist to delete: ");
            string nameToDelete = Console.ReadLine();
            var medalistToDelete = medalists.FirstOrDefault(m => m.Name.Equals(nameToDelete, StringComparison.OrdinalIgnoreCase));

            if (medalistToDelete != null)
            {
                medalists.Remove(medalistToDelete);
                Console.WriteLine($"{nameToDelete} has been deleted. ");
            }
            else
            {
                Console.WriteLine($"Medalist with name {nameToDelete} not found.");
            }
        }

        static void SearchMedalist()
        {
            Console.Write("Enter the name of the medalist to search： ");
            string searchName = Console.ReadLine();
            Console.WriteLine($"Searching for: {searchName}");

            var searchResults = Search(medalists, m => m.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase));
            if (searchResults.Any())
            {
                Console.WriteLine("Search Results: ");
                foreach (var result in searchResults)
                {
                    Console.WriteLine($"Name: {result.Name}, Country: {result.Country},Medals: {result.Medals}");
                }
            }
            else
            {
                Console.WriteLine($"No medalist with the name {searchName} found.");
            }
        }
        static IEnumerable<T> Search<T>(IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

    }

}