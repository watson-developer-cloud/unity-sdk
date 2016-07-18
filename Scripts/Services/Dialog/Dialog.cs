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

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
using System.Collections;
using System.IO;

namespace IBM.Watson.DeveloperCloud.Services.Dialog.v1
{
	/// <summary>
	/// This class wraps the Watson Dialog service. 
	/// <a href="http://www.ibm.com/watson/developercloud/dialog.html">Dialog Service</a>
	/// </summary>
	public class Dialog : IWatsonService
    {
        #region Public Types
        /// <summary>
        /// This callback is passed into GetDialogs().
        /// </summary>
        /// <param name="dialogs">The list of dialogs returned by GetDialogs().</param>
        public delegate void OnGetDialogs(Dialogs dialogs);
        /// <summary>
        /// The callback for UploadDialog().
        /// </summary>
        /// <param name="dialog_id"></param>
        public delegate void OnUploadDialog(string dialog_id);
        /// <summary>
        /// The callback for DeleteDialog().
        /// </summary>
        /// <param name="success"></param>
        public delegate void OnDialogCallback(bool success);
        /// <summary>
        /// The delegate for loading a file, used by UploadDialog().
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <returns>Should return a byte array of the file contents or null of failure.</returns>
        public delegate byte[] LoadFileDelegate(string filename);
        /// <summary>
        /// The delegate for saving a file, used by DownloadDialog().
        /// </summary>
        /// <param name="filename">The filename to save.</param>
        /// <param name="data">The data to save into the file.</param>
        public delegate void SaveFileDelegate(string filename, byte[] data);
        /// <summary>
        /// The callback delegate for the Converse() function.
        /// </summary>
        /// <param name="resp">The response object to a call to Converse().</param>
        public delegate void OnConverse(ConverseResponse resp);
        #endregion

        #region Public Properties
        /// <summary>
        /// Set this property to overload the internal file loading of this class.
        /// </summary>
        public LoadFileDelegate LoadFile { get; set; }
        /// <summary>
        /// Set this property to overload the internal file saving for this class.
        /// </summary>
        public SaveFileDelegate SaveFile { get; set; }
        #endregion

