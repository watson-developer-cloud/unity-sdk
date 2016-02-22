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

using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1
{
    
    [fsObject]
    public class EntityExtractionData
    {
        public string status { get; set; }
        public string language { get; set; }
        public string url { get; set; }
        public string text { get; set; }
        public EntityExtractionEntity[] entities { get; set; }

    };

    [fsObject]
    public class EntityExtractionEntity
    {
        public string type { get; set; }
        public string relevance { get; set; }
        public EntityExtractionEntityKnowledgeGraph knowledgeGraph{ get; set; }
        public string count { get; set; }
        public string text { get; set; }
        public EntityExtractionEntityDisambiguated disambiguated { get; set; }
        public EntityExtractionEntityQuotation[] quotations { get; set; }
        public EntityExtractionEntitySentiment sentiment{ get; set; }

    };

    [fsObject]
    public class EntityExtractionEntityKnowledgeGraph
    {
        public string typeHierarchy { get; set; }
    };

    [fsObject]
    public class EntityExtractionEntityDisambiguated
    {
        public string name { get; set; }
        public string subType { get; set; }
        public string website { get; set; }
        public string geo { get; set; }
        public string dbpedia { get; set; }
        public string yago { get; set; }
        public string opencyc { get; set; }
        public string umbel { get; set; }
        public string freebase { get; set; }
        public string ciaFactbook { get; set; }
        public string census { get; set; }
        public string geonames { get; set; }
        public string musicBrainz { get; set; }
        public string crunchbase { get; set; }
    };

    [fsObject]
    public class EntityExtractionEntityQuotation
    {
        public string quotation { get; set; }
    };

    [fsObject]
    public class EntityExtractionEntitySentiment
    {
        public string type { get; set; }
        public string score { get; set; }
        public string mixed { get; set; }
    };
}
