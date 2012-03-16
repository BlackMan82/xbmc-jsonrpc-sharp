using System;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc
{
    public class XbmcAudioPlaylist : XbmcMediaPlaylist<XbmcSong>
    {
        #region Constructor

        internal XbmcAudioPlaylist(JsonRpcClient client)
            : base("AudioPlaylist", client, 0)
        { }

        #endregion

        #region JSON RPC Calls

        #endregion

        #region Overrides of XbmcMediaPlaylist<XbmcVideo>

        public override XbmcSong GetCurrentItem()
        {
            return this.GetCurrentItem(null);
        }

        public override XbmcSong GetCurrentItem(string[] fields)
        {
            XbmcSong result; 
            
            this.client.LogMessage("XbmcAudioPlaylist.GetCurrentItem()");

            object[] properties;

            if (fields == null)
            {
                //properties = XbmcSong.Fields;
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
                this.client.LogErrorMessage("Playlist.GetCurrentItem(): Invalid response");

                return null;
            }

            JObject item = (JObject) query["item"];

            this.client.LogMessage("Trying to identify an audio playlist item from JSON");
            result = XbmcSong.FromJson(item, this.client);
            if (result == null) this.client.LogMessage("Result is null!!!");
            return result;
        }

        public override XbmcPlaylist<XbmcSong> GetItems(params string[] fields)
        {
            return this.GetItems(-1, -1, fields);
        }

        public override XbmcPlaylist<XbmcSong> GetItems(int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcAudioPlaylist.GetItems()");

            JObject query = this.getItems(fields, XbmcSong.Fields, start, end);
            if (query == null || query["result"] == null || (((JObject) query["result"])["items"]) == null)
            {
                this.client.LogErrorMessage("Playlist.GetItems(): Invalid response");

                return null;
            }

            XbmcPlaylist<XbmcSong> playlist = XbmcPlaylist<XbmcSong>.FromJson((JObject) query["result"], this.client);
            foreach (JObject item in (JArray)query["items"])
            {
                playlist.Add(XbmcSong.FromJson(item, this.client));
            }

            return playlist;
        }

        public bool Add(XbmcSong song)
        {
            this.client.LogMessage("XbmcAudioPlaylist.Add()");

            if (song == null)
            {
                throw new ArgumentNullException("song");
            }
            if (string.IsNullOrEmpty(song.File))
            {
                throw new ArgumentException("The given song has no file assigned to it.");
            }

            return base.Add(song.File);
        }

        #endregion
    }
}
