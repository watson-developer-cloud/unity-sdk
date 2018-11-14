/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

//! Uncomment to enable message debugging
//#define ENABLE_MESSAGE_DEBUGGING

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading;
#if !NETFX_CORE
using UnitySDK.WebSocketSharp;
#else
using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
#endif

namespace IBM.Watson.DeveloperCloud.Connection
{
    /// <summary>
    /// WebSocket connector class.
    /// </summary>
    public class WSConnector
    {
        #region Public Types
        public const string AUTHENTICATION_AUTHORIZATION_HEADER = "Authorization";
        /// <summary>
        /// Callback for a connector event.
        /// </summary>
        /// <param name="connection">The WSConnector object.</param>
        public delegate void ConnectorEvent(WSConnector connection);
        /// <summary>
        /// Callback for a message received on the connector.
        /// </summary>
        /// <param name="resp">The message object.</param>
        public delegate void MessageEvent(Message resp);

        /// <summary>
        /// ConnectionState enumeration describes the current state of this connector.
        /// </summary>
        public enum ConnectionState
        {
            /// <summary>
            /// We are trying to connect.
            /// </summary>
            CONNECTING,
            /// <summary>
            /// Connector is connected to the server.
            /// </summary>
            CONNECTED,
            /// <summary>
            /// Connected has lost connection to the server.
            /// </summary>
            DISCONNECTED,
            /// <summary>
            /// Connected has been closed to the server.
            /// </summary>
            CLOSED
        }

        /// <summary>
        /// The base abstract class for a Message that can be sent/received by this class.
        /// </summary>
        public abstract class Message
        { };

        /// <summary>
        /// BinaryMessage for sending raw binary data.
        /// </summary>
        public class BinaryMessage : Message
        {
            /// <summary>
            /// Constructor for a BinaryMessage object.
            /// </summary>
            /// <param name="data">The binary data to send as a message.</param>
            /// <param name="headers">The response headers.</param>
            public BinaryMessage(byte[] data, Dictionary<string, string> headers = null)
            {
                Data = data;
                Headers = headers;
            }

            #region Public Properties
            /// <summary>
            /// Binary payload.
            /// </summary>
            public byte[] Data { get; set; }
            /// <summary>
            /// The response headers
            /// </summary>
            public Dictionary<string, string> Headers { get; set; }
            #endregion
        };
        /// <summary>
        /// TextMessage is used for sending text messages (e.g. JSON, XML)
        /// </summary>
        public class TextMessage : Message
        {
            /// <summary>
            /// Constructor for a TextMessage object.
            /// </summary>
            /// <param name="text">The string of the text to send as a message.</param>
            /// <param name="headers">The response headers.</param>
            public TextMessage(string text, Dictionary<string, string> headers = null)
            {
                Text = text;
                Headers = headers;
            }

            #region Public Properties
            /// <summary>
            /// Text payload.
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// The response headers
            /// </summary>
            public Dictionary<string, string> Headers { get; set; }
            #endregion
        };
        #endregion

        #region Public Properties
        /// <summary>
        /// This delegate is invoked when the connection is closed.
        /// </summary>
        public ConnectorEvent OnClose { get; set; }
        /// <summary>
        /// This delegate is invoked when a message is received for a socket.
        /// </summary>
        public MessageEvent OnMessage { get; set; }
        /// <summary>
        /// The URL of the WebSocket.
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// Headers to pass when making the socket.
        /// </summary>
        private Dictionary<string, string> _headers;
        public Dictionary<string, string> Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new Dictionary<string, string>();
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }
        /// <summary>
        /// Credentials used to authenticate with the server.
        /// </summary>
        public Credentials Authentication { get; set; }
        /// <summary>
        /// The current state of this connector.
        /// </summary>
        public ConnectionState State { get { return _connectionState; } set { _connectionState = value; } }

        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
        }
#endregion

#region Private Data
        private ConnectionState _connectionState = ConnectionState.CLOSED;
#if !NETFX_CORE
        private Thread _sendThread = null;
#else
        private Task _sendTask = null;
#endif
        private AutoResetEvent _sendEvent = new AutoResetEvent(false);
        private Queue<Message> _sendQueue = new Queue<Message>();
        private AutoResetEvent _receiveEvent = new AutoResetEvent(false);
        private Queue<Message> _receiveQueue = new Queue<Message>();
        private int _receiverRoutine = 0;
        private static readonly string https = "https://";
        private static readonly string wss = "wss://";
