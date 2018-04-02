using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
//using System.Text.RegularExpressions;

namespace BizComponent
{
    public class SimpleSEOComponent
    {
        /// <summary>
        /// Analyze text or content from url
        /// </summary>
        /// <param name="textORurl">Url or Sentences</param>
        /// <param name="filterstopwords">Filter Stop-Words in analysis</param>
        /// <param name="analyzeword">Include word analysis</param>
        /// <param name="analyzemeta">Include meta analysis</param>
        /// <param name="includelinks">Show external links</param>
        /// <returns>Object ReturnResult</returns>
        public ReturnResult AnalyzeFromURL(string textORurl, bool filterstopwords, bool analyzeword, bool analyzemeta, bool includelinks)
        {
            ReturnResult result = new ReturnResult();
            result.FilterStopWords = filterstopwords;
            result.ShowWordOccurence = analyzeword;
            result.ShowMetaOccurence = analyzemeta;
            result.ShowLinks = includelinks;
            try
            {
                //validate url
                bool isURL = Uri.IsWellFormedUriString(textORurl, UriKind.RelativeOrAbsolute);
                //set result to true by default
                result.Result = true;
                
                if (!isURL) //if not url / is text
                {
                    //if not url, check for words occurences only                    
                    result.WordsOccurences = AnalyzeString(ExcludeString(textORurl), filterstopwords);

                    result.ShowMetaOccurence = false;
                    result.ShowLinks = false;
                    return result;
                }
                else //url is valid
                {
                    HtmlWeb hw = new HtmlWeb();
                    HtmlDocument doc = hw.Load(textORurl);

                    //links
                    if (includelinks)
                        result.Links = GetExternalLinks(doc.DocumentNode);

                    //meta
                    if (analyzemeta)
                        result.MetaOccurences = GetMetaOccurences(doc.DocumentNode, filterstopwords);

                    //words occurences
                    if (analyzeword)
                        result.WordsOccurences = GetWordOccurences(doc.DocumentNode, filterstopwords);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                return result;
            }
        }

        public ReturnResult GetResultFromString(HtmlDocument doc, bool filterstopwords)
        {
            ReturnResult result = new ReturnResult();
            try
            {
                //set result to true by default
                result.Result = true;

                //get links
                result.Links = GetExternalLinks(doc.DocumentNode);

                //meta
                result.MetaOccurences = GetMetaOccurences(doc.DocumentNode, filterstopwords);

                //words occurences
                result.WordsOccurences = GetWordOccurences(doc.DocumentNode, filterstopwords);

                return result;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.InnerException.ToString();
                return result;
            }
        }


        public List<WordOccurence> GetWordOccurences(HtmlNode node, bool filterstopwords)
        {
            List<WordOccurence> ret = new List<WordOccurence>();

            //remove script / style / image
            node.Descendants()
            .Where(n => n.Name.ToLower() == "script" || n.Name.ToLower() == "style" || n.Name.ToLower() == "img")
            .ToList()
            .ForEach(n => n.Remove());

            //select text inside body tag
            string htmls = ExcludeString(node.SelectSingleNode("//body").InnerText);

            List<string> words = new List<string>();
            if (!string.IsNullOrEmpty(htmls))
                return AnalyzeString(System.Net.WebUtility.HtmlDecode(htmls), filterstopwords);

            return ret;
        }

        public List<WordOccurence> GetMetaOccurences(HtmlNode node, bool filterstopwords)
        {
            List<WordOccurence> ret = new List<WordOccurence>();
            List<string> metas = new List<string>();
            string strmeta = GetMetaDataFromUrl(node);
            if (!string.IsNullOrEmpty(strmeta))
                return AnalyzeString(ExcludeString(strmeta), filterstopwords);

            return ret;
        }

        public List<WordOccurence> AnalyzeString(string strs, bool filterstopwords)
        {
            List<WordOccurence> ret = new List<WordOccurence>();
            List<string> lst = new List<string>();

            foreach (var item in strs.Trim().Split(' '))
                if (!string.IsNullOrEmpty(item) && !string.IsNullOrWhiteSpace(item) && item.Trim().Length > 1)
                {
                    if (filterstopwords)
                    {
                        if (!Common.GetStopWords().Contains(item.ToLower()))
                            lst.Add(item.Trim());
                    }
                    else
                        lst.Add(item.Trim());
                }

            //group by each words and get the count
            var mg = lst.GroupBy(p => p).Select(grp => new { Word = grp.Key, Count = grp.Count() }).OrderBy(o => o.Word).ToList();

            //insert group into result object
            foreach (var item in mg)
            {
                if (item.Word != "")
                {
                    ret.Add(new WordOccurence() { Word = item.Word, NoOfOccurences = item.Count });
                }
            }

            return ret;
        }

        public List<Link> GetExternalLinks(HtmlNode node)
        {
            List<Link> lnk = new List<Link>();
            foreach (var link in node.SelectNodes(@"//a[@href]"))
            {
                var att = link.Attributes["href"];
                if (att == null) continue;
                var href = att.Value;
                //skip javascript or markers
                if (href.StartsWith("javascript", StringComparison.InvariantCultureIgnoreCase) || href.StartsWith("#", StringComparison.InvariantCultureIgnoreCase)) continue;

                var urlNext = new Uri(href, UriKind.RelativeOrAbsolute);

                // Make it absolute if it's relative
                if (urlNext.IsAbsoluteUri)
                {
                    // Absolute so it's external
                    lnk.Add(new Link() { HREF = href, Text = link.InnerText });
                }
            }

            return lnk;
        }

        string GetMetaDataFromUrl(HtmlNode node)
        {
            var metaTags = node.SelectNodes("//meta");

            string metaInfo = "";

            if (metaTags != null)
            {
                int matchCount = 0;
                foreach (var tag in metaTags)
                {
                    var tagName = tag.Attributes["name"];
                    var tagContent = tag.Attributes["content"];
                    var tagProperty = tag.Attributes["property"];

                    if (tagName != null && tagContent != null)
                    {
                        switch (tagName.Value.ToLower())
                        {
                            case "title":
                            case "description":
                            case "twitter:title":
                            case "twitter:description":
                            case "keywords":
                            case "twitter:image":
                                metaInfo += tagContent.Value;
                                matchCount++;
                                break;
                        }
                    }
                    else if (tagProperty != null && tagContent != null)
                    {
                        switch (tagProperty.Value.ToLower())
                        {
                            case "og:title":
                            case "og:description":
                            case "og:image":
                                metaInfo += tagContent.Value;
                                matchCount++;
                                break;
                        }
                    }
                }
            }
            return metaInfo;
        }

        /// <summary>
        /// Exclude unnecessary characters for analysis
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public string ExcludeString(string strings)
        {
            return strings.Replace(".", "")
                .Replace(",", "")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }
    }
}
