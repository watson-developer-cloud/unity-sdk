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

        private string TRAINING_DATA = "How hot is it today?,temperature\nIs it hot outside?,temperature\nWill it be uncomfortably hot?,temperature\nWill it be sweltering?,temperature\nHow cold is it today?,temperature\nIs it cold outside?,temperature\nWill it be uncomfortably cold?,temperature\nWill it be frigid?,temperature\nWhat is the expected high for today?,temperature\nWhat is the expected temperature?,temperature\nWill high temperatures be dangerous?,temperature\nIs it dangerously cold?,temperature\nWhen will the heat subside?,temperature\nIs it hot?,temperature\nIs it cold?,temperature\nHow cold is it now?,temperature\nWill we have a cold day today?,temperature\nWhen will the cold subside?,temperature\nWhat highs are we expecting?,temperature\nWhat lows are we expecting?,temperature\nIs it warm?,temperature\nIs it chilly?,temperature\nWhat\'s the current temp in Celsius?,temperature\nWhat is the temperature in Fahrenheit?,temperature\nIs it windy?,conditions\nWill it rain today?,conditions\nWhat are the chances for rain?,conditions\nWill we get snow?,conditions\nAre we expecting sunny conditions?,conditions\nIs it overcast?,conditions\nWill it be cloudy?,conditions\nHow much rain will fall today?,conditions\nHow much snow are we expecting?,conditions\nIs it windy outside?,conditions\nHow much snow do we expect?,conditions\nIs the forecast calling for snow today?,conditions\nWill we see some sun?,conditions\nWhen will the rain subside?,conditions\nIs it cloudy?,conditions\nIs it sunny now?,conditions\nWill it rain?,conditions\nWill we have much snow?,conditions\nAre the winds dangerous?,conditions\nWhat is the expected snowfall today?,conditions\nWill it be dry?,conditions\nWill it be breezy?,conditions\nWill it be humid?,conditions\nWhat is today\'s expected humidity?,conditions\nWill the blizzard hit us?,conditions\nIs it drizzling?,conditions";

        public override IEnumerator RunTest()
        {
            m_NLC.FindClassifier( "TestNLC", OnFindClassifier );
            while(! m_FindClassifierTested )
                yield return null;

            if ( m_TrainClassifier )
            {
                Test( m_NLC.TrainClassifier( "TestNLC", "en", TRAINING_DATA, OnTrainClassifier ) );
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

