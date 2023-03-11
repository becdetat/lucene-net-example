using Lucene.Net.Store;

namespace LuceneNetExample
{
    public interface ISearchFactory
	{
		FSDirectory GetDirectory();
	}

	/// <summary>
	/// This is implemented as a factory because the IndexPath might need to
	/// be adjusted in different environments (eg. in production it might be
	/// ./data/app-lucene-index).
	/// </summary>
    public class SearchFactory : ISearchFactory
	{
		private const string IndexPath = "./app-lucene-index";

		/// <summary>
		/// The FSDirectory this returns must be disposed
		/// </summary>
		/// <returns></returns>
		public FSDirectory GetDirectory()
		{
			return FSDirectory.Open(IndexPath);
		}
	}
}

