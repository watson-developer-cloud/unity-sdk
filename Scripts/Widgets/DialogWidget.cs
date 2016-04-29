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

#pragma warning disable 414

using UnityEngine;
using System;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.Dialog.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.DataTypes;

namespace IBM.Watson.DeveloperCloud.Widgets
{
    /// <summary>
    /// This Widget class wraps the Dialog service. It will look for a given dialog by name, if not found then it can auto-upload the dialog
    /// </summary>
    public class DialogWidget : Widget
    {
        #region Inputs
        [SerializeField]
        private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
        #endregion

        #region Outputs
        [SerializeField]
        private Output m_ResultOutput = new Output(typeof(TextToSpeechData));
        #endregion

        #region Private Data
        [SerializeField, Tooltip("The name prefix of the dialog to use.")]
        private string m_DialogName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 24);      // NOTE: the limit of a dialog name is 24 characters, plus it has to be globally unique!
        [SerializeField, Tooltip("If no dialog is found by name, then this dialog will automatically be uploaded. (Editor Only)")]
        private string m_AutoUploadDialog = "/Watson/Editor/TestData/pizza_sample.xml";
        [SerializeField]
        private VerticalLayoutGroup m_DialogLayout = null;
        [SerializeField]
        private ScrollRect m_ScrollRect = null;
        [SerializeField]
        private GameObject m_QuestionPrefab = null;
        [SerializeField]
        private GameObject m_AnswerPrefab = null;
        [SerializeField]
        private int m_HistoryCount = 50;

        private Dialog m_Dialog = new Dialog();
        private string m_DialogID = null;
        private int m_ClientID = 0;
        private int m_ConversationID = 0;
        #endregion

        #region Widget Interface
        /// <exclude />
        protected override string GetName()
        {
            return "Dialog";
        }

        #endregion

        #region Public Functions
        /// <summary>
        /// Converse with the dialog system.
        /// </summary>
        /// <param name="dialog">Text to send to the dialog system.</param>
        public void Converse(string dialog)
        {
            if (!string.IsNullOrEmpty(m_DialogID))
                m_Dialog.Converse(m_DialogID, dialog, OnConverse, m_ConversationID, m_ClientID);
            else
                Log.Warning("DialogWidget", "m_DialogID is null.");
        }
        #endregion

        #region Event Handlers
        private void OnEnable()
        {
            m_Dialog.GetDialogs(OnGetDialogs);
        }

        private void OnConverse(ConverseResponse resp)
        {
            if (resp != null)
            {
                m_ClientID = resp.client_id;
                m_ConversationID = resp.conversation_id;

                foreach (var r in resp.response)
                {
                    Log.Debug("DialogWidget", "Response: {0}", r);
                    AddDialog(r, m_AnswerPrefab);

                    if (m_ResultOutput.IsConnected)
                        m_ResultOutput.SendData(new TextToSpeechData(r));
                }
            }
        }

        private void OnDialogUploaded(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Log.Debug("DialogDisplayWidget", "Dialog ID: {0}", id);
                m_DialogID = id;
            }
        }

        private void OnGetDialogs(Dialogs dialogs)
        {
            if (dialogs != null && dialogs.dialogs != null)
            {
                foreach (var d in dialogs.dialogs)
                {
                    Log.Debug("DialogDisplayWidget", "Name: {0}, ID: {1}", d.name, d.dialog_id);
                    if (d.name == m_DialogName)
                        m_DialogID = d.dialog_id;
                }
            }

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(m_DialogID))
            {
                m_Dialog.UploadDialog(m_DialogName, OnDialogUploaded,
                    Application.dataPath + m_AutoUploadDialog);
            }
#endif
        }

        private void OnSpeechInput(Data data)
        {
            SpeechResultList result = ((SpeechToTextData)data).Results;
            if (result != null && result.HasFinalResult())
            {
                string text = result.Results[0].Alternatives[0].Transcript;

                Converse(text);
                AddDialog(text, m_QuestionPrefab);
            }
        }
        #endregion

        #region Private Functions
        private void AddDialog(string add, GameObject prefab)
        {
            if (m_DialogLayout != null)
            {
                if (prefab == null)
                    throw new ArgumentNullException("prefab is null");

                int newLine = add.IndexOf('\n');
                if (newLine > 0)
                    add = add.Substring(0, newLine);

                GameObject textObject = Instantiate(prefab) as GameObject;
                textObject.GetComponentInChildren<Text>().text = Utility.RemoveTags(add);
                textObject.transform.SetParent(m_DialogLayout.transform, false);

                while (m_DialogLayout.transform.childCount > m_HistoryCount)
                    DestroyImmediate(m_DialogLayout.transform.GetChild(0).gameObject);

                Invoke("ScrollToEnd", 0.5f);
            }
        }

        private void ScrollToEnd()
        {
            if (m_ScrollRect != null)
                m_ScrollRect.verticalNormalizedPosition = 0.0f;
        }
        #endregion
    }
}