#endregion

        /// <summary>
        /// Helper function to convert a HTTP/HTTPS url into a WS/WSS URL.
        /// </summary>
        /// <param name="URL">The URL to fix up.</param>
        /// <returns>The fixed up URL.</returns>
        public static string FixupURL(string URL)
        {
#if UNITY_2018_2_OR_NEWER
#if NET_4_6
            //  Use standard endpoints since 2018.2 supports TLS 1.2
            if (URL.StartsWith("https://stream."))
            {
                URL = URL.Replace("https://stream.", "wss://stream.");
            }

            //  TLS 1.0 endpoint - Do not change this to TLS 1.2 endpoint since
            //  users may need to use the TLS 1.0 endpoint because of different
            //  platforms.
            else if (URL.StartsWith("https://stream-tls10."))
            {
                URL = URL.Replace("https://stream-tls10.", "wss://stream-tls10.");
            }
            //  Germany
            else if (URL.StartsWith("https://stream-fra."))
            {
                URL = URL.Replace("https://stream-fra.", "wss://stream-fra.");
            }
            //  US East
            else if (URL.StartsWith("https://gateway-wdc."))
            {
                URL = URL.Replace("https://gateway-wdc.", "wss://gateway-wdc.");
            }
            //  Sydney
            else if (URL.StartsWith("https://gateway-syd."))
            {
                URL = URL.Replace("https://gateway-syd.", "wss://gateway-syd.");
            }
            //  Tokyo
            else if (URL.StartsWith("https://gateway-tok."))
            {
                URL = URL.Replace("https://gateway-tok.", "wss://gateway-tok.");
            }
            else
            {
                URL = URL.Replace(https, wss);
                Log.Warning("WSConnector", "No case for URL for wss://. Replacing https:// with wss://.");
            }
#else
            //  Use TLS 1.0 endpoint if user is on .NET 3.5. US South is the 
            //  only region that supports this endpoint.
            if (URL.StartsWith("https://stream."))
            {
                URL = URL.Replace("https://stream.", "wss://stream-tls10.");
            }
            else if (URL.StartsWith("https://stream-tls10."))
            {
                URL = URL.Replace("https://stream-tls10.", "wss://stream-tls10.");
            }
            else
            {
                URL = URL.Replace(https, wss);
                Log.Warning("WSConnector", "No case for URL for wss://. Replacing https:// with wss://.");
                Log.Warning("WSConnector", "Streaming with TLS 1.0 is only available in US South. Please create your Speech to Text instance in US South. Alternatviely, use Unity 2018.2 with .NET 4.x Scripting Runtime Version enabled (File > Build Settings > Player Settings > Other Settings > Scripting Runtime Version).");
            }
#endif
#else
            //  Use TLS 1.0 endpoint if user is on .NET 3.5 or 4.6 if using Unity 2018.1 or older.
            //  US South is the only region that supports this endpoint.
            if (URL.StartsWith("https://stream."))
            {
                URL = URL.Replace("https://stream.", "wss://stream-tls10.");
            }
            else if (URL.StartsWith("https://stream-tls10."))
            {
                URL = URL.Replace("https://stream-tls10.", "wss://stream-tls10.");
            }
            else
            {
                URL = URL.Replace(https, wss);
                Log.Warning("WSConnector", "No case for URL for wss://. Replacing https:// with wss://.");
                Log.Warning("WSConnector", "Streaming with TLS 1.0 is only available in US South. Please create your Speech to Text instance in US South. Alternatviely, use Unity 2018.2 with .NET 4.x Scripting Runtime Version enabled (File > Build Settings > Player Settings > Other Settings > Scripting Runtime Version).");
            }
#endif
            return URL;
        }

        /// <summary>
        /// Create a WSConnector for the given service and function. 
        /// </summary>
        /// <param name="credentials">The credentials for the service.</param>
        /// <param name="function">The name of the function to connect.</param>
        /// <param name="args">Additional function arguments.</param>
        /// <returns>The WSConnector object or null or error.</returns>
        public static WSConnector CreateConnector(Credentials credentials, string function, string args)
        {
            WSConnector connector = new WSConnector();
            if (credentials.HasWatsonAuthenticationToken())
            {
                args += "&watson-token=" + credentials.WatsonAuthenticationToken;
            }
            else if (credentials.HasCredentials())
            {
                connector.Authentication = credentials;
            }
            else if (credentials.HasIamTokenData())
            {
                credentials.GetToken();
                connector.Headers.Add(AUTHENTICATION_AUTHORIZATION_HEADER, string.Format("Bearer {0}", credentials.IamAccessToken));
            }

            connector.URL = FixupURL(credentials.Url) + function + args;

            return connector;
        }

