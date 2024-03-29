﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace le_secret_venue
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "INSTAGRAM FOURSQUARE LOOKUP",
                url: "instagram/foursquarelookup/{foursquare_v2_id}/",
                defaults: new
                {
                    controller = "Instagram",
                    action = "FoursquareLookup",
                    foursquare_v2_id = UrlParameter.Optional,
                }
            );

            routes.MapRoute(
                name: "INSTAGRAM VENUE",
                url: "instagram/venue/{venue_id}/",
                defaults: new
                {
                    controller = "Instagram",
                    action = "Venue",
                    venue_id = UrlParameter.Optional,
                }
            );

            routes.MapRoute(
                name: "DATA API EXPLORE",
                url: "data/explore/{near}/",
                defaults: new
                {
                    controller = "Data",
                    action = "Explore",
                    near = UrlParameter.Optional,
                }
            );

            routes.MapRoute(
                name: "DATA API FOURSQUARE",
                url: "data/foursquare/{near}/",
                defaults: new
                {
                    controller = "Data",
                    action = "Foursquare",
                    near = UrlParameter.Optional,
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}