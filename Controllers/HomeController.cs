using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NasaImageApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text;

namespace NasaImageApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string NASAKEY = "biT9Yzsl7bh839ytJDg69fxgusFl19J2ZW7JnmMr";
        private const string HEADER = "application/json";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(bool trigger)
        {
            string PATH = System.IO.Directory.GetCurrentDirectory() + "\\Images";
            HttpClient client = new HttpClient();
            string earthDate = "2015-6-3";
            string MarsUrl = "https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/";
            client.BaseAddress = new Uri(MarsUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(HEADER));

            HttpResponseMessage response = client.GetAsync("photos?earth_date=" + earthDate + "&api_key=" + NASAKEY).Result;
            //if (response.IsSuccessStatusCode)
            //{
                var result = await response.Content.ReadAsStringAsync();
                ImagesModel ImageJsonList = JsonConvert.DeserializeObject<ImagesModel>(result);
                
                if(ImageJsonList != null && ImageJsonList.photos.Count > 0)
                {
                    foreach(Photos photo in ImageJsonList.photos)
                    {
                    
                    using (WebClient photoClient = new WebClient())
                    {
                        photoClient.DownloadFile(new Uri(photo.img_src), PATH + "//" + photo.img_src.Split("/").Last());
                    }
                }
                }
                ViewBag.Message = String.Format("Images downloaded");
                return View();
            //}
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
