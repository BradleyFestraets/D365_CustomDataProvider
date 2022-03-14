using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceX_CustomDataProvider.Models
{
    /// <summary>
    /// Represents Rockets from the SpaceX API
    /// </summary>
    public class Rocket
    {
        public string rocket_name { get; set; }
    }

    /// <summary>
    /// Represents Links from the SpaceX API
    /// </summary>
    public class Links
    {
        public string mission_patch { get; set; }
        public string mission_patch_small { get; set; }
        public string reddit_campaign { get; set; }
        public string reddit_launch { get; set; }
        public string reddit_recovery { get; set; }
        public string reddit_media { get; set; }
        public string presskit { get; set; }
        public string article_link { get; set; }
        public string wikipedia { get; set; }
        public string video_link { get; set; }
        public string youtube_id { get; set; }
        public List<object> flickr_images { get; set; }
    }

    /// <summary>
    /// Represents Launches from the SpaceX API
    /// </summary>
    public class Launch
    {
        public Rocket rocket { get; set; }
        public int flight_number { get; set; }
        public string mission_name { get; set; }
        public string launch_year { get; set; }
        public DateTime launch_date_utc { get; set; }
        public Links links { get; set; }
        public string details { get; set; }

        public Entity ToEntity(ITracingService tracingService)
        {
            Entity entity = new Entity("bf_spacexrocketlaunch");

            // Transform int unique value to Guid
            var id = flight_number;
            var uniqueIdentifier = CDPHelper.IntToGuid(id);
            tracingService.Trace("Flight Number: {0} transformed into Guid: {1}", flight_number, uniqueIdentifier);

            // Map data to entity
            entity["bf_spacex_rocket_launchid"] = uniqueIdentifier;
            entity["bf_name"] = mission_name;
            entity["bf_flight_number"] = flight_number;
            entity["bf_rocket"] = rocket.rocket_name;
            entity["bf_launch_year"] = launch_year;
            entity["bf_launch_date"] = launch_date_utc;
            entity["bf_mission_patch"] = links.mission_patch;
            entity["bf_presskit"] = links.presskit;
            entity["bf_video_link"] = links.video_link;
            entity["bf_wikipedia"] = links.wikipedia;
            entity["bf_details"] = details;

            return entity;
        }
    }
}
