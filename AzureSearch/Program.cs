using AzureSearch.Infrastructure.Models;
using AzureSearch.Services;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AzureSearch
{
    class Program
    {
        private static readonly SearchServiceClient _serviceClient = AzureSearchService.CreateSearchServiceClient();

        static void Main(string[] args)
        {
            ProcessSearchIndexForHotels();
            ProcessSearchIndexForPeople(true);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("{0}", "Complete.  Press any key to end application...\n");
            Console.ResetColor();
            Console.ReadKey();
        }


        #region people search index

        private static void ProcessSearchIndexForPeople(bool createIndex = false)
        {
            string peopleIndexName = "people";
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            if (createIndex)
            {
                Console.WriteLine("{0}", "Deleting index...\n");
                AzureSearchService.DeleteIndexIfExists(_serviceClient, peopleIndexName);

                Console.WriteLine("{0}", "Creating index...\n");
                AzureSearchService.CreateIndex<People>(_serviceClient, peopleIndexName);

                ISearchIndexClient indexClient = _serviceClient.Indexes.GetClient(peopleIndexName);

                Console.WriteLine("{0}", "Uploading documents...\n");
                AzureSearchService.UploadDocuments(indexClient, PopulatePeopleIndex());
            }

            ISearchIndexClient indexClientForQueries = AzureSearchService.CreateSearchIndexClient(peopleIndexName);

            RunPeopleQueries(indexClientForQueries);          
        }

        private static IEnumerable<IndexAction<People>> PopulatePeopleIndex()
        {
            List<IndexAction<People>> actions = new List<IndexAction<People>>();
            var fileLocation = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\Data\\people.json";

            using (StreamReader r = new StreamReader(fileLocation))
            {
                var peopleData = JsonConvert.DeserializeObject<IEnumerable<People>>(r.ReadToEnd());

                foreach (People p in peopleData)
                {
                    actions.Add(
                        IndexAction.MergeOrUpload(
                            new People()
                            {
                                Id = p.Id,
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                Email = p.Email,
                                Gender = p.Gender,
                                IpAddress = p.IpAddress
                            })
                        );
                }
            }
            return actions;
        }

        private static void RunPeopleQueries(ISearchIndexClient indexClient)
        {
            SearchParameters parameters;
            DocumentSearchResult<People> results;

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Search the entire index for people named 'Bjorn' and return firstName and LastName fields:\n");
            Console.ResetColor();

            parameters =
                new SearchParameters()
                {
                    Select = new[] { "first_name", "last_name" }
                };

            results = indexClient.Documents.Search<People>("Bjorn", parameters);

            WriteDocuments<People>(results);


            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Search the entire index and filter gender = 'Female':\n");
            Console.ResetColor();

            parameters =
               new SearchParameters()
               {
                   Filter = "gender eq 'Female'",
                   Select = new[] { "first_name", "last_name", "email" }
               };

            results = indexClient.Documents.Search<People>("*", parameters);

            WriteDocuments<People>(results);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Search top 5 people order by LastName:\n");
            Console.ResetColor();

            parameters =
               new SearchParameters()
               {
                   OrderBy = new[] { "last_name asc" },
                   Select = new[] { "first_name", "last_name", "email", "gender", "ip_address" },
                   Top = 5,
               };

            results = indexClient.Documents.Search<People>("*", parameters);


            WriteDocuments<People>(results);
        }

        #endregion people search index

        #region hotels search index

        private static void ProcessSearchIndexForHotels(bool createIndex = false)
        {
            string hotelsIndexName = "hotels";
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            if (createIndex)
            {
                Console.WriteLine("{0}", "Deleting index...\n");
                AzureSearchService.DeleteIndexIfExists(_serviceClient, hotelsIndexName);

                Console.WriteLine("{0}", "Creating index...\n");
                AzureSearchService.CreateIndex<Hotel>(_serviceClient, hotelsIndexName);

                ISearchIndexClient indexClient = _serviceClient.Indexes.GetClient(hotelsIndexName);

                Console.WriteLine("{0}", "Uploading documents...\n");
                AzureSearchService.UploadDocuments(indexClient, PopulateHotelIndex());
            }

            ISearchIndexClient indexClientForQueries = AzureSearchService.CreateSearchIndexClient(hotelsIndexName);

            RunHotelQueries(indexClientForQueries);
        }

        private static IEnumerable<IndexAction<Hotel>> PopulateHotelIndex()
        {
            var actions = Data.HotelsDummyData.GenerateActions();

            return actions;
        }

        private static void RunHotelQueries(ISearchIndexClient indexClient)
        {
            SearchParameters parameters;
            DocumentSearchResult<Hotel> results;

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Search the entire index for the term 'budget' and return only the hotelName field:\n");
            Console.ResetColor();

            parameters =
                new SearchParameters()
                {
                    Select = new[] { "hotelName" }
                };

            results = indexClient.Documents.Search<Hotel>("budget", parameters);

            WriteDocuments<Hotel>(results);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Apply a filter to the index to find hotels cheaper than $150 per night, ");
            Console.WriteLine("and return the hotelId and description:\n");
            Console.ResetColor();

            parameters =
                new SearchParameters()
                {
                    Filter = "baseRate lt 150",
                    Select = new[] { "hotelId", "description", "baseRate" }
                };

            results = indexClient.Documents.Search<Hotel>("*", parameters);

            WriteDocuments<Hotel>(results);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Search the entire index, order by a specific field (lastRenovationDate) ");
            Console.Write("in descending order, take the top two results, and show only hotelName and ");
            Console.WriteLine("lastRenovationDate:\n");
            Console.ResetColor();

            parameters =
                new SearchParameters()
                {
                    OrderBy = new[] { "lastRenovationDate desc" },
                    Select = new[] { "hotelName", "lastRenovationDate" },
                    Top = 2
                };

            results = indexClient.Documents.Search<Hotel>("*", parameters);

            WriteDocuments<Hotel>(results);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Search the entire index for the term 'motel':\n");
            Console.ResetColor();

            parameters = new SearchParameters();
            results = indexClient.Documents.Search<Hotel>("motel", parameters);

            WriteDocuments<Hotel>(results);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Search top 5 hotels (displays category, hotelName and baseRate):\n");
            Console.ResetColor();

            parameters =
               new SearchParameters()
               {
                   Select = new[] { "category", "hotelName", "baseRate" },
                   Top = 5
               };

            results = indexClient.Documents.Search<Hotel>("*", parameters);

            WriteDocuments<Hotel>(results);
        }

        #endregion hotels search index

        private static void WriteDocuments<T>(DocumentSearchResult<T> searchResults) where T : class
        {
            Console.ResetColor();
            foreach (SearchResult<T> result in searchResults.Results)
            {
                Console.WriteLine(result.Document);
            }
            Console.WriteLine();
        }
    }
}
