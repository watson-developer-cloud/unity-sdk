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

using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;

namespace IBM.Watson.Connection
{
    class WSConnector : IDisposable
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
        /// The class is returned by a Request object containing the response to a request made
        /// by the client.
        /// </summary>
        public abstract class Message
        {};

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
        /// This delegate is invoked when the connection is opened.
        /// </summary>
        public ConnectorEvent OnOpen { get; set; }
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
        private WebSocket m_WebSocket = null;
        private bool m_SendingMessages = false;
        private Queue<Message> m_SendQueue = new Queue<Message>();
        #endregion

        #region Connector Interface
        public bool Send(Message request)
        {
            m_SendQueue.Enqueue(request);
            if (!m_SendingMessages)
                Runnable.Run(SendMessages());

            return false;
        }
        public void Dispose()
        {
            if (m_WebSocket != null)
                m_WebSocket.Close();
        }
        #endregion

        IEnumerator SendMessages()
        {
            m_SendingMessages = true;
            yield return null;

            if (m_WebSocket == null)
            {
                m_ConnectionState = ConnectionState.CONNECTING;

                m_WebSocket = new WebSocket(URL);
                m_WebSocket.SetCredentials(Authentication.m_User, Authentication.m_Password, true);
                m_WebSocket.OnOpen += OnWSOpen;
                m_WebSocket.OnClose += OnWSClose;
                m_WebSocket.OnError += OnWSError;
                m_WebSocket.OnMessage += OnWSMessage;
                m_WebSocket.ConnectAsync();

                // wait for state to change from connecting..
                while (m_ConnectionState == ConnectionState.CONNECTING)
                    yield return null;
            }

            while (m_ConnectionState == ConnectionState.CONNECTED)
            {
                if (m_SendQueue.Count > 0)
                {
                    Message msg = m_SendQueue.Dequeue();
                    if (msg == null )
                        continue;

                    if ( msg is TextMessage )
                        m_WebSocket.SendAsync( ((TextMessage)msg).Text, OnMessageSent );
                    else if ( msg is BinaryMessage )
                        m_WebSocket.SendAsync( ((BinaryMessage)msg).Data, OnMessageSent );
                }
                else
                {
                    yield return null;
                }
            }

            m_SendingMessages = false;
        }

        private void OnWSOpen(object sender, System.EventArgs e)
        {
            m_ConnectionState = ConnectionState.CONNECTED;
            if ( OnOpen != null )
                OnOpen( this );
        }

        private void OnWSClose(object sender, CloseEventArgs e)
        {
            m_ConnectionState = e.WasClean ? ConnectionState.CLOSED : ConnectionState.DISCONNECTED;
            if ( OnClose != null )
                OnClose( this );
        }

        private void OnMessageSent( bool completed )
        {
            if (! completed )
            {
                Log.Error( "WSConnector", "Failed to send message." );
                m_ConnectionState = ConnectionState.DISCONNECTED;
                if ( OnClose != null )
                    OnClose( this );
            }
        }

        private void OnWSMessage(object sender, MessageEventArgs e)
        {
            Message msg = null;
            if ( e.Type == Opcode.Text )
                msg = new TextMessage( e.Data );
            else if ( e.Type == Opcode.Binary )
                msg = new BinaryMessage( e.RawData );

            if ( OnMessage != null )
                OnMessage( msg );
        }

        private void OnWSError(object sender, ErrorEventArgs e)
        {
            Log.Error("WSConnector", "WebSocket Error: {0}", e.Message);
        }
    }
}
