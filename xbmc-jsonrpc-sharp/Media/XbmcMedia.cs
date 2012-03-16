using System;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc
{
    public class XbmcMedia
    {
        #region Private variables

        protected static string[] fields = new string[] 
            {"title", "artist", "albumartist", "genre", "year", "rating", "album", "track", "duration", "comment", "lyrics", "musicbrainztrackid", "musicbrainzartistid", 
             "musicbrainzalbumid", "musicbrainzalbumartistid", "playcount", "fanart", "director", "trailer", "tagline", "plot", "plotoutline", "originaltitle", "lastplayed", 
             "writer", "studio", "mpaa", 
             //"cast", 
             "country", "imdbnumber", "premiered", "productioncode", "runtime", "set", "showlink", "streamdetails", "top250", 
             "votes", "firstaired", "season", "episode", "showtitle", "thumbnail", "file", 
             //"resume", 
             "artistid", "albumid", "tvshowid", "setid"};
            
            
            //{"title", "genre", "year", "rating", "director", "file",
            //                        "trailer", "tagline", "plot", "plotoutline", "originaltitle", 
            //                        "lastplayed", "duration", "playcount", "writer", "studio", 
            //                        "mpaa" };

        private int id;

        private string thumbnail;
        private string fanart;

        #endregion

        #region Internal variables

        internal static string[] Fields
        {
            get { return (fields != null ? fields : new string[0]); }
        }

        #endregion

        #region Public variables

        public virtual int Id
        {
            get { return this.id; }
        }

        public virtual string Thumbnail
        {
            get { return this.thumbnail; }
        }

        public virtual string Fanart
        {
            get { return this.fanart; }
        }

        #endregion

        #region Constructors

        protected XbmcMedia(int id, string thumbnail, string fanart)
        {
            this.id = id;
            this.thumbnail = thumbnail != null ? thumbnail : string.Empty;
            this.fanart = fanart != null ? fanart : string.Empty;
        }

        #endregion
    }
}
