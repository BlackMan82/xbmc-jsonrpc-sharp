﻿using System;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using System.Text;

namespace XBMC.JsonRpc
{
    public class XbmcJsonRpcConnection : IDisposable
    {
        #region Constants

        private const int AnnouncementPort = 9090;
        private const string AnnouncementEnd = "}}";
        private const string AnnouncementEndAlternative = "}\n}\n";
        //private const string AnnouncementMethod = "Announcement";
        private const string AnnouncementSender = "xbmc";
        private const string PingResponse = "pong";

        #endregion

        #region Private variables

        private bool disposed;

        private JsonRpcClient client;
        private Socket socket;

        private XbmcJsonRpc jsonRpc;
        private XbmcPlayer player;
        private XbmcSystem system;
        private XbmcGeneral xbmc;
        private XbmcFiles files;
        private XbmcPlaylist playlist;
        private XbmcLibrary library;

        #endregion

        #region Public variables

        public bool IsAlive
        {
            get
            {
                this.client.LogMessage("XbmcJsonRpcConnection.IsAlive");
                
                try
                {
                    if (this.socket == null || !this.socket.Connected)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    this.client.LogErrorMessage("Could not determine the state of the TCP socket", ex);
                    return false;
                }

                string ping = this.jsonRpc.Ping();
                if (string.IsNullOrEmpty(ping) || string.CompareOrdinal(ping, PingResponse) != 0)
                {
                    return false;
                }

                return true;
            }
        }

        public XbmcJsonRpc JsonRpc
        {
            get { return this.jsonRpc; }
        }

        public XbmcPlayer Player
        {
            get { return this.player; }
        }

        public XbmcSystem System
        {
            get { return this.system; }
        }

        public XbmcGeneral Xbmc
        {
            get { return this.xbmc; }
        }

        public XbmcFiles Files
        {
            get { return this.files; }
        }

        public XbmcPlaylist Playlist
        {
            get { return this.playlist; }
        }

        public XbmcLibrary Library
        {
            get { return this.library; }
        }

        #endregion

        #region Events

        public event EventHandler Connected;
        public event EventHandler Aborted;
        public event EventHandler<XbmcJsonRpcLogEventArgs> Log;
        public event EventHandler<XbmcJsonRpcLogErrorEventArgs> LogError;

        #endregion

        #region Constructors

        public XbmcJsonRpcConnection(Uri uri)
            : this(uri, null, null)
        { }

        public XbmcJsonRpcConnection(Uri uri, string username, string password)
        {
            this.client = new JsonRpcClient(uri, username, password);
            this.client.Log += onLog;
            this.client.LogError += onLogError;

            this.jsonRpc = new XbmcJsonRpc(this.client);
            this.player = new XbmcPlayer(this.client);
            this.system = new XbmcSystem(this.client);
            this.xbmc = new XbmcGeneral(this.client);
            this.files = new XbmcFiles(this.client);
            this.playlist = new XbmcPlaylist(this.client);
            this.library = new XbmcLibrary(this.client);
        }

        public XbmcJsonRpcConnection(string address, int port)
            : this(address, port, null, null)
        { }

        public XbmcJsonRpcConnection(string address, int port, string username, string password)
            : this(new Uri("http://" + address + ":" + port + "/jsonrpc"), username, password)
        { }

        #endregion

        #region Public functions

        public bool Open() 
        {
            this.client.LogMessage("Opening a connection to XBMC");

            try
            {
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.Connect(this.client.Uri.Host, AnnouncementPort);
                // Send a ping to XBMC
                if (!this.IsAlive)
                {
                    this.Close();
                    return false;
                }
                this.onConnected();

                this.receive(new SocketStateObject());
            }
            catch (Exception ex)
            {
                this.client.LogErrorMessage("Could not open a connection to XBMC", ex);
                return false;
            }

            return true;
        }

        public void Close()
        {
            this.client.LogMessage("Closing the connection");

            try
            {
                if (this.socket != null && this.socket.Connected)
                {
                    this.socket.Disconnect(false);
                }
            }
            catch (Exception ex)
            {
                this.client.LogErrorMessage("Could not disconnect from the TCP socket", ex);
            }
        }

        #endregion

        #region Implementations of IDisposable

