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

//#define TEST_DELETE

using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.DataModels;
using System.IO;
using UnityEngine;
using System;

namespace IBM.Watson.UnitTests
{
    public class TestNLC : UnitTest
    {
        NLC m_NLC = new NLC();
        bool m_FindClassifierTested = false;
        bool m_TrainClasifierTested = false;
        bool m_TrainClassifier = false;
        bool m_DeleteTested = false;
        string m_ClassifierId = null;
        bool m_ClassifyTested = false;

        public override IEnumerator RunTest()
        {
            m_NLC.FindClassifier( "TestNLC/", OnFindClassifier );
            while(! m_FindClassifierTested )
                yield return null;

            if ( m_TrainClassifier )
            {
                string trainingData = File.ReadAllText( Application.dataPath + "/Watson/Editor/TestData/weather_data_train.csv" );

                Test( m_NLC.TrainClassifier( "TestNLC/" + DateTime.Now.ToString(), "en", trainingData, OnTrainClassifier ) );
                while( !m_TrainClasifierTested )
                    yield return null;
            }
            else if ( !string.IsNullOrEmpty( m_ClassifierId ) )
            {
                Test( m_NLC.Classify( m_ClassifierId, "Is it hot outside", OnClassify ) );
                while(! m_ClassifyTested )
                    yield return null;
            }

#if TEST_DELETE
            if ( !string.IsNullOrEmpty( m_ClassifierId ) )
            {
                Test( m_NLC.DeleteClassifer( m_ClassifierId, OnDeleteClassifier ) );
                while(! m_DeleteTested ) 
                    yield return null;
            }
#endif

            yield break;
        }

        private void OnDeleteClassifier( bool success )
        {
            Test( success );
            m_DeleteTested = true;
        }

        private void OnFindClassifier( Classifier find )
        {
            if ( find != null )
            {
                Log.Status( "TestNLC", "Find Result, Classifier ID: {0}, Status: {1}", find.classifier_id, find.status );

                m_TrainClassifier = false;
                if ( find.status == "Available" )
                    m_ClassifierId = find.classifier_id;
            }   
            else
            {
                m_TrainClassifier = true;
            }
            m_FindClassifierTested = true;         
        }

        private void OnClassify( ClassifyResult result )
        {
            Test( result != null );
            if ( result != null )
            {
                Log.Status( "TestNLC", "Classify Result: {0}", result.top_class );
                Test( result.top_class == "temperature" );
            }
            m_ClassifyTested = true;
        }

        private void OnTrainClassifier( Classifier classifier )
        {
            Test( classifier != null );
            if ( classifier != null )
                Log.Status( "TestNLC", "Classifier ID: {0}, Status: {1}", classifier.classifier_id, classifier.status );

            m_TrainClasifierTested = true;
        }

    }
}

