namespace LuceneNetExample
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IdentifierFieldAttribute : Attribute
    {
        public static (string name, int id) GetIdentifier<T>(T obj)
        {
            var identifierFieldType = typeof(IdentifierFieldAttribute);
            var identifierField = typeof(T).GetFields()
                .Where(f => f.IsDefined(identifierFieldType, false))
                .Select(f => (f.Name, (int?)f.GetValue(obj) ?? int.MinValue))
                .Single();

            return identifierField;
        }
    }
}

