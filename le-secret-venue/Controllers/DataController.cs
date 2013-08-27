using System;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

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
            //var request = new RestRequest("venues/explore?near=90401&query=cocktails&client_id=HPRVYZP1WZ2KRHKAHZRIGAW51LPWW3JDO3SF5NPMIXS2LFFG&client_secret=Y2RCE2LUI1FUJKO2UD01PUNYOJMXWQQZMFTAZRYKXAR0AGFV&v=20130822");

            var response = client.Execute(request);
            var content = response.Content;

            return Content(content, "application/json");

            //content = "{ success: \"true\" }";


            //return Json(System.Web.Helpers.Json.Encode(content), JsonRequestBehavior.AllowGet);
            //return Json(content, JsonRequestBehavior.AllowGet);
            //return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Yelp()
        {
            var options = new Options()
            {
                AccessToken = "G5refmGzk0CQr4xGj3NDC7qGjRkodsjH",
                AccessTokenSecret = "nP5kn-njs6BBw3za0glL5M6U7-g",
                ConsumerKey = "SE-cxDms1CO3sxPh6wFRJg",
                ConsumerSecret = "r0s61wjnA9470NpBXZTnCLu1Xz8"
            };

            Yelp y = new Yelp(options);
            var result = y.Search("cocktails", "90401");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
