using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FitnessCenter.Models;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

namespace FitnessCenter.Controllers
{
    public class SupplementsController : Controller
    {
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();

        private readonly Dictionary<string, string> categories = new Dictionary<string, string>
        {
            {"Колаген","https://hardcoreshop.mk/kategorija/kolagen-i-negovi-efekti/"},
            {"Mass гејнери","https://hardcoreshop.mk/kategorija/mass-gejneri/" },
            {"Протеини","https://hardcoreshop.mk/kategorija/proteini/" },
            {"Согорувачи на масти","https://hardcoreshop.mk/kategorija/sogoruvachi-na-masti/" },
            {"Витамини","https://hardcoreshop.mk/kategorija/vitamini-kompleksi/vitamini/" },
        };

        public ActionResult Browse()
        {
            ViewBag.Categories = categories.Keys.ToList();
            return View();
        }

        public ActionResult Index()
        {
            var supplements = sqlDb.Supplements.ToList();
            return View(supplements);
        }

        public ActionResult BrowseCategory(string categoryName, int page = 1)
        {
            if (!categories.ContainsKey(categoryName))
                return HttpNotFound();

            var baseUrl = categories[categoryName];

            HtmlWeb web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";

            var url = $"{baseUrl}page/{page}/?per_page=6";
            var doc = web.Load(url);
            System.Threading.Thread.Sleep(1500);

            var mainProductsContainer = doc.DocumentNode
                .SelectSingleNode("//div[contains(@class,'elementor-element-ba880b3')]");

            var productNodes = mainProductsContainer
                .SelectNodes(".//div[contains(@class,'wd-hover-standard')]");

            var resultCountNode = doc.DocumentNode
                .SelectSingleNode("//p[contains(@class,'woocommerce-result-count')]");

            int totalProducts = 0;

            if (resultCountNode != null)
            {
                var text = resultCountNode.InnerText.Split(' ');

                if (text.Length >= 4 && int.TryParse(text[3], out int parsedTotal))
                    totalProducts = parsedTotal;
            }

            if (productNodes == null || productNodes.Count == 0)
                return Content("Нема производи на оваа страна.");

            var supplements = new List<Supplement>();

            foreach (var node in productNodes)
            {
                var nameNode = node.SelectSingleNode(".//h3[contains(@class,'wd-entities-title')]/a");

                var reducedPriceNode = node.SelectSingleNode(".//span[contains(@class,'price')]/ins/span[contains(@class,'amount')]/bdi");

                var regularPriceNode = node.SelectSingleNode(".//span[contains(@class,'price')]/span[contains(@class,'amount')]/bdi");

                var imageNode = node.SelectSingleNode(".//a[contains(@class,'product-image-link')]/img");

                string scrapedPrice = reducedPriceNode != null
                    ? reducedPriceNode.ChildNodes
                        .Where(n => n.NodeType == HtmlNodeType.Text)
                        .FirstOrDefault()?.InnerText.Trim() ?? ""
                    : regularPriceNode?.ChildNodes
                        .Where(n => n.NodeType == HtmlNodeType.Text)
                        .FirstOrDefault()?.InnerText.Trim() ?? "";

                var scrapedName = nameNode?.InnerText.Trim() ?? "";
                var scrapedLink = nameNode?.GetAttributeValue("href", "");

                string scrapedImage = "";

                if (imageNode != null)
                {
                    var dataSrc = imageNode.GetAttributeValue("data-src", "");

                    scrapedImage = !string.IsNullOrEmpty(dataSrc)
                        ? dataSrc
                        : imageNode.GetAttributeValue("src", "");
                }

                bool availability = false;

                if (!string.IsNullOrEmpty(scrapedLink))
                {
                    try
                    {
                        var productDoc = web.Load(scrapedLink);

                        var stockNode = productDoc.DocumentNode
                            .SelectSingleNode("//p[contains(@class,'stock')]");

                        if (stockNode != null)
                        {
                            var classAttr = stockNode.GetAttributeValue("class", "").ToLower();

                            if (classAttr.Contains("in-stock"))
                            {
                                availability = true;
                            }
                            else if (classAttr.Contains("out-of-stock"))
                            {
                                availability = false;
                            }
                            else
                            {
                                var text = stockNode.InnerText.ToLower();

                                availability =
                                    text.Contains("на залиха") ||
                                    text.Contains("in stock") ||
                                    text.Contains("available");
                            }
                        }
                        else
                        {
                            var form = productDoc.DocumentNode
                                .SelectSingleNode("//form[contains(@class,'variations_form')]");

                            var json = form?.GetAttributeValue("data-product_variations", "");

                            if (!string.IsNullOrEmpty(json))
                            {
                                json = HtmlEntity.DeEntitize(json);

                                if (json.TrimStart().StartsWith("["))
                                {
                                    try
                                    {
                                        var variations = JArray.Parse(json);

                                        availability = variations.Any(v =>
                                            v["is_in_stock"] != null &&
                                            v["is_in_stock"].Value<bool>());
                                    }
                                    catch
                                    {
                                        availability = false;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        availability = false;
                    }
                }

                string cleanedPrice = scrapedPrice
                    .Replace("ден", "")
                    .Replace(",", "")
                    .Replace("&nbsp;", "")
                    .Replace(" ", "")
                    .Trim();

                decimal.TryParse(cleanedPrice, out decimal parsedPrice);

                if (!string.IsNullOrEmpty(scrapedName))
                {
                    supplements.Add(new Supplement
                    {
                        Name = scrapedName,
                        Price = parsedPrice,
                        ImageUrl = scrapedImage,
                        ProductUrl = scrapedLink,
                        Availability = availability
                    });
                }
            }

            var productUrls = supplements.Select(s => s.ProductUrl).ToList();

            var existingInDb = sqlDb.Supplements
                .Where(s => productUrls.Contains(s.ProductUrl))
                .ToList();

            foreach (var scraped in supplements)
            {
                var existing = existingInDb
                    .FirstOrDefault(x => x.ProductUrl == scraped.ProductUrl);

                if (existing != null)
                {
                    existing.Name = scraped.Name;
                    existing.Price = scraped.Price;
                    existing.ImageUrl = scraped.ImageUrl;
                    existing.Availability = scraped.Availability;
                }
                else
                {
                    sqlDb.Supplements.Add(scraped);
                }
            }

            sqlDb.SaveChanges();

            var supplementsFromDb = sqlDb.Supplements
                .Where(s => productUrls.Contains(s.ProductUrl))
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.Category = categoryName;
            ViewBag.ProductCounter = totalProducts;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / 6);

            return View(supplementsFromDb);
        }

        private readonly Dictionary<string, List<string>> recomendationsByGoal =
            new Dictionary<string, List<string>>
        {
            {"WeightLoss", new List<string> {"Согорувачи на масти","Витамини"} },
            {"MuscleGain", new List<string> {"Протеини","Mass гејнери"} },
            {"Endurance", new List<string> {"Колаген","Витамини"} }
        };

        public ActionResult RecommendedForUser()
        {
            var userId = User.Identity.GetUserId();
            var user = sqlDb.Users.Find(userId);

            if (!recomendationsByGoal.TryGetValue(user.Goal.ToString(), out var recommendedCategories))
                recommendedCategories = new List<string>();

            var list = recommendedCategories
                .Where(c => categories.ContainsKey(c))
                .ToList();

            return PartialView("_RecommendationsPopup", list);
        }
    }
}