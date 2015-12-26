using demo02.Models;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
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

        public ActionResult Demo1()
        {
            return View();
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

            writer.Dispose();
            directory.Dispose();

            return Content("ok");
        }

        private FSDirectory CreateFSDirectory()
        {
            string indexPath = HttpContext.Server.MapPath("~/IndexData");

            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());

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

            list.Add(new News { Id = Guid.NewGuid(), Title = "ASP.NET验证问题", Content = "验证什么的是灰常重要的.... ASP.NET" });
            list.Add(new News { Id = Guid.NewGuid(), Title = "Web API验证问题", Content = "验证什么的是灰常重要的....Web API" });
            list.Add(new News { Id = Guid.NewGuid(), Title = "ASP.NET登录", Content = "登录问题也是一个大问题" });

            return list;
        }

        private Document NewsToDocument(News news)
        {
            Document document = new Document();

            document.Add(new Field("id", news.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("title", news.Title.ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            document.Add(new Field("content", news.Content.ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));

            return document;
        }
    }
}