using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessCenter.Models;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;

namespace FitnessCenter.Controllers
{
    public class SupplementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

        // GET: Supplements
        public ActionResult Index()
        {
            var supplements = db.Supplements.ToList();
            return View(supplements);
        }
        public ActionResult BrowseCategory(string categoryName, int page = 1)
        {
            if(!categories.ContainsKey(categoryName))
                return HttpNotFound();

            var baseUrl = categories[categoryName];
            HtmlWeb web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";

            var url = $"{baseUrl}page/{page}/?per_page=6";
            var doc = web.Load(url);
            System.Threading.Thread.Sleep(1500);

            var mainProductsContainer = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'elementor-element-ba880b3')]");
            var productNodes = mainProductsContainer.SelectNodes(".//div[contains(@class,'wd-hover-standard')]");

            var resultCountNode = doc.DocumentNode.SelectSingleNode("//p[contains(@class,'woocommerce-result-count')]");
            int totalProducts = 0;
            if(resultCountNode!=null)
            {
                var text = resultCountNode.InnerText.Split(' ');
                if(text.Length>=4 && int.TryParse(text[3], out int parsedTotal))
                {
                    totalProducts = parsedTotal;
                }
            }

            if (productNodes == null || productNodes.Count == 0)
            {
                return Content("Нема производи на оваа страна.");
            }

            var supplements = new List<Supplement>();
            foreach(var node in productNodes)
            {
                var nameNode = node.SelectSingleNode(".//h3[contains(@class,'wd-entities-title')]/a");
                var reducedPriceNode = node.SelectSingleNode(".//span[contains(@class,'price')]/ins/span[contains(@class,'amount')]/bdi");
                var regularPriceNode = node.SelectSingleNode(".//span[contains(@class,'price')]/span[contains(@class,'amount')]/bdi");
                var imageNode = node.SelectSingleNode(".//a[contains(@class,'product-image-link')]/img");
                var availabilityNode = node.SelectSingleNode(".//p[contains(@class,'wd-product-stock')]");

                string scrapedPrice = "";
                //Извлекување на цената така што се пребаруваат јазлите кои содржат чист текст и се враќа првиот таков
                if (reducedPriceNode != null)
                {
                    scrapedPrice = reducedPriceNode
                    .ChildNodes
                    .Where(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                    .FirstOrDefault()?.InnerText.Trim() ?? "";
                }
                else
                {
                    scrapedPrice = regularPriceNode?
                        .ChildNodes
                        .Where(n=>n.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                        .FirstOrDefault()?.InnerText.Trim() ?? "";
                }                

                var scrapedName = nameNode?.InnerText.Trim() ?? "";
                var scrapedLink = nameNode?.GetAttributeValue("href", "");
                var scrapedImage = imageNode?.GetAttributeValue("data-src", "") ?? imageNode.GetAttributeValue("src", "");
                string availabilityText = availabilityNode?.InnerText.Trim() ?? "Нема податок";

                string nbsp = "&nbsp;";
                string cleanedPrice = scrapedPrice
                    .Replace("ден", "")
                    .Replace(",", "")
                    .Replace(nbsp, "")
                    .Replace(" ", "")
                    .Trim();

                decimal.TryParse(cleanedPrice, out decimal parsedPrice);
                bool availability = availabilityText.Equals("На залиха");

                if (!string.IsNullOrEmpty(scrapedName))
                {
                    var supplement = new Supplement
                    {
                        Name = scrapedName,
                        Price = parsedPrice,
                        ImageUrl = scrapedImage,
                        ProductUrl = scrapedLink,
                        Availability = availability,
                    };
                    supplements.Add(supplement);
                }
            }
            var newOnes = supplements
                .Where(s=>!db.Supplements.Any(x=>x.ProductUrl == s.ProductUrl))
                .ToList();

            if (newOnes.Any())
            {
                db.Supplements.AddRange(newOnes);
                db.SaveChanges();
            }

            //земање на суплементите според url-то во базата
            var productUrls = supplements.Select(s => s.ProductUrl).ToList();
            var supplementsFromDb = db.Supplements
                .Where(s=>productUrls.Contains(s.ProductUrl))
                .ToList();
            
            ViewBag.CurrentPage = page;
            ViewBag.Category = categoryName;
            ViewBag.ProductCounter = totalProducts;
            //Наоѓање на бројот на страници, според тоа што на една страна се прикажуваат по 6 производи
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts/6);

            return View(supplementsFromDb);
        }
        public enum Goal
        {
            WeightLoss,
            MuscleGain,
            Endurance
        }
        private readonly Dictionary<string, List<string>> recomendationsByGoal = new Dictionary<string, List<string>>
        {
            {"WeightLoss",new List<string> {"Согорувачи на масти","Витамини"} },
            {"MuscleGain",new List<string> {"Протеини","Mass гејнери"} },
            {"Endurance",new List<string> {"Колаген","Витамини"} }
        };
        public ActionResult RecommendedForUser()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var userGoal = user.Goal;

            List<string> recommendedCategories;
            if(!recomendationsByGoal.TryGetValue(userGoal.ToString(), out recommendedCategories))
            {
                recommendedCategories = new List<string>();
            }

            var recommendedCategoriesList = recommendedCategories
                .Where(cat=>categories.ContainsKey(cat))
                .ToList();

            return PartialView("_RecommendationsPopup", recommendedCategoriesList);
        }
    }
}