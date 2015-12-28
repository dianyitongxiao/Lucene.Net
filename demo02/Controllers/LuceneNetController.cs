using demo02.Models;
using demo02.Util;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demo02.Controllers
{
    public class LuceneNetController : Controller
    {

        // GET: LuceneNet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Demo1(string searchKey = "")
        {
            ViewBag.SearchKey = searchKey;
            List<object> news = new List<object>();
            if (!string.IsNullOrEmpty(searchKey))
                news = Search(searchKey);
            return View(news);
        }

        public ActionResult CreateIndex()
        {
            var directory = CreateFSDirectory();
            bool isExist = IndexReader.IndexExists(directory);

            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);            
        
            GetNews().ForEach(m =>
            {
                writer.AddDocument(NewsToDocument(m));
            });

            Document doc = new Document();
            doc.Add(new Field("type", "video", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("title", "ASP.NET登录", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            writer.AddDocument(doc);

            writer.Dispose();
            directory.Dispose();

            return Content("ok");
        }

        private FSDirectory CreateFSDirectory()
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(GetIndexDataPath()), new NativeFSLockFactory());

            bool isExist = IndexReader.IndexExists(directory);
            if (isExist)
            {
                if (IndexWriter.IsLocked(directory))
                    IndexWriter.Unlock(directory);
            }

            return directory;
        }

        private List<News> GetNews()
        {
            List<News> list = new List<News>();

            list.Add(new News { Id = new Guid("1C4507C7-BB2E-4A8D-ABFF-0E5D2B274DEB"), Title = "ASP.NET验证问题", Content = "验证什么验证的是灰常重要的验证问题.... " });
            list.Add(new News { Id = new Guid("85BB5EA3-D63B-43C4-BC79-909112658417"), Title = "Web API验证问题", Content = "验证什么的是灰常重要的...." });
            list.Add(new News { Id = new Guid("36B53DE4-C23A-4DFE-9094-C09D2EBDFCDB"), Title = "ASP.NET登录", Content = "问题也是一个大问题" });

            return list;
        }

        private Document NewsToDocument(News news)
        {
            Document document = new Document();

            document.Add(new Field("id", news.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("type", "news", Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("title", news.Title.ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            document.Add(new Field("content", news.Content.ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));

            return document;
        }

        private News DocumentToNews(string keyword, Document doc)
        {
            if (doc == null) return null;
            News news = new News();

            news.Id = new Guid(doc.Get("id"));
            news.Title = doc.Get("title");
            news.Title = SplitContent.HightLight(keyword, news.Title);
            news.Content = doc.Get("content");
            news.Content = SplitContent.HightLight(keyword, news.Content);

            return news;
        }

        private Video DocumentToVideo(string keyword, Document doc)
        {
            if (doc == null) return null;

            Video video = new Video();
            video.Title = doc.Get("title");
            video.Title = SplitContent.HightLight(keyword, video.Title);
        
            return video;
        }

        private List<object> Search(string searchKey)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(GetIndexDataPath()), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);

            //PhraseQuery query = new PhraseQuery();
            //PhraseQuery query1 = new PhraseQuery();
            //foreach (var item in Util.SplitContent.SplitWords(searchKey))
            //{
            //    query.Add(new Term("content", item));
            //    query1.Add(new Term("title", item));
            //}
            //query.Slop = 100;

            // MultiPhraseQuery queryResult = new MultiPhraseQuery();
            //// queryResult.Combine(new Query[] { query, query1 });

            // foreach (var item in Util.SplitContent.SplitWords(searchKey))
            // {
            //     queryResult.Add(new Term("content", item));
            //     queryResult.Add(new Term("title", item));
            // }



            string[] fields = new string[] { "title", "content" };
            //Occur[] occurs = new Occur[] { Occur.SHOULD, Occur.SHOULD };
            //Query query = MultiFieldQueryParser.Parse(Lucene.Net.Util.Version.LUCENE_30, searchKey, fields, occurs, new PanGuAnalyzer());

            MultiFieldQueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, fields, new PanGuAnalyzer());
            var query = parser.Parse(searchKey);

            TopScoreDocCollector collector = TopScoreDocCollector.Create(1000, true);
            searcher.Search(query, null, collector);
            ScoreDoc[] docs = collector.TopDocs(0, collector.TotalHits).ScoreDocs; //达到分页效果

            List<object> list = new List<object>();

            foreach (var item in docs)
            {
                int docId = item.Doc;
                Document doc = searcher.Doc(docId);
                if (doc.Get("type") == "news")
                    list.Add(DocumentToNews(searchKey, doc));
                else
                    list.Add(DocumentToVideo(searchKey, doc));
            }

            return list;
        }

        private string GetIndexDataPath()
        {
            return HttpContext.Server.MapPath("~/IndexData");
        }
    }
}