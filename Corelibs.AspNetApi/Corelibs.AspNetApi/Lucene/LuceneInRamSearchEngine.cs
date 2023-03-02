using Corelibs.Basic.Searching;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Corelibs.AspNetApi.Lucene
{
    public class LuceneInRamSearchEngine<T> : ISearchEngine<T>
    {
        private const LuceneVersion Version = LuceneVersion.LUCENE_48;

        private readonly StandardAnalyzer _analazer;
        private readonly RAMDirectory _directory;
        private readonly IndexWriter _indexWriter;

        public LuceneInRamSearchEngine() 
        {
            _analazer = new StandardAnalyzer(Version);
            _directory = new RAMDirectory();

            var config = new IndexWriterConfig(Version, _analazer);
            _indexWriter = new IndexWriter(_directory, config);
        }

        public void Add(SearchIndexData data)
        {
            var doc = new Document
            {
                new StringField("id", data.ID, Field.Store.YES),
                new TextField("name", data.Name, Field.Store.YES)
            };

            _indexWriter.AddDocument(doc);
            _indexWriter.Commit();
        }

        public void Add(IEnumerable<SearchIndexData> data)
        {
            foreach (var i in data)
            {
                var doc = new Document
                {
                    new StringField("id", i.ID, Field.Store.YES),
                    new TextField("name", i.Name, Field.Store.YES)
                };
                _indexWriter.AddDocument(doc);
            }

            _indexWriter.Commit();
        }

        public SearchIndexData[] Search(string searchTerm, SearchType searchType = SearchType.Full)
        {
            var directoryReader = DirectoryReader.Open(_directory);
            var indexSearcher = new IndexSearcher(directoryReader);

            string[] fields = { "name" };
            var queryParser = new MultiFieldQueryParser(Version, fields, _analazer);
            var query = queryParser.Parse(searchTerm);

            var searchResult = indexSearcher.Search(query, 1000);

            var result = new List<SearchIndexData>(searchResult.ScoreDocs.Length);

            foreach (var hit in searchResult.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);

                var id = doc.Get("id");
                var name = doc.Get("name");

                result.Add(new SearchIndexData(id, name));
            }

            return result.ToArray();
        }
    }
}
