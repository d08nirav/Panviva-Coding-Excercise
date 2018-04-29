using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using Newtonsoft.Json;
using Panviva_Coding_Excersise.Models;

namespace Panviva_Coding_Excersise.Controllers
{
    public class HomeController : Controller
    {
        // Variables to store user data from github. variable names are same as property values.
        Int64 id = 10,public_repos,public_gists,followers,following;
        string avatar_url, html_url, followers_url, following_url, gists_url, started_url, subscriptions_url, repos_url, name, company, blog, location, email, hireable, bio;

        // dictoionary to store details
        Dictionary<string, string> AllRepos = new Dictionary<string, string>();
        Dictionary<string, string> AllGists = new Dictionary<string, string>();
        ArrayList<string> Data = new ArrayList<string>();
        ArrayList<string> AllFollowings = new ArrayList<string>();
        
        public async Task<IActionResult> Index(string input)
        {
            //Varibles to store data
            //to keep showing the input (username) in the search box
            ViewData["Input"] = input;
            //Check if the input is empty.
            if (String.IsNullOrEmpty(input))
            {
                //ViewData["UserName"] = "empty";
                return View();
            }
            ViewData["UserName"] = "Username not found";
            //When the input is not empty
            // Check if there is a username for give input. if not return null
            String usernameURL = await CheckIfUserExistAsync(input);
            ViewData["DEBUG"] = usernameURL;
            if (!usernameURL.Equals("Found"))
            {
                
                return View();
            }   
            ViewData["UserName"] = input;
            //set corresponding data to viewdata
            await BuildViewDataAsync();
            return View();
        }

        //Builds view data. done in a sepearte method and trough C# variables first for ease of modifacation
        private async Task BuildViewDataAsync()
        {
            
            ViewData["id"] = id;
            ViewData["public_repos"] = public_repos;
            ViewData["public_gists"] = public_gists;
            ViewData["followers"] = followers;
            ViewData["following"] = following;
            //string //followers_url, following_url, gists_url, started_url, subscriptions_url, repos_url, 
            ViewData["avatar_url"] = avatar_url;
            ViewData["html_url"] = html_url; 
            ViewData["name"] = name;
            if (company != null)
                ViewData["company"] = company;
            else
                ViewData["company"] = "N/A";
            if (blog != null)
            ViewData["blog"] = blog;
            else
                ViewData["blog"] = "N/A";
            if (location !=null)
            ViewData["location"] = location;
            else
                ViewData["location"] = "N/A";
            if (email !=null)
            ViewData["email"] = email;
            else
                ViewData["email"] = "N/A";
            if (hireable != null)
                ViewData["hireable"] = hireable;
            else
                ViewData["hireable"] = "N/A";
            if (bio != null)
            ViewData["bio"] = bio;
            else
                ViewData["bio"] = "N/A";
            await GetSubDetailsAsync(followers_url);
            ViewBag.AllFollowers = Data;
            await GetSubDetailsAsync(following_url);
            ViewBag.AllFollowings = Data;
            await GetReposAsync();
            ViewBag.AllRepos = AllRepos;
            await GetGistsAsync();
            ViewBag.AllGists = AllGists;
        }

