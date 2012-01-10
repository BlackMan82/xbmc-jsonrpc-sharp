using System;
using Newtonsoft.Json.Linq;

namespace XBMC.JsonRpc
{
    public class XbmcMediaPlayer : XbmcJsonRpcNamespace
    {
        #region Private variables

        private int id;
        //private int id { get { return id; } set { this.id = value; } }
        private string playerName;
        private string infoLabelName;

        #endregion

        #region Protected variables

        protected XbmcPlayerState state
        {
            get
            {
                this.client.LogMessage("XbmcMediaPlayer.State");

                return this.parsePlayerState(this.getPlayerProperties(new object[] { "speed", "partymode" }));
            }
        }

        #endregion

        #region Constructors

        protected XbmcMediaPlayer(string playerName, JsonRpcClient client, int id)
            : this(playerName, null, client, id)
        { }

        protected XbmcMediaPlayer(string playerName, string infoLabelName, JsonRpcClient client, int id)
            : base(client)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException();
            }
            if (string.IsNullOrEmpty(infoLabelName))
            {
                infoLabelName = playerName;
            }

            this.playerName = playerName;
            this.infoLabelName = infoLabelName;
            this.id = (id == -1) ? (id = 1) : id;
        }

        #endregion

        #region JSON RPC Calls

        //public virtual XbmcPlayerState PlayPause()
        //{
        //    this.client.LogMessage("XbmcMediaPlayer.PlayPause()");

        //    return this.parsePlayerState(this.client.Call(this.playerName + ".PlayPause") as JObject);
        //}

        //public virtual bool Stop()
        //{
        //    this.client.LogMessage("XbmcMediaPlayer.Stop()");

        //    return (this.client.Call(this.playerName + ".Stop") != null);
        //}

        //public virtual bool SkipPrevious()
        //{
        //    this.client.LogMessage("XbmcMediaPlayer.SkipPrevious()");

        //    return (this.client.Call(this.playerName + ".SkipPrevious") != null);
        //}

        //public virtual bool SkipNext()
        //{
        //    this.client.LogMessage("XbmcMediaPlayer.SkipNext()");

        //    return (this.client.Call(this.playerName + ".SkipNext") != null);
        //}

        #endregion

        #region JSON RPC Info Labels

        public virtual int Speed
        {
            get
            {
                this.client.LogMessage("XbmcMediaPlayer.Speed");

                return this.getPlaySpeed();
            }
        }

        public virtual bool Random
        {
            get
            {
                this.client.LogMessage("XbmcMediaPlayer.Random");

                JObject obj = this.getPlayerProperties(new object[] { "shuffled" });
                return (((obj != null) && (obj["shuffled"] != null)) && ((bool)obj["shuffled"]));
            }
        }

        public virtual XbmcRepeatTypes Repeat
        {
            get
            {
                this.client.LogMessage("XbmcMediaPlayer.Repeat");

                JObject obj = this.getPlayerProperties(new object[] { "Repeat" });
                if ((obj != null) && (obj["Repeat"] != null))
                {
                    if (((string)obj["Repeat"]) == "One")
                    {
                        return XbmcRepeatTypes.One;
                    }
                    if (((string)obj["Repeat"]) == "All")
                    {
                        return XbmcRepeatTypes.All;
                    }
                }
                return XbmcRepeatTypes.Off;
            }
        }

        #endregion

        #region Helper functions

        //protected bool bigSkipBackward()
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".()");

        //    return (this.client.Call(this.playerName + ".BigSkipBackward") != null);
        //}

        //protected bool bigSkipForward()
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".BigSkipForward()");

        //    return (this.client.Call(this.playerName + ".BigSkipForward") != null);
        //}

        //protected bool smallSkipBackward()
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".SmallSkipBackward()");

        //    return (this.client.Call(this.playerName + ".SmallSkipBackward") != null);
        //}

        //protected bool smallSkipForward()
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".SmallSkipForward()");

        //    return (this.client.Call(this.playerName + ".SmallSkipForward") != null);
        //}

        //protected bool rewind()
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".Rewind()");

        //    return (this.client.Call(this.playerName + ".Rewind") != null);
        //}

        //protected bool forward()
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".Forward()");

        //    return (this.client.Call(this.playerName + ".Forward") != null);
        //}

        protected XbmcPlayerState getTime(out TimeSpan currentPosition, out TimeSpan totalLength)
        {
            this.client.LogMessage("Xbmc" + this.playerName + ".GetTime()");

            currentPosition = new TimeSpan();
            totalLength = new TimeSpan();
            
            JObject obj = this.getPlayerProperties(new object[] { "time", "totaltime", "speed" });

            if (((obj != null) && (obj["time"] != null)) && ((obj["totaltime"] != null) && (obj["speed"] != null)))
            {
                JObject objTime = obj["time"] as JObject;
                JObject objTotalTime = obj["totaltime"] as JObject;
                int iSpeed = (int) obj["speed"];
                currentPosition = new TimeSpan((int) objTime["hours"], (int) objTime["minutes"], (int) objTime["seconds"]);
                totalLength = new TimeSpan((int) objTotalTime["hours"], (int) objTotalTime["minutes"], (int) objTotalTime["seconds"]);
                if (iSpeed > 0)
                {
                    return XbmcPlayerState.Playing;
                }
                return XbmcPlayerState.Paused;
            }
            
            this.client.LogErrorMessage("Xbmc" + this.playerName + ".GetTime(): Invalid response");

            return XbmcPlayerState.Unavailable;
        }

        protected double getPercentage()
        {
            this.client.LogMessage("Xbmc" + this.playerName + ".GetPercentage()");


            JObject obj = new JObject();
            obj.Add("playerid", this.id);
            obj.Add("properties", new JArray("percentage"));
            JObject properties = this.getPlayerProperties(new object[] { "percentage" });
            if ((properties == null) && (properties["percentage"] == null))
            {
                this.client.LogErrorMessage(this.playerName + ".GetPercentage(): Invalid response");
                return -1;
            }
            return (double) properties["percentage"];
        }

        protected JObject getPlayerProperties(params object[] properties)
        {
            JObject args = new JObject();
            args.Add("playerid", this.id);
            args.Add("properties", new JArray(properties));
            return (this.client.Call("Player.GetProperties", args) as JObject);
        }

        protected int getPlaySpeed()
        {
            JObject obj = this.getPlayerProperties(new object[] { "speed" });
            if ((obj != null) && (obj["speed"] != null))
            {
                return (int)obj["speed"];
            }
            return 0;
        }

        //protected bool seekTime(int seconds)
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".SeekTime(" + seconds + ")");

        //    if (seconds < 0)
        //    {
        //        seconds = 0;
        //    }

        //    return (this.client.Call(this.playerName + ".SeekTime", seconds) != null);
        //}

        //protected bool seekTime(TimeSpan position)
        //{
        //    return this.seekTime(Convert.ToInt32(position.TotalSeconds));
        //}

        //protected bool seekPercentage(int percentage)
        //{
        //    this.client.LogMessage("Xbmc" + this.playerName + ".SeekPercentage(" + percentage + ")");

        //    if (percentage < 0)
        //    {
        //        percentage = 0;
        //    }
        //    else if (percentage > 100)
        //    {
        //        percentage = 100;
        //    }

        //    return (this.client.Call(this.playerName + ".SeekPercentage", percentage) != null);
        //}

        protected XbmcPlayerState parsePlayerState(JObject obj)
        {
            if (obj == null || obj["speed"] == null || obj["partymode"] == null)
            {
                return XbmcPlayerState.Unavailable;
            }

            XbmcPlayerState state = XbmcPlayerState.Unavailable;

            int num = (int)obj["speed"];

            if (num > 0)
            {
                state = XbmcPlayerState.Playing;
            }
            else
            {
                state = XbmcPlayerState.Paused;
            }

            if ((bool)obj["partymode"])
            {
                state |= XbmcPlayerState.PartyMode;
            }

            return state;
        }

        #endregion
    }
}
