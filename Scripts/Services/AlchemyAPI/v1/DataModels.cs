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
        public Feed[] feeds { get; set; }

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

        private System.DateTime _dateValue = default(System.DateTime);

        /// <summary>
        /// Gets the date value.
        /// </summary>
        /// <value>The date value.</value>
        public System.DateTime DateValue
        {
            get
            {
                if (_dateValue == default(System.DateTime) && !string.IsNullOrEmpty(date) && date.Length > 8)
                {
                    //19840101T000000
                    System.DateTime.TryParseExact(date.Remove(8),
                        "yyyyddMM",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out _dateValue);

                }
                return _dateValue;
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

        private PositionOnMap _geoLocation = null;
        /// <summary>
        /// Gets the geo location.
        /// </summary>
        /// <value>The geo location.</value>
        public PositionOnMap GeoLocation
        {
            get
            {
                if (_geoLocation == null)
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
                                        _geoLocation = new PositionOnMap(latitute, longitutde, entities[i].disambiguated.name);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return _geoLocation;
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

        private double _score = 0;
        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <value>The score.</value>
        public double Score
        {
            get
            {
                if (_score == 0)
                {
                    if (!string.IsNullOrEmpty(score))
                    {
                        double.TryParse(score, out _score);
                    }
                }

                return _score;
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
        public const string OriginalUrl = "original.url";
        public const string EnrichedUrlImage = "enriched.url.image";
        public const string EnrichedUrlImagekeywords = "enriched.url.imagekeywords";
        public const string EnrichedUrlImagekeywordsImagekeywordText = "enriched.url.imagekeywords.imagekeyword.text";
        public const string EnrichedUrlImagekeywordsImagekeywordScore = "enriched.url.imagekeywords.imagekeyword.score";
        public const string EnrichedUrlFeeds = "enriched.url.feeds";
        public const string EnrichedUrlFeedsFeedFeed = "enriched.url.feeds.feed.feed";
        public const string EnrichedUrlUrl = "enriched.url.url";
        public const string EnrichedUrlTitle = "enriched.url.title";
        public const string EnrichedUrlCleanedtitle = "enriched.url.cleanedtitle";
        public const string EnrichedUrlLanguage = "enriched.url.language";
        public const string EnrichedUrlPublicationdateDate = "enriched.url.publicationdate.date";
        public const string EnrichedUrlPublicationdateConfident = "enriched.url.publicationdate.confident";
        public const string EnrichedUrlText = "enriched.url.text";
        public const string EnrichedUrlAuthor = "enriched.url.author";
        public const string EnrichedUrlKeywords = "enriched.url.keywords";
        public const string EnrichedUrlKeywordsKeywordText = "enriched.url.keywords.keyword.text";
        public const string EnrichedUrlKeywordsKeywordRelevance = "enriched.url.keywords.keyword.relevance";
        public const string EnrichedUrlKeywordsKeywordSentimentType = "enriched.url.keywords.keyword.sentiment.type";
        public const string EnrichedUrlKeywordsKeywordSentimentScore = "enriched.url.keywords.keyword.sentiment.score";
        public const string EnrichedUrlKeywordsKeywordSentimentMixed = "enriched.url.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEntities = "enriched.url.entities";
        public const string EnrichedUrlEntitiesEntityText = "enriched.url.entities.entity.text";
        public const string EnrichedUrlEntitiesEntityType = "enriched.url.entities.entity.type";
        public const string EnrichedUrlEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEntitiesEntityCount = "enriched.url.entities.entity.count";
        public const string EnrichedUrlEntitiesEntityRelevance = "enriched.url.entities.entity.relevance";
        public const string EnrichedUrlEntitiesEntitySentimentType = "enriched.url.entities.entity.sentiment.type";
        public const string EnrichedUrlEntitiesEntitySentimentScore = "enriched.url.entities.entity.sentiment.score";
        public const string EnrichedUrlEntitiesEntitySentimentMixed = "enriched.url.entities.entity.sentiment.mixed";
        public const string EnrichedUrlEntitiesEntityDisambiguatedName = "enriched.url.entities.entity.disambiguated.name";
        public const string EnrichedUrlEntitiesEntityDisambiguatedGeo = "enriched.url.entities.entity.disambiguated.geo";
        public const string EnrichedUrlEntitiesEntityDisambiguatedDbpedia = "enriched.url.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlEntitiesEntityDisambiguatedWebsite = "enriched.url.entities.entity.disambiguated.website";
        public const string EnrichedUrlEntitiesEntityDisambiguatedSubtype = "enriched.url.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlEntitiesEntityQuotations = "enriched.url.entities.entity.quotations";
        public const string EnrichedUrlEntitiesEntityQuotationsQuotationQuotation = "enriched.url.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlConcepts = "enriched.url.concepts";
        public const string EnrichedUrlConceptsConceptText = "enriched.url.concepts.concept.text";
        public const string EnrichedUrlConceptsConceptRelevance = "enriched.url.concepts.concept.relevance";
        public const string EnrichedUrlConceptsConceptKnowledgegraphTypehierarchy = "enriched.url.concepts.concept.knowledgegraph.typehierarchy";
        public const string EnrichedUrlRelations = "enriched.url.relations";
        public const string EnrichedUrlRelationsRelationSentence = "enriched.url.relations.relation.sentence";
        public const string EnrichedUrlRelationsRelationSubjectText = "enriched.url.relations.relation.subject.text";
        public const string EnrichedUrlRelationsRelationSubjectEntities = "enriched.url.relations.relation.subject.entities";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityText = "enriched.url.relations.relation.subject.entities.entity.text";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityType = "enriched.url.relations.relation.subject.entities.entity.type";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.relations.relation.subject.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityCount = "enriched.url.relations.relation.subject.entities.entity.count";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityRelevance = "enriched.url.relations.relation.subject.entities.entity.relevance";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntitySentimentType = "enriched.url.relations.relation.subject.entities.entity.sentiment.type";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntitySentimentScore = "enriched.url.relations.relation.subject.entities.entity.sentiment.score";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntitySentimentMixed = "enriched.url.relations.relation.subject.entities.entity.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityDisambiguatedName = "enriched.url.relations.relation.subject.entities.entity.disambiguated.name";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityDisambiguatedGeo = "enriched.url.relations.relation.subject.entities.entity.disambiguated.geo";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityDisambiguatedDbpedia = "enriched.url.relations.relation.subject.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityDisambiguatedWebsite = "enriched.url.relations.relation.subject.entities.entity.disambiguated.website";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityDisambiguatedSubtype = "enriched.url.relations.relation.subject.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.relations.relation.subject.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityQuotations = "enriched.url.relations.relation.subject.entities.entity.quotations";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityQuotationsQuotationQuotation = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlRelationsRelationSubjectEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.relations.relation.subject.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationSubjectKeywords = "enriched.url.relations.relation.subject.keywords";
        public const string EnrichedUrlRelationsRelationSubjectKeywordsKeywordText = "enriched.url.relations.relation.subject.keywords.keyword.text";
        public const string EnrichedUrlRelationsRelationSubjectKeywordsKeywordRelevance = "enriched.url.relations.relation.subject.keywords.keyword.relevance";
        public const string EnrichedUrlRelationsRelationSubjectKeywordsKeywordSentimentType = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.type";
        public const string EnrichedUrlRelationsRelationSubjectKeywordsKeywordSentimentScore = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.score";
        public const string EnrichedUrlRelationsRelationSubjectKeywordsKeywordSentimentMixed = "enriched.url.relations.relation.subject.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationSubjectKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.relations.relation.subject.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlRelationsRelationSubjectSentimentType = "enriched.url.relations.relation.subject.sentiment.type";
        public const string EnrichedUrlRelationsRelationSubjectSentimentScore = "enriched.url.relations.relation.subject.sentiment.score";
        public const string EnrichedUrlRelationsRelationSubjectSentimentMixed = "enriched.url.relations.relation.subject.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationActionText = "enriched.url.relations.relation.action.text";
        public const string EnrichedUrlRelationsRelationActionLemmatized = "enriched.url.relations.relation.action.lemmatized";
        public const string EnrichedUrlRelationsRelationActionVerbText = "enriched.url.relations.relation.action.verb.text";
        public const string EnrichedUrlRelationsRelationActionVerbTense = "enriched.url.relations.relation.action.verb.tense";
        public const string EnrichedUrlRelationsRelationActionVerbNegated = "enriched.url.relations.relation.action.verb.negated";
        public const string EnrichedUrlRelationsRelationObjectText = "enriched.url.relations.relation.object.text";
        public const string EnrichedUrlRelationsRelationObjectEntities = "enriched.url.relations.relation.object.entities";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityText = "enriched.url.relations.relation.object.entities.entity.text";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityType = "enriched.url.relations.relation.object.entities.entity.type";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.relations.relation.object.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityCount = "enriched.url.relations.relation.object.entities.entity.count";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityRelevance = "enriched.url.relations.relation.object.entities.entity.relevance";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntitySentimentType = "enriched.url.relations.relation.object.entities.entity.sentiment.type";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntitySentimentScore = "enriched.url.relations.relation.object.entities.entity.sentiment.score";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntitySentimentMixed = "enriched.url.relations.relation.object.entities.entity.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityDisambiguatedName = "enriched.url.relations.relation.object.entities.entity.disambiguated.name";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityDisambiguatedGeo = "enriched.url.relations.relation.object.entities.entity.disambiguated.geo";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityDisambiguatedDbpedia = "enriched.url.relations.relation.object.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityDisambiguatedWebsite = "enriched.url.relations.relation.object.entities.entity.disambiguated.website";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityDisambiguatedSubtype = "enriched.url.relations.relation.object.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.relations.relation.object.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityQuotations = "enriched.url.relations.relation.object.entities.entity.quotations";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityQuotationsQuotationQuotation = "enriched.url.relations.relation.object.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.relations.relation.object.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.relations.relation.object.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlRelationsRelationObjectEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.relations.relation.object.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationObjectKeywords = "enriched.url.relations.relation.object.keywords";
        public const string EnrichedUrlRelationsRelationObjectKeywordsKeywordText = "enriched.url.relations.relation.object.keywords.keyword.text";
        public const string EnrichedUrlRelationsRelationObjectKeywordsKeywordRelevance = "enriched.url.relations.relation.object.keywords.keyword.relevance";
        public const string EnrichedUrlRelationsRelationObjectKeywordsKeywordSentimentType = "enriched.url.relations.relation.object.keywords.keyword.sentiment.type";
        public const string EnrichedUrlRelationsRelationObjectKeywordsKeywordSentimentScore = "enriched.url.relations.relation.object.keywords.keyword.sentiment.score";
        public const string EnrichedUrlRelationsRelationObjectKeywordsKeywordSentimentMixed = "enriched.url.relations.relation.object.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationObjectKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.relations.relation.object.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlRelationsRelationObjectSentimentType = "enriched.url.relations.relation.object.sentiment.type";
        public const string EnrichedUrlRelationsRelationObjectSentimentScore = "enriched.url.relations.relation.object.sentiment.score";
        public const string EnrichedUrlRelationsRelationObjectSentimentMixed = "enriched.url.relations.relation.object.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationObjectSentimentfromsubjectType = "enriched.url.relations.relation.object.sentimentfromsubject.type";
        public const string EnrichedUrlRelationsRelationObjectSentimentfromsubjectScore = "enriched.url.relations.relation.object.sentimentfromsubject.score";
        public const string EnrichedUrlRelationsRelationObjectSentimentfromsubjectMixed = "enriched.url.relations.relation.object.sentimentfromsubject.mixed";
        public const string EnrichedUrlRelationsRelationLocationText = "enriched.url.relations.relation.location.text";
        public const string EnrichedUrlRelationsRelationLocationSentimentType = "enriched.url.relations.relation.location.sentiment.type";
        public const string EnrichedUrlRelationsRelationLocationSentimentScore = "enriched.url.relations.relation.location.sentiment.score";
        public const string EnrichedUrlRelationsRelationLocationSentimentMixed = "enriched.url.relations.relation.location.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationLocationEntities = "enriched.url.relations.relation.location.entities";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityText = "enriched.url.relations.relation.location.entities.entity.text";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityType = "enriched.url.relations.relation.location.entities.entity.type";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.relations.relation.location.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityCount = "enriched.url.relations.relation.location.entities.entity.count";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityRelevance = "enriched.url.relations.relation.location.entities.entity.relevance";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntitySentimentType = "enriched.url.relations.relation.location.entities.entity.sentiment.type";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntitySentimentScore = "enriched.url.relations.relation.location.entities.entity.sentiment.score";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntitySentimentMixed = "enriched.url.relations.relation.location.entities.entity.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityDisambiguatedName = "enriched.url.relations.relation.location.entities.entity.disambiguated.name";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityDisambiguatedGeo = "enriched.url.relations.relation.location.entities.entity.disambiguated.geo";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityDisambiguatedDbpedia = "enriched.url.relations.relation.location.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityDisambiguatedWebsite = "enriched.url.relations.relation.location.entities.entity.disambiguated.website";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityDisambiguatedSubtype = "enriched.url.relations.relation.location.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.relations.relation.location.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityQuotations = "enriched.url.relations.relation.location.entities.entity.quotations";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityQuotationsQuotationQuotation = "enriched.url.relations.relation.location.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.relations.relation.location.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.relations.relation.location.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlRelationsRelationLocationEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.relations.relation.location.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationLocationKeywords = "enriched.url.relations.relation.location.keywords";
        public const string EnrichedUrlRelationsRelationLocationKeywordsKeywordText = "enriched.url.relations.relation.location.keywords.keyword.text";
        public const string EnrichedUrlRelationsRelationLocationKeywordsKeywordRelevance = "enriched.url.relations.relation.location.keywords.keyword.relevance";
        public const string EnrichedUrlRelationsRelationLocationKeywordsKeywordSentimentType = "enriched.url.relations.relation.location.keywords.keyword.sentiment.type";
        public const string EnrichedUrlRelationsRelationLocationKeywordsKeywordSentimentScore = "enriched.url.relations.relation.location.keywords.keyword.sentiment.score";
        public const string EnrichedUrlRelationsRelationLocationKeywordsKeywordSentimentMixed = "enriched.url.relations.relation.location.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlRelationsRelationLocationKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.relations.relation.location.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlRelationsRelationTemporalText = "enriched.url.relations.relation.temporal.text";
        public const string EnrichedUrlRelationsRelationTemporalDecodedType = "enriched.url.relations.relation.temporal.decoded.type";
        public const string EnrichedUrlRelationsRelationTemporalDecodedValue = "enriched.url.relations.relation.temporal.decoded.value";
        public const string EnrichedUrlRelationsRelationTemporalDecodedStart = "enriched.url.relations.relation.temporal.decoded.start";
        public const string EnrichedUrlRelationsRelationTemporalDecodedEnd = "enriched.url.relations.relation.temporal.decoded.end";
        public const string EnrichedUrlDocsentimentType = "enriched.url.docsentiment.type";
        public const string EnrichedUrlDocsentimentScore = "enriched.url.docsentiment.score";
        public const string EnrichedUrlDocsentimentMixed = "enriched.url.docsentiment.mixed";
        public const string EnrichedUrlTaxonomy = "enriched.url.taxonomy";
        public const string EnrichedUrlTaxonomyTaxonomyLabel = "enriched.url.taxonomy.taxonomy.label";
        public const string EnrichedUrlTaxonomyTaxonomyScore = "enriched.url.taxonomy.taxonomy.score";
        public const string EnrichedUrlTaxonomyTaxonomyConfident = "enriched.url.taxonomy.taxonomy.confident";
        public const string EnrichedUrlEnrichedtitleKeywords = "enriched.url.enrichedtitle.keywords";
        public const string EnrichedUrlEnrichedtitleKeywordsKeywordText = "enriched.url.enrichedtitle.keywords.keyword.text";
        public const string EnrichedUrlEnrichedtitleKeywordsKeywordRelevance = "enriched.url.enrichedtitle.keywords.keyword.relevance";
        public const string EnrichedUrlEnrichedtitleKeywordsKeywordSentimentType = "enriched.url.enrichedtitle.keywords.keyword.sentiment.type";
        public const string EnrichedUrlEnrichedtitleKeywordsKeywordSentimentScore = "enriched.url.enrichedtitle.keywords.keyword.sentiment.score";
        public const string EnrichedUrlEnrichedtitleKeywordsKeywordSentimentMixed = "enriched.url.enrichedtitle.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleEntities = "enriched.url.enrichedtitle.entities";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityText = "enriched.url.enrichedtitle.entities.entity.text";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityType = "enriched.url.enrichedtitle.entities.entity.type";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityCount = "enriched.url.enrichedtitle.entities.entity.count";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityRelevance = "enriched.url.enrichedtitle.entities.entity.relevance";
        public const string EnrichedUrlEnrichedtitleEntitiesEntitySentimentType = "enriched.url.enrichedtitle.entities.entity.sentiment.type";
        public const string EnrichedUrlEnrichedtitleEntitiesEntitySentimentScore = "enriched.url.enrichedtitle.entities.entity.sentiment.score";
        public const string EnrichedUrlEnrichedtitleEntitiesEntitySentimentMixed = "enriched.url.enrichedtitle.entities.entity.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityDisambiguatedName = "enriched.url.enrichedtitle.entities.entity.disambiguated.name";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityDisambiguatedGeo = "enriched.url.enrichedtitle.entities.entity.disambiguated.geo";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityDisambiguatedDbpedia = "enriched.url.enrichedtitle.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityDisambiguatedWebsite = "enriched.url.enrichedtitle.entities.entity.disambiguated.website";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityDisambiguatedSubtype = "enriched.url.enrichedtitle.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.enrichedtitle.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityQuotations = "enriched.url.enrichedtitle.entities.entity.quotations";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityQuotationsQuotationQuotation = "enriched.url.enrichedtitle.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.enrichedtitle.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.enrichedtitle.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlEnrichedtitleEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.enrichedtitle.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleConcepts = "enriched.url.enrichedtitle.concepts";
        public const string EnrichedUrlEnrichedtitleConceptsConceptText = "enriched.url.enrichedtitle.concepts.concept.text";
        public const string EnrichedUrlEnrichedtitleConceptsConceptRelevance = "enriched.url.enrichedtitle.concepts.concept.relevance";
        public const string EnrichedUrlEnrichedtitleConceptsConceptKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.concepts.concept.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleRelations = "enriched.url.enrichedtitle.relations";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSentence = "enriched.url.enrichedtitle.relations.relation.sentence";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectText = "enriched.url.enrichedtitle.relations.relation.subject.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntities = "enriched.url.enrichedtitle.relations.relation.subject.entities";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityText = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityType = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityCount = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.count";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityRelevance = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.relevance";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntitySentimentType = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntitySentimentScore = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntitySentimentMixed = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityDisambiguatedName = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.disambiguated.name";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityDisambiguatedGeo = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.disambiguated.geo";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityDisambiguatedDbpedia = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityDisambiguatedWebsite = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.disambiguated.website";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityDisambiguatedSubtype = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityQuotations = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.quotations";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityQuotationsQuotationQuotation = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.enrichedtitle.relations.relation.subject.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectKeywords = "enriched.url.enrichedtitle.relations.relation.subject.keywords";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectKeywordsKeywordText = "enriched.url.enrichedtitle.relations.relation.subject.keywords.keyword.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectKeywordsKeywordRelevance = "enriched.url.enrichedtitle.relations.relation.subject.keywords.keyword.relevance";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectKeywordsKeywordSentimentType = "enriched.url.enrichedtitle.relations.relation.subject.keywords.keyword.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectKeywordsKeywordSentimentScore = "enriched.url.enrichedtitle.relations.relation.subject.keywords.keyword.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectKeywordsKeywordSentimentMixed = "enriched.url.enrichedtitle.relations.relation.subject.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.relations.relation.subject.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectSentimentType = "enriched.url.enrichedtitle.relations.relation.subject.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectSentimentScore = "enriched.url.enrichedtitle.relations.relation.subject.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationSubjectSentimentMixed = "enriched.url.enrichedtitle.relations.relation.subject.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationActionText = "enriched.url.enrichedtitle.relations.relation.action.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationActionLemmatized = "enriched.url.enrichedtitle.relations.relation.action.lemmatized";
        public const string EnrichedUrlEnrichedtitleRelationsRelationActionVerbText = "enriched.url.enrichedtitle.relations.relation.action.verb.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationActionVerbTense = "enriched.url.enrichedtitle.relations.relation.action.verb.tense";
        public const string EnrichedUrlEnrichedtitleRelationsRelationActionVerbNegated = "enriched.url.enrichedtitle.relations.relation.action.verb.negated";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectText = "enriched.url.enrichedtitle.relations.relation.object.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntities = "enriched.url.enrichedtitle.relations.relation.object.entities";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityText = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityType = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityCount = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.count";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityRelevance = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.relevance";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntitySentimentType = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntitySentimentScore = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntitySentimentMixed = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityDisambiguatedName = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.disambiguated.name";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityDisambiguatedGeo = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.disambiguated.geo";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityDisambiguatedDbpedia = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityDisambiguatedWebsite = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.disambiguated.website";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityDisambiguatedSubtype = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityQuotations = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.quotations";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityQuotationsQuotationQuotation = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.enrichedtitle.relations.relation.object.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectKeywords = "enriched.url.enrichedtitle.relations.relation.object.keywords";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectKeywordsKeywordText = "enriched.url.enrichedtitle.relations.relation.object.keywords.keyword.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectKeywordsKeywordRelevance = "enriched.url.enrichedtitle.relations.relation.object.keywords.keyword.relevance";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectKeywordsKeywordSentimentType = "enriched.url.enrichedtitle.relations.relation.object.keywords.keyword.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectKeywordsKeywordSentimentScore = "enriched.url.enrichedtitle.relations.relation.object.keywords.keyword.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectKeywordsKeywordSentimentMixed = "enriched.url.enrichedtitle.relations.relation.object.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.relations.relation.object.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectSentimentType = "enriched.url.enrichedtitle.relations.relation.object.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectSentimentScore = "enriched.url.enrichedtitle.relations.relation.object.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectSentimentMixed = "enriched.url.enrichedtitle.relations.relation.object.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectSentimentfromsubjectType = "enriched.url.enrichedtitle.relations.relation.object.sentimentfromsubject.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectSentimentfromsubjectScore = "enriched.url.enrichedtitle.relations.relation.object.sentimentfromsubject.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationObjectSentimentfromsubjectMixed = "enriched.url.enrichedtitle.relations.relation.object.sentimentfromsubject.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationText = "enriched.url.enrichedtitle.relations.relation.location.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationSentimentType = "enriched.url.enrichedtitle.relations.relation.location.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationSentimentScore = "enriched.url.enrichedtitle.relations.relation.location.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationSentimentMixed = "enriched.url.enrichedtitle.relations.relation.location.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntities = "enriched.url.enrichedtitle.relations.relation.location.entities";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityText = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityType = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityCount = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.count";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityRelevance = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.relevance";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntitySentimentType = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntitySentimentScore = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntitySentimentMixed = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityDisambiguatedName = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.disambiguated.name";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityDisambiguatedGeo = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.disambiguated.geo";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityDisambiguatedDbpedia = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.disambiguated.dbpedia";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityDisambiguatedWebsite = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.disambiguated.website";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityDisambiguatedSubtype = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.disambiguated.subtype";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityDisambiguatedSubtypeSubtype = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.disambiguated.subtype.subtype";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityQuotations = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.quotations";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityQuotationsQuotationQuotation = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.quotations.quotation.quotation";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityQuotationsQuotationSentimentType = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.quotations.quotation.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityQuotationsQuotationSentimentScore = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.quotations.quotation.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationEntitiesEntityQuotationsQuotationSentimentMixed = "enriched.url.enrichedtitle.relations.relation.location.entities.entity.quotations.quotation.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationKeywords = "enriched.url.enrichedtitle.relations.relation.location.keywords";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationKeywordsKeywordText = "enriched.url.enrichedtitle.relations.relation.location.keywords.keyword.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationKeywordsKeywordRelevance = "enriched.url.enrichedtitle.relations.relation.location.keywords.keyword.relevance";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationKeywordsKeywordSentimentType = "enriched.url.enrichedtitle.relations.relation.location.keywords.keyword.sentiment.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationKeywordsKeywordSentimentScore = "enriched.url.enrichedtitle.relations.relation.location.keywords.keyword.sentiment.score";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationKeywordsKeywordSentimentMixed = "enriched.url.enrichedtitle.relations.relation.location.keywords.keyword.sentiment.mixed";
        public const string EnrichedUrlEnrichedtitleRelationsRelationLocationKeywordsKeywordKnowledgegraphTypehierarchy = "enriched.url.enrichedtitle.relations.relation.location.keywords.keyword.knowledgegraph.typehierarchy";
        public const string EnrichedUrlEnrichedtitleRelationsRelationTemporalText = "enriched.url.enrichedtitle.relations.relation.temporal.text";
        public const string EnrichedUrlEnrichedtitleRelationsRelationTemporalDecodedType = "enriched.url.enrichedtitle.relations.relation.temporal.decoded.type";
        public const string EnrichedUrlEnrichedtitleRelationsRelationTemporalDecodedValue = "enriched.url.enrichedtitle.relations.relation.temporal.decoded.value";
        public const string EnrichedUrlEnrichedtitleRelationsRelationTemporalDecodedStart = "enriched.url.enrichedtitle.relations.relation.temporal.decoded.start";
        public const string EnrichedUrlEnrichedtitleRelationsRelationTemporalDecodedEnd = "enriched.url.enrichedtitle.relations.relation.temporal.decoded.end";
        public const string EnrichedUrlEnrichedtitleDocsentimentType = "enriched.url.enrichedtitle.docsentiment.type";
        public const string EnrichedUrlEnrichedtitleDocsentimentScore = "enriched.url.enrichedtitle.docsentiment.score";
        public const string EnrichedUrlEnrichedtitleDocsentimentMixed = "enriched.url.enrichedtitle.docsentiment.mixed";
        public const string EnrichedUrlEnrichedtitleTaxonomy = "enriched.url.enrichedtitle.taxonomy";
        public const string EnrichedUrlEnrichedtitleTaxonomyTaxonomyLabel = "enriched.url.enrichedtitle.taxonomy.taxonomy.label";
        public const string EnrichedUrlEnrichedtitleTaxonomyTaxonomyScore = "enriched.url.enrichedtitle.taxonomy.taxonomy.score";
        public const string EnrichedUrlEnrichedtitleTaxonomyTaxonomyConfident = "enriched.url.enrichedtitle.taxonomy.taxonomy.confident";
    }
    #endregion
}
