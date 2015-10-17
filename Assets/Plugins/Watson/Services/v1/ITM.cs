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


using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.Services.v1
{
    /// <summary>
    /// This class wraps the ITM back-end service.
    /// </summary>
    public class ITM
    {
        #region Public Types
        public enum WordPosition
        {
            INVALID = -1,
            NOUN,
            PRONOUN,
            ADJECTIVE,
            DETERMINIER,
            VERB,
            ADVERB,
            PREPOSITION,
            CONJUNCTION,
            INTERJECTION
        };
        public class ParseWord
        {
            public string Word { get; set; }
            public WordPosition Pos { get; set; }
            public string Slot { get; set; }
            public string [] Features { get; set; }

            public string PosName {
                set {
                    WordPosition pos = WordPosition.INVALID;
                    if (! sm_WordPositions.TryGetValue( value, out pos ) )
                        Log.Error( "ITM", "Failed to find position type for {0}, Word: {1}", value, Word );
                    Pos = pos;
                }
            }
        };
        public class ParseData
        {
            public string Id { get; set; }
            public string Rev { get; set; }
            public long TransactionId { get; set; }
            public ParseWord [] Words { get; set; }
            public string [] Heirarchy { get; set; }
            public string [] Flags { get; set; }
        };
        public delegate void OnParseData( ParseData data );
        #endregion

        #region Private Data
        private static Dictionary<string,WordPosition> sm_WordPositions = new Dictionary<string, WordPosition>()
        {
            { "noun", WordPosition.NOUN },
            { "pronoun", WordPosition.PRONOUN },        // ?
            { "adj", WordPosition.ADJECTIVE },
            { "det", WordPosition.DETERMINIER },
            { "verb", WordPosition.VERB },
            { "adverb", WordPosition.ADVERB },          // ?
            { "prep", WordPosition.PREPOSITION },
            { "conj", WordPosition.CONJUNCTION },       // ?
            { "inter", WordPosition.INTERJECTION },     // ?
        };
        private const string SERVICE_ID = "ITMV1";
        private const string TEST_PARSE_DATA = "{\"_id\":\"47f26baa682b4c939cda4164f5b9059b\",\"_rev\":\"1-c353e16c83c2a903f367c30042a4eba1\",\"transactionId\":-1773927182,\"parse\":{\"pos\":[{\"text\":\"What\",\"value\":\"noun\"},{\"text\":\"is\",\"value\":\"verb\"},{\"text\":\"the\",\"value\":\"det\"},{\"text\":\"best\",\"value\":\"adj\"},{\"text\":\"treatment\",\"value\":\"noun\"},{\"text\":\"for\",\"value\":\"prep\"},{\"text\":\"an\",\"value\":\"det\"},{\"text\":\"african american\",\"value\":\"noun\"},{\"text\":\"male\",\"value\":\"noun\"},{\"text\":\"with\",\"value\":\"prep\"},{\"text\":\"heart\",\"value\":\"noun\"},{\"text\":\"failure\",\"value\":\"noun\"}],\"slot\":[{\"text\":\"What\",\"value\":\"subj\"},{\"text\":\"is\",\"value\":\"top\"},{\"text\":\"the\",\"value\":\"ndet\"},{\"text\":\"best\",\"value\":\"nadj\"},{\"text\":\"treatment\",\"value\":\"pred\"},{\"text\":\"for\",\"value\":\"ncomp\"},{\"text\":\"an\",\"value\":\"ndet\"},{\"text\":\"african american\",\"value\":\"nadj\"},{\"text\":\"male\",\"value\":\"objprep\"},{\"text\":\"with\",\"value\":\"nprep\"},{\"text\":\"heart\",\"value\":\"nnoun\"},{\"text\":\"failure\",\"value\":\"objprep\"}],\"features\":[{\"text\":\"What\",\"value\":[\"pron\",\"sg\",\"wh\",\"whnom\"]},{\"text\":\"is\",\"value\":[\"vfin\",\"vpres\",\"sg\",\"wh\",\"whnom\",\"vsubj\",\"absubj\",\"auxv\"]},{\"text\":\"the\",\"value\":[\"sg\",\"def\",\"the\",\"ingdet\"]},{\"text\":\"best\",\"value\":[\"superl\",\"adjnoun\"]},{\"text\":\"treatment\",\"value\":[\"cn\",\"sg\",\"evnt\",\"act\",\"abst\",\"cognsa\",\"activity\",\"groupact\",\"(latrwd 0.051600)\",\"(vform treat)\"]},{\"text\":\"for\",\"value\":[\"pprefv\",\"nonlocp\",\"pobjp\"]},{\"text\":\"an\",\"value\":[\"sg\",\"indef\"]},{\"text\":\"african american\",\"value\":[\"propn\",\"sg\",\"glom\",\"notfnd\",\"unkph\"]},{\"text\":\"male\",\"value\":[\"cn\",\"sg\",\"m\",\"h\",\"physobj\",\"anim\",\"anml\",\"liv\",\"(latrwd 0.051600)\"]},{\"text\":\"with\",\"value\":[\"pprefv\",\"nonlocp\"]},{\"text\":\"heart\",\"value\":[\"cn\",\"sg\",\"abst\",\"cognsa\",\"(latrwd 0.032630)\"]},{\"text\":\"failure\",\"value\":[\"cn\",\"sg\",\"abst\",\"massn\",\"illness\",\"cond\",\"state\",\"(* heart failure)\",\"(vform fail)\"]}],\"hierarchy\":[\"african american\"],\"words\":[\"What\",\"is\",\"the\",\"best\",\"treatment\",\"for\",\"an\",\"african american\",\"male\",\"with\",\"heart\",\"failure\"],\"flags\":[\"african american\"]},\"preContext\":0,\"postContext\":0,\"sessionKey\":null}";
        #endregion

        #region Question and Answer Functions


        /// <summary>
        /// This returns the parse data for specific transaction ID.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool GetParseData( long transactionId, OnParseData callback )
        {
            if ( transactionId == 0 )
                throw new ArgumentNullException( "transactionId" );
            if (callback == null )
                throw new ArgumentNullException("callback");

            callback( CreateParseData( TEST_PARSE_DATA ) );

            return true;
        }

        private ParseData CreateParseData( string jsonResponse )
        {
            ParseData parse = new ParseData();
            try {

                IDictionary json = Json.Deserialize( jsonResponse ) as IDictionary;
                parse.Id = (string)json["_id"];
                parse.Rev = (string)json["_rev"];
                parse.TransactionId = (long)json["transactionId"];

                IDictionary iparse = (IDictionary)json["parse"];
                List<string> heirarchy = new List<string>();
                IList iheirarchy = (IList)iparse["hierarchy"];
                foreach( var h in iheirarchy )
                    heirarchy.Add( (string)h );
                parse.Heirarchy = heirarchy.ToArray();

                List<string> flags = new List<string>();
                IList iflags = (IList)iparse["flags"];
                foreach( var f in iflags )
                    flags.Add( (string)f );
                parse.Flags = flags.ToArray();

                List<ParseWord> words = new List<ParseWord>();

                IList iWords = (IList)iparse["words"];
                for(int i=0;i<iWords.Count;++i)
                {
                    ParseWord word = new ParseWord();
                    word.Word = (string)iWords[i];
                    
                    IList iPos = (IList)iparse["pos"];
                    if ( iPos.Count != iWords.Count )
                        throw new WatsonException( "ipos.Count != iwords.Count" );
                    word.PosName = (string)((IDictionary)iPos[i])["value"]; 

                    IList iSlots = (IList)iparse["slot"];
                    if (iSlots.Count != iWords.Count )
                        throw new WatsonException( "islots.Count != iwords.Count" );
                    word.Slot = (string)((IDictionary)iSlots[i])["value"];

                    IList iFeatures = (IList)iparse["features"];
                    if ( iFeatures.Count != iWords.Count )
                        throw new WatsonException( "ifeatures.Count != iwords.Count" );
                    
                    List<string> features = new List<string>();
                    IList iWordFeatures = (IList)((IDictionary)iFeatures[i])["value"];
                    foreach( var k in iWordFeatures )
                        features.Add( (string)k );
                    word.Features = features.ToArray();                            
                                                          
                    words.Add( word );                    
                }

                parse.Words = words.ToArray();
            }
            catch( Exception e )
            {
                Log.Error( "ITM", "Exception during parse: {0}", e.ToString() );
                parse = null;
            }

            return parse;
        }
        #endregion
    }

}

