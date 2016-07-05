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

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System;

namespace IBM.Watson.DeveloperCloud.Services.RetriveAndRank.v1
{
    public class RetriveAndRank : IWatsonService
    {
        #region GetClusters
        /// <summary>
        /// Gets a list of all Solr clusters.
        /// </summary>
        #endregion

        #region CreateClusters
        #endregion

        #region DeleteClusters
        #endregion

        #region GetClusterInfo
        #endregion

        #region ListClusterConfig
        #endregion

        #region DeleteClusterConfig
        #endregion

        #region GetClusterConfig
        #endregion

        #region UploadClusterConfig
        #endregion

        #region CollectionRequest
        #endregion

        #region IndexDocuments
        #endregion

        #region Search
        #endregion

        #region RankedSearch
        #endregion

        #region GetRankers
        #endregion

        #region CreateRanker
        #endregion

        #region Rank
        #endregion

        #region DeleteRanker
        #endregion

        #region GetRankerInfo
        #endregion

        #region IWatsonService Interface
        public string GetServiceID()
        {
            throw new NotImplementedException();
        }

        public void GetServiceStatus(ServiceStatus callback)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
