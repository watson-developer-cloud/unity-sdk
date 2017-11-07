﻿/**
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
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections;
using FullSerializer;
using System;
using System.IO;

public class ExampleAlchemyLanguage : MonoBehaviour
{
    private string _apikey = null;
    private string _url = null;

    private AlchemyAPI _alchemyAPI;

    private string _exampleURL_watsonJeopardy = "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html";
    private string _exampleURL_microformats = "http://microformats.org/wiki/hcard";
    private string _exampleText_watsonJeopardy = "Computer Wins on 'Jeopardy!': Trivial, It's Not\nBy JOHN MARKOFF\nYORKTOWN HEIGHTS, N.Y. — In the end, the humans on \"Jeopardy!\" surrendered meekly.\n\nFacing certain defeat at the hands of a room-size I.B.M. computer on Wednesday evening, Ken Jennings, famous for winning 74 games in a row on the TV quiz show, acknowledged the obvious. \"I, for one, welcome our new computer overlords,\" he wrote on his video screen, borrowing a line from a \"Simpsons\" episode.\n\nFrom now on, if the answer is \"the computer champion on \"Jeopardy!,\" the question will be, \"What is Watson?\"\n\nFor I.B.M., the showdown was not merely a well-publicized stunt and a $1 million prize, but proof that the company has taken a big step toward a world in which intelligent machines will understand and respond to humans, and perhaps inevitably, replace some of them.\n\nWatson, specifically, is a \"question answering machine\" of a type that artificial intelligence researchers have struggled with for decades — a computer akin to the one on \"Star Trek\" that can understand questions posed in natural language and answer them.\n\nWatson showed itself to be imperfect, but researchers at I.B.M. and other companies are already developing uses for Watson's technologies that could have a significant impact on the way doctors practice and consumers buy products.\n\n\"Cast your mind back 20 years and who would have thought this was possible?\" said Edward Feigenbaum, a Stanford University computer scientist and a pioneer in the field.\n\nIn its \"Jeopardy!\" project, I.B.M. researchers were tackling a game that requires not only encyclopedic recall, but also the ability to untangle convoluted and often opaque statements, a modicum of luck, and quick, strategic button pressing.\n\nThe contest, which was taped in January here at the company's T. J. Watson Research Laboratory before an audience of I.B.M. executives and company clients, played out in three televised episodes concluding Wednesday. At the end of the first day, Watson was in a tie with Brad Rutter, another ace human player, at $5,000 each, with Mr. Jennings trailing with $2,000.\n\nBut on the second day, Watson went on a tear. By night's end, Watson had a commanding lead with a total of $35,734, compared with Mr. Rutter's $10,400 and Mr. Jennings's $4,800.\n\nVictory was not cemented until late in the third match, when Watson was in Nonfiction. \"Same category for $1,200,\" it said in a manufactured tenor, and lucked into a Daily Double. Mr. Jennings grimaced.\n\nEven later in the match, however, had Mr. Jennings won another key Daily Double it might have come down to Final Jeopardy, I.B.M. researchers acknowledged.\n\nThe final tally was $77,147 to Mr. Jennings's $24,000 and Mr. Rutter's $21,600.\n\nMore than anything, the contest was a vindication for the academic field of artificial intelligence, which began with great promise in the 1960s with the vision of creating a thinking machine and which became the laughingstock of Silicon Valley in the 1980s, when a series of heavily financed start-up companies went bankrupt.\n\nDespite its intellectual prowess, Watson was by no means omniscient. On Tuesday evening during Final Jeopardy, the category was U.S. Cities and the clue was: \"Its largest airport is named for a World War II hero; its second largest for a World War II battle.\"\n\nWatson drew guffaws from many in the television audience when it responded \"What is Toronto?????\"\n\nThe string of question marks indicated that the system had very low confidence in its response, I.B.M. researchers said, but because it was Final Jeopardy, it was forced to give a response. The machine did not suffer much damage. It had wagered just $947 on its result. (The correct answer is, \"What is Chicago?\")\n\n\"We failed to deeply understand what was going on there,\" said David Ferrucci, an I.B.M. researcher who led the development of Watson. \"The reality is that there's lots of data where the title is U.S. cities and the answers are countries, European cities, people, mayors. Even though it says U.S. cities, we had very little confidence that that's the distinguishing feature.\"\n\nThe researchers also acknowledged that the machine had benefited from the \"buzzer factor.\"\n\nBoth Mr. Jennings and Mr. Rutter are accomplished at anticipating the light that signals it is possible to \"buzz in,\" and can sometimes get in with virtually zero lag time. The danger is to buzz too early, in which case the contestant is penalized and \"locked out\" for roughly a quarter of a second.\n\nWatson, on the other hand, does not anticipate the light, but has a weighted scheme that allows it, when it is highly confident, to hit the buzzer in as little as 10 milliseconds, making it very hard for humans to beat. When it was less confident, it took longer to  buzz in. In the second round, Watson beat the others to the buzzer in 24 out of 30 Double Jeopardy questions.\n\n\"It sort of wants to get beaten when it doesn't have high confidence,\" Dr. Ferrucci said. \"It doesn't want to look stupid.\"\n\nBoth human players said that Watson's button pushing skill was not necessarily an unfair advantage. \"I beat Watson a couple of times,\" Mr. Rutter said.\n\nWhen Watson did buzz in, it made the most of it. Showing the ability to parse language, it responded to, \"A recent best seller by Muriel Barbery is called 'This of the Hedgehog,' \" with \"What is Elegance?\"\n\nIt showed its facility with medical diagnosis. With the answer: \"You just need a nap. You don't have this sleep disorder that can make sufferers nod off while standing up,\" Watson replied, \"What is narcolepsy?\"\n\nThe coup de grâce came with the answer, \"William Wilkenson's 'An Account of the Principalities of Wallachia and Moldavia' inspired this author's most famous novel.\" Mr. Jennings wrote, correctly, Bram Stoker, but realized that he could not catch up with Watson's winnings and wrote out his surrender.\n\nBoth players took the contest and its outcome philosophically.\n\n\"I had a great time and I would do it again in a heartbeat,\" said Mr. Jennings. \"It's not about the results; this is about being part of the future.\"\n\nFor I.B.M., the future will happen very quickly, company executives said. On Thursday it plans to announce that it will collaborate with Columbia University and the University of Maryland to create a physician's assistant service that will allow doctors to query a cybernetic assistant. The company also plans to work with Nuance Communications Inc. to add voice recognition to the physician's assistant, possibly making the service available in as little as 18 months.\n\n\"I have been in medical education for 40 years and we're still a very memory-based curriculum,\" said Dr. Herbert Chase, a professor of clinical medicine at Columbia University who is working with I.B.M. on the physician's assistant. \"The power of Watson- like tools will cause us to reconsider what it is we want students to do.\"\n\nI.B.M. executives also said they are in discussions with a major consumer electronics retailer to develop a version of Watson, named after I.B.M.'s founder, Thomas J. Watson, that would be able to interact with consumers on a variety of subjects like buying decisions and technical support.\n\nDr. Ferrucci sees none of the fears that have been expressed by theorists and science fiction writers about the potential of computers to usurp humans.\n\n\"People ask me if this is HAL,\" he said, referring to the computer in \"2001: A Space Odyssey.\" \"HAL's not the focus; the focus is on the computer on 'Star Trek,' where you have this intelligent information seek dialogue, where you can ask follow-up questions and the computer can look at all the evidence and tries to ask follow-up questions. That's very cool.\"\n\nThis article has been revised to reflect the following correction:\n\nCorrection: February 24, 2011\n\n\nAn article last Thursday about the I.B.M. computer Watson misidentified the academic field vindicated by Watson's besting of two human opponents on \"Jeopardy!\" It is artificial intelligence — not computer science, a broader field that includes artificial intelligence.";
    private string _watson_beats_jeopardy_html;

    private bool _getAuthorsURLTested = false;
    private bool _getAuthorsHTMLTested = false;

    private bool _getRankedConceptsHTMLTested = false;
    private bool _getRankedConceptsURLTested = false;
    private bool _getRankedConceptsTextTested = false;

    private bool _getDatesHTMLTested = false;
    private bool _getDatesURLTested = false;
    private bool _getDatesTextTested = false;

    private bool _getEmotionHTMLTested = false;
    private bool _getEmotionURLTested = false;
    private bool _getEmotionTextTested = false;

    private bool _getEntityExtractionHTMLTested = false;
    private bool _getEntityExtractionURLTested = false;
    private bool _getEntityExtractionTextTested = false;

    private bool _detectFeedsURLTested = false;
    //private bool _detectFeedsHTMLTested = false;

    private bool _getKeywordExtractionHTMLTested = false;
    private bool _getKeywordExtractionURLTested = false;
    private bool _getKeywordExtractionTextTested = false;

    private bool _getLanguageHTMLTested = false;
    private bool _getLanguageURLTested = false;
    private bool _getLanguageTextTested = false;

    private bool _getMicroformatURLTested = false;
    //private bool _getMicroformatHTMLTested = false;

    private bool _getPubDateURLTested = false;
    private bool _getPubDateHTMLTested = false;

    private bool _getRelationsHTMLTested = false;
    private bool _getRelationsURLTested = false;
    private bool _getRelationsTextTested = false;

    private bool _getTextSentimentHTMLTested = false;
    private bool _getTextSentimentURLTested = false;
    private bool _getTextSentimentTextTested = false;

    private bool _getTargetedSentimentHTMLTested = false;
    private bool _getTargetedSentimentURLTested = false;
    private bool _getTargetedSentimentTextTested = false;

    private bool _getRankedTaxonomyHTMLTested = false;
    private bool _getRankedTaxonomyURLTested = false;
    private bool _getRankedTaxonomyTextTested = false;

    private bool _getTextHTMLTested = false;
    private bool _getTextURLTested = false;

    private bool _getRawTextHTMLTested = false;
    private bool _getRawTextURLTested = false;

    private bool _getTitleHTMLTested = false;
    private bool _getTitleURLTested = false;

    private bool _getCombinedDataHTMLTested = false;
    private bool _getCombinedDataURLTested = false;
    private bool _getCombinedDataTextTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_apikey, _url);

        _alchemyAPI = new AlchemyAPI(credentials);

        _watson_beats_jeopardy_html = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
        
        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //  Get Author URL POST
        if (!_alchemyAPI.GetAuthors(OnGetAuthorsUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetAuthors()", "Failed to get authors URL POST!");
        while (!_getAuthorsURLTested)
            yield return null;

        //Get Author HTML POST
        if (!_alchemyAPI.GetAuthors(OnGetAuthorsHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetAuthors()", "Failed to get authors HTML POST!");
        while (!_getAuthorsHTMLTested)
            yield return null;

        ////Get Concepts Text POST
        if (!_alchemyAPI.GetRankedConcepts(OnGetConceptsText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetRankedConcepts()", "Failed to get concepts Text POST!");
        while (!_getRankedConceptsTextTested)
            yield return null;

        //Get Concepts HTML POST
        if (!_alchemyAPI.GetRankedConcepts(OnGetConceptsHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetRankedConcepts()", "Failed to get concepts HTML POST!");
        while (!_getRankedConceptsHTMLTested)
            yield return null;

        //Get Concepts URL POST
        if (!_alchemyAPI.GetRankedConcepts(OnGetConceptsUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetRankedConcepts()", "Failed to get concepts url POST!");
        while (!_getRankedConceptsURLTested)
            yield return null;

        //Get Date URL POST
        if (!_alchemyAPI.GetDates(OnGetDatesUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetDates()", "Failed to get dates by URL POST");
        while (!_getDatesURLTested)
            yield return null;

        //Get Date Text POST
        if (!_alchemyAPI.GetDates(OnGetDatesText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetDates()", "Failed to get dates by text POST");
        while (!_getDatesTextTested)
            yield return null;

        //Get Date HTML POST
        if (!_alchemyAPI.GetDates(OnGetDatesHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetDates()", "Failed to get dates by HTML POST");
        while (!_getDatesHTMLTested)
            yield return null;

        //Get Emotions URL POST
        if (!_alchemyAPI.GetEmotions(OnGetEmotionsUrl, _exampleURL_watsonJeopardy, true))
            Log.Debug("ExampleAlchemyLanguage.GetEmotions()", "Failed to get emotions by URL POST");
        while (!_getEmotionURLTested)
            yield return null;

        //Get Emotions Text POST
        if (!_alchemyAPI.GetEmotions(OnGetEmotionsText, _exampleText_watsonJeopardy, true))
            Log.Debug("ExampleAlchemyLanguage.GetEmotions()", "Failed to get emotions by text POST");
        while (!_getEmotionTextTested)
            yield return null;

        //Get Emotions HTML POST
        if (!_alchemyAPI.GetEmotions(OnGetEmotionsHtml, _watson_beats_jeopardy_html, true))
            Log.Debug("ExampleAlchemyLanguage.GetEmotions()", "Failed to get emotions by HTML POST");
        while (!_getEmotionHTMLTested)
            yield return null;

        //Extract Entities URL POST
        if (!_alchemyAPI.ExtractEntities(OnExtractEntitiesUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.ExtractEntities()", "Failed to get entities by URL POST");
        while (!_getEntityExtractionURLTested)
            yield return null;

        //Extract Entities Text POST
        if (!_alchemyAPI.ExtractEntities(OnExtractEntitiesText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.ExtractEntities()", "Failed to get entities by text POST");
        while (!_getEntityExtractionTextTested)
            yield return null;

        //Extract Entities HTML POST
        if (!_alchemyAPI.ExtractEntities(OnExtractEntitiesHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.ExtractEntities()", "Failed to get entities by HTML POST");
        while (!_getEntityExtractionHTMLTested)
            yield return null;

        //Detect Feeds URL POST
        if (!_alchemyAPI.DetectFeeds(OnDetectFeedsUrl, "http://time.com/newsfeed/"))
            Log.Debug("ExampleAlchemyLanguage.DetectFeeds()", "Failed to get feeds by URL POST");
        while (!_detectFeedsURLTested)
            yield return null;

        ////Detect Feeds HTML POST
        //if (!_alchemyAPI.DetectFeeds(OnDetectFeedsHtml, ycombinator_html))
        //    Log.Debug("ExampleAlchemyLanguage.DetectFeeds()", "Failed to get feeds by HTML POST");
        //while (!_detectFeedsHTMLTested)
        //    yield return null;

        //Extract Keywords URL POST
        if (!_alchemyAPI.ExtractKeywords(OnExtractKeywordsUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.ExtractKeywords()", "Failed to get keywords by URL POST");
        while (!_getKeywordExtractionURLTested)
            yield return null;

        //Extract Keywords Text POST
        if (!_alchemyAPI.ExtractKeywords(OnExtractKeywordsText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.ExtractKeywords()", "Failed to get keywords by text POST");
        while (!_getKeywordExtractionTextTested)
            yield return null;

        //Extract Keywords HTML POST
        if (!_alchemyAPI.ExtractKeywords(OnExtractKeywordsHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.ExtractKeywords()", "Failed to get keywords by HTML POST");
        while (!_getKeywordExtractionHTMLTested)
            yield return null;

        //Extract Languages URL POST
        if (!_alchemyAPI.GetLanguages(OnGetLanguagesUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetLanguages()", "Failed to get languages by text POST");
        while (!_getLanguageURLTested)
            yield return null;

        //Extract Languages Text POST
        if (!_alchemyAPI.GetLanguages(OnGetLanguagesText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetLanguages()", "Failed to get languages by text POST");
        while (!_getLanguageTextTested)
            yield return null;

        //Extract Languages HTML POST
        if (!_alchemyAPI.GetLanguages(OnGetLanguagesHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetLanguages()", "Failed to get languages by HTML POST");
        while (!_getLanguageHTMLTested)
            yield return null;

        //Get Microformats URL POST
        if (!_alchemyAPI.GetMicroformats(OnGetMicroformatsUrl, _exampleURL_microformats))
            Log.Debug("ExampleAlchemyLanguage.GetMicroformats()", "Failed to get microformats by text POST");
        while (!_getMicroformatURLTested)
            yield return null;

        //Get Microformats HTML POST
        //if (!_alchemyAPI.GetMicroformats(OnGetMicroformatsHtml, microformats_html))
        //    Log.Debug("ExampleAlchemyLanguage.GetMicroformats()", "Failed to get microformats by text POST");
        //while (!_getMicroformatHTMLTested)
        //    yield return null;

        //Get PublicationDate URL POST
        if (!_alchemyAPI.GetPublicationDate(OnGetPublicationDateUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetPublicationDate()", "Failed to get publication dates by url POST");
        while (!_getPubDateURLTested)
            yield return null;

        //Get PublicationDate HTML POST
        if (!_alchemyAPI.GetPublicationDate(OnGetPublicationDateHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetPublicationDate()", "Failed to get publication dates by html POST");
        while (!_getPubDateHTMLTested)
            yield return null;

        //Get Relations URL POST
        if (!_alchemyAPI.GetRelations(OnGetRelationsUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetRelations()", "Failed to get relations by text POST");
        while (!_getRelationsURLTested)
            yield return null;

        //Get Relations Text POST
        if (!_alchemyAPI.GetRelations(OnGetRelationsText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetRelations()", "Failed to get relations by text POST");
        while (!_getRelationsTextTested)
            yield return null;

        //Get Relations HTML POST
        if (!_alchemyAPI.GetRelations(OnGetRelationsHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetRelations()", "Failed to get relations by HTML POST");
        while (!_getRelationsHTMLTested)
            yield return null;

        //Get Sentiment URL POST
        if (!_alchemyAPI.GetTextSentiment(OnGetTextSentimentUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetTextSentiment()", "Failed to get sentiment by text POST");
        while (!_getTextSentimentURLTested)
            yield return null;

        //Get Sentiment Text POST
        if (!_alchemyAPI.GetTextSentiment(OnGetTextSentimentText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetTextSentiment()", "Failed to get sentiment by text POST");
        while (!_getTextSentimentTextTested)
            yield return null;

        //Get Sentiment HTML POST
        if (!_alchemyAPI.GetTextSentiment(OnGetTextSentimentHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetTextSentiment()", "Failed to get sentiment by HTML POST");
        while (!_getTextSentimentHTMLTested)
            yield return null;

        //Get Targeted Sentiment URL POST
        if (!_alchemyAPI.GetTargetedSentiment(OnGetTargetedSentimentUrl, _exampleURL_watsonJeopardy, "Jeopardy|Jennings|Watson"))
            Log.Debug("ExampleAlchemyLanguage.GetTargetedSentiment()", "Failed to get targeted sentiment by text POST");
        while (!_getTargetedSentimentURLTested)
            yield return null;

        //Get Targeted Sentiment Text POST
        if (!_alchemyAPI.GetTargetedSentiment(OnGetTargetedSentimentText, _exampleText_watsonJeopardy, "Jeopardy|Jennings|Watson"))
            Log.Debug("ExampleAlchemyLanguage.GetTargetedSentiment()", "Failed to get targeted sentiment by text POST");
        while (!_getTargetedSentimentTextTested)
            yield return null;

        //Get Targeted Sentiment HTML POST
        if (!_alchemyAPI.GetTargetedSentiment(OnGetTargetedSentimentHtml, _watson_beats_jeopardy_html, "Jeopardy|Jennings|Watson"))
            Log.Debug("ExampleAlchemyLanguage.GetTargetedSentiment()", "Failed to get targeted sentiment by HTML POST");
        while (!_getTargetedSentimentHTMLTested)
            yield return null;

        //Get Taxonomy URL POST
        if (!_alchemyAPI.GetRankedTaxonomy(OnGetRankedTaxonomyUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetRankedTaxonomy()", "Failed to get ranked taxonomy by text POST");
        while (!_getRankedTaxonomyURLTested)
            yield return null;

        //Get Taxonomy Text POST
        if (!_alchemyAPI.GetRankedTaxonomy(OnGetRankedTaxonomyText, _exampleText_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetRankedTaxonomy9", "Failed to get ranked taxonomy by text POST");
        while (!_getRankedTaxonomyTextTested)
            yield return null;

        //Get Taxonomy HTML POST
        if (!_alchemyAPI.GetRankedTaxonomy(OnGetRankedTaxonomyHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetRankedTaxonomy()", "Failed to get ranked taxonomy by HTML POST");
        while (!_getRankedTaxonomyHTMLTested)
            yield return null;

        //Get Text HTML POST
        if (!_alchemyAPI.GetText(OnGetTextHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetText()", "Failed to get text by text POST");
        while (!_getTextHTMLTested)
            yield return null;

        //Get Text URL POST
        if (!_alchemyAPI.GetText(OnGetTextUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetText()", "Failed to get text by text POST");
        while (!_getTextURLTested)
            yield return null;

        //Get Raw Text HTML POST
        if (!_alchemyAPI.GetRawText(OnGetRawTextHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetRawText()", "Failed to get raw text by text POST");
        while (!_getRawTextHTMLTested)
            yield return null;

        //Get Raw Text URL POST
        if (!_alchemyAPI.GetRawText(OnGetRawTextUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetRawText()", "Failed to get raw text by text POST");
        while (!_getRawTextURLTested)
            yield return null;

        //Get Title HTML POST
        if (!_alchemyAPI.GetTitle(OnGetTitleHtml, _watson_beats_jeopardy_html))
            Log.Debug("ExampleAlchemyLanguage.GetTitle()", "Failed to get title by text POST");
        while (!_getTitleHTMLTested)
            yield return null;

        //Get Title URL POST
        if (!_alchemyAPI.GetTitle(OnGetTitleUrl, _exampleURL_watsonJeopardy))
            Log.Debug("ExampleAlchemyLanguage.GetTitle()", "Failed to get title by text POST");
        while (!_getTitleURLTested)
            yield return null;

        //  Get Combined Data URL POST
        if (!_alchemyAPI.GetCombinedData(OnGetCombinedDataUrl, _exampleURL_watsonJeopardy, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true))
            Log.Debug("ExampleAlchemyLanguage.GetCombinedData()", "Failed to get combined data by text POST");
        while (!_getCombinedDataURLTested)
            yield return null;

        //Get Combined Data Text POST
        if (!_alchemyAPI.GetCombinedData(OnGetCombinedDataText, _exampleText_watsonJeopardy, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true))
            Log.Debug("ExampleAlchemyLanguage.GetCombinedData()", "Failed to get combined data by text POST");
        while (!_getCombinedDataTextTested)
            yield return null;

        //Get Combined Data HTML POST
        if (!_alchemyAPI.GetCombinedData(OnGetCombinedDataHtml, _watson_beats_jeopardy_html, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true))
            Log.Debug("ExampleAlchemyLanguage.GetCombinedData()", "Failed to get combined data by HTML POST");
        while (!_getCombinedDataHTMLTested)
            yield return null;

        Log.Debug("ExampleAlchemyLanguage.Examples()", "Alchemy Language examples complete");
    }

    private void OnGetAuthorsHtml(RESTConnector.ParsedResponse<AuthorsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetAuthorsHtml()", "Alchemy Language - Get authors response html: {0}", resp.JSON);
        _getAuthorsHTMLTested = true;
    }

    private void OnGetAuthorsUrl(RESTConnector.ParsedResponse<AuthorsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetAuthorsUrl()", "Alchemy Language - Get authors response url: {0}", resp.JSON);
        _getAuthorsURLTested = true;
    }

    private void OnGetConceptsHtml(RESTConnector.ParsedResponse<ConceptsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetAuthorsUrl()", "Alchemy Language - Get ranked concepts response html: {0}", resp.JSON);
        _getRankedConceptsHTMLTested = true;
    }

    private void OnGetConceptsUrl(RESTConnector.ParsedResponse<ConceptsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetConceptsUrl()", "Alchemy Language - Get ranked concepts response url: {0}", resp.JSON);
        _getRankedConceptsURLTested = true;
    }

    private void OnGetConceptsText(RESTConnector.ParsedResponse<ConceptsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetConceptsText()", "Alchemy Language - Get ranked concepts response text: {0}", resp.JSON);
        _getRankedConceptsTextTested = true;
    }

    private void OnGetDatesHtml(RESTConnector.ParsedResponse<DateData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetDatesHtml()", "Alchemy Language - Get dates response html: {0}", resp.JSON);
        _getDatesHTMLTested = true;
    }

    private void OnGetDatesUrl(RESTConnector.ParsedResponse<DateData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetDatesUrl()", "Alchemy Language - Get dates response url: {0}", resp.JSON);
        _getDatesURLTested = true;
    }

    private void OnGetDatesText(RESTConnector.ParsedResponse<DateData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetDatesText()", "Alchemy Language - Get dates response text: {0}", resp.JSON);
        _getDatesTextTested = true;
    }

    private void OnGetEmotionsHtml(RESTConnector.ParsedResponse<EmotionData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetEmotionsHtml()", "Alchemy Language - Get emotions response html: {0}", resp.JSON);
        _getEmotionHTMLTested = true;
    }

    private void OnGetEmotionsUrl(RESTConnector.ParsedResponse<EmotionData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetEmotionsUrl()", "Alchemy Language - Get emotions response html: {0}", resp.JSON);
        _getEmotionURLTested = true;
    }

    private void OnGetEmotionsText(RESTConnector.ParsedResponse<EmotionData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetEmotionsText()", "Alchemy Language - Get emotions response html: {0}", resp.JSON);
        _getEmotionTextTested = true;
    }

    private void OnExtractEntitiesHtml(RESTConnector.ParsedResponse<EntityData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnExtractEntitiesHtml()", "Alchemy Language - Extract entities response html: {0}", resp.JSON);
        _getEntityExtractionHTMLTested = true;
    }

    private void OnExtractEntitiesUrl(RESTConnector.ParsedResponse<EntityData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnExtractEntitiesUrl()", "Alchemy Language - Extract entities response url: {0}", resp.JSON);
        _getEntityExtractionURLTested = true;
    }

    private void OnExtractEntitiesText(RESTConnector.ParsedResponse<EntityData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnExtractEntitiesText()", "Alchemy Language - Extract entities response text: {0}", resp.JSON);
        _getEntityExtractionTextTested = true;
    }

    //private void OnDetectFeedsHtml(RESTConnector.ParsedResponse<FeedData> resp)
    //{
    //    Log.Debug("ExampleAlchemyLanguage.OnDetectFeedsHtml()", "Alchemy Language - Detect feeds response html: {0}", resp.JSON);
    //    _detectFeedsHTMLTested = true;
    //}

    private void OnDetectFeedsUrl(RESTConnector.ParsedResponse<FeedData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnDetectFeedsUrl()", "Alchemy Language - Detect feeds response url: {0}", resp.JSON);
        _detectFeedsURLTested = true;
    }

    private void OnExtractKeywordsHtml(RESTConnector.ParsedResponse<KeywordData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnExtractKeywordsHtml()", "Alchemy Language - Extract keywords response html: {0}", resp.JSON);
        _getKeywordExtractionHTMLTested = true;
    }

    private void OnExtractKeywordsUrl(RESTConnector.ParsedResponse<KeywordData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnExtractKeywordsUrl()", "Alchemy Language - Extract keywords response url: {0}", resp.JSON);
        _getKeywordExtractionURLTested = true;
    }

    private void OnExtractKeywordsText(RESTConnector.ParsedResponse<KeywordData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnExtractKeywordsText()", "Alchemy Language - Extract keywords response text: {0}", resp.JSON);
        _getKeywordExtractionTextTested = true;
    }

    private void OnGetLanguagesHtml(RESTConnector.ParsedResponse<LanguageData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetLanguagesHtml()", "Alchemy Language - Get languages response html: {0}", resp.JSON);
        _getLanguageHTMLTested = true;
    }

    private void OnGetLanguagesUrl(RESTConnector.ParsedResponse<LanguageData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetLanguagesUrl()", "Alchemy Language - Get languages response url: {0}", resp.JSON);
        _getLanguageURLTested = true;
    }

    private void OnGetLanguagesText(RESTConnector.ParsedResponse<LanguageData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetLanguagesText()", "Alchemy Language - Get languages response text: {0}", resp.JSON);
        _getLanguageTextTested = true;
    }

    //private void OnGetMicroformatsHtml(RESTConnector.ParsedResponse<MicroformatData> resp)
    //{
    //    Log.Debug("ExampleAlchemyLanguage.OnGetMicroformatsHtml()", "Alchemy Language - Get microformats response html: {0}", resp.JSON);
    //    _getMicroformatHTMLTested = true;
    //}

    private void OnGetMicroformatsUrl(RESTConnector.ParsedResponse<MicroformatData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetMicroformatsUrl()", "Alchemy Language - Get microformats response url: {0}", resp.JSON);
        _getMicroformatURLTested = true;
    }

    private void OnGetPublicationDateHtml(RESTConnector.ParsedResponse<PubDateData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetPublicationDateHtml()", "Alchemy Language - Get publication date response html: {0}", resp.JSON);
        _getPubDateHTMLTested = true;
    }

    private void OnGetPublicationDateUrl(RESTConnector.ParsedResponse<PubDateData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetPublicationDateUrl()", "Alchemy Language - Get publication date response url: {0}", resp.JSON);
        _getPubDateURLTested = true;
    }
    
    private void OnGetRelationsHtml(RESTConnector.ParsedResponse<RelationsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRelationsHtml()", "Alchemy Language - Get relations response html: {0}", resp.JSON);
        _getRelationsHTMLTested = true;
    }

    private void OnGetRelationsUrl(RESTConnector.ParsedResponse<RelationsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRelationsUrl()", "Alchemy Language - Get relations response url: {0}", resp.JSON);
        _getRelationsURLTested = true;
    }

    private void OnGetRelationsText(RESTConnector.ParsedResponse<RelationsData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRelationsText()", "Alchemy Language - Get relations response text: {0}", resp.JSON);
        _getRelationsTextTested = true;
    }

    private void OnGetTextSentimentHtml(RESTConnector.ParsedResponse<SentimentData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTextSentimentHtml()", "Alchemy Language - Get text sentiment response html: {0}", resp.JSON);
        _getTextSentimentHTMLTested = true;
    }

    private void OnGetTextSentimentUrl(RESTConnector.ParsedResponse<SentimentData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTextSentimentUrl()", "Alchemy Language - Get text sentiment response url: {0}", resp.JSON);
        _getTextSentimentURLTested = true;
    }

    private void OnGetTextSentimentText(RESTConnector.ParsedResponse<SentimentData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTextSentimentText()", "Alchemy Language - Get text sentiment response text: {0}", resp.JSON);
        _getTextSentimentTextTested = true;
    }

    private void OnGetTargetedSentimentHtml(RESTConnector.ParsedResponse<TargetedSentimentData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTargetedSentimentHtml()", "Alchemy Language - Get targeted sentiment response html: {0}", resp.JSON);
        _getTargetedSentimentHTMLTested = true;
    }

    private void OnGetTargetedSentimentUrl(RESTConnector.ParsedResponse<TargetedSentimentData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTargetedSentimentUrl()", "Alchemy Language - Get targeted sentiment response url: {0}", resp.JSON);
        _getTargetedSentimentURLTested = true;
    }

    private void OnGetTargetedSentimentText(RESTConnector.ParsedResponse<TargetedSentimentData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTargetedSentimentText()", "Alchemy Language - Get targeted sentiment response text: {0}", resp.JSON);
        _getTargetedSentimentTextTested = true;
    }

    private void OnGetRankedTaxonomyHtml(RESTConnector.ParsedResponse<TaxonomyData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRankedTaxonomyHtml()", "Alchemy Language - Get ranked taxonomy response html: {0}", resp.JSON);
        _getRankedTaxonomyHTMLTested = true;
    }

    private void OnGetRankedTaxonomyUrl(RESTConnector.ParsedResponse<TaxonomyData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRankedTaxonomyUrl()", "Alchemy Language - Get ranked taxonomy response url: {0}", resp.JSON);
        _getRankedTaxonomyURLTested = true;
    }

    private void OnGetRankedTaxonomyText(RESTConnector.ParsedResponse<TaxonomyData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRankedTaxonomyText()", "Alchemy Language - Get ranked taxonomy response text: {0}", resp.JSON);
        _getRankedTaxonomyTextTested = true;
    }

    private void OnGetTextHtml(RESTConnector.ParsedResponse<TextData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTextHtml()", "Alchemy Language - Get Text HTML response: {0}", resp.JSON);
        _getTextHTMLTested = true;
    }

    private void OnGetTextUrl(RESTConnector.ParsedResponse<TextData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTextUrl()", "Alchemy Language - Get Text Url response: {0}", resp.JSON);
        _getTextURLTested = true;
    }

    private void OnGetRawTextHtml(RESTConnector.ParsedResponse<TextData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRawTextHtml()", "Alchemy Language - Get raw text HTML response: {0}", resp.JSON);
        _getRawTextHTMLTested = true;
    }

    private void OnGetRawTextUrl(RESTConnector.ParsedResponse<TextData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetRawTextUrl()", "Alchemy Language - Get raw text Url response: {0}", resp.JSON);
        _getRawTextURLTested = true;
    }

    private void OnGetTitleHtml(RESTConnector.ParsedResponse<Title> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTitleHtml()", "Alchemy Language - Get Title HTML response: {0}", resp.JSON);
        _getTitleHTMLTested = true;
    }

    private void OnGetTitleUrl(RESTConnector.ParsedResponse<Title> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetTitleUrl()", "Alchemy Language - Get Title Url response: {0}", resp.JSON);
        _getTitleURLTested = true;
    }

    private void OnGetCombinedDataHtml(RESTConnector.ParsedResponse<CombinedCallData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetCombinedDataHtml()", "Alchemy Language - Get Combined Data HTML response: {0}", resp.JSON);
        _getCombinedDataHTMLTested = true;
    }

    private void OnGetCombinedDataUrl(RESTConnector.ParsedResponse<CombinedCallData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetCombinedDataUrl()", "Alchemy Language - Get Combined Data Url response: {0}", resp.JSON);
        _getCombinedDataURLTested = true;
    }

    private void OnGetCombinedDataText(RESTConnector.ParsedResponse<CombinedCallData> resp)
    {
        Log.Debug("ExampleAlchemyLanguage.OnGetCombinedDataText()", "Alchemy Language - Get Combined Data Text response: {0}", resp.JSON);
        _getCombinedDataTextTested = true;
    }
}
