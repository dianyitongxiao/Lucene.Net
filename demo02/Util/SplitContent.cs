using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Analysis.Tokenattributes;
using PanGu;
using PanGu.HighLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace demo02.Util
{
    public class SplitContent
    {
        public static string[] SplitWords(string content)
        {
            List<string> list = new List<string>();

            Analyzer analyzer = new PanGuAnalyzer();
            TokenStream tokenStream = analyzer.TokenStream("", new StringReader(content));            

            while (tokenStream.IncrementToken())
            {
                ITermAttribute termAttribute = tokenStream.GetAttribute<ITermAttribute>();
                list.Add(termAttribute.Term);
            }

            return list.ToArray();            
        }

        public static string HightLight(string keyword, string content)
        {
            if (string.IsNullOrEmpty(keyword) || !content.ToLower().Contains(keyword.ToLower())) return content;

            SimpleHTMLFormatter simpleHTMLFormatter = 
                new SimpleHTMLFormatter("<font style=\"font-style:normal;color:#cc0000;\"><b>", "</b></font>");

            Highlighter highlighter = new Highlighter(simpleHTMLFormatter, new Segment());
            highlighter.FragmentSize = 1000; //设置摘要的字段数

            return highlighter.GetBestFragment(keyword, content);
        }

    }
}