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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

//! Uncomment to enable message debugging
#define ENABLE_MESSAGE_DEBUGGING

using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using WebSocketSharp;

namespace IBM.Watson.Connection
{
    class WSConnector
    {
        #region Public Types
        public delegate void ConnectorEvent(WSConnector connection);
        public delegate void MessageEvent( Message resp);

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
        {};

        /// <summary>
        /// BinaryMessage for sending raw binaray data.
        /// </summary>
        public class BinaryMessage : Message
        {
            public BinaryMessage(byte[] data)
            {
                Data = data;
            }

            #region Public Properties
            /// <summary>
            /// Binary payload.
            /// </summary>
            public byte[] Data { get; set; }
            #endregion
        };
        /// <summary>
        /// TextMessage is used for sending text messages (e.g. JSON, XML)
        /// </summary>
        public class TextMessage : Message
        {
            public TextMessage( string text )
            {
                Text = text;
            }

            #region Public Properties
            /// <summary>
            /// Text payload.
            /// </summary>
            public string Text { get; set; }
            #endregion
        };
        #endregion

        #region Public Properties
        /// <summary>
        /// This delegeta is invoked when the connection is closed.
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
        /// Credentials used to authenticate with the server.
        /// </summary>
        public Config.CredentialsInfo Authentication { get; set; }
        /// <summary>
        /// The current state of this connector.
        /// </summary>
        public ConnectionState State { get { return m_ConnectionState; } set { m_ConnectionState = value; } }
        #endregion

        #region Private Data
        private ConnectionState m_ConnectionState = ConnectionState.CLOSED;
        private Thread m_SendThread = null;
        private AutoResetEvent m_SendEvent = new AutoResetEvent(false);
        private Queue<Message> m_SendQueue = new Queue<Message>();
        private Queue<Message> m_ReceiveQueue = new Queue<Message>();
        private int m_ReceiverID = 0;
        #endregion

        #region Public Functions
        /// <summary>
        /// This function sends the given message object.
        /// </summary>
        /// <param name="msg">This is either a BinaryMessage or TextMessage object.</param>
        public void Send(Message msg)
        {
#if ENABLE_MESSAGE_DEBUGGING
            Log.Debug( "WSConnector", "Sending {0} message: {1}",
                msg is TextMessage ? "TextMessage" : "BinaryMessage", 
                msg is TextMessage ? ((TextMessage)msg).Text : ((BinaryMessage)msg).Data.Length.ToString() + " bytes" );
#endif
            lock( m_SendQueue )
            {
                m_SendQueue.Enqueue(msg);
                m_SendEvent.Set();
            }

            if (m_SendThread == null )
            {
                m_ConnectionState = ConnectionState.CONNECTING;
                m_SendThread = new Thread( SendMessages );
                m_SendThread.Start();
            }

            if ( m_ReceiverID == 0 )
                m_ReceiverID = Runnable.Run( ProcessReceiveQueue() ); 
        }
        public void Close()
        {
            m_ConnectionState = ConnectionState.CLOSED;

            if ( m_ReceiverID != 0 )
            {
                Runnable.Stop( m_ReceiverID );
                m_ReceiverID = 0;
            }
        }
        #endregion

        #region Private Functions
        private IEnumerator ProcessReceiveQueue()
        {
            while( m_ConnectionState == ConnectionState.CONNECTED 
                || m_ConnectionState == ConnectionState.CONNECTING )
            {
                yield return null;

                lock( m_ReceiveQueue )
                {
                    while( m_ReceiveQueue.Count > 0 )
                    {
                        Message msg = m_ReceiveQueue.Dequeue();
#if ENABLE_MESSAGE_DEBUGGING
                        Log.Debug( "WSConnector", "Received {0} message: {1}",
                            msg is TextMessage ? "TextMessage" : "BinaryMessage", 
                            msg is TextMessage ? ((TextMessage)msg).Text : ((BinaryMessage)msg).Data.Length.ToString() + " bytes" );
#endif 
                        if ( OnMessage != null )
                            OnMessage( msg );
                    }
                }
            }

            if ( OnClose != null )
                OnClose( this );
            m_ReceiverID = 0;
        }
        #endregion

        #region Threaded Functions
        // NOTE: ALl functions in this region are operating in a background thread, do NOT call any Unity functions!

        void SendMessages()
        {
            WebSocket ws = null;

            ws = new WebSocket(URL);
            ws.SetCredentials(Authentication.m_User, Authentication.m_Password, true);
            ws.OnOpen += OnWSOpen;
            ws.OnClose += OnWSClose;
            ws.OnError += OnWSError;
            ws.OnMessage += OnWSMessage;
            ws.Connect();

            while (m_ConnectionState == ConnectionState.CONNECTED)
            {
                m_SendEvent.WaitOne();

                Message msg = null;
                lock( m_SendQueue )
                {
                    if (m_SendQueue.Count > 0)
                        msg = m_SendQueue.Dequeue();
                }

                if (msg == null )
                    continue;

                if ( msg is TextMessage )
                    ws.Send( ((TextMessage)msg).Text );
                else if ( msg is BinaryMessage )
                    ws.Send( ((BinaryMessage)msg).Data );
            }

            ws.Close();
        }

        private void OnWSOpen(object sender, System.EventArgs e)
        {
            m_ConnectionState = ConnectionState.CONNECTED;
        }

        private void OnWSClose(object sender, CloseEventArgs e)
        {
            m_ConnectionState = e.WasClean ? ConnectionState.CLOSED : ConnectionState.DISCONNECTED;
        }

        private void OnWSMessage(object sender, MessageEventArgs e)
        {
            Message msg = null;
            if ( e.Type == Opcode.Text )
                msg = new TextMessage( e.Data );
            else if ( e.Type == Opcode.Binary )
                msg = new BinaryMessage( e.RawData );
            else
                Log.Warning( "WSConnector", "Unsupported opcode {0}", e.Type.ToString() );

            lock( m_ReceiveQueue )
                m_ReceiveQueue.Enqueue( msg );
        }

        private void OnWSError(object sender, ErrorEventArgs e)
        {
            Dictionary<string,object> err = new Dictionary<string, object>();
            err["error"] = string.Format( "WebSocket Error: {0}", e.Message );

            lock( m_ReceiveQueue )
                m_ReceiveQueue.Enqueue( new TextMessage( MiniJSON.Json.Serialize( err ) ) );
        }
        #endregion
    }
}
