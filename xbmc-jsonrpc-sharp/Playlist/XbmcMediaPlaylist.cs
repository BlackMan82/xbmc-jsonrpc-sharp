using System;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc {
    public abstract class XbmcMediaPlaylist<TMediaType>
        : XbmcJsonRpcNamespace
        where TMediaType : XbmcPlayable {
        #region Private variables

        private string playlistName;
        private int id;

        #endregion

        #region Constructor

        protected XbmcMediaPlaylist(string playlistName, JsonRpcClient client, int id)
            : base(client) {
            if (string.IsNullOrEmpty(playlistName)) {
                throw new ArgumentException();
            }

            this.playlistName = playlistName;
            this.id = id;
        }

        #endregion

        // TODO: Playlists features moved to Player namespace (Play (renamed to Open), Shuffle, UnShuffle, Repeat)
        // TODO: Removed Playlist.SkipPrevious and Playlist.SkipNext (use Player.GoPrevious and Player.GoNext instead)
        #region JSON RPC Call

        public virtual bool Play() {
            this.client.LogMessage("XbmcMediaPlaylist.Play()");

            return (this.client.Call(this.playlistName + ".Play") != null);
        }

        public virtual bool Play(int itemIndex) {
            this.client.LogMessage("XbmcMediaPlaylist.(" + itemIndex + ")");

            return (this.client.Call(this.playlistName + ".Play", itemIndex) != null);
        }

        public virtual bool SkipPrevious() {
            this.client.LogMessage("XbmcMediaPlaylist.SkipPrevious()");

            return (this.client.Call(this.playlistName + ".SkipPrevious") != null);
        }

        public virtual bool SkipNext() {
            this.client.LogMessage("XbmcMediaPlaylist.SkipNext()");

            return (this.client.Call(this.playlistName + ".SkipNext") != null);
        }

        // TODO: Get current playing item information
        //public abstract TMediaType GetCurrentItem(params string[] fields);

        public abstract XbmcPlaylist<TMediaType> GetItems(params string[] fields);

        public abstract XbmcPlaylist<TMediaType> GetItems(int start, int end, params string[] fields);

        public virtual bool Add(string file) {
            this.client.LogMessage("XbmcMediaPlaylist.Add(" + file + ")");

            if (string.IsNullOrEmpty(file)) {
                throw new ArgumentException("file");
            }

            JObject args = new JObject();
            args.Add(new JProperty("file", file));

            return (this.client.Call("Playlist.Add", args) != null);
        }

        public virtual bool Clear() {
            this.client.LogMessage("XbmcMediaPlaylist.Clear()");

            return (this.client.Call("Playlist.Clear") != null);
        }

        public virtual bool Shuffle() {
            this.client.LogMessage("XbmcMediaPlaylist.Shuffle()");

            return (this.client.Call(this.playlistName + ".Shuffle") != null);
        }

        public virtual bool UnShuffle() {
            this.client.LogMessage("XbmcMediaPlaylist.UnShuffle()");

            return (this.client.Call(this.playlistName + ".UnShuffle") != null);
        }

        #endregion

        #region Helper functions

        protected JObject getItems(string[] fields, string[] defaultFields, int start, int end) {
            JObject args = new JObject();
            if (fields != null && fields.Length > 0) {
                args.Add(new JProperty("properties", fields));
            }
            else {
                args.Add(new JProperty("properties", defaultFields));
            }
            JObject limits = new JObject();
            
            if (start >= 0) {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0) {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));
            args.Add(new JProperty("playlistid", this.id));

            return (this.client.Call("Playlist.GetItems", args) as JObject);
        }

        #endregion
    }
}