#region Public Functions
        /// <summary>
        /// This function sends the given message object.
        /// </summary>
        /// <param name="msg">This is either a BinaryMessage or TextMessage object.</param>
        /// <param name="queue">If true, then this function will not signal or start the sending thread.</param>
        public void Send(Message msg, bool queue = false)
        {
#if ENABLE_MESSAGE_DEBUGGING
            Log.Debug( "WSConnector.Send()", "Sending {0} message: {1}",
                msg is TextMessage ? "TextMessage" : "BinaryMessage", 
                msg is TextMessage ? ((TextMessage)msg).Text : ((BinaryMessage)msg).Data.Length.ToString() + " bytes" );
#endif
            lock (_sendQueue)
            {
                _sendQueue.Enqueue(msg);
                if (!queue)
                    _sendEvent.Set();
            }

#if !NETFX_CORE
            if (!queue && _sendThread == null)
            {
                _connectionState = ConnectionState.CONNECTING;

                // start an actual thread for working with the WebSocket, otherwise
                // we'll get errors from deep inside the library code.
                _sendThread = new Thread(SendMessages);
                _sendThread.Start();
            }
#else
            if (!queue && _sendTask == null)
            {
                _connectionState = ConnectionState.CONNECTING;
                _sendTask = Task.Run(() => SendMessagesAsync());
            }
#endif

            // Run our receiver as a co-routine so it can invoke functions 
            // on the main thread.
            if (_receiverRoutine == 0)
                _receiverRoutine = Runnable.Run(ProcessReceiveQueue());
        }

        /// <summary>
        /// This closes this connector, it will block until the send thread exits.
        /// </summary>
        public void Close()
        {
            // setting the state to closed will make the SendThread automatically exit.
            _connectionState = ConnectionState.CLOSED;
        }
#endregion

#region Private Functions
        private IEnumerator ProcessReceiveQueue()
        {
            while (_connectionState == ConnectionState.CONNECTED
                || _connectionState == ConnectionState.CONNECTING)
            {
                yield return null;

                // check for a signal with a timeout of 0, this it just a quicker way to know if we have messages
                // without having to lock the _receiveQueue object.
                if (_receiveEvent.WaitOne(0))
                {
                    lock (_receiveQueue)
                    {
                        while (_receiveQueue.Count > 0)
                        {
                            Message msg = _receiveQueue.Dequeue();
#if ENABLE_MESSAGE_DEBUGGING
                            Log.Debug( "WSConnector.ProcessReceiveQueue()", "Received {0} message: {1}",
                                msg is TextMessage ? "TextMessage" : "BinaryMessage", 
                                msg is TextMessage ? ((TextMessage)msg).Text : ((BinaryMessage)msg).Data.Length.ToString() + " bytes" );
#endif
                            if (OnMessage != null)
                                OnMessage(msg);
                        }
                    }
                }
            }
            if (OnClose != null)
                OnClose(this);
        }
#endregion

#region Threaded Functions
        // NOTE: All functions in this region are operating in a background thread, do NOT call any Unity functions!
