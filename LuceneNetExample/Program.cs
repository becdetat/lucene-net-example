using LuceneNetExample;

Console.WriteLine("Hi! This is an example of Lucene.net based search.");

ISearchFactory searchFactory = new SearchFactory();
IRepository<Person> repository = new PersonRepository();
ISearchService<Person> searchService = new SearchService<Person>(searchFactory, repository);

Console.WriteLine("Clearing index");
searchService.ClearIndex();

Console.WriteLine("Indexing data");
var counter = 0;
foreach (var person in PeopleDatabase.People)
{
    searchService.Index(person);
    counter++;
}
Console.WriteLine($"Finished indexing {counter} people");

Console.WriteLine("Starting search");
DoSearch("patrick");
DoSearch("rebecca");
DoSearch("smith");
DoSearch("kelvin");
Console.WriteLine("Complete");
Console.ReadKey();

void DoSearch(string phrase)
{
    Console.WriteLine($"Searching for \"{phrase}\"");

    var results = searchService.Search(phrase).ToArray();

    if (results.Length == 0)
    {
        Console.WriteLine("No results");
    }

    foreach (var person in results)
    {
        Dump(person);
    }
}

static void Dump(Person person)
{
    Console.WriteLine($"Id: {person.Id}, Name: {person.FirstName} {person.LastName}, Age: {person.Age}");
}