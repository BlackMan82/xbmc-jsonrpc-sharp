using System;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc
{
    public class XbmcPlayer : XbmcJsonRpcNamespace
    {
        #region Private variables

        //private int id;
        private XbmcAudioPlayer audio;
        private XbmcVideoPlayer video;
        private XbmcPicturePlayer pictures;

        #endregion

        #region Public variables

        public XbmcAudioPlayer Audio
        {
            get 
            {
                bool audioPlayer, videoPlayer, picturePlayer;
                int id;
                if (!this.GetActivePlayers(out videoPlayer, out audioPlayer, out picturePlayer, out id) || !audioPlayer)
                {
                    return null;
                }

                return this.audio;
            }
        }

        public XbmcVideoPlayer Video
        {
            get
            {
                bool audioPlayer, videoPlayer, picturePlayer;
                int id;
                if (!this.GetActivePlayers(out videoPlayer, out audioPlayer, out picturePlayer, out id) || !videoPlayer)
                {
                    return null;
                }

                return this.video;
            }
        }

        public XbmcPicturePlayer Pictures
        {
            get
            {
                bool audioPlayer, videoPlayer, picturePlayer;
                int id;
                if (!this.GetActivePlayers(out videoPlayer, out audioPlayer, out picturePlayer, out id) || !picturePlayer)
                {
                    return null;
                }

                return this.pictures;
            }
        }

        #endregion

        #region Events

        public event EventHandler<XbmcPlayerPlaybackChangedEventArgs> PlaybackStarted;
        public event EventHandler<XbmcPlayerPlaybackPositionChangedEventArgs> PlaybackPaused;
        public event EventHandler<XbmcPlayerPlaybackPositionChangedEventArgs> PlaybackResumed;
        public event EventHandler PlaybackStopped;
        public event EventHandler PlaybackEnded;

        public event EventHandler<XbmcPlayerPlaybackPositionChangedEventArgs> PlaybackSeek;
        public event EventHandler<XbmcPlayerPlaybackPositionChangedEventArgs> PlaybackSeekChapter;
        public event EventHandler<XbmcPlayerPlaybackSpeedChangedEventArgs> PlaybackSpeedChanged;

        #endregion

        #region Constructor

        internal XbmcPlayer(JsonRpcClient client)
            : base(client)
        {
            //this.id = -1;
            this.audio = new XbmcAudioPlayer(client);
            this.video = new XbmcVideoPlayer(client);
            this.pictures = new XbmcPicturePlayer(client);
        }

        #endregion

        #region JSON RPC Calls

        public bool GetActivePlayers(out bool video, out bool audio, out bool picture, out int id)
        {

            //JArray emptyArray = new JArray();

            this.client.LogMessage("XbmcPlayer.GetActivePlayers()");

            video = false;
            audio = false;
            picture = false;
            id = -1;

            JObject query = this.client.Call("Player.GetActivePlayers") as JObject;

            //if (queryOrig.Equals(emptyArray))
            //{
            //    this.client.LogMessage("Player.GetActivePlayers(): No active players found");

            //    return false;
            //}
            
            if (query == null)
            {
                this.client.LogErrorMessage("Player.GetActivePlayers(): Invalid response");

                return false;
            }

            //if (query["video"] != null)
            //{
            //    video = (bool)query["video"];
            //}
            //if (query["audio"] != null)
            //{
            //    audio = (bool)query["audio"];
            //}
            //if (query["picture"] != null)
            //{
            //    picture = (bool)query["picture"];
            //}

            id = (int) query["playerid"];
            switch (((string) query["type"]))
            {
                case "video":
                    video = true;
                    break;

                case "audio":
                    audio = true;
                    break;

                case "picture":
                    picture = true;
                    break;
            }

            return true;
        }

        #endregion

        #region Internal functions

        internal void OnPlaybackStarted()
        {
            if (this.PlaybackStarted == null)
            {
                return;
            }

            XbmcMediaPlayer player = this.getActivePlayer();
            if (player == null) 
            {
                return;
            }

            this.PlaybackStarted(this, new XbmcPlayerPlaybackChangedEventArgs(player));
        }

        internal void OnPlaybackPaused()
        {
            if (this.PlaybackPaused == null)
            {
                return;
            }

            TimeSpan current, total;
            XbmcMediaPlayer player = this.getProgress(out current, out total);
            if (player == null)
            {
                return;
            }

            this.PlaybackPaused(this, new XbmcPlayerPlaybackPositionChangedEventArgs(player, current, total));
        }

        internal void OnPlaybackResumed()
        {
            if (this.PlaybackResumed == null)
            {
                return;
            }

            XbmcMediaPlayer player = this.getActivePlayer();
            if (player == null)
            {
                return;
            }

            TimeSpan current = new TimeSpan();
            TimeSpan total = new TimeSpan();
            if (player is XbmcVideoPlayer)
            {
                ((XbmcVideoPlayer)player).GetTime(out current, out total);
            }
            else if (player is XbmcAudioPlayer)
            {
                ((XbmcAudioPlayer)player).GetTime(out current, out total);
            }

            this.PlaybackResumed(this, new XbmcPlayerPlaybackPositionChangedEventArgs(player, current, total));
        }

        internal void OnPlaybackStopped()
        {
            if (this.PlaybackStopped == null)
            {
                return;
            }

            this.PlaybackStopped(this, null);
        }

        internal void OnPlaybackEnded()
        {
            // TODO: First a QueueNextItem is sent
            if (this.PlaybackEnded == null)
            {
                return;
            }

            this.PlaybackEnded(this, null);
        }

        internal void OnPlaybackSeek()
        {
            if (this.PlaybackSeek == null)
            {
                return;
            }

            TimeSpan current, total;
            XbmcMediaPlayer player = this.getProgress(out current, out total);
            if (player == null)
            {
                return;
            }

            this.PlaybackSeek(this, new XbmcPlayerPlaybackPositionChangedEventArgs(player, current, total));
        }

        internal void OnPlaybackSeekChapter()
        {
            if (this.PlaybackSeekChapter == null)
            {
                return;
            }

            TimeSpan current, total;
            XbmcMediaPlayer player = this.getProgress(out current, out total);
            if (player == null)
            {
                return;
            }

            this.PlaybackSeekChapter(this, new XbmcPlayerPlaybackPositionChangedEventArgs(player, current, total));
        }

        internal void OnPlaybackSpeedChanged()
        {
            if (this.PlaybackSpeedChanged == null)
            {
                return;
            }

            TimeSpan current, total;
            XbmcMediaPlayer player = this.getProgress(out current, out total);
            //current = TimeSpan.Parse(this.getInfo<string>("Player.SeekTime")); 
            if (player == null)
            {
                return;
            }

            this.PlaybackSpeedChanged(this, new XbmcPlayerPlaybackSpeedChangedEventArgs(player, current, total, player.Speed));
        }

        #endregion

        #region Helper functions

        private XbmcMediaPlayer getActivePlayer()
        {
            bool video, audio, picture;
            int id;
            if (!this.GetActivePlayers(out video, out audio, out picture, out id))
            {
                return null;
            }

            if (video)
            {
                return this.video;
            }
            if (audio)
            {
                return this.audio;
            }
            if (picture)
            {
                return this.pictures;
            }
            
            return null;
        }

        private XbmcMediaPlayer getProgress(out TimeSpan current, out TimeSpan total)
        {
            current = new TimeSpan();
            total = new TimeSpan();

            XbmcMediaPlayer player = this.getActivePlayer();
            if (player == null)
            {
                return null;
            }

            if (player is XbmcVideoPlayer)
            {
                ((XbmcVideoPlayer)player).GetTime(out current, out total);
            }
            else if (player is XbmcAudioPlayer)
            {
                ((XbmcAudioPlayer)player).GetTime(out current, out total);
            }

            return player;
        }

        #endregion
    }
}
