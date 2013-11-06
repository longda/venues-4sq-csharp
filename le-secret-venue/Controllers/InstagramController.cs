using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace le_secret_venue.Controllers
{
    public class InstagramController : Controller
    {
        // foursquare to instagram location id lookup:
        // https://api.instagram.com/v1/locations/search?foursquare_v2_id=509a18d8e4b08bf010b68112&client_id=d00aa6e4a8744e72ad267ea8d09b905f
        // 
        // instagram location lookup:
        // https://api.instagram.com/v1/locations/50347341/media/recent?client_id=d00aa6e4a8744e72ad267ea8d09b905f


        //
        // GET: /Instagram/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Venue(string venue_id)
        {
            this.ViewBag.venue_id = venue_id;
            //this.ViewBag.venue_id = "50347341"; // no vacancy to start
            return View();
        }


        // test: http://localhost:52382/instagram/foursquarelookup/509a18d8e4b08bf010b68112

        public ActionResult FoursquareLookup(string foursquare_v2_id)
        {
            var sb = new StringBuilder();
            sb.Append("locations/search?");

            sb.AppendFormat("foursquare_v2_id={0}", foursquare_v2_id);
            sb.AppendFormat("&client_id={0}", "d00aa6e4a8744e72ad267ea8d09b905f");

            var client = new RestClient("https://api.instagram.com/v1/");
            var request = new RestRequest(sb.ToString());

            var response = client.Execute(request);
            var content = response.Content;

            var data = JObject.Parse(content);
            var instagram_id = data["data"][0]["id"].ToString();

            //return Content(content, "application/json");

            //return Json(new { id=instagram_id } , JsonRequestBehavior.AllowGet); 

            return new RedirectResult(string.Format("/instagram/venue/{0}/", instagram_id));
        }
    }
}
