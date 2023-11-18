using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Website.Utils;

namespace Website.Controllers
{
    public class SitemapController : BaseController
    {
        public readonly ProductManager _productManager;
        private readonly WebsiteModuleManager _websiteModuleManager;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ProductManager _productDa;
        public SitemapController()
        {
            _productManager = new ProductManager(WebConfig.ConnectionString);
            _websiteModuleManager = new WebsiteModuleManager(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _productDa = new ProductManager(WebConfig.ConnectionString);
        }
        public ActionResult Rss() => View(_websiteModuleDa.GetCategory());
        public async Task<IActionResult> RssFeed(string NameAscii)
        {
            try
            {
                if (!string.IsNullOrEmpty(NameAscii))
                {
                    WebsiteModulesItem module = await _websiteModuleManager.GetByNameAscii(NameAscii);
                    if (module != null)
                    {
                        SearchModel search = new()
                        {
                            lang = Lang()
                        };
                        IEnumerable<WebsiteModulesItem> child = await _websiteModuleManager.GetListChidrentAsync(module.ID);
                        string url = WebConfig.Website;
                        SyndicationFeed feed = new(module.SEOTitle, null, new Uri(url), null, DateTime.Now)
                        {
                            Description = new TextSyndicationContent(module.SeoDescription)
                        };
                        List<SyndicationItem> items = new();
                        List<WebsiteProductItemJson> products = _productManager.GetListProductRss(search, module.ID, string.Join(",", child.Select(x => x.ID)));
                        if (products != null)
                        {
                            foreach (WebsiteProductItemJson item in products)
                            {
                                SyndicationItem it = new(item.Name, item.SeoDescription, new Uri(url + "/" + item.NameAscii))
                                {
                                    Id = item.ID.ToString(),
                                    PublishDate = DateTime.Parse(item.CreatedDate.ToString(), null)
                                };
                                it.Authors.Add(new SyndicationPerson(WebConfig.WebsiteGenerator, WebConfig.WebsiteGenerator, url));
                                XmlDocument doc = new();
                                XmlElement content = doc.CreateElement("content", "encoded", url);
                                content.AppendChild(doc.CreateCDataSection(item.SeoDescription));
                                it.ElementExtensions.Add(content);
                                items.Add(it);
                            }
                        }
                        feed.Items = items;
                        XmlWriterSettings settings = new()
                        {
                            Encoding = Encoding.UTF8,
                            NewLineHandling = NewLineHandling.Entitize,
                            NewLineOnAttributes = true,
                            Indent = true
                        };
                        using (MemoryStream stream = new())
                        {
                            using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
                            {
                                Rss20FeedFormatter rssFormatter = new(feed, false);
                                rssFormatter.WriteTo(xmlWriter);
                                xmlWriter.Flush();
                            }
                            //return this.Content(content.ToString(), "text/xml", Encoding.UTF8);
                            //return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
                            string content = Utility.BytesToString(stream.ToArray());
                            return this.Content(content.ToString(), "text/xml", Encoding.UTF8);
                        }

                        //using (var rssWriter = XmlWriter.Create("wwwroot/rss/" + NameAscii + "_rss.xml"))
                        //{
                        //    new Rss20FeedFormatter(feed).WriteTo(rssWriter);
                        //}
                        //using (var sr = new StreamReader("wwwroot/rss/" + NameAscii + "_rss.xml"))
                        //{
                        //    xml = sr.ReadToEnd();
                        //}
                    }
                }
                Response.StatusCode = 404;
                return View(@"~/Views/Error/Error404.cshtml");

            }
            catch (Exception e)
            {
                Common.AddLogError(e);
                Response.StatusCode = 404;
                return View(@"~/Views/Error/Error404.cshtml");
            }
        }
        public IActionResult Index(string code)
        {
            string xml = string.Empty;
            try
            {
                List<SitemapJson> sitemaps = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                switch (code)
                {
                    case "sitemap":
                        {
                            IReadOnlyCollection<SitemapNode> sitemap = GetSitemapNodes(sitemaps.Where(x => x.ParentID != 0).ToList());
                            xml = GetSitemapNodeDocument(sitemap);
                            break;
                        }
                    // default:
                    //     {
                    //         SitemapJson sitemapItem = sitemaps.FirstOrDefault(x => x.Code == code);
                    //         if (sitemapItem != null)
                    //         {
                    //             IReadOnlyCollection<SitemapNode> sitemap = GetSitemapNodes(sitemaps.Where(x => x.ParentID == sitemapItem.ID).ToList());
                    //             xml = GetSitemapNodeDocument(sitemap);
                    //         }
                    //         break;
                    //     }
                }
            }
            catch (Exception e)
            {
                Common.AddLogError(e);
            }
            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public IActionResult Feed(int? page)
        {
            if (!page.HasValue)
            {
                int Total = _productManager.CountAllProduct();
                FeedItem feed = new()
                {
                    Total_page = Total <= 100 ? 1 : Total % 100 > 0 ? (Total / 100 + 1) : (Total / 100)
                };
                return Ok(feed);
            }
            StringBuilder xml = new();
            try
            {
                int p = page ?? 1;
                List<ProductItem> products = _productManager.GetProductXml(p, 100);
                XElement root = new("Products");
                foreach (ProductItem item in products)
                {
                    XElement urlElement = new("Product");
                    urlElement.Add(new XElement("simple_sku", "<![CDATA[ " + item.ProductCode + " ]]>"));
                    urlElement.Add(new XElement("category_1", "<![CDATA[ " + item.ModuleName + " ]]>"));
                    urlElement.Add(new XElement("product_name", "<![CDATA[ " + item.Name_ + " ]]>"));
                    urlElement.Add(new XElement("description", "<![CDATA[ ]]>"));
                    urlElement.Add(new XElement("promotion", "<![CDATA[ " + Utility.RemoveHTMLTag(Utility.ReplaceSpecialText(item.PromotionText)) + " ]]>"));
                    urlElement.Add(new XElement("parent_sku", "<![CDATA[ ]]>"));
                    urlElement.Add(new XElement("availability_instock", "<![CDATA[ " + (item.Status == 1 ? "true" : "false") + " ]]>"));
                    string url_picture = string.Empty;
                    if (!string.IsNullOrEmpty(item.UrlPicture) && (item.UrlPicture.Contains("http://") || item.UrlPicture.Contains("https://")))
                    {
                        url_picture = item.UrlPicture;
                    }
                    else if (!string.IsNullOrEmpty(item.UrlPicture))
                    {
                        url_picture = WebConfig.Website + item.UrlPicture;
                    }
                    urlElement.Add(new XElement("picture_url", @"<![CDATA[ " + url_picture + " ]]>"));
                    urlElement.Add(new XElement("price", @"<![CDATA[ " + item.Price + " ]]>"));
                    urlElement.Add(new XElement("currency", @"<![CDATA[ vnd ]]>"));
                    urlElement.Add(new XElement("vat_status", @"<![CDATA[ 1 ]]>"));
                    urlElement.Add(new XElement("URL", @"<![CDATA[ " + (WebConfig.Website + "/" + item._NameAscii) + " ]]>"));
                    root.Add(urlElement);
                }
                XDocument document = new(new XDeclaration("1.0", Encoding.UTF8.ToString(), null), root);
                //using (TextWriter writer = new StringWriter(xml))
                //{
                //    document.Save(writer);
                //}
                return this.Content(document.ToString().Replace("&lt;", "<").Replace("&gt;", ">"), "text/xml", Encoding.UTF8);
            }
            catch (Exception e)
            {
                AddLogError(e.Message);
            }
            return new ContentResult { Content = xml.ToString(), ContentType = "text/xml", StatusCode = (int)HttpStatusCode.OK, };
        }
        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
        public IReadOnlyCollection<SitemapNode> GetSitemapNodes(List<SitemapJson> SitemapJsons)
        {
            List<SitemapNode> nodes = new();
            foreach (SitemapJson item in SitemapJsons)
            {
                nodes.Add(new SitemapNode() { Url = item.Url, Priority = item.Priority, Frequency = item.ChangeFrequency, LastModified = item.LastModified });
            }
            return nodes;
        }
        public string GetSitemapNodeDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new(xmlns + "urlset");
            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-dd")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.ToLowerInvariant()), sitemapNode.Priority == null ? null : new XElement(xmlns + "priority", (sitemapNode.Priority / 100)));
                root.Add(urlElement);
            }
            // XDocument document = new(new XProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"google-sitemap.xsl\""), root)
            // {
            //     Declaration = new XDeclaration("1.0", "utf-8", "true")
            // };
           XDocument document = new(new XDeclaration("1.0", "utf-8", "yes"), root);
            return document.ToString();
        }
        public string GetSubSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace schemaLocation = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd");
            XElement root = new(xmlns + "sitemapindex", new XAttribute(XNamespace.Xmlns + "xsi", xsi), new XAttribute(xsi + "schemaLocation", schemaLocation));
            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                //XElement urlElement = new(
                //xmlns + "sitemap",
                //new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)), sitemapNode.LastModified == null ? null : new XElement(xmlns + "lastmod", sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-dd")));
                //root.Add(urlElement);
            }
            //XDocument document = new(new XDeclaration("1.0", "utf-8", "yes"), new XProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"google-sitemap.xsl\""), root);
            XDocument document = new(new XDeclaration("1.0", "utf-8", "yes"), "", root);
            return document.ToString();
        }
        private class FeedItem
        {
            public string Feed_url { get; set; } = WebConfig.Website + "/feed";
            public int Total_page { get; set; }
            public string Page_param { get; set; } = "page";
        }
    }
}
