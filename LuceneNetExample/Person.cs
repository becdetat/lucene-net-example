namespace LuceneNetExample
{
    public record Person
	{
		[IdentifierField]
		public required int Id;
		[IndexField]
		public required string FirstName;
		[IndexField]
		public required string LastName;
		public int Age;
	}
}