        private async Task GetGistsAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    AllGists = new Dictionary<string, string>();
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, gists_url);
                    // Add our custom headers https://api.github.com/users/nirav/followers
                    requestMessage.Headers.Add("User-Agent", "User-Agent-Here");
                    requestMessage.Headers.Add("Authorization", "token e74f57caee7c8c22ff71d49590f63ee66b8921ad");
                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();
                    var Result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    var stringResult = Result.ToString();
                    JsonTextReader textReader = new JsonTextReader(new StringReader(stringResult));
                    while (textReader.Read())
                    {
                        if (textReader.Value != null)
                        {
                            if (textReader.Value.Equals("git_push_url"))
                            {
                                ReadData(textReader, 3);
                                string namedescription ="", url="";
                               url = textReader.Value.ToString();
                                while (textReader.Read())
                                {
                                    if (textReader.Value != null)
                                    {
                                        if (textReader.Value.Equals("filename"))
                                        {
                                            ReadData(textReader, 1);
                                            if (textReader.Value != null)
                                                namedescription = textReader.Value.ToString();
                                            while (textReader.Read())
                                            {
                                                if (textReader.Value != null)
                                                {
                                                    if (textReader.Value.Equals("description"))
                                                    {
                                                        ReadData(textReader, 1);
                                                        if (textReader.Value != null)
                                                            namedescription = namedescription+"&&&"+ textReader.Value.ToString();
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                                /*if (String.IsNullOrEmpty(name))
                                    continue;*/
                                AllGists.Add(url, namedescription);
                            }
                        }

                    }
                    return;
                }
                catch (HttpRequestException httpRequestException)
                {
                    return;// "Can't reach GitHub. check your internet connection.";
                }
            }
        }

        private async Task GetReposAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    AllRepos = new Dictionary<string, string>();
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, repos_url);
                    // Add our custom headers https://api.github.com/users/nirav/followers
                    requestMessage.Headers.Add("User-Agent", "User-Agent-Here");
                    requestMessage.Headers.Add("Authorization", "token e74f57caee7c8c22ff71d49590f63ee66b8921ad");
                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();
                    var Result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    var stringResult = Result.ToString();
                    JsonTextReader textReader = new JsonTextReader(new StringReader(stringResult));
                    while (textReader.Read())
                    {
                        if (textReader.Value != null)
                        {
                            if (textReader.Value.Equals("id"))
                            {
                                ReadData(textReader, 3);
                                string name,description="";
                                name = textReader.Value.ToString();
                                while (textReader.Read())
                                {
                                    if (textReader.Value != null)
                                    {
                                        if (textReader.Value.Equals("description"))
                                        {
                                            ReadData(textReader, 1);
                                            if (textReader.Value!=null)
                                                description = textReader.Value.ToString();
                                            break;
                                        }
                                    }
                                }                                                          
                                /*if (String.IsNullOrEmpty(name))
                                    continue;*/
                                AllRepos.Add(name,description);
                            }
                        }

                    }
                    return;
                }
                catch (HttpRequestException httpRequestException)
                {
                    return;// "Can't reach GitHub. check your internet connection.";
                }
            }
        }

        private async Task GetSubDetailsAsync(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    Data = new ArrayList<string>();
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    // Add our custom headers https://api.github.com/users/nirav/followers
                    requestMessage.Headers.Add("User-Agent", "User-Agent-Here");
                    requestMessage.Headers.Add("Authorization", "token e74f57caee7c8c22ff71d49590f63ee66b8921ad");
                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();
                    var Result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    var stringResult = Result.ToString();
                    JsonTextReader textReader = new JsonTextReader(new StringReader(stringResult));
                    while (textReader.Read()) {                        
                        if (textReader.Value != null)
                        {
                            if (textReader.Value.Equals("login"))
                            {                                
                                ReadData(textReader, 1);
                                string name;
                                name = textReader.Value.ToString();                                
                                while (textReader.Read())
                                {
                                    if (textReader.Value != null)
                                    {
                                        if (textReader.Value.Equals("type"))
                                        {
                                            ReadData(textReader, 1);
                                            if (!textReader.Value.Equals("User"))
                                                name = null;
                                            break;
                                        }
                                    }
                                }
                                if (String.IsNullOrEmpty(name))
                                    continue;
                                Data.Add(name);                               
                            }
                        }
                    
                    }
                    return;
                }
                catch (HttpRequestException httpRequestException)
                {
                    return;// "Can't reach GitHub. check your internet connection.";//BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}" + i);
                }
            }
        }
        
        //fills up program variables based on the data found on GitHub for a perticular if username found. else returns the error msg;
        private async Task<string> CheckIfUserExistAsync(string input)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/users/" + input);
                    // Add our custom headers https://api.github.com/users/nirav
                    requestMessage.Headers.Add("User-Agent", "User-Agent-Here");
                    requestMessage.Headers.Add("Authorization", "token e74f57caee7c8c22ff71d49590f63ee66b8921ad");
                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();
                    var Result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    var stringResult = Result.ToString();
                    JsonTextReader textReader = new JsonTextReader(new StringReader(stringResult));
                    textReader.Read();
                    textReader.Read();
                    if (textReader.Value.Equals("login"))
                    {                        
                        textReader.Read();
                        if (!input.Equals(textReader.Value.ToString()))
                            return "Username not found";
                        ReadData(textReader, 2);
                        id = Int64.Parse(textReader.Value.ToString());
                        ReadData(textReader, 2);
                        avatar_url = textReader.Value.ToString();
                        ReadData(textReader, 6);
                        html_url = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        followers_url = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        following_url = textReader.Value.ToString().Remove(textReader.Value.ToString().Length-13,13);
                        ReadData(textReader, 2);
                        gists_url = textReader.Value.ToString().Remove(textReader.Value.ToString().Length - 10, 10);
                        ReadData(textReader, 2);
                        started_url = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        subscriptions_url = textReader.Value.ToString();
                        ReadData(textReader, 4);
                        repos_url = textReader.Value.ToString();
                        ReadData(textReader, 10);
                        if (textReader.Value != null)
                            name = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        if (textReader.Value != null)
                            company = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        if (textReader.Value != null)
                            blog = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        if (textReader.Value != null)
                            location = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        if (textReader.Value != null)
                            email = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        if (textReader.Value != null)
                            hireable = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        if (textReader.Value != null)
                            bio = textReader.Value.ToString();
                        ReadData(textReader, 2);
                        public_repos = Int64.Parse(textReader.Value.ToString());
                        ReadData(textReader, 2);
                        public_gists = Int64.Parse(textReader.Value.ToString());
                        ReadData(textReader, 2);
                        followers = Int64.Parse(textReader.Value.ToString());
                        ReadData(textReader, 2);
                        following = Int64.Parse(textReader.Value.ToString());
                        return "Found";
                    }
                    return "Username not found.";
                }
                catch (HttpRequestException httpRequestException)
                {
                    return "Can't reach GitHub or more than 60 reqst/hr limit crossed. Check your internet connection or try again letter";// +httpRequestException.ToString();
                }
            }
        }

        //Reads the unnessary data out
        private void ReadData(JsonTextReader textReader, int v)
        {
            for (; v > 0; v--)
                textReader.Read();
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
