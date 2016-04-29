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

    [fsObject]
    public class EntityExtractionData
    {
        public string status { get; set; }
        public string language { get; set; }
        public string url { get; set; }
        public string text { get; set; }
        public Entity[] entities { get; set; }
        public KnowledgeGraph knowledgeGraph { get; set; }

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

        private PositionOnMap m_GeoLocation = null;
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
    public class KeywoardExtractionData
    {
        public string status { get; set; }
        public string language { get; set; }
        public string url { get; set; }
        public string text { get; set; }
        public Keyword[] keywords { get; set; }
        public KnowledgeGraph knowledgeGraph { get; set; }

        public bool HasData
        {
            get
            {
                return keywords != null && keywords.Length > 0;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int indexKeyword = 0; keywords != null && indexKeyword < keywords.Length; indexKeyword++)
            {
                stringBuilder.Append("\n\t");
                stringBuilder.Append(keywords[indexKeyword].ToString());
            }
            return string.Format("[KeywoardExtractionData: status={0}, language={1}, url={2}, text={3}, keywords={4}]", status, language, url, text, stringBuilder.ToString());
        }

    };


    public class PositionOnMap
    {
        public string PositionName;
        public double Latitude;    //Y : North - south
        public double Longitude;   //X : West - East

        public double X { get { return Longitude; } }
        public double Y { get { return Latitude; } }

        public PositionOnMap(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public PositionOnMap(double latitude, double longitude, string positionName)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.PositionName = positionName;
        }

        public override string ToString()
        {
            return string.Format("[PositionOnMap: Name: {0}, Latitude:{1}, Longitude:{2}]", PositionName, Latitude.ToString(), Longitude.ToString());
        }
    }


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

    [fsObject]
    public class Entity
    {
        public string type { get; set; }
        public string relevance { get; set; }
        public KnowledgeGraph knowledgeGraph { get; set; }
        public string count { get; set; }
        public string text { get; set; }
        public Disambiguated disambiguated { get; set; }
        public Quotation[] quotations { get; set; }
        public Sentiment sentiment { get; set; }

        private EntityPrimaryType _EntityType = EntityPrimaryType.NONE;
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

        private double m_Score = 0;
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

        public override string ToString()
        {
            return string.Format("[Sentiment: type={0}, score={1}, mixed={2}]", type, score, mixed);
        }
    };

    [fsObject]
    public class CombinedCallData
    {
        public string status { get; set; }
        public string totalTransactions { get; set; }
        public string language { get; set; }
        public string text { get; set; }
        public Keyword[] keywords { get; set; }
        public Entity[] entities { get; set; }
        public Sentiment docSentiment { get; set; }
        public Concept[] concepts { get; set; }
        public Relation[] relations { get; set; }
        public Taxonomy[] taxonomy { get; set; }
        public DocEmotions[] docEmotions { get; set; }
        public DateData[] dates { get; set; }

        public bool HasData
        {
            get
            {
                return EntityCombined != null && EntityCombined.Count > 0;
            }
        }

        private List<string> _EntityCombined = null;
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

        public string EntityCombinedCommaSeperated
        {
            get
            {
                if (EntityCombined.Count > 0)
                    return string.Join(",", EntityCombined.ToArray());
                return "";
            }
        }

        public string ToLongString()
        {
            StringBuilder stringBuilder = new StringBuilder(string.Format("[CombinedCallData: status={0}, totalTransactions={1}, language={2}, text={3}", status, totalTransactions, language, text));

            stringBuilder.Append(EntityCombinedCommaSeperated);
            for (int i = 0; dates != null && i < dates.Length; i++)
            {
                stringBuilder.Append(" Date: " + dates[i].DateValue.ToString());
            }

            return stringBuilder.ToString();
        }

    };


    [fsObject]
    public class Keyword
    {
        public string text { get; set; }
        public string relevance { get; set; }
        public KnowledgeGraph knowledgeGraph { get; set; }
        public Sentiment sentiment { get; set; }

    };


    [fsObject]
    public class Concept
    {
        public string text { get; set; }
        public string relevance { get; set; }
        public KnowledgeGraph knowledgeGraph { get; set; }
        public string website { get; set; }
        public string geo { get; set; }
        public string dbpedia { get; set; }
        public string freebase { get; set; }
        public string yago { get; set; }
        public string opencyc { get; set; }
        public string ciaFactbook { get; set; }
        public string census { get; set; }
        public string geonames { get; set; }
        public string musicBrainz { get; set; }
        public string crunchbase { get; set; }
    };

    [fsObject]
    public class Relation
    {
        public string sentence { get; set; }
        public Subject subject { get; set; }
        public Entity entity { get; set; }
        public Action action { get; set; }
        public ObjectData @object { get; set; }
    };

    [fsObject]
    public class Subject
    {
        public string text { get; set; }
        public Sentiment sentiment { get; set; }
    };

    [fsObject]
    public class Action
    {
        public string text { get; set; }
        public string lemmatized { get; set; }
        public Verb verb { get; set; }
    };

    [fsObject]
    public class Verb
    {
        public string text { get; set; }
        public string tense { get; set; }
        public string negated { get; set; }
    };

    [fsObject]
    public class ObjectData
    {
        public string text { get; set; }
        public Sentiment sentiment { get; set; }
        public Sentiment sentimentFromSubject { get; set; }
        public Entity entity { get; set; }
    };

    [fsObject]
    public class Taxonomy
    {
        public string label { get; set; }
        public string score { get; set; }
        public string confident { get; set; }
    };

    [fsObject]
    public class DocEmotions
    {
        public string anger { get; set; }
        public string disgust { get; set; }
        public string fear { get; set; }
        public string joy { get; set; }
        public string sadness { get; set; }
    };

    [fsObject]
    public class DateData
    {
        public string text { get; set; }
        public string date { get; set; }

        private System.DateTime m_dateValue = default(System.DateTime);
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

}
