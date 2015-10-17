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

using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;

namespace IBM.Watson.UnitTests
{
    public class TestITM : UnitTest
    {
        ITM m_ITM = new ITM();
        bool m_GetPipelinesTested = false;
        bool m_ParseTested = false;

        public override IEnumerator RunTest()
        {
            m_ITM.GetPipelines( OnGetPipelines );
            while(! m_GetPipelinesTested )
                yield return null;

            m_ITM.GetParseData( -1773927182, OnGetParseData );
            while(! m_ParseTested )
                yield return null;

            yield break;
        }

        private void OnGetPipelines( ITM.Pipeline [] pipelines )
        {
            Test( pipelines != null );
            for(int i=0;i<pipelines.Length;++i)
                Log.Status( "TestITM", "Pipeline: {0}", pipelines[i].Label );

            m_GetPipelinesTested = true;
        }
        private void OnGetParseData( ITM.ParseData parse)
        {
            Test ( parse != null );
            m_ParseTested = true;
        }
    }
}

