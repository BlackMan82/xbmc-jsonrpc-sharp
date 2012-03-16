﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc
{
    public class XbmcVideoLibrary : XbmcMediaLibrary
    {
        #region Constructor

        internal XbmcVideoLibrary(JsonRpcClient client)
            : base("VideoLibrary", client)
        { }

        #endregion

        #region JSON RPC Calls

        // TODO: XbmcVideoLibrary: Add sorting

        public ICollection<XbmcMovie> GetMovies(params string[] fields)
        {
            return this.GetMovies(-1, -1, fields);
        }

        public ICollection<XbmcMovie> GetMovies(int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetMovies()");

            JObject args = new JObject();
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 2];
                fieldCopy[0] = "movieid";
                fieldCopy[1] = "title";
                Array.Copy(fields, 0, fieldCopy, 2, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcMovie.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));

            JObject query = this.client.Call("VideoLibrary.GetMovies", args) as JObject;
            if (query == null || query["movies"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetMovies(): Invalid response");
                
                return null;
            }

            List<XbmcMovie> movies = new List<XbmcMovie>();
            foreach (JObject item in (JArray)query["movies"])
            {
                movies.Add(XbmcMovie.FromJson(item, this.client));
            }

            return movies;
        }

        public ICollection<XbmcTvShow> GetTvShows(params string[] fields)
        {
            return this.GetTvShows(-1, -1, fields);
        }

        public ICollection<XbmcTvShow> GetTvShows(int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetTvShows()");

            JObject args = new JObject();
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 2];
                fieldCopy[0] = "tvshowid";
                fieldCopy[1] = "title";
                Array.Copy(fields, 0, fieldCopy, 2, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcTvShow.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));
            

            JObject query = this.client.Call("VideoLibrary.GetTVShows", args) as JObject;
            if (query == null || query["tvshows"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetTVShows(): Invalid response");

                return null;
            }

            List<XbmcTvShow> shows = new List<XbmcTvShow>();
            foreach (JObject item in (JArray)query["tvshows"])
            {
                shows.Add(XbmcTvShow.FromJson(item, this.client));
            }

            return shows;
        }

        public ICollection<XbmcTvSeason> GetTvSeasons(XbmcTvShow tvshow, params string[] fields)
        {
            if (tvshow == null)
            {
                throw new ArgumentNullException("tvshow");
            }

            return this.getTvSeasons(tvshow.Id, -1, -1, fields);
        }

        public ICollection<XbmcTvSeason> GetTvSeasons(XbmcTvShow tvshow, int start, int end, params string[] fields)
        {
            if (tvshow == null)
            {
                throw new ArgumentNullException("tvshow");
            }

            return this.getTvSeasons(tvshow.Id, start, end, fields);
        }

        public ICollection<XbmcTvEpisode> GetTvEpisodes(XbmcTvShow tvshow, int season, params string[] fields)
        {
            if (tvshow == null)
            {
                throw new ArgumentNullException("tvshow");
            }

            return this.getTvEpisodes(tvshow.Id, season, -1, -1, fields);
        }

        public ICollection<XbmcTvEpisode> GetTvEpisodes(XbmcTvShow tvshow, int season, int start, int end, params string[] fields)
        {
            if (tvshow == null)
            {
                throw new ArgumentNullException("tvshow");
            }

            return this.getTvEpisodes(tvshow.Id, season, start, end, fields);
        }

        public ICollection<XbmcMusicVideo> GetMusicVideos(params string[] fields)
        {
            return this.GetMusicVideos(-1, -1, -1, -1, fields);
        }

        public ICollection<XbmcMusicVideo> GetMusicVideos(int start, int end, params string[] fields)
        {
            return this.GetMusicVideos(-1, -1, start, end, fields);
        }

        private ICollection<XbmcMusicVideo> GetMusicVideos(XbmcArtist artist, XbmcAlbum album, params string[] fields)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist");
            }
            if (album == null)
            {
                throw new ArgumentNullException("album");
            }

            return this.GetMusicVideos(artist.Id, album.Id, -1, -1, fields);
        }

        private ICollection<XbmcMusicVideo> GetMusicVideos(XbmcArtist artist, XbmcAlbum album, int start, int end, params string[] fields)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist");
            }
            if (album == null)
            {
                throw new ArgumentNullException("album");
            }

            return this.GetMusicVideos(artist.Id, album.Id, start, end, fields);
        }

        private ICollection<XbmcMusicVideo> GetMusicVideos(int artistId, int albumId, int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetMusicVideos()");

            JObject args = new JObject();
            if (artistId >= 0)
            {
                args.Add(new JProperty("artistid", artistId));
            }
            if (albumId >= 0)
            {
                args.Add(new JProperty("albumid", albumId));
            }
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 4];
                //fieldCopy[0] = "musicvideoid";
                //fieldCopy[0] = "title";
                //fieldCopy[1] = "artist";
                //fieldCopy[1] = "album";
                fieldCopy[0] = "musicvideoid";
                fieldCopy[1] = "title";
                fieldCopy[2] = "artist";
                fieldCopy[3] = "album";
                Array.Copy(fields, 0, fieldCopy, 4, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcMusicVideo.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));

            JObject query = this.client.Call("VideoLibrary.GetMusicVideos", args) as JObject;
            if (query == null || query["musicvideos"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetMusicVideos(): Invalid response");

                return null;
            }

            List<XbmcMusicVideo> musicVideos = new List<XbmcMusicVideo>();
            foreach (JObject item in (JArray)query["musicvideos"])
            {
                musicVideos.Add(XbmcMusicVideo.FromJson(item));
            }

            return musicVideos;
        }

        private ICollection<XbmcMusicVideo> GetMusicVideos(XbmcArtist artist, params string[] fields)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist");
            }

            return this.GetMusicVideos(artist.Id, -1, -1, -1, fields);
        }

        private ICollection<XbmcMusicVideo> GetMusicVideos(XbmcArtist artist, int start, int end, params string[] fields)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist");
            }

            return this.GetMusicVideos(artist.Id, -1, start, end, fields);
        }

        private ICollection<XbmcMusicVideo> GetMusicVideos(XbmcAlbum album, params string[] fields)
        {
            if (album == null)
            {
                throw new ArgumentNullException("album");
            }

            return this.GetMusicVideos(-1, album.Id, -1, -1, fields);
        }

        private ICollection<XbmcMusicVideo> GetMusicVideos(XbmcAlbum album, int start, int end, params string[] fields)
        {
            if (album == null)
            {
                throw new ArgumentNullException("album");
            }

            return this.GetMusicVideos(-1, album.Id, start, end, fields);
        }

        public ICollection<XbmcMovie> GetRecentlyAddedMovies(params string[] fields)
        {
            return this.GetRecentlyAddedMovies(-1, -1, fields);
        }

        public ICollection<XbmcMovie> GetRecentlyAddedMovies(int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetRecentlyAddedMovies()");

            JObject args = new JObject();
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 2];
                fieldCopy[0] = "movieid";
                fieldCopy[1] = "title";
                Array.Copy(fields, 0, fieldCopy, 2, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcMovie.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));

            JObject query = this.client.Call("VideoLibrary.GetRecentlyAddedMovies", args) as JObject;
            if (query == null || query["movies"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetRecentlyAddedMovies(): Invalid response");

                return null;
            }

            List<XbmcMovie> movies = new List<XbmcMovie>();
            foreach (JObject item in (JArray)query["movies"])
            {
                movies.Add(XbmcMovie.FromJson(item, this.client));
            }

            return movies;
        }

        public ICollection<XbmcTvEpisode> GetRecentlyAddedTvEpisodes(params string[] fields)
        {
            return this.GetRecentlyAddedTvEpisodes(-1, -1, fields);
        }

        public ICollection<XbmcTvEpisode> GetRecentlyAddedTvEpisodes(int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetRecentlyAddedTvEpisodes()");

            JObject args = new JObject();
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 5];
                //fieldCopy[0] = "episodeid";
                //fieldCopy[0] = "title";
                //fieldCopy[1] = "season";
                //fieldCopy[1] = "episode";
                //fieldCopy[2] = "showtitle";
                fieldCopy[0] = "id";
                fieldCopy[1] = "title";
                fieldCopy[2] = "season";
                fieldCopy[3] = "episode";
                fieldCopy[4] = "showtitle";
                Array.Copy(fields, 0, fieldCopy, 5, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcTvEpisode.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));

            JObject query = this.client.Call("VideoLibrary.GetRecentlyAddedEpisodes", args) as JObject;
            if (query == null || query["episodes"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetRecentlyAddedEpisodes(): Invalid response");

                return null;
            }

            List<XbmcTvEpisode> episodes = new List<XbmcTvEpisode>();
            foreach (JObject item in (JArray)query["episodes"])
            {
                episodes.Add(XbmcTvEpisode.FromJson(item, this.client));
            }

            return episodes;
        }

        public ICollection<XbmcMusicVideo> GetRecentlyAddedMusicVideos(params string[] fields)
        {
            return this.GetRecentlyAddedMusicVideos(-1, -1, fields);
        }

        public ICollection<XbmcMusicVideo> GetRecentlyAddedMusicVideos(int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetRecentlyAddedMusicVideos()");

            JObject args = new JObject();
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 4];
                //fieldCopy[0] = "musicvideoid";
                //fieldCopy[0] = "title";
                //fieldCopy[1] = "artist";
                //fieldCopy[1] = "album";
                fieldCopy[0] = "musicvideoid";
                fieldCopy[1] = "title";
                fieldCopy[2] = "artist";
                fieldCopy[3] = "album";
                Array.Copy(fields, 0, fieldCopy, 4, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcMusicVideo.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));

            JObject query = this.client.Call("VideoLibrary.GetRecentlyAddedMusicVideos", args) as JObject;
            if (query == null || query["musicvideos"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetRecentlyAddedMusicVideos(): Invalid response");

                return null;
            }

            List<XbmcMusicVideo> musicVideos = new List<XbmcMusicVideo>();
            foreach (JObject item in (JArray)query["musicvideos"])
            {
                musicVideos.Add(XbmcMusicVideo.FromJson(item));
            }

            return musicVideos;
        }

        #endregion

        #region Helper functions

        private ICollection<XbmcTvSeason> getTvSeasons(int tvshowId, int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetTvSeasons()");

            if (tvshowId < 0)
            {
                throw new ArgumentException("Invalid TvShow Id (" + tvshowId + ")");
            }

            JObject args = new JObject();
            args.Add(new JProperty("tvshowid", tvshowId));
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 3];
                fieldCopy[0] = "title";
                fieldCopy[1] = "season";
                fieldCopy[2] = "showtitle";
                Array.Copy(fields, 0, fieldCopy, 3, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcTvSeason.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));

            JObject query = this.client.Call("VideoLibrary.GetSeasons", args) as JObject;
            if (query == null || query["seasons"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetSeasons(): Invalid response");

                return null;
            }

            List<XbmcTvSeason> seasons = new List<XbmcTvSeason>();
            foreach (JObject item in (JArray)query["seasons"])
            {
                seasons.Add(XbmcTvSeason.FromJson(item, this.client));
            }

            return seasons;
        }

        private ICollection<XbmcTvEpisode> getTvEpisodes(int tvshowId, int season, int start, int end, params string[] fields)
        {
            this.client.LogMessage("XbmcVideoLibrary.GetTvEpisodes()");

            if (tvshowId < 0)
            {
                throw new ArgumentException("Invalid TvShow Id (" + tvshowId + ")");
            }
            if (season < 0)
            {
                throw new ArgumentException("Invalid Season (" + season + ")");
            }

            JObject args = new JObject();
            args.Add(new JProperty("tvshowid", tvshowId));
            args.Add(new JProperty("season", season));
            if (fields != null && fields.Length > 0)
            {
                string[] fieldCopy = new string[fields.Length + 5];
                //fieldCopy[0] = "episodeid";
                //fieldCopy[0] = "id";
                //fieldCopy[0] = "title";
                //fieldCopy[1] = "season";
                //fieldCopy[1] = "episode";
                //fieldCopy[2] = "showtitle";
                fieldCopy[0] = "id";
                fieldCopy[1] = "title";
                fieldCopy[2] = "season";
                fieldCopy[3] = "episode";
                fieldCopy[4] = "showtitle";
                Array.Copy(fields, 0, fieldCopy, 5, fields.Length);
                args.Add(new JProperty("fields", fieldCopy));
            }
            else
            {
                args.Add(new JProperty("fields", XbmcTvEpisode.Fields));
            }
            JObject limits = new JObject();
            if (start >= 0)
            {
                limits.Add(new JProperty("start", start));
            }
            if (end >= 0)
            {
                limits.Add(new JProperty("end", end));
            }
            args.Add(new JProperty("limits", limits));

            JObject query = this.client.Call("VideoLibrary.GetEpisodes", args) as JObject;
            if (query == null || query["episodes"] == null)
            {
                this.client.LogErrorMessage("VideoLibrary.GetEpisodes(): Invalid response");

                return null;
            }

            List<XbmcTvEpisode> episodes = new List<XbmcTvEpisode>();
            foreach (JObject item in (JArray)query["episodes"])
            {
                episodes.Add(XbmcTvEpisode.FromJson(item, this.client));
            }

            return episodes;
        }

        #endregion
    }
}
