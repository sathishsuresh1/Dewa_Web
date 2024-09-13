using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Helpers.Models.YoutubeAPI
{
    public class VideoDurationResponse
    {
        public List<Item> items { get; set; }
    }
    public class ContentDetails
    {
        public string duration { get; set; }
        public string FriendlyDuration
        {
            get
            {
                if (string.IsNullOrEmpty(this.duration) || !this.duration.Contains("PT")) return string.Empty;
                short hrs = 0, mins = 0, secs = 0;
                try
                {
                    string d1 = this.duration.Remove(0, 2);

                    if (this.duration.Contains("H"))
                    {
                        string[] t1 = d1.Split(new char[] { 'H' });
                        short.TryParse(t1[0], out hrs);
                        if (this.duration.Contains("M"))
                        {
                            string[] t2 = t1[1].Split(new char[] { 'M' });
                            short.TryParse(t2[0], out mins);
                            if (this.duration.Contains("S"))
                            {
                                string[] t3 = t2[0].Split(new char[] { 'S' });
                                short.TryParse(t3[0], out secs);
                            }
                        }
                        else if (this.duration.Contains("S"))
                        {
                            string[] t4 = t1[1].Split(new char[] { 'S' });
                            short.TryParse(t4[0], out secs);
                        }
                    }
                    else if (this.duration.Contains("M"))
                    {
                        string[] t1 = d1.Split(new char[] { 'M' });
                        short.TryParse(t1[0], out mins);
                        if (this.duration.Contains("S"))
                        {
                            string[] t4 = t1[1].Split(new char[] { 'S' });
                            short.TryParse(t4[0], out secs);
                        }
                    }
                    else if (this.duration.Contains("S"))
                    {
                        string[] t1 = d1.Split(new char[] { 'S' });
                        short.TryParse(t1[0], out secs);
                    }
                    //mins = (short) ((hrs * 60) + mins);

                    if (secs > 0) mins++;

                    return string.Format("{0}{1}", hrs > 0 ? hrs.ToString() + ": " : "", mins < 10 ? "0" + mins.ToString() : mins.ToString());
                }
                catch (Exception ex)
                {
                    Foundation.Logger.LogService.Error(ex, this);
                }

                return this.duration;
            }
        }
    }

    public class Item
    {
        public ContentDetails contentDetails { get; set; }
    }
    
}