#if !NETFX_CORE
        private void SendMessages()
        {
            try
            {
                WebSocket ws = null;

                ws = new WebSocket(URL);
                if (Headers != null)
                    ws.CustomHeaders = Headers;
                if (Authentication != null)
                    ws.SetCredentials(Authentication.Username, Authentication.Password, true);
                ws.OnOpen += OnWSOpen;
                ws.OnClose += OnWSClose;
                ws.OnError += OnWSError;
                ws.OnMessage += OnWSMessage;

                if (DisableSslVerification)
                {
                    ws.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
                }
                else
                {
                    ws.SslConfiguration.ServerCertificateValidationCallback = null;
                }
#if NET_4_6
                //  Enable TLS 1.1 and TLS 1.2 if we are on .NET 4.x
                ws.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.None;
#else
                //  .NET 3.x does not support TLS 1.1 or TLS 1.2
                ws.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls;
#endif
                ws.Connect();

                while (_connectionState == ConnectionState.CONNECTED)
                {
                    _sendEvent.WaitOne(50);

                    Message msg = null;
                    lock (_sendQueue)
                    {
                        if (_sendQueue.Count > 0)
                            msg = _sendQueue.Dequeue();
                    }

                    while (msg != null)
                    {
                        if (msg is TextMessage)
                            ws.Send(((TextMessage)msg).Text);
                        else if (msg is BinaryMessage)
                            ws.Send(((BinaryMessage)msg).Data);

                        msg = null;
                        lock (_sendQueue)
                        {
                            if (_sendQueue.Count > 0)
                                msg = _sendQueue.Dequeue();
                        }
                    }
                }

                ws.Close();
            }
            catch (System.Exception e)
            {
                _connectionState = ConnectionState.DISCONNECTED;
                Log.Error("WSConnector", "Caught WebSocket exception: {0}", e.ToString());
            }
        }

        private void OnWSOpen(object sender, System.EventArgs e)
        {
            _connectionState = ConnectionState.CONNECTED;
        }

        private void OnWSClose(object sender, CloseEventArgs e)
        {
            _connectionState = e.WasClean ? ConnectionState.CLOSED : ConnectionState.DISCONNECTED;
        }

        private void OnWSMessage(object sender, MessageEventArgs e)
        {
            Message msg = null;
            if (e.IsText)
                msg = new TextMessage(e.Data, e.Headers);
            else if (e.IsBinary)
                msg = new BinaryMessage(e.RawData, e.Headers);

            lock (_receiveQueue)
                _receiveQueue.Enqueue(msg);
            _receiveEvent.Set();
        }

        private void OnWSError(object sender, ErrorEventArgs e)
        {
            _connectionState = ConnectionState.DISCONNECTED;
        }
#else
        private async Task SendMessagesAsync()
        {
            try
            {
                MessageWebSocket webSocket = new MessageWebSocket();

                if (Authentication != null)
                {
                    PasswordCredential credential = new PasswordCredential(Authentication.Url, Authentication.Username, Authentication.Password);
                    webSocket.Control.ServerCredential = credential;
                }

                webSocket.MessageReceived += WebSocket_MessageReceived;
                webSocket.Closed += WebSocket_Closed;

                DataWriter messageWriter = new DataWriter(webSocket.OutputStream);

                await webSocket.ConnectAsync(new Uri(URL));

                _connectionState = ConnectionState.CONNECTED;

                while (_connectionState == ConnectionState.CONNECTED)
                {
                    _sendEvent.WaitOne(50);

                    Message msg = null;
                    lock (_sendQueue)
                    {
                        if (_sendQueue.Count > 0)
                            msg = _sendQueue.Dequeue();
                    }

                    while (msg != null)
                    {
                        if (msg is TextMessage)
                        {
                            webSocket.Control.MessageType = SocketMessageType.Utf8;
                            messageWriter.WriteString(((TextMessage)msg).Text);
                            await messageWriter.StoreAsync();
                        }
                        else if (msg is BinaryMessage)
                        {
                            webSocket.Control.MessageType = SocketMessageType.Binary;
                            messageWriter.WriteBytes(((BinaryMessage)msg).Data);
                            await messageWriter.StoreAsync();
                        }

                        msg = null;
                        lock (_sendQueue)
                        {
                            if (_sendQueue.Count > 0)
                                msg = _sendQueue.Dequeue();
                        }
                    }
                }

                webSocket.Close(1000, "Complete");
            }
            catch (System.Exception e)
            {
                _connectionState = ConnectionState.DISCONNECTED;
                Log.Error("WSConnector.SendMessagesAsync()", "Caught WebSocket exception: {0}", e.ToString());
            }
        }

        private void WebSocket_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            _connectionState = (args.Code == 1000) ? ConnectionState.CLOSED : ConnectionState.DISCONNECTED;
        }

        private void WebSocket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                Message msg = null;
                if (args.MessageType == SocketMessageType.Utf8)
                {
                    using (var dataReader = args.GetDataReader())
                    {
                        dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                        string data = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                        msg = new TextMessage(data);
                    }
                }
                else if (args.MessageType == SocketMessageType.Binary)
                {
                    using (var dataReader = args.GetDataReader())
                    {
                        uint length = dataReader.UnconsumedBufferLength;
                        if (length > 0)
                        {
                            byte[] data = new byte[length];
                            dataReader.ReadBytes(data);
                            msg = new BinaryMessage(data);
                        }
                    }
                }

                lock (_receiveQueue)
                    _receiveQueue.Enqueue(msg);
                _receiveEvent.Set();
            }
            catch (System.Exception e)
            {
                Log.Error("WSConnector.SendMessagesAsync()", "Caught WebSocket exception: {0}", e.ToString());
            }
        }
#endif
#endregion
    }
}
