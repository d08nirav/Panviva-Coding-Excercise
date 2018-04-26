using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Panviva_Coding_Excersise.Models;

namespace Panviva_Coding_Excersise.Controllers
{
    public class HomeController : Controller
    {
        private static int i = 0;
        public string Welcome(string name, int numTimes = 1)
        {
            ViewData["UserName"] = "search required for :" +name;
            return HtmlEncoder.Default.Encode($"Hello {name}, NumTimes is: {numTimes}");
        }

        public async Task<IActionResult> Index(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                ViewData["UserName"] = "empty";
                return View();
            }
            using (var client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/search/users?q=" + username);
                    // Add our custom headers
                    requestMessage.Headers.Add("User-Agent", "User-Agent-Here");
                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    //client.BaseAddress = new Uri("https://api.github.com/search/users?q=d08nirav");
                    //var response = await client.GetAsync($"/search/users?q={username}&appid=YOUR_API_KEY_HERE&units=metric");
                    //var response = await client.GetAsync("https://api.github.com/search/users?q=d08nirav");//.GetAsync($"/search/users?q=d08nirav");
                    response.EnsureSuccessStatusCode();
                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeather = JsonConvert.DeserializeObject(stringResult);
                    ViewData["UserName"] = rawWeather;
                    return View();
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}" + i);
                }
            }
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
