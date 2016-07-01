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
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1
{
    #region Combined Call
    /// <summary>
    /// Combined call data.
    /// </summary>
    [fsObject]
    public class CombinedCallData
    { 
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string title { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public string image { get; set; }

        /// <summary>
        /// Gets or sets the image keywords.
        /// </summary>
        /// <value>The image keywords.</value>
        public ImageKeyword[] imageKeywords { get; set; }

        /// <summary>
        /// Gets or sets the publication date.
        /// </summary>
        /// <value>The publication date.</value>
        public PublicationDate publicationDate { get; set; }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        /// <value>The authors.</value>
        public Authors authors { get; set; }

        /// <summary>
        /// Gets or sets the document sentiment.
        /// </summary>
        /// <value>The document sentiment.</value>
        public DocSentiment docSentiment { get; set; }

        /// <summary>
        /// Gets or sets the feeds.
        /// </summary>
        /// <value>The feeds.</value>
        public Feed[]feeds { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public Keyword[] keywords { get; set; }

        /// <summary>
        /// Gets or sets the concepts.
        /// </summary>
        /// <value>The concepts.</value>
        public Concept[] concepts { get; set; }

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public Entity[] entities { get; set; }

        /// <summary>
        /// Gets or sets the relations.
        /// </summary>
        /// <value>The relations.</value>
        public Relation[] relations { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy.
        /// </summary>
        /// <value>The taxonomy.</value>
        public Taxonomy[] taxonomy { get; set; }

        /// <summary>
        /// Gets or sets the dates.
        /// </summary>
        /// <value>The dates.</value>
        public Date[] dates { get; set; }

        /// <summary>
        /// Gets or sets the document emotions.
        /// </summary>
        /// <value>The document emotions.</value>
        public DocEmotions[] docEmotions { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has data.
        /// </summary>
        /// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
        public bool HasData
        {
            get
            {
                return EntityCombined != null && EntityCombined.Count > 0;
            }
        }

        private List<string> _EntityCombined = null;
        /// <summary>
        /// The entity combined.
        /// </summary>
        public List<string> EntityCombined
        {
            get
            {
                if (_EntityCombined == null)
                {
                    _EntityCombined = new List<string>();

                    for (int i = 0; keywords != null && i < keywords.Length; i++)
                    {
                        if (!_EntityCombined.Contains(keywords[i].text))
                            _EntityCombined.Add(keywords[i].text);
                    }

                    for (int i = 0; entities != null && i < entities.Length; i++)
                    {
                        if (!_EntityCombined.Contains(entities[i].text))
                            _EntityCombined.Add(entities[i].text);
                    }
                }

                return _EntityCombined;
            }
        }

        /// <summary>
        /// Gets the entity combined comma seperated.
        /// </summary>
        /// <value>The entity combined comma seperated.</value>
        public string EntityCombinedCommaSeperated
        {
            get
            {
                if (EntityCombined.Count > 0)
                    return string.Join(",", EntityCombined.ToArray());
                return "";
            }
        }

        /// <summary>
        /// Tos the long string.
        /// </summary>
        /// <returns>The long string.</returns>
        public string ToLongString()
        {
            StringBuilder stringBuilder = new StringBuilder(string.Format("[CombinedCallData: status={0}, language={1}, text={2}", status, language, text));

            stringBuilder.Append(EntityCombinedCommaSeperated);
            for (int i = 0; dates != null && i < dates.Length; i++)
            {
                stringBuilder.Append(" Date: " + dates[i].DateValue.ToString());
            }

            return stringBuilder.ToString();
        }

    };

    /// <summary>
    /// Image keyword.
    /// </summary>
    [fsObject]
    public class ImageKeyword
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public string score { get; set; }
    }
    #endregion

    #region GetAuthors
    /// <summary>
    /// Authors data.
    /// </summary>
    [fsObject]
    public class AuthorsData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        /// <value>The authors.</value>
        public Authors authors { get; set; }
    }

    /// <summary>
    /// Authors.
    /// </summary>
    [fsObject]
    public class Authors
    {
        /// <summary>
        /// Gets or sets the confident.
        /// </summary>
        /// <value>The confident.</value>
        public string confident { get; set; }

        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>The names.</value>
        public string[] names { get; set; }
    }
    #endregion

    #region GetRankedConcepts
    /// <summary>
    /// Concepts data.
    /// </summary>
    [fsObject]
    public class ConceptsData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the concepts.
        /// </summary>
        /// <value>The concepts.</value>
        public Concept[] concepts { get; set; }
    }

    /// <summary>
    /// Concept.
    /// </summary>
    [fsObject]
    public class Concept
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the relevance.
        /// </summary>
        /// <value>The relevance.</value>
        public string relevance { get; set; }

        /// <summary>
        /// Gets or sets the knowledge graph.
        /// </summary>
        /// <value>The knowledge graph.</value>
        public KnowledgeGraph knowledgeGraph { get; set; }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        /// <value>The website.</value>
        public string website { get; set; }

        /// <summary>
        /// Gets or sets the geo.
        /// </summary>
        /// <value>The geo.</value>
        public string geo { get; set; }

        /// <summary>
        /// Gets or sets the dbpedia.
        /// </summary>
        /// <value>The dbpedia.</value>
        public string dbpedia { get; set; }

        /// <summary>
        /// Gets or sets the freebase.
        /// </summary>
        /// <value>The freebase.</value>
        public string freebase { get; set; }

        /// <summary>
        /// Gets or sets the yago.
        /// </summary>
        /// <value>The yago.</value>
        public string yago { get; set; }

        /// <summary>
        /// Gets or sets the opencyc.
        /// </summary>
        /// <value>The opencyc.</value>
        public string opencyc { get; set; }

        /// <summary>
        /// Gets or sets the cia factbook.
        /// </summary>
        /// <value>The cia factbook.</value>
        public string ciaFactbook { get; set; }

        /// <summary>
        /// Gets or sets the census.
        /// </summary>
        /// <value>The census.</value>
        public string census { get; set; }

        /// <summary>
        /// Gets or sets the geonames.
        /// </summary>
        /// <value>The geonames.</value>
        public string geonames { get; set; }

        /// <summary>
        /// Gets or sets the music brainz.
        /// </summary>
        /// <value>The music brainz.</value>
        public string musicBrainz { get; set; }

        /// <summary>
        /// Gets or sets the crunchbase.
        /// </summary>
        /// <value>The crunchbase.</value>
        public string crunchbase { get; set; }
    };
    #endregion

    #region ExtractDates
    /// <summary>
    /// Date data.
    /// </summary>
    [fsObject]
    public class DateData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the dates.
        /// </summary>
        /// <value>The dates.</value>
        public Date[] dates { get; set; }
    }

    /// <summary>
    /// Date.
    /// </summary>
    [fsObject]
    public class Date
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public string date { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        private System.DateTime m_dateValue = default(System.DateTime);

        /// <summary>
        /// Gets the date value.
        /// </summary>
        /// <value>The date value.</value>
        public System.DateTime DateValue
        {
            get
            {
                if (m_dateValue == default(System.DateTime) && !string.IsNullOrEmpty(date) && date.Length > 8)
                {
                    //19840101T000000
                    System.DateTime.TryParseExact(date.Remove(8),
                        "yyyyddMM",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out m_dateValue);

                }
                return m_dateValue;
            }
        }
    };
    #endregion

    #region GetEmotion
    /// <summary>
    /// Emotion data.
    /// </summary>
    [fsObject]
    public class EmotionData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the document emotions.
        /// </summary>
        /// <value>The document emotions.</value>
        public DocEmotions docEmotions { get; set; }
    }

    /// <summary>
    /// Document emotions.
    /// </summary>
    [fsObject]
    public class DocEmotions
    {
        /// <summary>
        /// Gets or sets the anger.
        /// </summary>
        /// <value>The anger.</value>
        public string anger { get; set; }

        /// <summary>
        /// Gets or sets the disgust.
        /// </summary>
        /// <value>The disgust.</value>
        public string disgust { get; set; }

        /// <summary>
        /// Gets or sets the fear.
        /// </summary>
        /// <value>The fear.</value>
        public string fear { get; set; }

        /// <summary>
        /// Gets or sets the joy.
        /// </summary>
        /// <value>The joy.</value>
        public string joy { get; set; }

        /// <summary>
        /// Gets or sets the sadness.
        /// </summary>
        /// <value>The sadness.</value>
        public string sadness { get; set; }
    };
    #endregion

    #region GetRankedNamedEntities
    /// <summary>
    /// Entity data.
    /// </summary>
    [fsObject]
    public class EntityData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public Entity[] entities { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has data.
        /// </summary>
        /// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
        public bool HasData
        {
            get
            {
                return entities != null && entities.Length > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has geographic information.
        /// </summary>
        /// <value><c>true</c> if this instance has geographic information; otherwise, <c>false</c>.</value>
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

        private PositionOnMap m_GeoLocation = null;
        /// <summary>
        /// Gets the geo location.
        /// </summary>
        /// <value>The geo location.</value>
        public PositionOnMap GeoLocation
        {
            get
            {
                if (m_GeoLocation == null)
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
                                        m_GeoLocation = new PositionOnMap(latitute, longitutde, entities[i].disambiguated.name);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return m_GeoLocation;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.EntityData"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.EntityData"/>.</returns>
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

    /// <summary>
    /// Entity.
    /// </summary>
    [fsObject]
    public class Entity
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the relevance.
        /// </summary>
        /// <value>The relevance.</value>
        public string relevance { get; set; }

        /// <summary>
        /// Gets or sets the knowledge graph.
        /// </summary>
        /// <value>The knowledge graph.</value>
        public KnowledgeGraph knowledgeGraph { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public string count { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the disambiguated.
        /// </summary>
        /// <value>The disambiguated.</value>
        public Disambiguated disambiguated { get; set; }

        /// <summary>
        /// Gets or sets the quotations.
        /// </summary>
        /// <value>The quotations.</value>
        public Quotation[] quotations { get; set; }

        /// <summary>
        /// Gets or sets the sentiment.
        /// </summary>
        /// <value>The sentiment.</value>
        public DocSentiment sentiment { get; set; }

        private EntityPrimaryType _EntityType = EntityPrimaryType.NONE;
        /// <summary>
        /// The type of the entity.
        /// </summary>
        public EntityPrimaryType EntityType
        {
            get
            {
                if (_EntityType == EntityPrimaryType.NONE && !string.IsNullOrEmpty(type))
                {
                    for (int i = (int)EntityPrimaryType.NONE; i < (int)EntityPrimaryType.NAN; i++)
                    {
                        if (string.Compare(type, ((EntityPrimaryType)i).ToString()) == 0)
                        {
                            _EntityType = ((EntityPrimaryType)i);
                            break;
                        }
                    }
                }
                return _EntityType;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.Entity"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.Entity"/>.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int indexQuatation = 0; quotations != null && indexQuatation < quotations.Length; indexQuatation++)
            {
                stringBuilder.Append("\n\t");
                stringBuilder.Append(quotations[indexQuatation].ToString());
            }

            return string.Format("[Entity: type={0} - EntityType={8}, relevance={1}, knowledgeGraph={2}, count={3}, text={4}, disambiguated={5}, quotations={6}, sentiment={7}]", type, relevance, knowledgeGraph, count, text, disambiguated, stringBuilder.ToString(), sentiment, EntityType);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has geographic information.
        /// </summary>
        /// <value><c>true</c> if this instance has geographic information; otherwise, <c>false</c>.</value>
        public bool HasGeographicInformation
        {
            get
            {
                string geoString = null;
                if (disambiguated != null)
                {
                    geoString = disambiguated.geo;
                }
                return !string.IsNullOrEmpty(geoString);
            }
        }

        private PositionOnMap _GeoLocation = null;
        /// <summary>
        /// Gets the geo location.
        /// </summary>
        /// <value>The geo location.</value>
        public PositionOnMap GeoLocation
        {
            get
            {
                if (_GeoLocation == null)
                {
                    string geoString = null;
                    if (disambiguated != null)
                    {
                        geoString = disambiguated.geo;
                        if (!string.IsNullOrEmpty(geoString))
                        {
                            string[] geoValues = geoString.Split(' ');
                            if (geoValues != null && geoValues.Length == 2)
                            {
                                double latitute = 0;
                                double longitutde = 0;

                                if (double.TryParse(geoValues[0], out latitute) && double.TryParse(geoValues[1], out longitutde))
                                {
                                    _GeoLocation = new PositionOnMap(latitute, longitutde, disambiguated.name);
                                }
                            }
                        }
                    }
                }
                return _GeoLocation;
            }
        }
    };

    /// <summary>
    /// Position on map.
    /// </summary>
    public class PositionOnMap
    {
        /// <summary>
        /// The name of the position.
        /// </summary>
        public string PositionName;

        /// <summary>
        /// The latitude.
        /// </summary>
        public double Latitude;    //Y : North - south

        /// <summary>
        /// The longitude.
        /// </summary>
        public double Longitude;   //X : West - East

        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <value>The x.</value>
        public double X { get { return Longitude; } }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>The y.</value>
        public double Y { get { return Latitude; } }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.PositionOnMap"/> class.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        public PositionOnMap(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.PositionOnMap"/> class.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <param name="positionName">Position name.</param>
        public PositionOnMap(double latitude, double longitude, string positionName)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.PositionName = positionName;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.PositionOnMap"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.PositionOnMap"/>.</returns>
        public override string ToString()
        {
            return string.Format("[PositionOnMap: Name: {0}, Latitude:{1}, Longitude:{2}]", PositionName, Latitude.ToString(), Longitude.ToString());
        }
    }
    #endregion

    #region EntityTypes
    /// <summary>
    /// Entity primary type.
    /// </summary>
    public enum EntityPrimaryType
    {
        NONE = -1,
        Anatomy,
        Anniversary,
        Automobile,
        City,
        Company,
        Continent,
        Country,
        Crime,
        Degree,
        Drug,
        EntertainmentAward,
        Facility,
        FieldTerminology,
        FinancialMarketIndex,
        GeographicFeature,
        HealthCondition,
        Holiday,
        JobTitle,
        Movie,
        MusicGroup,
        NaturalDisaster,
        OperatingSystem,
        Organization,
        Person,
        PrintMedia,
        Product,
        ProfessionalDegree,
        RadioProgram,
        RadioStation,
        Region,
        Sport,
        SportingEvent,
        StateOrCounty,
        Technology,
        TelevisionShow,
        TelevisionStation,
        EmailAddress,
        TwitterHandle,
        Hashtag,
        IPAddress,
        Quantity,
        Money,
        NAN //At the end
    }

    /// <summary>
    /// Entity sub type.
    /// </summary>
    public enum EntitySubType
    {
        AdministrativeDivision,
        AircraftManufacturer,
        Airport,
        AirportOperator,
        AwardWinner,
        BodyOfWater,
        Broadcast,
        Building,
        ChineseAutonomousCounty,
        CityTown,
        CollegeUniversity,
        Company,
        Country,
        Cuisine,
        Dedicatee,
        Disease,
        DutchMunicipality,
        EnglishCivilParish,
        EnglishMetropolitanBorough,
        Facility,
        FictionalUniverse,
        FilmScreeningVenue,
        FootballTeam,
        FrenchDepartment,
        GeographicFeature,
        GermanState,
        GermanUrbanDistrict,
        GovernmentalJurisdiction,
        HumanLanguage,
        IndianCity,
        IndonesianCity,
        Island,
        ItalianComune,
        JapaneseDesignatedCity,
        JapanesePrefecture,
        Kingdom,
        Lake,
        Location,
        MilitaryConflict,
        MilitaryPost,
        Mountain,
        Museum,
        MusicalArtist,
        Neighborhood,
        OlympicBiddingCity,
        OlympicHostCity,
        Organization,
        Person,
        PlaceOfWorship,
        PlaceWithNeighborhoods,
        PoliticalDistrict,
        RadioStation,
        RecordProducer,
        Region,
        River,
        School,
        SchoolDistrict,
        ScottishCouncilArea,
        SoccerClub,
        SportsTeam,
        TouristAttraction,
        USCounty,
        USIndianReservation,
        VietnameseProvincialCities,
        Waterfall,
        WineRegion,
        Accommodation,
        Airline,
        AwardNominee,
        AwardPresentingOrganization,
        BasketballConference,
        Brand,
        Bridge,
        BroadcastArtist,
        BroadcastContent,
        BroadcastDistributor,
        Cemetery,
        CompanyDivision,
        CompanyFounder,
        Composer,
        ConductedEnsemble,
        DrinkingEstablishment,
        FashionDesigner,
        FashionLabel,
        Film,
        FilmCinematographer,
        FilmCompany,
        FilmCostumerDesigner,
        FilmDirector,
        FilmDistributor,
        FilmFestival,
        FilmProducer,
        HallOfFame,
        HistoricPlace,
        Hospital,
        House,
        Inventor,
        Magazine,
        MembershipOrganization,
        MusicalAlbum,
        MusicalGroup,
        MusicalInstrumentCompany,
        Newspaper,
        OperaCompany,
        Orchestra,
        PeriodicalPublisher,
        ProductionCompany,
        PublicLibrary,
        RadioNetwork,
        RecordLabel,
        RecurringEvent,
        Road,
        ShoppingCenter,
        SportsFacility,
        Stadium,
        Station,
        TelevisionShow,
        TelevisionStation,
        Theater,
        TVChannel,
        TVNetwork,
        TVProducer,
        TVWriter,
        University,
        VentureFundedCompany,
        VideoGameDesigner,
        VideoGameDeveloper,
        VideoGameEngineDeveloper,
        VideoGamePublisher,
        Website,
        WineProducer,
        BoardMember,
        Family,
        FrenchRegion,
        IslandGroup,
        Monastery,
        NobleTitle,
        RoyalLine,
        UKOverseasTerritory,
        Award,
        ArchitecturalContractor,
        Lighthouse,
        MountainPass,
        OlympicVenue,
        Park,
        ProjectParticipant,
        Skyscraper,
        DiseaseCause,
        Actor,
        Book,
        Celebrity,
        Composition,
        ConcertFilm,
        FilmActor,
        FilmEditor,
        FilmSeries,
        Play,
        PublishedWork,
        TVActor,
        TVEpisode,
        WorkOfFiction,
        DisasterSurvivor,
        DisasterVictim,
        FilmMusicContributor,
        Guitarist,
        HallOfFameInductee,
        MilitaryPerson,
        MusicalGroupMember,
        BuildingComplex,
        CauseOfDeath,
        DiseaseOrMedicalCondition,
        InfectiousDisease,
        MilitaryUnit,
        PerformanceVenue,
        Academic,
        AcademicInstitution,
        AircraftDesigner,
        AmericanIndianGroup,
        Appellation,
        Appointer,
        Architect,
        ArchitectureFirm,
        ArmedForce,
        Astronaut,
        Astronomer,
        AstronomicalObservatory,
        Athlete,
        AutomobileCompany,
        AutomobileModel,
        AutomotiveDesigner,
        AwardDiscipline,
        AwardJudge,
        BasketballPlayer,
        Bassist,
        Beer,
        Beverage,
        BicycleManufacturer,
        Blog,
        Blogger,
        BoardMemberTitle,
        Boxer,
        CandyBarManufacturer,
        CharacterOccupation,
        CharacterSpecies,
        Cheese,
        ChemicalCompound,
        ChivalricOrderMember,
        Club,
        Collector,
        College,
        Comedian,
        ComicBookCreator,
        ComicBookEditor,
        ComicBookFictionalUniverse,
        ComicBookPenciler,
        ComicBookPublisher,
        ComicBookSeries,
        ComicBookWriter,
        ComicStripArtist,
        ComicStripSyndicate,
        CompanyAdvisor,
        CompanyShareholder,
        CompetitiveSpace,
        ComputerDesigner,
        ComputerPeripheral,
        ComputerScientist,
        ComputingPlatform,
        ConcertTour,
        Conductor,
        ConferenceSeries,
        ConsumerProduct,
        CricketAdministrativeBody,
        CricketTeam,
        Criminal,
        CriminalOffense,
        Dedicator,
        DietFollower,
        Dish,
        Distillery,
        Drummer,
        EndorsedProduct,
        Engine,
        Engineer,
        EngineeringFirm,
        FictionalUniverseCreator,
        FieldOfStudy,
        FileFormat,
        FilmArtDirector,
        FilmCharacter,
        FilmCrewmember,
        FilmCritic,
        FilmFestivalFocus,
        FilmProductionDesigner,
        FilmTheorist,
        FilmWriter,
        FootballLeague,
        FootballOrganization,
        FootballPlayer,
        FoundingFigure,
        Game,
        GameDesigner,
        GamePublisher,
        Golfer,
        GovernmentAgency,
        GovernmentalBody,
        GovernmentOfficeOrTitle,
        Governor,
        Guitar,
        Hobbyist,
        HockeyConference,
        HockeyTeam,
        HonoraryDegreeRecipient,
        Illustrator,
        Industry,
        Invention,
        ItalianRegion,
        JobTitle,
        Journal,
        LandscapeProject,
        LanguageCreator,
        LanguageWritingSystem,
        Lyricist,
        ManufacturingPlant,
        MartialArt,
        MartialArtist,
        MedicalSpecialty,
        MedicalTreatment,
        MemberOfParliament,
        MeteorologicalService,
        MilitaryCommander,
        Monarch,
        Mountaineer,
        MountainRange,
        MusicalGameSong,
        MusicalPerformanceRole,
        MusicalTrack,
        MusicFestival,
        NaturalOrCulturalPreservationAgency,
        NoblePerson,
        NonProfitOrganisation, //Non-ProfitOrganisation
        OfficeHolder,
        OlympicAthlete,
        OlympicEvent,
        OperaCharacter,
        OperaHouse,
        OperaSinger,
        OperatingSystemDeveloper,
        OrganizationSector,
        PeriodicalEditor,
        Philosopher,
        Physician,
        PoliticalAppointer,
        PoliticalParty,
        Politician,
        President,
        ProcessorManufacturer,
        Profession,
        ProfessionalSportsTeam,
        ProgrammingLanguageDesigner,
        ProgrammingLanguageDeveloper,
        ProtectedArea,
        Protocol,
        ProtocolProvider,
        Rank,
        RecordingEngineer,
        RecurringCompetition,
        Religion,
        ReligiousLeader,
        ReligiousOrder,
        ReligiousOrganization,
        ReportIssuingInstitution,
        RiskFactor,
        RocketEngineDesigner,
        RocketManufacturer,
        Saint,
        Satellite,
        SchoolFounder,
        SchoolNewspaper,
        SchoolSportsTeam,
        Scientist,
        Senator,
        ShipBuilder,
        ShipDesigner,
        SkiArea,
        Software,
        SoftwareDeveloper,
        SoftwareLicense,
        Songwriter,
        Soundtrack,
        SpaceAgency,
        SpacecraftManufacturer,
        Spaceport,
        Sport,
        SportsAssociation,
        SportsLeagueAwardWinner,
        StudentOrganization,
        Supercouple,
        Surgeon,
        Symptom,
        TheaterActor,
        TheaterCharacter,
        TheaterProduction,
        TheatricalComposer,
        TheatricalLyricist,
        TopLevelDomainRegistry,
        TradeUnion,
        TransitLine,
        TransportOperator,
        TransportTerminus,
        TVCharacter,
        TVDirector,
        TVPersonality,
        USCongressperson, //U.S.Congressperson
        USPresident,
        USTerritory,
        USVicePresident,
        VideoGame,
        VideoGameActor,
        VideoGameEngine,
        VideoGamePlatform,
        VisualArtist,
        Wrestler,
        Writer,
        Adherents,
        Appointee,
        ArchitectureFirmPartner,
        BasketballCoach,
        BritishRoyalty,
        Cardinal,
        Chancellor,
        Chef,
        ChivalricOrderFounder,
        ChivalricOrderOfficer,
        ChristianBishop,
        CollegeCoach,
        ComicBookInker,
        ComicBookLetterer,
        CreativeWork,
        CricketBowler,
        CricketCoach,
        CricketPlayer,
        CulinaryTool,
        Cyclist,
        Deity,
        ElectionCampaign,
        ElementDiscoverer,
        FilmCastingDirector,
        FilmSetDesigner,
        FootballCoach,
        FootballManager,
        HockeyCoach,
        HockeyPlayer,
        Holiday,
        Journalist,
        Judge,
        LandscapeArchitect,
        Mayor,
        Model,
        MusicalRelease,
        OperaDirector,
        OrganismClassification,
        ProfessionalField,
        ProgrammingLanguage,
        RugbyPlayer,
        SchoolMascot,
        SportsOfficial,
        TennisPlayer,
        TennisTournamentChampion,
        TheaterChoreographer,
        TheaterDesigner,
        TheaterDirector,
        TheaterProducer,
        TVCrewmember,
        TVThemeSong,
        VaccineDeveloper,
        RadioProgram,
        USState
    }
    #endregion

    #region GetFeedLinks
    /// <summary>
    /// Feed data.
    /// </summary>
    [fsObject]
    public class FeedData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the feeds.
        /// </summary>
        /// <value>The feeds.</value>
        public Feed[] feeds { get; set; }
    }

    /// <summary>
    /// Feed.
    /// </summary>
    [fsObject]
    public class Feed
    {
        /// <summary>
        /// Gets or sets the feed.
        /// </summary>
        /// <value>The feed.</value>
        public string feed { get; set; }
    }
    #endregion

    #region GetRankedKeyworkds
    /// <summary>
    /// Keyword data.
    /// </summary>
    [fsObject]
    public class KeywordData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public Keyword[] keywords { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has data.
        /// </summary>
        /// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
        public bool HasData
        {
            get
            {
                return keywords != null && keywords.Length > 0;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.KeywordData"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.KeywordData"/>.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int indexKeyword = 0; keywords != null && indexKeyword < keywords.Length; indexKeyword++)
            {
                stringBuilder.Append("\n\t");
                stringBuilder.Append(keywords[indexKeyword].ToString());
            }
            return string.Format("[KeywordExtractionData: status={0}, language={1}, url={2}, text={3}, keywords={4}]", status, language, url, text, stringBuilder.ToString());
        }
    };

    /// <summary>
    /// Keyword.
    /// </summary>
    [fsObject]
    public class Keyword
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the relevance.
        /// </summary>
        /// <value>The relevance.</value>
        public string relevance { get; set; }

        /// <summary>
        /// Gets or sets the knowledge graph.
        /// </summary>
        /// <value>The knowledge graph.</value>
        public KnowledgeGraph knowledgeGraph { get; set; }

        /// <summary>
        /// Gets or sets the sentiment.
        /// </summary>
        /// <value>The sentiment.</value>
        public DocSentiment sentiment { get; set; }
    };
    #endregion

    #region GetLanguage
    /// <summary>
    /// Language data.
    /// </summary>
    [fsObject]
    public class LanguageData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the iso 639 1.
        /// </summary>
        /// <value>The iso 639 1.</value>
        [fsProperty("iso-639-1")]
        public string iso_639_1 { get; set; }

        /// <summary>
        /// Gets or sets the iso 639 2.
        /// </summary>
        /// <value>The iso 639 2.</value>
        [fsProperty("iso-639-2")]
        public string iso_639_2 { get; set; }

        /// <summary>
        /// Gets or sets the iso 639 3.
        /// </summary>
        /// <value>The iso 639 3.</value>
        [fsProperty("iso-639-3")]
        public string iso_639_3 { get; set; }

        /// <summary>
        /// Gets or sets the ethnologue.
        /// </summary>
        /// <value>The ethnologue.</value>
        public string ethnologue { get; set; }

        /// <summary>
        /// Gets or sets the native speakers.
        /// </summary>
        /// <value>The native speakers.</value>
        [fsProperty("native-speakers")]
        public string native_speakers { get; set; }

        /// <summary>
        /// Gets or sets the wikipedia.
        /// </summary>
        /// <value>The wikipedia.</value>
        public string wikipedia { get; set; }
    }
    #endregion

    #region GetMicroformatData
    /// <summary>
    /// Microformat data.
    /// </summary>
    [fsObject]
    public class MicroformatData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the microformats.
        /// </summary>
        /// <value>The microformats.</value>
        public Microformat[] microformats { get; set; }
    }

    /// <summary>
    /// Microformat.
    /// </summary>
    [fsObject]
    public class Microformat
    {
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field.</value>
        public string field { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public string data { get; set; }
    }
    #endregion

    #region GetPublicationDate
    /// <summary>
    /// Pub date data.
    /// </summary>
    [fsObject]
    public class PubDateData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the publication date.
        /// </summary>
        /// <value>The publication date.</value>
        public PublicationDate publicationDate { get; set; }
    }
    #endregion

    #region GetRelations
    /// <summary>
    /// Relations data.
    /// </summary>
    [fsObject]
    public class RelationsData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the relations.
        /// </summary>
        /// <value>The relations.</value>
        public Relation[] relations { get; set; }
    }
    #endregion

    #region GetSentiment
    /// <summary>
    /// Sentiment data.
    /// </summary>
    [fsObject]
    public class SentimentData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the document sentiment.
        /// </summary>
        /// <value>The document sentiment.</value>
        public DocSentiment docSentiment { get; set; }
    }
    #endregion

    #region GetTargetedSentiment
    /// <summary>
    /// Targeted sentiment data.
    /// </summary>
    [fsObject]
    public class TargetedSentimentData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public TargetedSentiment[] results { get; set; }
    }

    /// <summary>
    /// Targeted sentiment.
    /// </summary>
    [fsObject]
    public class TargetedSentiment
    {
        /// <summary>
        /// Gets or sets the sentiment.
        /// </summary>
        /// <value>The sentiment.</value>
        public DocSentiment sentiment { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }
    }
    #endregion

    #region GetRankedTaxonomy
    /// <summary>
    /// Taxonomy data.
    /// </summary>
    [fsObject]
    public class TaxonomyData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy.
        /// </summary>
        /// <value>The taxonomy.</value>
        public Taxonomy[] taxonomy { get; set; }
    }
    #endregion

    #region GetRawText
    /// <summary>
    /// Text data.
    /// </summary>
    [fsObject]
    public class TextData
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string language { get; set; }
    }
    #endregion

    #region GetTitle
    /// <summary>
    /// Title.
    /// </summary>
    [fsObject]
    public class Title
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string title { get; set; }
    }
    #endregion

    #region InlineModels
    /// <summary>
    /// Knowledge graph.
    /// </summary>
    [fsObject]
    public class KnowledgeGraph
    {
        /// <summary>
        /// Gets or sets the type hierarchy.
        /// </summary>
        /// <value>The type hierarchy.</value>
        public string typeHierarchy { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.KnowledgeGraph"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.KnowledgeGraph"/>.</returns>
        public override string ToString()
        {
            return string.Format("[KnowledgeGraph: typeHierarchy={0}]", typeHierarchy);
        }
    }

    /// <summary>
    /// Disambiguated.
    /// </summary>
    [fsObject]
    public class Disambiguated
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the type of the sub.
        /// </summary>
        /// <value>The type of the sub.</value>
        public string subType { get; set; }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        /// <value>The website.</value>
        public string website { get; set; }

        /// <summary>
        /// Gets or sets the geo.
        /// </summary>
        /// <value>The geo.</value>
        public string geo { get; set; }

        /// <summary>
        /// Gets or sets the dbpedia.
        /// </summary>
        /// <value>The dbpedia.</value>
        public string dbpedia { get; set; }

        /// <summary>
        /// Gets or sets the yago.
        /// </summary>
        /// <value>The yago.</value>
        public string yago { get; set; }

        /// <summary>
        /// Gets or sets the opencyc.
        /// </summary>
        /// <value>The opencyc.</value>
        public string opencyc { get; set; }

        /// <summary>
        /// Gets or sets the umbel.
        /// </summary>
        /// <value>The umbel.</value>
        public string umbel { get; set; }

        /// <summary>
        /// Gets or sets the freebase.
        /// </summary>
        /// <value>The freebase.</value>
        public string freebase { get; set; }

        /// <summary>
        /// Gets or sets the cia factbook.
        /// </summary>
        /// <value>The cia factbook.</value>
        public string ciaFactbook { get; set; }

        /// <summary>
        /// Gets or sets the census.
        /// </summary>
        /// <value>The census.</value>
        public string census { get; set; }

        /// <summary>
        /// Gets or sets the geonames.
        /// </summary>
        /// <value>The geonames.</value>
        public string geonames { get; set; }

        /// <summary>
        /// Gets or sets the music brainz.
        /// </summary>
        /// <value>The music brainz.</value>
        public string musicBrainz { get; set; }

        /// <summary>
        /// Gets or sets the crunchbase.
        /// </summary>
        /// <value>The crunchbase.</value>
        public string crunchbase { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.Disambiguated"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.Disambiguated"/>.</returns>
        public override string ToString()
        {
            return string.Format("[Disambiguated: name={0}, subType={1}, website={2}, geo={3}, dbpedia={4}, yago={5}, opencyc={6}, umbel={7}, freebase={8}, ciaFactbook={9}, census={10}, geonames={11}, musicBrainz={12}, crunchbase={13}]", name, subType, website, geo, dbpedia, yago, opencyc, umbel, freebase, ciaFactbook, census, geonames, musicBrainz, crunchbase);
        }
    }
   
    /// <summary>
    /// Quotation.
    /// </summary>
    [fsObject]
    public class Quotation
    {

        /// <summary>
        /// Gets or sets the quotation.
        /// </summary>
        /// <value>The quotation.</value>
        public string quotation { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.Quotation"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.Quotation"/>.</returns>
        public override string ToString()
        {
            return string.Format("[Quotation: quotation={0}]", quotation);
        }
    }

    /// <summary>
    /// Document sentiment.
    /// </summary>
    [fsObject]
    public class DocSentiment
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public string score { get; set; }

        /// <summary>
        /// Gets or sets the mixed.
        /// </summary>
        /// <value>The mixed.</value>
        public string mixed { get; set; }

        private double m_Score = 0;
        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <value>The score.</value>
        public double Score
        {
            get
            {
                if (m_Score == 0)
                {
                    if (!string.IsNullOrEmpty(score))
                    {
                        double.TryParse(score, out m_Score);
                    }
                }

                return m_Score;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.DocSentiment"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1.DocSentiment"/>.</returns>
        public override string ToString()
        {
            return string.Format("[Sentiment: type={0}, score={1}, mixed={2}]", type, score, mixed);
        }
    }

    /// <summary>
    /// Publication date.
    /// </summary>
    [fsObject]
    public class PublicationDate
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public string date { get; set; }
        /// <summary>
        /// Gets or sets the confident.
        /// </summary>
        /// <value>The confident.</value>
        public string confident { get; set; }
    }

    /// <summary>
    /// Relation.
    /// </summary>
    [fsObject]
    public class Relation
    {
        /// <summary>
        /// Gets or sets the sentence.
        /// </summary>
        /// <value>The sentence.</value>
        public string sentence { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public Subject subject { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public Action action { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>The object.</value>
        public ObjectData @object { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Location location { get; set; }

        /// <summary>
        /// Gets or sets the temporal.
        /// </summary>
        public Temporal temporal { get; set; }
    }
    
    /// <summary>
    /// Subject.
    /// </summary>
    [fsObject]
    public class Subject
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the sentiment.
        /// </summary>
        /// <value>The sentiment.</value>
        public DocSentiment sentiment { get; set; }

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public Entity entities { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public Keyword keywords { get; set; }
    }

    /// <summary>
    /// Action.
    /// </summary>
    [fsObject]
    public class Action
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the lemmatized.
        /// </summary>
        /// <value>The lemmatized.</value>
        public string lemmatized { get; set; }

        /// <summary>
        /// Gets or sets the verb.
        /// </summary>
        /// <value>The verb.</value>
        public Verb verb { get; set; }
    }

    /// <summary>
    /// Object data.
    /// </summary>
    [fsObject]
    public class ObjectData
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the sentiment.
        /// </summary>
        /// <value>The sentiment.</value>
        public DocSentiment sentiment { get; set; }

        /// <summary>
        /// Gets or sets the sentiment from subject.
        /// </summary>
        /// <value>The sentiment from subject.</value>
        public DocSentiment sentimentFromSubject { get; set; }

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public Entity entity { get; set; }
    }

    /// <summary>
    /// Verb.
    /// </summary>
    [fsObject]
    public class Verb
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the tense.
        /// </summary>
        /// <value>The tense.</value>
        public string tense { get; set; }

        /// <summary>
        /// Gets or sets the negated.
        /// </summary>
        /// <value>The negated.</value>
        public string negated { get; set; }
    }

    /// <summary>
    /// Taxonomy.
    /// </summary>
    [fsObject]
    public class Taxonomy
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string label { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public string score { get; set; }

        /// <summary>
        /// Gets or sets the confident.
        /// </summary>
        /// <value>The confident.</value>
        public string confident { get; set; }
    };

    /// <summary>
    /// Location.
    /// </summary>
    [fsObject]
    public class Location
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public Entity[] entities { get; set; }
    }

    /// <summary>
    /// Temporal.
    /// </summary>
    [fsObject]
    public class Temporal
    {
        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Gets or sets the decoded values
        /// </summary>
        public Decoded decoded { get; set; }
    }

    /// <summary>
    /// Decoded values.
    /// </summary>
    [fsObject]
    public class Decoded
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public string start { get; set; }
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public string end { get; set; }
    }
    #endregion

    #region GetNews
    /// <summary>
    /// The NewsResponse.
    /// </summary>
    [fsObject]
    public class NewsResponse
    {
        /// <summary>
        ///  THe status of the request.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The results.
        /// </summary>
        public Result result { get; set; }
    }

    /// <summary>
    /// The Result data.
    /// </summary>
    [fsObject]
    public class Result
    {
        /// <summary>
        /// The next string.
        /// </summary>
        public string next { get; set; }
        /// <summary>
        /// The request status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The docs data.
        /// </summary>
        public Docs[] docs { get; set; }
    }
    #endregion

    #region Docs
    /// <summary>
    /// The Docs data.
    /// </summary>
    [fsObject]
    public class Docs
    {
        /// <summary>
        /// The request ID.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The source data.
        /// </summary>
        public Source source { get; set; }
        /// <summary>
        /// Timestamp of the request.
        /// </summary>
        public string timestamp { get; set; }
    }
    #endregion

    #region Source
    /// <summary>
    /// The Source data.
    /// </summary>
    [fsObject]
    public class Source
    {
        /// <summary>
        /// Original data.
        /// </summary>
        public Original original { get; set; }
		/// <summary>
		/// The enriched data.
		/// </summary>
		/// <value>The enriched.</value>
		public Enriched enriched { get; set; }
    }
    #endregion

    #region Originial
    /// <summary>
    /// The Original data.
    /// </summary>
    [fsObject]
    public class Original
    {
        /// <summary>
        /// The URL.
        /// </summary>
        public string url { get; set; }
    }
    #endregion

    #region Enriched
    /// <summary>
    /// The Enriched data.
    /// </summary>
    [fsObject]
    public class Enriched
    {
        /// <summary>
        /// The URL.
        /// </summary>
        public URL url { get; set; }
    }
    #endregion

    #region URL
    /// <summary>
    /// The URL data.
    /// </summary>
    [fsObject]
    public class URL
    {
        /// <summary>
        /// The image data.
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// The image keywords.
        /// </summary>
        public ImageKeyword[] imageKeywords { get; set; }
        /// <summary>
        /// The RSS Feed data.
        /// </summary>
        public Feed[] feeds { get; set; }
        /// <summary>
        /// The URL.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The raw title.
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// The cleaned title.
        /// </summary>
        public string cleanedTitle { get; set; }
        /// <summary>
        /// Detected language.
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The PublicationDate data.
        /// </summary>
        public PublicationDate publicationDate { get; set; }
        /// <summary>
        /// Text data.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// /The Author data.
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// Keyword Extraction data.
        /// </summary>
        public Keyword[] keywords { get; set; }
        /// <summary>
        /// Entity extraction data.
        /// </summary>
        public Entity[] entities { get; set; }
        /// <summary>
        /// The concept data.
        /// </summary>
        public Concept[] concepts { get; set; }
        /// <summary>
        /// The relations data.
        /// </summary>
        public Relation relations { get; set; }
        /// <summary>
        /// The DocSentiment.
        /// </summary>
        public DocSentiment docSentiment { get; set; }
        /// <summary>
        /// The taxonomy data.
        /// </summary>
        public Taxonomy taxonomy { get; set; }
        /// <summary>
        /// The enriched title data.
        /// </summary>
        public EnrichedTitle[] enrichedTitle { get; set; }
    }
    #endregion

    #region EnrichedTitle
    /// <summary>
    /// The EnrichedTitle data.
    /// </summary>
    [fsObject]
    public class EnrichedTitle
    {
        /// <summary>
        /// Keyword extraction data.
        /// </summary>
        public Keyword[] keywords { get; set; }
        /// <summary>
        /// Entity extraction data.
        /// </summary>
        public Entity[] entities { get; set; }
        /// <summary>
        /// The Concepts data.
        /// </summary>
        public Concept[] concepts { get; set; }
        /// <summary>
        /// Relations data.
        /// </summary>
        public Relation[] relations { get; set; }
        /// <summary>
        /// The DocSentiment.
        /// </summary>
        public DocSentiment docSentiment { get; set; }
        /// <summary>
        /// Taxonomy data.
        /// </summary>
        public Taxonomy[] taxonomy { get; set; }
    }
    #endregion

    #region AlchemyData News Fields
    /// <summary>
    /// All fields for querying and receiving data.
    /// </summary>
    public class Fields
    {
        public const string ORIGINAL_URL = "original.url";
        public const string ENRICHED_URL_IMAGE = "enriched.url.image";
        public const string ENRICHED_URL_IMAGEKEYWORDS = "enriched.url.imageKeywords";
        public const string ENRICHED_URL_IMAGEKEYWORDS_IMAGEKEYWORD_TEXT = "enriched.url.imageKeywords.imageKeyword.text";
        public const string ENRICHED_URL_IMAGEKEYWORDS_IMAGEKEYWORD_SCORE = "enriched.url.imageKeywords.imageKeyword.score";
        public const string ENRICHED_URL_FEEDS = "enriched.url.feeds";
        public const string ENRICHED_URL_FEEDS_FEED_FEED = "enriched.url.feeds.feed.feed";
        public const string ENRICHED_URL_URL = "enriched.url.url";
        public const string ENRICHED_URL_TITLE = "enriched.url.title";
        public const string ENRICHED_URL_CLEANEDTITLE = "enriched.url.cleanedTitle";
        public const string ENRICHED_URL_LANGUAGE = "enriched.url.language";
        public const string ENRICHED_URL_PUBLICATIONDATE_DATE = "enriched.url.publicationDate.date";
        public const string ENRICHED_URL_PUBLICATIONDATE_CONFIDENT = "enriched.url.publicationDate.confident";
        public const string ENRICHED_URL_TEXT = "enriched.url.text";
        public const string ENRICHED_URL_AUTHOR = "enriched.url.author";
        public const string ENRICHED_URL_KEYWORDS = "enriched.url.keywords";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_TEXT = "enriched.url.keywords.keyword.text";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.keywords.keyword.relevance";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENTITIES = "enriched.url.entities";
        public const string ENRICHED_URL_ENTITIES_ENTITY_TEXT = "enriched.url.entities.entity.text";
        public const string ENRICHED_URL_ENTITIES_ENTITY_TYPE = "enriched.url.entities.entity.type";
        public const string ENRICHED_URL_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENTITIES_ENTITY_COUNT = "enriched.url.entities.entity.count";
        public const string ENRICHED_URL_ENTITIES_ENTITY_RELEVANCE = "enriched.url.entities.entity.relevance";
        public const string ENRICHED_URL_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.entities.entity.quotations";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_CONCEPTS = "enriched.url.concepts";
        public const string ENRICHED_URL_CONCEPTS_CONCEPT_TEXT = "enriched.url.concepts.concept.text";
        public const string ENRICHED_URL_CONCEPTS_CONCEPT_RELEVANCE = "enriched.url.concepts.concept.relevance";
        public const string ENRICHED_URL_CONCEPTS_CONCEPT_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.concepts.concept.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS = "enriched.url.relations";
        public const string ENRICHED_URL_RELATIONS_RELATION_SENTENCE = "enriched.url.relations.relation.sentence";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_TEXT = "enriched.url.relations.relation.subject.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES = "enriched.url.relations.relation.subject.entities";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.relations.relation.subject.entities.entity.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.relations.relation.subject.entities.entity.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.subject.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.relations.relation.subject.entities.entity.count";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.relations.relation.subject.entities.entity.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.relations.relation.subject.entities.entity.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.relations.relation.subject.entities.entity.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.relations.relation.subject.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.relations.relation.subject.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.relations.relation.subject.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.relations.relation.subject.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.relations.relation.subject.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.relations.relation.subject.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.relations.relation.subject.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.relations.relation.subject.entities.entity.quotations";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS = "enriched.url.relations.relation.subject.keywords";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.relations.relation.subject.keywords.keyword.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.relations.relation.subject.keywords.keyword.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.subject.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_SENTIMENT_TYPE = "enriched.url.relations.relation.subject.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_SENTIMENT_SCORE = "enriched.url.relations.relation.subject.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_SUBJECT_SENTIMENT_MIXED = "enriched.url.relations.relation.subject.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_TEXT = "enriched.url.relations.relation.action.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_LEMMATIZED = "enriched.url.relations.relation.action.lemmatized";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_VERB_TEXT = "enriched.url.relations.relation.action.verb.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_VERB_TENSE = "enriched.url.relations.relation.action.verb.tense";
        public const string ENRICHED_URL_RELATIONS_RELATION_ACTION_VERB_NEGATED = "enriched.url.relations.relation.action.verb.negated";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_TEXT = "enriched.url.relations.relation.object.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES = "enriched.url.relations.relation.object.entities";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.relations.relation.object.entities.entity.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.relations.relation.object.entities.entity.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.object.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.relations.relation.object.entities.entity.count";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.relations.relation.object.entities.entity.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.relations.relation.object.entities.entity.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.relations.relation.object.entities.entity.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.relations.relation.object.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.relations.relation.object.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.relations.relation.object.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.relations.relation.object.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.relations.relation.object.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.relations.relation.object.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.relations.relation.object.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.relations.relation.object.entities.entity.quotations";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.relations.relation.object.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS = "enriched.url.relations.relation.object.keywords";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.relations.relation.object.keywords.keyword.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.relations.relation.object.keywords.keyword.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.relations.relation.object.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.relations.relation.object.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.relations.relation.object.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.object.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENT_TYPE = "enriched.url.relations.relation.object.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENT_SCORE = "enriched.url.relations.relation.object.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENT_MIXED = "enriched.url.relations.relation.object.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_TYPE = "enriched.url.relations.relation.object.sentimentFromSubject.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_SCORE = "enriched.url.relations.relation.object.sentimentFromSubject.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_MIXED = "enriched.url.relations.relation.object.sentimentFromSubject.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_TEXT = "enriched.url.relations.relation.location.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_SENTIMENT_TYPE = "enriched.url.relations.relation.location.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_SENTIMENT_SCORE = "enriched.url.relations.relation.location.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_SENTIMENT_MIXED = "enriched.url.relations.relation.location.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES = "enriched.url.relations.relation.location.entities";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TEXT = "enriched.url.relations.relation.location.entities.entity.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TYPE = "enriched.url.relations.relation.location.entities.entity.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.location.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_COUNT = "enriched.url.relations.relation.location.entities.entity.count";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_RELEVANCE = "enriched.url.relations.relation.location.entities.entity.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.relations.relation.location.entities.entity.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.relations.relation.location.entities.entity.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.relations.relation.location.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.relations.relation.location.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.relations.relation.location.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.relations.relation.location.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.relations.relation.location.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.relations.relation.location.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.relations.relation.location.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.relations.relation.location.entities.entity.quotations";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.relations.relation.location.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS = "enriched.url.relations.relation.location.keywords";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_TEXT = "enriched.url.relations.relation.location.keywords.keyword.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.relations.relation.location.keywords.keyword.relevance";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.relations.relation.location.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.relations.relation.location.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.relations.relation.location.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.relations.relation.location.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_TEXT = "enriched.url.relations.relation.temporal.text";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_TYPE = "enriched.url.relations.relation.temporal.decoded.type";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_VALUE = "enriched.url.relations.relation.temporal.decoded.value";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_START = "enriched.url.relations.relation.temporal.decoded.start";
        public const string ENRICHED_URL_RELATIONS_RELATION_TEMPORAL_DECODED_END = "enriched.url.relations.relation.temporal.decoded.end";
        public const string ENRICHED_URL_DOCSENTIMENT_TYPE = "enriched.url.docSentiment.type";
        public const string ENRICHED_URL_DOCSENTIMENT_SCORE = "enriched.url.docSentiment.score";
        public const string ENRICHED_URL_DOCSENTIMENT_MIXED = "enriched.url.docSentiment.mixed";
        public const string ENRICHED_URL_TAXONOMY = "enriched.url.taxonomy";
        public const string ENRICHED_URL_TAXONOMY_TAXONOMY__LABEL = "enriched.url.taxonomy.taxonomy_.label";
        public const string ENRICHED_URL_TAXONOMY_TAXONOMY__SCORE = "enriched.url.taxonomy.taxonomy_.score";
        public const string ENRICHED_URL_TAXONOMY_TAXONOMY__CONFIDENT = "enriched.url.taxonomy.taxonomy_.confident";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS = "enriched.url.enrichedTitle.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES = "enriched.url.enrichedTitle.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS = "enriched.url.enrichedTitle.concepts";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS_CONCEPT_TEXT = "enriched.url.enrichedTitle.concepts.concept.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS_CONCEPT_RELEVANCE = "enriched.url.enrichedTitle.concepts.concept.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_CONCEPTS_CONCEPT_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.concepts.concept.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS = "enriched.url.enrichedTitle.relations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SENTENCE = "enriched.url.enrichedTitle.relations.relation.sentence";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_TEXT = "enriched.url.enrichedTitle.relations.relation.subject.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES = "enriched.url.enrichedTitle.relations.relation.subject.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS = "enriched.url.enrichedTitle.relations.relation.subject.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.subject.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.subject.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.subject.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_SUBJECT_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.subject.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_TEXT = "enriched.url.enrichedTitle.relations.relation.action.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_LEMMATIZED = "enriched.url.enrichedTitle.relations.relation.action.lemmatized";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_VERB_TEXT = "enriched.url.enrichedTitle.relations.relation.action.verb.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_VERB_TENSE = "enriched.url.enrichedTitle.relations.relation.action.verb.tense";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_ACTION_VERB_NEGATED = "enriched.url.enrichedTitle.relations.relation.action.verb.negated";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_TEXT = "enriched.url.enrichedTitle.relations.relation.object.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES = "enriched.url.enrichedTitle.relations.relation.object.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS = "enriched.url.enrichedTitle.relations.relation.object.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.object.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_TYPE = "enriched.url.enrichedTitle.relations.relation.object.sentimentFromSubject.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_SCORE = "enriched.url.enrichedTitle.relations.relation.object.sentimentFromSubject.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_OBJECT_SENTIMENTFROMSUBJECT_MIXED = "enriched.url.enrichedTitle.relations.relation.object.sentimentFromSubject.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_TEXT = "enriched.url.enrichedTitle.relations.relation.location.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES = "enriched.url.enrichedTitle.relations.relation.location.entities";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TEXT = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_TYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_COUNT = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.count";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_NAME = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.name";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_GEO = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.geo";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_DBPEDIA = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.dbpedia";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_WEBSITE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.website";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.subType";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_DISAMBIGUATED_SUBTYPE_SUBTYPE_ = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.disambiguated.subType.subType_";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__QUOTATION = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.quotation";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_ENTITIES_ENTITY_QUOTATIONS_QUOTATION__SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.entities.entity.quotations.quotation_.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS = "enriched.url.enrichedTitle.relations.relation.location.keywords";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_TEXT = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_RELEVANCE = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.relevance";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_TYPE = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.sentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_SCORE = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.sentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_SENTIMENT_MIXED = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.sentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_LOCATION_KEYWORDS_KEYWORD_KNOWLEDGEGRAPH_TYPEHIERARCHY = "enriched.url.enrichedTitle.relations.relation.location.keywords.keyword.knowledgeGraph.typeHierarchy";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_TEXT = "enriched.url.enrichedTitle.relations.relation.temporal.text";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_TYPE = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_VALUE = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.value";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_START = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.start";
        public const string ENRICHED_URL_ENRICHEDTITLE_RELATIONS_RELATION_TEMPORAL_DECODED_END = "enriched.url.enrichedTitle.relations.relation.temporal.decoded.end";
        public const string ENRICHED_URL_ENRICHEDTITLE_DOCSENTIMENT_TYPE = "enriched.url.enrichedTitle.docSentiment.type";
        public const string ENRICHED_URL_ENRICHEDTITLE_DOCSENTIMENT_SCORE = "enriched.url.enrichedTitle.docSentiment.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_DOCSENTIMENT_MIXED = "enriched.url.enrichedTitle.docSentiment.mixed";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY = "enriched.url.enrichedTitle.taxonomy";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY_TAXONOMY__LABEL = "enriched.url.enrichedTitle.taxonomy.taxonomy_.label";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY_TAXONOMY__SCORE = "enriched.url.enrichedTitle.taxonomy.taxonomy_.score";
        public const string ENRICHED_URL_ENRICHEDTITLE_TAXONOMY_TAXONOMY__CONFIDENT = "enriched.url.enrichedTitle.taxonomy.taxonomy_.confident";
    }
    #endregion
}
