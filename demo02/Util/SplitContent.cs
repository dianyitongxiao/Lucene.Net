using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
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

            while (tokenStream.HasAttribute<Token>())
            {
                list.Add(tokenStream.GetAttribute<Token>().Term);
            }

            return list.ToArray();            
        }

            
    }
}