        public void Dispose()
        {
            if (!this.disposed) 
            {
                try
                {
                    lock (this.socket)
                    {
                        this.Close();
                        this.socket.Close();
                    }
                }
                catch (Exception ex)
                {
                    this.client.LogErrorMessage("Could not close the TCP socket", ex);
                }
                finally
                {
                    this.disposed = true;
                }
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Private functions

        private void onConnected()
        {
            if (this.Connected == null)
            {
                return;
            }

            this.Connected(this, null);
        }

        private void onAborted()
        {
            if (this.Aborted == null)
            {
                return;
            }

            this.Aborted(this, null);
        }

        private void onAnnouncement(string data)
        {
            JObject announcement = JObject.Parse(data);
            JObject param = announcement["params"] as JObject;
            if (announcement["method"] == null || param == null || param["sender"] == null || string.CompareOrdinal((string)param["sender"], AnnouncementSender) != 0)
            {
                this.client.LogErrorMessage("Wrong format of announcement");
                return;
            }

            string type = (string)announcement["method"];

            if (string.CompareOrdinal(type, "Player.OnPlay") == 0)
            {
                this.player.OnPlaybackStarted();
            }
            else if (string.CompareOrdinal(type, "Player.OnPause") == 0)
            {
                this.player.OnPlaybackPaused();
            }
            //else if (string.CompareOrdinal(type, "PlaybackResumed") == 0)
            //{
            //    this.player.OnPlaybackResumed();
            //}
            else if (string.CompareOrdinal(type, "Player.OnStop") == 0)
            {
                this.player.OnPlaybackStopped();
            }
            //else if (string.CompareOrdinal(type, "PlaybackEnded") == 0)
            //{
            //    this.player.OnPlaybackEnded();
            //}
            else if (string.CompareOrdinal(type, "Player.OnSeek") == 0)
            {
                this.player.OnPlaybackSeek();
            }
            //else if (string.CompareOrdinal(type, "PlaybackSeekChapter") == 0)
            //{
            //    this.player.OnPlaybackSeekChapter();
            //}
            else if (string.CompareOrdinal(type, "Player.OnSpeedChanged") == 0)
            {
                this.player.OnPlaybackSpeedChanged();
            }
            //else if (string.CompareOrdinal(type, "QueueNextItem") == 0)
            //{
            //    this.playlist.OnItemQueued();
            //}
            else if (string.CompareOrdinal(type, "System.OnQuit") == 0)
            {
                this.Close();
                this.onAborted();
            }
            //else if (string.CompareOrdinal(type, "Shutdown") == 0)
            //{
            //    this.Close();
            //    this.system.OnShutdown();
            //}
            //else if (string.CompareOrdinal(type, "Suspend") == 0)
            //{
            //    this.Close();
            //    this.system.OnSuspend();
            //}
            //else if (string.CompareOrdinal(type, "Hibernate") == 0)
            //{
            //    this.Close();
            //    this.system.OnHibernate();
            //}
            //else if (string.CompareOrdinal(type, "System.OnRestart") == 0)
            //{
            //    this.Close();
            //    this.system.OnReboot();
            //}
            else if (string.CompareOrdinal(type, "System.OnSleep") == 0)
            {
                this.Close();
                this.system.OnSleep();
            }
            else if (string.CompareOrdinal(type, "System.OnWake") == 0)
            {
                this.system.OnWake();
            }
            //else if (string.CompareOrdinal(type, "Resume") == 0)
            //{
            //    this.system.OnResume();
            //}
            else if (string.CompareOrdinal(type, "System.OnLowBattery") == 0)
            {
                this.system.OnLowBattery();
            }
        }

        private void receiveAnnouncements(IAsyncResult result)
        {
            if (this.disposed)
            {
                return;
            }

            lock (this.socket)
            {
                SocketStateObject state = result.AsyncState as SocketStateObject;
                if (state == null || this.socket == null || !this.socket.Connected)
                {
                    return;
                }

                int read = 0;
                try
                {
                    read = this.socket.EndReceive(result);
                }
                catch (Exception ex)
                {
                    this.client.LogErrorMessage("Could not read the TCP socket", ex);
                    this.Close();
                    this.onAborted();
                }

                if (read > 0)
                {
                    state.Builder.Append(Encoding.UTF8.GetString(state.Buffer, 0, read));

                    this.receive(state);
                }

                string data = state.Builder.ToString();
                if (data.Length > 0 && data.Contains(AnnouncementEnd) || data.Contains(AnnouncementEndAlternative))
                {
                    this.client.LogMessage("JSON RPC Announcement received: " + data);
                    
                    // TODO: Find the LAST AnnouncementEnd!!!
                    //int pos = data.IndexOf(AnnouncementEnd);
                    int pos = data.LastIndexOf(AnnouncementEnd);
                    if (pos < 0)
                    {
//                        pos = data.IndexOf(AnnouncementEndAlternative) + AnnouncementEndAlternative.Length;
                        pos = data.LastIndexOf(AnnouncementEndAlternative) + AnnouncementEndAlternative.Length;
                    }
                    else
                    {
                        pos += AnnouncementEnd.Length;
                    }
                    state.Builder.Remove(0, pos);
                    this.onAnnouncement(data.Substring(0, pos));
                }

                this.receive(state);
            }
        }

        private void receive(SocketStateObject state)
        {
            if (state == null || this.socket == null || !this.socket.Connected)
            {
                return;
            }

            try
            {
                this.socket.BeginReceive(state.Buffer, 0, SocketStateObject.BufferSize,
                    0, new AsyncCallback(this.receiveAnnouncements), state);
            }
            catch (Exception ex)
            {
                this.client.LogErrorMessage("Could not start receiving from the TCP socket", ex);
                this.Close();
                this.onAborted();
            }
        }

        private void onLog(object sender, XbmcJsonRpcLogEventArgs e)
        {
            if (this.Log != null)
            {
                this.Log(this, e);
            }
        }

        private void onLogError(object sender, XbmcJsonRpcLogErrorEventArgs e)
        {
            if (this.LogError != null)
            {
                this.LogError(this, e);
            }
        }

        #endregion
    }
}
