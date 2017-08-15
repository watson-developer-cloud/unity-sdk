///**
//* Copyright 2015 IBM Corp. All Rights Reserved.
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*      http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License.
//*
//*/


//using IBM.Watson.DeveloperCloud.Logging;
//using IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v2;
//using IBM.Watson.DeveloperCloud.Utilities;
//using System.Collections;

//namespace IBM.Watson.DeveloperCloud.UnitTests
//{
//    public class TestLanguageTranslation : UnitTest
//    {
//        private LanguageTranslation m_Translate = new LanguageTranslation(new Credentials());
//        private bool m_GetModelTested = false;
//        private bool m_GetModelsTested = false;
//        private bool m_GetLanguagesTested = false;
//        private bool m_IdentifyTested = false;
//        private bool m_TranslateTested = false;

//        public override IEnumerator RunTest()
//        {
//            if (Config.Instance.FindCredentials(m_Translate.GetServiceID()) == null)
//                yield break;

//            m_Translate.GetModel("en-es", OnGetModel);
//            while (!m_GetModelTested)
//                yield return null;

//            m_Translate.GetModels(OnGetModels);
//            while (!m_GetModelsTested)
//                yield return null;

//            m_Translate.GetLanguages(OnGetLanguages);
//            while (!m_GetLanguagesTested)
//                yield return null;

//            m_Translate.Identify("What does the fox say?", OnIdentify);
//            while (!m_IdentifyTested)
//                yield return null;

//            m_Translate.GetTranslation("What does the fox say?", "en", "es", OnGetTranslation);
//            while (!m_TranslateTested)
//                yield return null;

//            yield break;
//        }

//        private void OnGetModel(TranslationModel model, string customData)
//        {
//            Test(model != null);
//            if (model != null)
//            {
//                Log.Status("TestTranslate", "ModelID: {0}, Source: {1}, Target: {2}, Domain: {3}",
//                    model.model_id, model.source, model.target, model.domain);
//            }
//            m_GetModelTested = true;
//        }

//        private void OnGetModels(TranslationModels models, string customData)
//        {
//            Test(models != null);
//            if (models != null)
//            {
//                foreach (var model in models.models)
//                {
//                    Log.Status("TestTranslate", "ModelID: {0}, Source: {1}, Target: {2}, Domain: {3}",
//                        model.model_id, model.source, model.target, model.domain);
//                }
//            }
//            m_GetModelsTested = true;
//        }

//        private void OnGetTranslation(Translations translation, string customData)
//        {
//            Test(translation != null);
//            if (translation != null && translation.translations.Length > 0)
//                Log.Status("TestTranslate", "Translation: {0}", translation.translations[TestCredentialIndex].translation);
//            m_TranslateTested = true;
//        }

//        private void OnIdentify(string lang, string customData)
//        {
//            Test(lang != null);
//            if (lang != null)
//                Log.Status("TestTranslate", "Identified Language as {0}", lang);
//            m_IdentifyTested = true;
//        }

//        private void OnGetLanguages(Languages languages, string customData)
//        {
//            Test(languages != null);
//            if (languages != null)
//            {
//                foreach (var lang in languages.languages)
//                    Log.Status("TestTranslate", "Language: {0}, Name: {1}", lang.language, lang.name);
//            }

//            m_GetLanguagesTested = true;
//        }
//    }
//}
