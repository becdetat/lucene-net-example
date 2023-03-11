using System;
namespace LuceneNetExample
{
	public interface IRepository<T>
	{
		T Get(int id);
	}

	public class PersonRepository : IRepository<Person>
	{
        public Person Get(int id)
        {
			return PeopleDatabase.People.Single(x => x.Id == id);
        }
    }
}

