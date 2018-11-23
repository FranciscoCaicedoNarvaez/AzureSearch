using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace AzureSearch.Services
{
    public static class AzureSearchService
    {
        private static readonly string _searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
        private static readonly string _searchAdminApiKey = ConfigurationManager.AppSettings["SearchServiceAdminApiKey"];
        private static readonly string _searchQueryApiKey = ConfigurationManager.AppSettings["SearchServiceQueryApiKey"];

        public static SearchServiceClient CreateSearchServiceClient()
        {
            SearchServiceClient serviceClient = new SearchServiceClient(_searchServiceName, new SearchCredentials(_searchAdminApiKey));
            return serviceClient;
        }

        public static SearchIndexClient CreateSearchIndexClient(string indexName)
        {
            SearchIndexClient indexClient = new SearchIndexClient(_searchServiceName, indexName, new SearchCredentials(_searchQueryApiKey));
            return indexClient;
        }

        public static void DeleteIndexIfExists(SearchServiceClient serviceClient, string indexName)
        {
            if (serviceClient.Indexes.Exists(indexName))
            {
                serviceClient.Indexes.Delete(indexName);
            }
        }

        public static void CreateIndex<T>(SearchServiceClient serviceClient, string indexName)
        {
            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<T>()
            };

            serviceClient.Indexes.Create(definition);
        }

        public static void UploadDocuments<T>(ISearchIndexClient indexClient, IEnumerable<IndexAction<T>> actions) where T : class
        {
            var batch = IndexBatch.New(actions);

            try
            {
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Waiting for documents to be indexed...\n");
            Console.ResetColor();
            Thread.Sleep(2000);
        }
    }
}