        #region Private Data
        private const string SERVICE_ID = "DialogV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region GetDialogs
        /// <summary>
        /// Grabs a list of all available dialogs from the service.
        /// </summary>
        /// <param name="callback">The callback to receive the list of dialogs.</param>
        /// <returns>Returns true if request has been sent.</returns>
        public bool GetDialogs(OnGetDialogs callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/dialogs");
            if (connector == null)
                return false;

            GetDialogsReq req = new GetDialogsReq();
            req.Callback = callback;
            req.OnResponse = OnGetDialogsResp;

            return connector.Send(req);
        }
        private class GetDialogsReq : RESTConnector.Request
        {
            public OnGetDialogs Callback { get; set; }
        };
        private void OnGetDialogsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dialogs classifiers = new Dialogs();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classifiers;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Dialog", "GetDialogs Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetDialogsReq)req).Callback != null)
                ((GetDialogsReq)req).Callback(resp.Success ? classifiers : null);
        }
        #endregion

        #region Download Dialog
        /// <summary>
        /// Format enumeration used by DownloadDialog.
        /// </summary>
        public enum DialogFormat
        {
            /// <summary>
            /// XML format.
            /// </summary>
            XML,
            /// <summary>
            /// JSON format.
            /// </summary>
            JSON,
            /// <summary>
            /// BINARY format.
            /// </summary>
            BINARY
        };

        /// <summary>
        /// Downloads the dialog by the dialog ID into the provided filename.
        /// </summary>
        /// <param name="dialogId">The ID of the dialog to download.</param>
        /// <param name="dialogFileName">The filename to download the dialog into.</param>
        /// <param name="callback">The callback to invoke on failure or success.</param>
        /// <param name="format">The format to download.</param>
        /// <returns>Returns true if request is sent.</returns>
        public bool DownloadDialog(string dialogId, string dialogFileName, OnDialogCallback callback, DialogFormat format = DialogFormat.XML)
        {
            if (string.IsNullOrEmpty(dialogId))
                throw new ArgumentNullException("dialogId");
            if (string.IsNullOrEmpty(dialogFileName))
                throw new ArgumentNullException("dialogFileName");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/dialogs/" + dialogId);
            if (connector == null)
                return false;

            DownloadDialogReq req = new DownloadDialogReq();
            req.DialogFileName = dialogFileName;
            req.Callback = callback;
            req.OnResponse = OnDownloadDialogResp;
            if (format == DialogFormat.XML)
                req.Headers["Accept"] = "application/wds+xml";
            else if (format == DialogFormat.JSON)
                req.Headers["Accept"] = "application/wds+json";
            else
                req.Headers["Accept"] = "application/octet-stream";

            return connector.Send(req);
        }

        private class DownloadDialogReq : RESTConnector.Request
        {
            public string DialogFileName { get; set; }
            public OnDialogCallback Callback { get; set; }
        };

        private void OnDownloadDialogResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DownloadDialogReq downloadReq = req as DownloadDialogReq;
            if (downloadReq == null)
                throw new WatsonException("Unexpected type.");

            if (resp.Success)
            {
                try
                {
                    if (SaveFile != null)
                        SaveFile(downloadReq.DialogFileName, resp.Data);
                    else
                    {
#if !UNITY_WEBPLAYER
                        File.WriteAllBytes(downloadReq.DialogFileName, resp.Data);
#endif
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Dialog", "Caught exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((DownloadDialogReq)req).Callback != null)
                ((DownloadDialogReq)req).Callback(resp.Success);
        }
        #endregion

        #region UploadDialog
        /// <summary>
        /// This creates a new dialog from a local dialog file.
        /// </summary>
        /// <param name="dialogName">The name of the dialog.</param>
        /// <param name="callback">The callback to receive the dialog ID.</param>
        /// <param name="dialogFileName">The filename of the dialog file to upload.</param>
        /// <returns>Returns true if the upload was submitted.</returns>
        public bool UploadDialog(string dialogName, OnUploadDialog callback, string dialogFileName)
        {
            if (string.IsNullOrEmpty(dialogFileName))
                throw new ArgumentNullException("dialogFileName");

            byte[] dialogData = null;
            if (LoadFile != null)
            {
                dialogData = LoadFile(dialogFileName);
            }
            else
            {
#if !UNITY_WEBPLAYER
                dialogData = File.ReadAllBytes(dialogFileName);
#endif
            }

            if (dialogData == null)
            {
                Log.Error("Dialog", "Failed to load dialog file data {0}", dialogFileName);
                return false;
            }

            return UploadDialog(dialogName, callback, dialogData, Path.GetFileName(dialogFileName));
        }

        /// <summary>
        /// This uploads a new dialog to the service.
        /// </summary>
        /// <param name="dialogName">The name of the dialog.</param>
        /// <param name="callback">The callback to receive the dialog ID.</param>
        /// <param name="dialogData">The raw byte data of the dialog.</param>
        /// <param name="dataFileName">This must be the filename including the extension so the dialog service knows how to parse the data.</param>
        /// <returns>Returns true if the upload was submitted.</returns>
        public bool UploadDialog(string dialogName, OnUploadDialog callback, byte[] dialogData, string dataFileName)
        {
            if (string.IsNullOrEmpty(dialogName))
                throw new ArgumentNullException("dialogName");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (dialogData == null)
                throw new ArgumentNullException("dialogData");
            if (string.IsNullOrEmpty(dataFileName))
                throw new ArgumentNullException("dataFileName");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/dialogs");
            if (connector == null)
                return false;

            UploadDialogReq req = new UploadDialogReq();
            req.Callback = callback;
            req.OnResponse = OnCreateDialogResp;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["name"] = new RESTConnector.Form(dialogName);
            req.Forms["file"] = new RESTConnector.Form(dialogData, dataFileName);
            req.Timeout = 10 * 60.0f;       // increase timeout to 10 minutes

            return connector.Send(req);
        }

        private class UploadDialogReq : RESTConnector.Request
        {
            public OnUploadDialog Callback { get; set; }
        };
        private void OnCreateDialogResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string id = null;
            if (resp.Success)
            {
                try
                {
                    IDictionary json = Json.Deserialize(Encoding.UTF8.GetString(resp.Data)) as IDictionary;
                    id = (string)json["dialog_id"];
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "UploadDialog Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((UploadDialogReq)req).Callback != null)
                ((UploadDialogReq)req).Callback(id);
        }
        #endregion

        #region Delete Dialog
        /// <summary>
        /// Deletes a dialog by ID.
        /// </summary>
        /// <param name="dialogId">The ID of the dialog to delete.</param>
        /// <param name="callback">The callback to invoke on success or failure.</param>
        /// <returns>Returns true if request is sent.</returns>
        public bool DeleteDialog(string dialogId, OnDialogCallback callback)
        {
            if (string.IsNullOrEmpty(dialogId))
                throw new ArgumentNullException("dialogId");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/dialogs/" + dialogId);
            if (connector == null)
                return false;

            DeleteDialogReq req = new DeleteDialogReq();
            req.Callback = callback;
            req.OnResponse = OnDeleteDialogResp;
            req.Delete = true;

            return connector.Send(req);
        }

        private class DeleteDialogReq : RESTConnector.Request
        {
            public OnDialogCallback Callback { get; set; }
        };

        private void OnDeleteDialogResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((DeleteDialogReq)req).Callback != null)
                ((DeleteDialogReq)req).Callback(resp.Success);
        }
        #endregion

        #region Converse
        /// <summary>
        /// Converse with the dialog system.
        /// </summary>
        /// <param name="dialogId">The dialog ID of the dialog to use.</param>
        /// <param name="input">The text input.</param>
        /// <param name="callback">The callback for receiving the responses.</param>
        /// <param name="conversation_id">The conversation ID to use, if 0 then a new conversation will be started.</param>
        /// <param name="client_id">The client ID of the user.</param>
        /// <returns>Returns true if the request was submitted to the back-end.</returns>
        public bool Converse(string dialogId, string input, OnConverse callback,
            int conversation_id = 0, int client_id = 0)
        {
            if (string.IsNullOrEmpty(dialogId))
                throw new ArgumentNullException("dialogId");
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/dialogs");
            if (connector == null)
                return false;

            ConverseReq req = new ConverseReq();
            req.Function = "/" + dialogId + "/conversation";
            req.Callback = callback;
            req.OnResponse = ConverseResp;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["input"] = new RESTConnector.Form(input);
            if (conversation_id != 0)
                req.Forms["conversation_id"] = new RESTConnector.Form(conversation_id);
            if (client_id != 0)
                req.Forms["client_id"] = new RESTConnector.Form(client_id);

            return connector.Send(req);
        }
        private class ConverseReq : RESTConnector.Request
        {
            public OnConverse Callback { get; set; }
        };
        private void ConverseResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ConverseResponse response = new ConverseResponse();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = response;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "ConverseResp Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((ConverseReq)req).Callback != null)
                ((ConverseReq)req).Callback(resp.Success ? response : null);
        }
        #endregion

        #region IWatsonService interface
        /// <exclude />
        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
            {
                if (callback != null && callback.Target != null)
                {
                    callback(SERVICE_ID, false);
                }
            }
        }

        private class CheckServiceStatus
        {
            private Dialog m_Service = null;
            private ServiceStatus m_Callback = null;
            private int m_DialogCount = 0;

            public CheckServiceStatus(Dialog service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                string customServiceID = Config.Instance.GetVariableValue(SERVICE_ID + "_ID");

                //If custom classifierID is defined then we are using it to check the service health
                if (!string.IsNullOrEmpty(customServiceID))
                {

                    if (!m_Service.Converse(customServiceID, "Hello", OnDialog))
                        OnFailure("Failed to invoke Converse().");
                    else
                        m_DialogCount += 1;
                }
                else
                {
                    if (!m_Service.GetDialogs(OnGetDialogs))
                        OnFailure("Failed to invoke GetDialogs().");
                }


            }

            private void OnGetDialogs(Dialogs dialogs)
            {
                if (m_Callback != null)
                {
                    foreach (var dialog in dialogs.dialogs)
                    {
                        if (!m_Service.Converse(dialog.dialog_id, "Hello", OnDialog))
                            OnFailure("Failed to invoke Converse().");
                        else
                            m_DialogCount += 1;
                    }
                }
                else
                    OnFailure("GetDialogs() failed.");
            }

            private void OnDialog(ConverseResponse resp)
            {
                if (m_DialogCount > 0)
                {
                    m_DialogCount -= 1;
                    if (resp != null)
                    {
                        if (m_DialogCount == 0 && m_Callback != null && m_Callback.Target != null)
                            m_Callback(SERVICE_ID, true);
                    }
                    else
                        OnFailure("ConverseResponse is null.");
                }
            }

            private void OnFailure(string msg)
            {
                Log.Error("Dialog", msg);
                m_Callback(SERVICE_ID, false);
                m_DialogCount = 0;
            }
        };
        #endregion
    }
}
