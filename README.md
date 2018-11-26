# AzureSearch
Use AzureSearch service with .Net SDK

## In Azure Portal

Login to azure portal and find **“Azure Search”** item from the marketplace.

![azure search in marketplace](https://user-images.githubusercontent.com/8690373/49047071-0c103a00-f222-11e8-9f05-9728157c3d4c.png)

Create the search service, navigate to it and locate the menu option **“Keys”**. Two keys are necessary to connect and perform queries.
The first key needed is the **“Primary admin key”**. This is used to create a search index instance in the .Net application.

![admin key](https://user-images.githubusercontent.com/8690373/49047135-34983400-f222-11e8-9f04-fe81ab70cb09.png)

The second key needed is the **“Query key”**. This allow to retrieve data from the search index service.

![query key](https://user-images.githubusercontent.com/8690373/49047166-4d084e80-f222-11e8-9eee-11351a419768.png)

## In .Net application

In the .Net application the following NuGet packages are required:
*	Microsoft.Azure.Search
*	Microsoft.Azure.Search.Common
*	Microsoft.Azure.Search.Data
*	Microsoft.Azure.Search.Service
*	Microsoft.Rest.ClientRuntime
*	Microsoft.Rest.ClientRuntime.Azure

It is necessary to create a model type for the search index. The model used in this example is called *"People"* and contains the index fields and the attributes that allow to query documents.

```c#
public class People
{
    [System.ComponentModel.DataAnnotations.Key]
    [IsFilterable]
    [JsonProperty("id")]
    public string Id { get; set; }

    [IsSearchable]
    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [IsSearchable, IsFilterable, IsSortable]
    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [IsSearchable]
    public string Email { get; set; }

    [IsSearchable, IsFilterable, IsSortable, IsFacetable]
    public string Gender { get; set; }

    [IsSearchable]
    [JsonProperty("ip_address")]
    public string IpAddress { get; set; }
}
```

JsonProperty attributes are used to map a sample Json file. This property names will be also used as the document field names when the search index is created.
Note that there are other attributes such as **IsSearchable, IsFilterable, IsSortable, IsFacetable** used in the property fields. Those denote how the fields are going to be used for search results. 
For example, **“IsSearchable”** authorises the field to be full-text searchable. More information about these attributes can be found in
[Setting attibutes](https://docs.microsoft.com/en-us/azure/search/search-create-index-portal#design-guidance-for-setting-attributes)

Now that the initial set up is ready in the application, it is time to create and perform queries on the index.

### Create search index

The first step consists in creating a search service client instance
``` c#
SearchServiceClient serviceClient =
  new SearchServiceClient(<your_search_service_name>, 
  new SearchCredentials(<your_search_admin_apikey>)
);
```
As a safety measure it is always useful to check whether or not the index has being already created. For this example the search index is called *"people"*
``` c#
if (serviceClient.Indexes.Exists("people"))
{
  serviceClient.Indexes.Delete("people");
}

var definition = new Index()
{
  Name = indexName,
  Fields = FieldBuilder.BuildForType<People>()
};

serviceClient.Indexes.Create(definition);
```

Next step is to get an instance of the index previously created. 
``` c#
ISearchIndexClient indexClient = serviceClient.Indexes.GetClient(“people”);
```
It is also required to generate the document objects to populate the index. These are created as a collection of **IndexAction** items, and then added to an **IndexBatch**.

``` c#
List<IndexAction<People>> actions = new List<IndexAction<People>>();

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
        }),
        
        ...
        
);

var batch = IndexBatch.New(actions);
```

The action used in this example is **"MergeOrUpload"** which updates the document if this exist already or creates a new one if does not.  More information about Index actions can be found in
[Index actions](https://docs.microsoft.com/en-us/azure/search/search-import-data-dotnet)

Finally when the index batch is ready, it is time to populate the search index.
``` c#
indexClient.Documents.Index(batch);
```

### Run Queries on index

A search index client instance is required to run queries. Using the index name *"people"* the following line shows how to create the index client instance:
``` c#
ISearchIndexClient indexClientForQueries = AzureSearchService.CreateSearchIndexClient(“people”);
```
It is also necessary to define and instance of **SearchParameters** to perform some queries. These might contain options such as **Filter, OrderBy, Select, Top** and more. However, these are optional. The following example shows how to query documents (people items) applying a filter where gender = Female, then ordering the results by lastName and finally getting the first 10 results. Results will only contain First name, Last name and email fields.
``` c#
SearchParameters  parameters = new SearchParameters()
    {
        Filter = "gender eq 'Female'",
        OrderBy = new[] { "last_name asc" },
        Select = new[] { "first_name", "last_name", "email" }
        Top = 10
    };

DocumentSearchResult<People>  searchResults = indexClient.Documents.Search<People>("*", parameters);

foreach (SearchResult<People> result in searchResults.Results)
{
    var singleDocument = result.Document;
    ...
}
```

Another way to query documents can be done without parameters but only the search text. 
``` c#
DocumentSearchResult<People>  searchResults = indexClient.Documents.Search<People>(" phil* ", new SearchParameters());
```
Previous query will perform a full-text search in the index returning all documents that contain the word *"phil"*. Same query run in the Azure portal will look like this:

![query results](https://user-images.githubusercontent.com/8690373/49047196-5f828800-f222-11e8-8290-22feecd80332.png)

You will need an active Azure subscription to run the console application. After source code is downloaded, locate **App.config** and replace the following keys with the ones you generated

``` xml
  <appSettings>
      <add key="SearchServiceName" value="your_index_name"/>
      <add key="SearchServiceAdminApiKey" value="your_search_index_admin_key"/>
      <add key="SearchServiceQueryApiKey" value="your_search_index_query_key"/>
  </appSettings>
```

Using the Azure Search SDK for .Net allow us to perform operations with Lucene indexes proficiently.


