using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.Analysis.Standard;

namespace LuceneNetExample
{
    public static class FSDirectoryExtensions
	{
        private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

		/// <summary>
		/// The IndexWriter this returns must be disposed
		/// </summary>
		/// <param name="directory"></param>
		/// <returns></returns>
		public static IndexWriter GetWriter(this FSDirectory directory)
		{
			var analyzer = new StandardAnalyzer(AppLuceneVersion);
			var config = new IndexWriterConfig(AppLuceneVersion, analyzer);
			var writer = new IndexWriter(directory, config);

			return writer;
		}

		/// <summary>
		/// The IndexReader this returns must be disposed
		/// </summary>
		/// <param name="directory"></param>
		/// <returns></returns>
		public static IndexReader GetReader(this FSDirectory directory)
		{
			return DirectoryReader.Open(directory);
		}
	}

}

