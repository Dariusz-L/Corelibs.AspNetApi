using Corelibs.Basic.Searching;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Corelibs.AspNetApi.Lucene
{
    public class LuceneInRamSearchEngine<T> : ISearchEngine<T>, IDisposable
    {
        private const LuceneVersion Version = LuceneVersion.LUCENE_48;

        private readonly Analyzer _analyzer;
        private readonly RAMDirectory _directory;
        private readonly IndexWriter _indexWriter;

        private static readonly object _lock = new();

        public LuceneInRamSearchEngine() 
        {
            _analyzer = new SimpleAnalyzer(Version);
            
            _directory = new RAMDirectory();

            var config = new IndexWriterConfig(Version, _analyzer);
            _indexWriter = new IndexWriter(_directory, config);
        }

        public void Dispose()
        {
            _indexWriter.Dispose();
            _directory.Dispose();
            _analyzer.Dispose();
        }

        public bool Add(SearchIndexData data)
        {
            var doc = new Document
            {
                new StringField("id", data.ID, Field.Store.YES),
                new TextField("name", data.Name, Field.Store.YES)
            };

            return LockAndTry(() =>
            {
                _indexWriter.AddDocument(doc);
                _indexWriter.Commit();
            });
        }

        public bool Add(IEnumerable<SearchIndexData> data)
        {
            return LockAndTry(() =>
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
            });
        }

        public bool Update(SearchIndexData data, string newName)
        {
            return LockAndTry(() =>
            {
                var term = new Term(data.ID, data.Name);
                var doc = new Document
                {
                    new StringField("id", data.ID, Field.Store.YES),
                    new TextField("name", newName, Field.Store.YES)
                };

                _indexWriter.UpdateDocument(term, doc);
                _indexWriter.Commit();
            });
        }

        public bool Delete(SearchIndexData data)
        {
            return LockAndTry(() =>
            {
                var term = new Term(data.ID, data.Name);

                _indexWriter.DeleteDocuments(term);
                _indexWriter.Commit();
            });
        }

        public SearchIndexData[] Search(string nameSubstring, SearchType searchType = SearchType.Substring)
        {
            var directoryReader = DirectoryReader.Open(_directory);
            var indexSearcher = new IndexSearcher(directoryReader);

            var queryParser = new QueryParser(Version, "name", _analyzer);
            queryParser.AllowLeadingWildcard = true;

            var query = queryParser.Parse($"*{nameSubstring}*");
            
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

        private bool LockAndTry(Action action)
        {
            Monitor.Enter(_lock);

            try
            {
                action();
            }
            catch (SynchronizationLockException)
            {
                return false;
            }
            finally
            {
                Monitor.Exit(_lock);
            }

            return true;
        }
    }
}
