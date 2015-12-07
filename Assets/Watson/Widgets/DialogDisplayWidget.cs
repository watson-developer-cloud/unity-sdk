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

namespace IBM.Watson.Widgets
{
    using UnityEngine;
    using System;
    using UnityEngine.UI;
    using IBM.Watson.Logging;
    using IBM.Watson.Utilities;
    using IBM.Watson.DataModels;
    using IBM.Watson.Services.v1;
    using IBM.Watson.DataTypes;

    /// <summary>
    /// Dialog widget.
    /// </summary>
    public class DialogDisplayWidget : Widget
    {
        [SerializeField]
        private VerticalLayoutGroup m_DialogLayout = null;
        [SerializeField]
        private ScrollRect m_ScrollRect = null;
        [SerializeField]
        private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
        [SerializeField]
        private Output m_ResultOutput = new Output(typeof(TextToSpeechData));
        [SerializeField]
        private GameObject m_QuestionPrefab;
        [SerializeField]
        private GameObject m_AnswerPrefab;
        [SerializeField]
        private int m_HistoryCount = 50;

        private Dialog m_Dialog = new Dialog();
        private string m_DialogID = null;
        private int m_ClientID = 0;
        private int m_ConversationID = 0;
        private bool isDialogAvailable = false;
        const string DIALOG_NAME = "ut_20151029_5";

        #region implemented abstract members of Widget

        protected override string GetName()
        {
            return "Dialog";
        }

        #endregion


        void OnEnable()
        {
            m_Dialog.GetDialogs(OnGetDialogs);
        }

        /// <summary>
        /// Converse the specified dialog.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        public void Converse(string dialog)
        {
            if (!string.IsNullOrEmpty(m_DialogID))
            {
                m_Dialog.Converse(m_DialogID, dialog, OnConverse, m_ConversationID, m_ClientID);
            }
        }

        /// <summary>
        /// Callback for conversing with Watson.
        /// </summary>
        /// <param name="resp">Resp.</param>
        private void OnConverse(ConverseResponse resp)
        {
            if (resp != null)
            {
                m_ClientID = resp.client_id;
                m_ConversationID = resp.conversation_id;

                foreach (var r in resp.response)
                {
                    Log.Debug("DialogDisplayWidget", "Response: {0}", r);
                    AddDialog(r, m_AnswerPrefab);
                    m_ResultOutput.SendData(new TextToSpeechData(r));
                }
            }
        }

        /// <summary>
        /// Callback for uploading dialog.
        /// </summary>
        /// <param name="id">Identifier.</param>
        private void OnDialogUploaded(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Log.Debug("DialogDisplayWidget", "Dialog ID: {0}", id);
                m_DialogID = id;
                Converse(" ");
            }
        }

        /// <summary>
        /// Callback for getting dialog from server.
        /// </summary>
        /// <param name="dialogs">Dialogs.</param>
        private void OnGetDialogs(Dialogs dialogs)
        {
            if (dialogs != null && dialogs.dialogs != null)
            {
                foreach (var d in dialogs.dialogs)
                {
                    Log.Debug("DialogDisplayWidget", "Name: {0}, ID: {1}", d.name, d.dialog_id);
                    if (d.name == DIALOG_NAME)
                    {
                        isDialogAvailable = true;
                        m_DialogID = d.dialog_id;
                        Converse(" ");
                    }
                }
            }

            if (!isDialogAvailable)
                m_Dialog.UploadDialog(DIALOG_NAME, OnDialogUploaded, Application.dataPath + "/Watson/Editor/TestData/pizza_sample.xml");
        }

        /// <summary>
        /// Callback for speech input.
        /// </summary>
        /// <param name="data">Data.</param>
        private void OnSpeechInput(Data data)
        {
            SpeechResultList result = ((SpeechToTextData)data).Results;
            if (result != null && result.Results.Length > 0)
            {
                foreach (SpeechResult res in result.Results)
                {
                    foreach (SpeechAlt alt in res.Alternatives)
                    {
                        if (res.Final)
                        {
                            string text = alt.Transcript;
                            Converse(text);
                            AddDialog(text, m_QuestionPrefab);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds lines of dialog to the window.
        /// </summary>
        /// <param name="add">Add.</param>
        /// <param name="prefab">Prefab.</param>
        private void AddDialog(string add, GameObject prefab)
        {
            if (m_DialogLayout == null)
                throw new WatsonException("m_DialogLayout is null.");
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

        /// <summary>
        /// Scrolls to end of dialog window.
        /// </summary>
        private void ScrollToEnd()
        {
            if (m_ScrollRect != null)
                m_ScrollRect.verticalNormalizedPosition = 0.0f;
        }
    }
}