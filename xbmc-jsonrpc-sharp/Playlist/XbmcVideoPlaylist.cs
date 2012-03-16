using System;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc
{
    public class XbmcVideoPlaylist : XbmcMediaPlaylist<XbmcVideo>
    {
        #region Constructor

        internal XbmcVideoPlaylist(JsonRpcClient client)
            : base("Playlist", client, 1)
        { }

        #endregion

        #region JSON RPC Calls

        #endregion

        #region Overrides of XbmcMediaPlaylist<XbmcVideo>

        public override XbmcVideo GetCurrentItem()
        {
            return this.GetCurrentItem(null);
        }

        public override XbmcVideo GetCurrentItem(string[] fields)
        {
            XbmcVideo result;
            
            this.client.LogMessage("XbmcVideoPlaylist.GetCurrentItem()");

            object[] properties;

            if (fields == null)
            {
                //properties = XbmcVideo.Fields;
                properties = XbmcMedia.Fields;
            }
            else
            {
                properties = fields;
            }

            JObject args = new JObject();
            args.Add("playerid", this.id);
            args.Add("properties", new JArray(properties));
            JObject query = this.client.Call("Player.GetItem", args) as JObject;

            if (query == null || query["item"] == null)
            {
                this.client.LogErrorMessage("XbmcVideoPlaylist.GetCurrentItem(): Invalid response");

                return null;
            }

            JObject item = (JObject) query["item"];

            this.client.LogMessage("Trying to identify a video playlist item from JSON");
            result = XbmcVideo.FromJson(item, this.client);
            if (result == null) this.client.LogMessage("Result is null!!!");
            return result;
        }

        public override XbmcPlaylist<XbmcVideo> GetItems(params string[] fields)
        {
            return this.GetItems(-1, -1, fields);
        }

        public override XbmcPlaylist<XbmcVideo> GetItems(int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoPlaylist.GetItems()");

            JObject query = this.getItems(fields, XbmcVideo.Fields, start, end);
            if (query == null || query["items"] == null)
            {
                this.client.LogErrorMessage("Playlist.GetItems(): Invalid response");

                return null;
            }

            XbmcPlaylist<XbmcVideo> playlist = XbmcPlaylist<XbmcVideo>.FromJson(query, this.client);
            foreach (JObject item in (JArray)query["items"])
            {
                playlist.Add(XbmcVideo.FromJson(item, this.client));
            }

            return playlist;
        }

        public bool Add(XbmcVideo video)
        {
            this.client.LogMessage("XbmcVideoPlaylist.Add()");

            if (video == null)
            {
                throw new ArgumentNullException("video");
            }
            if (string.IsNullOrEmpty(video.File))
            {
                throw new ArgumentException("The given video has no file assigned to it.");
            }

            return base.Add(video.File);
        }

        #endregion
    }
}
