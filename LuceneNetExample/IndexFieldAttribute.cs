namespace LuceneNetExample
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IndexFieldAttribute : Attribute
	{
        public static IEnumerable<(string name, string value)> GetIndexFields<T>(T obj)
        {
            var indexFieldType = typeof(IndexFieldAttribute);
            var indexFields = typeof(T).GetFields()
                .Where(f => f.IsDefined(indexFieldType, false))
                .Select(f => (f.Name, f.GetValue(obj) as string ?? string.Empty));

            return indexFields;
        }

        public static IEnumerable<string> GetIndexFieldNames<T>()
        {
            var indexFieldType = typeof(IndexFieldAttribute);
            var indexFieldNames = typeof(T).GetFields()
                .Where(x => x.IsDefined(indexFieldType, false))
                .Select(x => x.Name);

            return indexFieldNames;
        }
    }
}

