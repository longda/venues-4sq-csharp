using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using le_secret_venue.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using YelpSharp;


namespace le_secret_venue.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/


        // http://msdn.microsoft.com/en-us/library/system.web.helpers.json(v=vs.111).aspx
        // https://developer.foursquare.com/overview/auth.html
        // https://developer.foursquare.com/overview/versioning
        // https://api.foursquare.com/v2/venues/explore?near=90401&query=cocktails&client_id=HPRVYZP1WZ2KRHKAHZRIGAW51LPWW3JDO3SF5NPMIXS2LFFG&client_secret=Y2RCE2LUI1FUJKO2UD01PUNYOJMXWQQZMFTAZRYKXAR0AGFV&v=20130822
        // https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=34.0176,-118.4907&radius=5000&sensor=false&types=bar&key=AIzaSyBxw6-IwqZJu6NABNQ6DZk68nlMV6vArIg
        // yelp require creating dynamic hashes of all inputs so can't hit url directly (not really REST)


        public ActionResult Index()
        { 
            return View();
        }

        public ActionResult Explore(string near = "90401")
        {
            var sb = new StringBuilder();
            sb.Append("venues/explore?");

            // iterate through params here
            sb.AppendFormat("near={0}", near);
            sb.AppendFormat("&query={0}", "cocktails");
            sb.AppendFormat("&client_id={0}", "HPRVYZP1WZ2KRHKAHZRIGAW51LPWW3JDO3SF5NPMIXS2LFFG");
            sb.AppendFormat("&client_secret={0}", "Y2RCE2LUI1FUJKO2UD01PUNYOJMXWQQZMFTAZRYKXAR0AGFV");
            sb.AppendFormat("&v={0}", "20130822");

            var client = new RestClient("https://api.foursquare.com/v2/");
            var request = new RestRequest(sb.ToString());

            var response = client.Execute(request);
            var content = response.Content;


            var idx_phone = new Dictionary<string, string>();
            var output = new Dictionary<string, Dictionary<string, JObject>>();
            var data_foursquare = JObject.Parse(content);


            var fsqa_items =
                from i in data_foursquare["response"]["groups"][0]["items"]
                select i;

            foreach (var item in fsqa_items)
            {
                var name = (string)item["venue"]["name"];
                var phone = (string)item["venue"]["contact"]["phone"];
                if (!output.ContainsKey(name))
                    output[name] = new Dictionary<string, JObject>();

                output[name]["fsq"] = JObject.Parse(item["venue"].ToString());

                if (!string.IsNullOrEmpty(phone)) idx_phone[phone] = name;
            }


            //  Add in Yelp

            var data_yelp = JObject.Parse(GetYelpData(near));
            var y_items =
                from i in data_yelp["Result"]["businesses"]
                select i;

            foreach (var item in y_items)
            {
                var name = (string)item["name"];
                var phone = (string)item["phone"];
                if (output.ContainsKey(name))
                {
                    output[name]["yelp"] = JObject.Parse(item.ToString());
                }
                else if (idx_phone.ContainsKey(phone))
                {
                    output[idx_phone[phone]]["yelp"] = JObject.Parse(item.ToString());
                    output[idx_phone[phone]]["yelp-dice"] = JObject.Parse(JsonConvert.SerializeObject(new { dice = name.DiceCoefficient(idx_phone[phone]) }));
                }
                else
                {
                    // we've found something new perhaps?
                    output[name] = new Dictionary<string, JObject>();
                    output[name]["yelp"] = JObject.Parse(item.ToString()); 
                }
            }


            // Add in Google/Zagat

            var data_goog = JObject.Parse(GetGoogleData(near));
            var g_items =
                from i in data_goog["results"]
                select i;

            foreach (var item in g_items)
            {
                var name = (string)item["name"];
                var isNew = true;
                foreach (var key in output.Keys)
                {
                    var dice = name.DiceCoefficient(key);
                    var vicinity = output[key].ContainsKey("fsq") ? string.Format("{0}, {1}", output[key]["fsq"]["location"]["address"], output[key]["fsq"]["location"]["city"]) : "";
                    if (output.ContainsKey(name) || (!output[key].ContainsKey("goog") && dice > 0.6 && vicinity.DiceCoefficient((string)item["vicinity"]) > 0.6))
                    {
                        output[key]["goog"] = JObject.Parse(item.ToString());
                        output[key]["goog-dice"] = JObject.Parse(JsonConvert.SerializeObject(new { dice = dice }));
                        isNew = false;
                        break;
                    }
                }

                if (isNew)
                {
                    // we've found something new perhaps?
                    output[name] = new Dictionary<string, JObject>();
                    output[name]["goog"] = JObject.Parse(item.ToString());
                }
            }

            return Content(JsonConvert.SerializeObject(output), "application/json");
            //return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Foursquare(string near = "90401")
        {
            var sb = new StringBuilder();
            sb.Append("venues/explore?");

            // iterate through params here
            sb.AppendFormat("near={0}", near);
            sb.AppendFormat("&query={0}", "cocktails");
            sb.AppendFormat("&client_id={0}", "HPRVYZP1WZ2KRHKAHZRIGAW51LPWW3JDO3SF5NPMIXS2LFFG");
            sb.AppendFormat("&client_secret={0}", "Y2RCE2LUI1FUJKO2UD01PUNYOJMXWQQZMFTAZRYKXAR0AGFV");
            sb.AppendFormat("&v={0}", "20130822");

            var client = new RestClient("https://api.foursquare.com/v2/");
            var request = new RestRequest(sb.ToString());

            var response = client.Execute(request);
            var content = response.Content;

            return Content(content, "application/json");
        }

        public ActionResult Yelp(string near = "90401")
        {
            //var options = new Options()
            //{
            //    AccessToken = "G5refmGzk0CQr4xGj3NDC7qGjRkodsjH",
            //    AccessTokenSecret = "nP5kn-njs6BBw3za0glL5M6U7-g",
            //    ConsumerKey = "SE-cxDms1CO3sxPh6wFRJg",
            //    ConsumerSecret = "r0s61wjnA9470NpBXZTnCLu1Xz8"
            //};

            //Yelp y = new Yelp(options);
            //var result = y.Search("cocktails", near);
            var result = GetYelpData(near);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Google(string near = "90401")
        {
            //var sb = new StringBuilder();
            //sb.Append("place/nearbysearch/json?");

            //sb.AppendFormat("location={0}", "34.0176,-118.4907");
            //sb.AppendFormat("&radius={0}", "5000");
            //sb.AppendFormat("&sensor={0}", "false");
            //sb.AppendFormat("&types={0}", "bar");
            //sb.AppendFormat("&key={0}", "AIzaSyBxw6-IwqZJu6NABNQ6DZk68nlMV6vArIg");

            //var client = new RestClient("https://maps.googleapis.com/maps/api/");
            //var request = new RestRequest(sb.ToString());

            //var response = client.Execute(request);
            //var content = response.Content;

            var content = GetGoogleData(near);
            return Content(content, "application/json");
        }

        private static string GetYelpData(string near)
        {
            var options = new Options()
            {
                AccessToken = "G5refmGzk0CQr4xGj3NDC7qGjRkodsjH",
                AccessTokenSecret = "nP5kn-njs6BBw3za0glL5M6U7-g",
                ConsumerKey = "SE-cxDms1CO3sxPh6wFRJg",
                ConsumerSecret = "r0s61wjnA9470NpBXZTnCLu1Xz8"
            };

            Yelp y = new Yelp(options);
            var result = y.Search("cocktails", near);

            return JsonConvert.SerializeObject(result);
        }

        private static string GetGoogleData(string near)
        {
            var sb = new StringBuilder();
            sb.Append("place/nearbysearch/json?");

            sb.AppendFormat("location={0}", "34.0176,-118.4907");
            sb.AppendFormat("&radius={0}", "5000");
            sb.AppendFormat("&sensor={0}", "false");
            sb.AppendFormat("&types={0}", "bar");
            sb.AppendFormat("&key={0}", "AIzaSyBxw6-IwqZJu6NABNQ6DZk68nlMV6vArIg");

            var client = new RestClient("https://maps.googleapis.com/maps/api/");
            var request = new RestRequest(sb.ToString());

            var response = client.Execute(request);
            var content = response.Content;

            return content;
        }
    }
}
