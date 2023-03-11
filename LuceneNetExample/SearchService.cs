using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LuceneNetExample
{
    public interface ISearchService<T>
    {
        IEnumerable<T> Search(string phrase);
        void Index(T t);
        void Remove(T t);
        void ClearIndex();
    }

	public class SearchService<T> : ISearchService<T>
	{
        private readonly ISearchFactory _searchFactory;
        private readonly IRepository<T> _repository;
        private const int SearchHitLimit = 1000;

        public SearchService(ISearchFactory searchFactory, IRepository<T> repository)
		{
            this._searchFactory = searchFactory;
            this._repository = repository;
        }

        public IEnumerable<T> Search(string phrase)
        {
            var fieldNames = IndexFieldAttribute.GetIndexFieldNames<T>()
                .ToArray();

            // Fuzzy queries
            var fuzzyQueries = fieldNames
                .Select(fieldName => new Term(fieldName, phrase))
                .Select(term => new FuzzyQuery(term));

            // Wildcard queries
            var wildcardKeywords = phrase.Split(' ')
                .Select(token => $"*{token}*");
            var wildcardQueries =
                from keyword in wildcardKeywords
                from fieldName in fieldNames
                select new Term(fieldName, keyword)
                into term
                select new WildcardQuery(term);

            // Combine into boolean query
            var booleanQuery = new BooleanQuery();
            foreach (var query in fuzzyQueries)
            {
                booleanQuery.Add(query, Occur.SHOULD);
            }
            foreach (var query in wildcardQueries)
            {
                booleanQuery.Add(query, Occur.SHOULD);
            }

            // Do the actual search and get a list of IDs
            using var directory = _searchFactory.GetDirectory();
            using var reader = directory.GetReader();
            var searcher = new IndexSearcher(reader);
            var hits = searcher.Search(booleanQuery, SearchHitLimit);
            var docIds = hits.ScoreDocs.Select(x => x.Doc);
            var docs = docIds.Select(docId => searcher.Doc(docId));
            var ids = docs
                .Select(doc => int.Parse(doc.Get("Id")))
                .ToArray();
            var results = ids.Select(id => _repository.Get(id))
                .ToArray();

            return results;
        }

        public void Index(T t)
        {
            var (idName, idValue) = IdentifierFieldAttribute.GetIdentifier(t);
            var document = new Document
            {
                new Int64Field(idName, idValue, Field.Store.YES)
            };
            var indexFields =
                from fieldName in IndexFieldAttribute.GetIndexFieldNames<T>()
                let fieldInfo = typeof(T).GetField(fieldName)
                where fieldInfo is not null
                let fieldValue = (fieldInfo.GetValue(t) ?? string.Empty).ToString()
                select new StringField(fieldName, fieldValue, Field.Store.YES);

            foreach (var field in indexFields)
            {
                document.Add(field);
            }

            using var directory = _searchFactory.GetDirectory();
            using var writer = directory.GetWriter();

            writer.AddDocument(document);
            writer.Flush(true, true);                
        }

        public void Remove(T t)
        {
            using var directory = _searchFactory.GetDirectory();
            using var writer = directory.GetWriter();

            var (idName, idValue) = IdentifierFieldAttribute.GetIdentifier(t);
            var term = new Term(idName, idValue.ToString());

            writer.DeleteDocuments(term);
            writer.Commit();            
        }

        /// <summary>
        /// Note that this clears the ENTIRE index, not just the documents of
        /// type T. This should be used prior to reindexing the entire directory.
        /// </summary>
        public void ClearIndex()
        {
            using var directory = _searchFactory.GetDirectory();
            using var writer = directory.GetWriter();

            writer.DeleteAll();
            writer.Commit();
        }
    }
}

