﻿using System;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc
{
    public class XbmcSong : XbmcAudio
    {
        #region Private variables

        private string file;

        private string album;
        private int track;
        //private int disc;
        private TimeSpan duration;
        private string comment;
        private string lyrics;

        #endregion

        #region Internal variables

        //internal static new string[] Fields
        //{
        //    get { return (fields != null ? fields : new string[0]); }
        //}

        #endregion

        #region Public variables

        public string File
        {
            get { return this.file; }
        }

        public string Album
        {
            get { return this.album; }
        }

        public int Track
        {
            get { return this.track; }
        }

        //public int Disc
        //{
        //    get { return this.disc; }
        //}

        public TimeSpan Duration
        {
            get { return this.duration; }
        }

        public string Comment
        {
            get { return this.comment; }
        }

        public string Lyrics
        {
            get { return this.lyrics; }
        }

        #endregion

        #region Constructors

        //static XbmcSong()
        //{
        //    fields = new string[] { "title", "artist", "genre", "year", "file",
        //                            "rating", "album", "track", 
        //                            "duration", "comment", "lyrics" };
        //}

        //private XbmcSong(int id, string thumbnail, string fanart, string file,
        //                 string title, string artist, string genre, int year,
        //                 int rating, string album, int track, int disc,
        //                 int duration, string comment, string lyrics)
        private XbmcSong(int id, string thumbnail, string fanart, string file,
                         string title, string artist, string genre, int year,
                         int rating, string album, int track,
                         int duration, string comment, string lyrics)
            : base(id, thumbnail, fanart, title, artist, genre, year, rating)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("file");
            }

            this.file = file;
            this.album = album;
            this.track = track;
            //this.disc = disc;
            this.duration = TimeSpan.FromSeconds(duration);
            this.comment = comment;
            this.lyrics = lyrics;
        }

        #endregion

        #region Internal static functions

        internal static XbmcSong FromJson(JObject obj)
        {
            return FromJson(obj, null);
        }
        
        internal static XbmcSong FromJson(JObject obj, JsonRpcClient logger)
        {
            if (obj == null)
            {
                return null;
            }

            try 
            {
                return new XbmcSong(JsonRpcClient.GetField<int>(obj, "songid"),
                                    JsonRpcClient.GetField<string>(obj, "thumbnail"),
                                    JsonRpcClient.GetField<string>(obj, "fanart"),
                                    JsonRpcClient.GetField<string>(obj, "file"),
                                    JsonRpcClient.GetField<string>(obj, "title"),
                                    JsonRpcClient.GetField<string>(obj, "artist"),
                                    JsonRpcClient.GetField<string>(obj, "genre", string.Empty),
                                    JsonRpcClient.GetField<int>(obj, "year"),
                                    JsonRpcClient.GetField<int>(obj, "rating"),
                                    JsonRpcClient.GetField<string>(obj, "album", string.Empty),
                                    JsonRpcClient.GetField<int>(obj, "track"),
                                    //JsonRpcClient.GetField<int>(obj, "tracknumber"),
                    //JsonRpcClient.GetField<int>(obj, "discnumber"),
                                    JsonRpcClient.GetField<int>(obj, "duration"),
                                    JsonRpcClient.GetField<string>(obj, "comment", string.Empty),
                                    JsonRpcClient.GetField<string>(obj, "lyrics", string.Empty));
            }
            catch (Exception ex)
            {
                if (logger != null) logger.LogErrorMessage("EXCEPTION in XbmcSong.FromJson()!!!", ex);
                return null;
            }
        }

        #endregion
    }
}
