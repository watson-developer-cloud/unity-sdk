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
using System.Text;
using IBM.Watson.DeveloperCloud.Services.ESRI.v1;

namespace IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1
{
    
    [fsObject]
    public class EntityExtractionData
    {
        public string status { get; set; }
        public string language { get; set; }
        public string url { get; set; }
        public string text { get; set; }
        public Entity[] entities { get; set; }

        public bool HasData
        {
            get
            {
                return entities != null && entities.Length > 0;
            }
        }

        public bool HasGeographicInformation
        {
            get
            {
                string geoString = null;
                for (int i = 0; entities != null && i < entities.Length; i++)
                {
                    if (entities[i].disambiguated != null)
                    {
                        geoString = entities[i].disambiguated.geo;
                        if (!string.IsNullOrEmpty(geoString))
                            break;
                    }
                }
                return !string.IsNullOrEmpty(geoString);
            }
        }

        private PositionOnMap _GeoLocation = null;
        public PositionOnMap GeoLocation
        {
            get
            {
                if (_GeoLocation == null)
                {
                    string geoString = null;
                    for (int i = 0; entities != null && i < entities.Length; i++)
                    {
                        if (entities[i].disambiguated != null)
                        {
                            geoString = entities[i].disambiguated.geo;
                            if (!string.IsNullOrEmpty(geoString))
                            {
                                string[] geoValues = geoString.Split(' ');
                                if (geoValues != null && geoValues.Length == 2)
                                {
                                    double latitute = 0;
                                    double longitutde = 0;

                                    if (double.TryParse(geoValues[0], out latitute) && double.TryParse(geoValues[1], out longitutde))
                                    {
                                        _GeoLocation = new PositionOnMap(latitute, longitutde, entities[i].disambiguated.name);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return _GeoLocation;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int indexEntity = 0; entities != null && indexEntity < entities.Length; indexEntity++)
            {
                stringBuilder.Append("\n\t");
                stringBuilder.Append(entities[indexEntity].ToString());
            }
            return string.Format("[EntityExtractionData: status={0}, language={1}, url={2}, text={3}, entities={4}]", status, language, url, text, stringBuilder.ToString());
        }

    };

    [fsObject]
    public class Entity
    {
        public string type { get; set; }
        public string relevance { get; set; }
        public KnowledgeGraph knowledgeGraph{ get; set; }
        public string count { get; set; }
        public string text { get; set; }
        public Disambiguated disambiguated { get; set; }
        public Quotation[] quotations { get; set; }
        public Sentiment sentiment{ get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int indexQuatation = 0; quotations != null && indexQuatation < quotations.Length; indexQuatation++)
            {
                stringBuilder.Append("\n\t\t");
                stringBuilder.Append(quotations[indexQuatation].ToString());
            }

            return string.Format("[Entity: \ttype={0}, \trelevance={1}, \tknowledgeGraph={2}, \tcount={3}, \ttext={4}, \tdisambiguated={5}, \tquotations={6}, \tsentiment={7}]", type, relevance, knowledgeGraph, count, text, disambiguated, stringBuilder.ToString(), sentiment);
        }
    };

    [fsObject]
    public class KnowledgeGraph
    {
        public string typeHierarchy { get; set; }

        public override string ToString()
        {
            return string.Format("[KnowledgeGraph: typeHierarchy={0}]", typeHierarchy);
        }
    };

    [fsObject]
    public class Disambiguated
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

        public override string ToString()
        {
            return string.Format("[Disambiguated: name={0}, subType={1}, website={2}, geo={3}, dbpedia={4}, yago={5}, opencyc={6}, umbel={7}, freebase={8}, ciaFactbook={9}, census={10}, geonames={11}, musicBrainz={12}, crunchbase={13}]", name, subType, website, geo, dbpedia, yago, opencyc, umbel, freebase, ciaFactbook, census, geonames, musicBrainz, crunchbase);
        }
    };

    [fsObject]
    public class Quotation
    {
        public string quotation { get; set; }

        public override string ToString()
        {
            return string.Format("[Quotation: quotation={0}]", quotation);
        }
    };

    [fsObject]
    public class Sentiment
    {
        public string type { get; set; }
        public string score { get; set; }
        public string mixed { get; set; }

        public override string ToString()
        {
            return string.Format("[Sentiment: type={0}, score={1}, mixed={2}]", type, score, mixed);
        }
    };
}
