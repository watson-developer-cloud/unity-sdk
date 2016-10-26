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

using System.Collections;
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using System.Collections.Generic;

#pragma warning disable 219
#pragma warning disable 0414

namespace IBM.Watson.DeveloperCloud.UnitTests
{
  public class TestAlchemyAPI : UnitTest
  {
    AlchemyAPI m_AlchemyAPI = new AlchemyAPI();
    bool m_GetAuthorsURLTested = false;
    bool m_GetAuthorsHTMLTested = false;

    bool m_GetRankedConceptsHTMLTested = false;
    bool m_GetRankedConceptsURLTested = false;
    bool m_GetRankedConceptsTextTested = false;

    bool m_GetDatesHTMLTested = false;
    bool m_GetDatesURLTested = false;
    bool m_GetDatesTextTested = false;

    bool m_GetEmotionHTMLTested = false;
    bool m_GetEmotionURLTested = false;
    bool m_GetEmotionTextTested = false;

    bool m_GetEntityExtractionHTMLTested = false;
    bool m_GetEntityExtractionURLTested = false;
    bool m_GetEntityExtractionTextTested = false;

    bool m_DetectFeedsURLTested = false;
    bool m_DetectFeedsHTMLTested = false;

    bool m_GetKeywordExtractionHTMLTested = false;
    bool m_GetKeywordExtractionURLTested = false;
    bool m_GetKeywordExtractionTextTested = false;

    bool m_GetLanguageHTMLTested = false;
    bool m_GetLanguageURLTested = false;
    bool m_GetLanguageTextTested = false;

    bool m_GetMicroformatURLTested = false;
    bool m_GetMicroformatHTMLTested = false;

    bool m_GetPubDateURLTested = false;
    bool m_GetPubldateHTMLTested = false;

    bool m_GetRelationsHTMLTested = false;
    bool m_GetRelationsURLTested = false;
    bool m_GetRelationsTextTested = false;

    bool m_GetTextSentimentHTMLTested = false;
    bool m_GetTextSentimentURLTested = false;
    bool m_GetTextSentimentTextTested = false;

    bool m_GetTargetedSentimentHTMLTested = false;
    bool m_GetTargetedSentimentURLTested = false;
    bool m_GetTargetedSentimentTextTested = false;

    bool m_GetRankedTaxonomyHTMLTested = false;
    bool m_GetRankedTaxonomyURLTested = false;
    bool m_GetRankedTaxonomyTextTested = false;

    bool m_GetTextHTMLTested = false;
    bool m_GetTextURLTested = false;

    bool m_GetRawTextHTMLTested = false;
    bool m_GetRawTextURLTested = false;

    bool m_GetTitleHTMLTested = false;
    bool m_GetTitleURLTested = false;

    bool m_GetCombinedDataHTMLTested = false;
    bool m_GetCombinedDataURLTested = false;
    bool m_GetCombinedDataTextTested = false;

    bool m_GetNewsTested = false;

    private string m_ExampleURL_article = "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html";
    private string m_ExampleURL_microformats = "http://microformats.org/wiki/hcard";
    private string m_ExampleURL_feed = "https://news.ycombinator.com/";
    private string m_ExampleText_article = "Computer Wins on 'Jeopardy!': Trivial, It's Not\nBy JOHN MARKOFF\nYORKTOWN HEIGHTS, N.Y. — In the end, the humans on \"Jeopardy!\" surrendered meekly.\n\nFacing certain defeat at the hands of a room-size I.B.M. computer on Wednesday evening, Ken Jennings, famous for winning 74 games in a row on the TV quiz show, acknowledged the obvious. \"I, for one, welcome our new computer overlords,\" he wrote on his video screen, borrowing a line from a \"Simpsons\" episode.\n\nFrom now on, if the answer is \"the computer champion on \"Jeopardy!,\" the question will be, \"What is Watson?\"\n\nFor I.B.M., the showdown was not merely a well-publicized stunt and a $1 million prize, but proof that the company has taken a big step toward a world in which intelligent machines will understand and respond to humans, and perhaps inevitably, replace some of them.\n\nWatson, specifically, is a \"question answering machine\" of a type that artificial intelligence researchers have struggled with for decades — a computer akin to the one on \"Star Trek\" that can understand questions posed in natural language and answer them.\n\nWatson showed itself to be imperfect, but researchers at I.B.M. and other companies are already developing uses for Watson's technologies that could have a significant impact on the way doctors practice and consumers buy products.\n\n\"Cast your mind back 20 years and who would have thought this was possible?\" said Edward Feigenbaum, a Stanford University computer scientist and a pioneer in the field.\n\nIn its \"Jeopardy!\" project, I.B.M. researchers were tackling a game that requires not only encyclopedic recall, but also the ability to untangle convoluted and often opaque statements, a modicum of luck, and quick, strategic button pressing.\n\nThe contest, which was taped in January here at the company's T. J. Watson Research Laboratory before an audience of I.B.M. executives and company clients, played out in three televised episodes concluding Wednesday. At the end of the first day, Watson was in a tie with Brad Rutter, another ace human player, at $5,000 each, with Mr. Jennings trailing with $2,000.\n\nBut on the second day, Watson went on a tear. By night's end, Watson had a commanding lead with a total of $35,734, compared with Mr. Rutter's $10,400 and Mr. Jennings's $4,800.\n\nVictory was not cemented until late in the third match, when Watson was in Nonfiction. \"Same category for $1,200,\" it said in a manufactured tenor, and lucked into a Daily Double. Mr. Jennings grimaced.\n\nEven later in the match, however, had Mr. Jennings won another key Daily Double it might have come down to Final Jeopardy, I.B.M. researchers acknowledged.\n\nThe final tally was $77,147 to Mr. Jennings's $24,000 and Mr. Rutter's $21,600.\n\nMore than anything, the contest was a vindication for the academic field of artificial intelligence, which began with great promise in the 1960s with the vision of creating a thinking machine and which became the laughingstock of Silicon Valley in the 1980s, when a series of heavily financed start-up companies went bankrupt.\n\nDespite its intellectual prowess, Watson was by no means omniscient. On Tuesday evening during Final Jeopardy, the category was U.S. Cities and the clue was: \"Its largest airport is named for a World War II hero; its second largest for a World War II battle.\"\n\nWatson drew guffaws from many in the television audience when it responded \"What is Toronto?????\"\n\nThe string of question marks indicated that the system had very low confidence in its response, I.B.M. researchers said, but because it was Final Jeopardy, it was forced to give a response. The machine did not suffer much damage. It had wagered just $947 on its result. (The correct answer is, \"What is Chicago?\")\n\n\"We failed to deeply understand what was going on there,\" said David Ferrucci, an I.B.M. researcher who led the development of Watson. \"The reality is that there's lots of data where the title is U.S. cities and the answers are countries, European cities, people, mayors. Even though it says U.S. cities, we had very little confidence that that's the distinguishing feature.\"\n\nThe researchers also acknowledged that the machine had benefited from the \"buzzer factor.\"\n\nBoth Mr. Jennings and Mr. Rutter are accomplished at anticipating the light that signals it is possible to \"buzz in,\" and can sometimes get in with virtually zero lag time. The danger is to buzz too early, in which case the contestant is penalized and \"locked out\" for roughly a quarter of a second.\n\nWatson, on the other hand, does not anticipate the light, but has a weighted scheme that allows it, when it is highly confident, to hit the buzzer in as little as 10 milliseconds, making it very hard for humans to beat. When it was less confident, it took longer to  buzz in. In the second round, Watson beat the others to the buzzer in 24 out of 30 Double Jeopardy questions.\n\n\"It sort of wants to get beaten when it doesn't have high confidence,\" Dr. Ferrucci said. \"It doesn't want to look stupid.\"\n\nBoth human players said that Watson's button pushing skill was not necessarily an unfair advantage. \"I beat Watson a couple of times,\" Mr. Rutter said.\n\nWhen Watson did buzz in, it made the most of it. Showing the ability to parse language, it responded to, \"A recent best seller by Muriel Barbery is called 'This of the Hedgehog,' \" with \"What is Elegance?\"\n\nIt showed its facility with medical diagnosis. With the answer: \"You just need a nap. You don't have this sleep disorder that can make sufferers nod off while standing up,\" Watson replied, \"What is narcolepsy?\"\n\nThe coup de grâce came with the answer, \"William Wilkenson's 'An Account of the Principalities of Wallachia and Moldavia' inspired this author's most famous novel.\" Mr. Jennings wrote, correctly, Bram Stoker, but realized that he could not catch up with Watson's winnings and wrote out his surrender.\n\nBoth players took the contest and its outcome philosophically.\n\n\"I had a great time and I would do it again in a heartbeat,\" said Mr. Jennings. \"It's not about the results; this is about being part of the future.\"\n\nFor I.B.M., the future will happen very quickly, company executives said. On Thursday it plans to announce that it will collaborate with Columbia University and the University of Maryland to create a physician's assistant service that will allow doctors to query a cybernetic assistant. The company also plans to work with Nuance Communications Inc. to add voice recognition to the physician's assistant, possibly making the service available in as little as 18 months.\n\n\"I have been in medical education for 40 years and we're still a very memory-based curriculum,\" said Dr. Herbert Chase, a professor of clinical medicine at Columbia University who is working with I.B.M. on the physician's assistant. \"The power of Watson- like tools will cause us to reconsider what it is we want students to do.\"\n\nI.B.M. executives also said they are in discussions with a major consumer electronics retailer to develop a version of Watson, named after I.B.M.'s founder, Thomas J. Watson, that would be able to interact with consumers on a variety of subjects like buying decisions and technical support.\n\nDr. Ferrucci sees none of the fears that have been expressed by theorists and science fiction writers about the potential of computers to usurp humans.\n\n\"People ask me if this is HAL,\" he said, referring to the computer in \"2001: A Space Odyssey.\" \"HAL's not the focus; the focus is on the computer on 'Star Trek,' where you have this intelligent information seek dialogue, where you can ask follow-up questions and the computer can look at all the evidence and tries to ask follow-up questions. That's very cool.\"\n\nThis article has been revised to reflect the following correction:\n\nCorrection: February 24, 2011\n\n\nAn article last Thursday about the I.B.M. computer Watson misidentified the academic field vindicated by Watson's besting of two human opponents on \"Jeopardy!\" It is artificial intelligence — not computer science, a broader field that includes artificial intelligence.";

    public override IEnumerator RunTest()
    {
      string example_article_html = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";
      string example_microformats_html = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/microformats.html";
      string example_feed_html = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/ycombinator_news.html";

      Log.Debug("TestAlchemyAPI", "Attempting GetAuthors URL");
      m_AlchemyAPI.GetAuthors(OnGetAuthorsURL, m_ExampleURL_article, "OnGetAuthorsURL");
      while (!m_GetAuthorsURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetAuthors HTML");
      m_AlchemyAPI.GetAuthors(OnGetAuthorsHTML, example_article_html, "OnGetAuthorsHTML");
      while (!m_GetAuthorsHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetRankedConcepts HTML");
      m_AlchemyAPI.GetRankedConcepts(OnGetRankedConceptsHTML, example_article_html, 8, true, true, true, "OnGetRankedConceptsHTML");
      while (!m_GetRankedConceptsHTMLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetRankedConcepts URL");
      m_AlchemyAPI.GetRankedConcepts(OnGetRankedConceptsURL, m_ExampleURL_article, 8, true, true, true, "OnGetRankedConceptsURL");
      while (!m_GetRankedConceptsHTMLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetRankedConcepts Text");
      m_AlchemyAPI.GetRankedConcepts(OnGetRankedConceptsText, m_ExampleText_article, 8, true, true, true, "OnGetRankedConceptsText");
      while (!m_GetRankedConceptsHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetDates URL");
      m_AlchemyAPI.GetDates(OnGetDatesURL, m_ExampleURL_article, null, true, "OnGetDatesURL");
      while (!m_GetDatesURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetDates Text");
      m_AlchemyAPI.GetDates(OnGetDatesText, m_ExampleText_article, null, true, "OnGetDatesText");
      while (!m_GetDatesTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetDates HTML");
      m_AlchemyAPI.GetDates(OnGetDatesHTML, example_article_html, null, true, "OnGetDatesHTML");
      while (!m_GetDatesHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetDates URL");
      m_AlchemyAPI.GetEmotions(OnGetEmotionsURL, m_ExampleURL_article, true, "OnGetEmotionsURL");
      while (!m_GetEmotionURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetDates Text");
      m_AlchemyAPI.GetEmotions(OnGetEmotionsText, m_ExampleText_article, true, "OnGetEmotionsText");
      while (!m_GetEmotionTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetDates HTML");
      m_AlchemyAPI.GetEmotions(OnGetEmotionsHTML, example_article_html, true, "OnGetEmotionsHTML");
      while (!m_GetEmotionHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting ExtractEntities URL");
      m_AlchemyAPI.ExtractEntities(OnExtractEntitiesURL, m_ExampleURL_article);
      while (!m_GetEntityExtractionURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting ExtractEntities Text");
      m_AlchemyAPI.ExtractEntities(OnExtractEntitiesText, m_ExampleText_article, 50, true, true, true, true, true, true, true, true, "OnExtractEntitiesText");
      while (!m_GetEntityExtractionTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting ExtractEntities HTML");
      m_AlchemyAPI.ExtractEntities(OnExtractEntitiesHTML, example_article_html, 50, true, true, true, true, true, true, true, true, "OnExtractEntitiesHTML");
      while (!m_GetEntityExtractionHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting DetectFeeds URL");
      m_AlchemyAPI.DetectFeeds(OnDetectFeedsURL, m_ExampleURL_feed, "OnDetectFeedsURL");
      while (!m_DetectFeedsURLTested)
        yield return null;

      //            Log.Debug("TestAlchemyAPI", "Attempting DetectFeeds HTML");
      //            m_AlchemyLanguage.DetectFeeds(OnDetectFeedsHTML, example_feed_html, "OnDetectFeedsHTML");
      //            while(!m_DetectFeedsHTMLTested)
      //                yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting ExtractKeywords URL");
      m_AlchemyAPI.ExtractKeywords(OnExtractKeywordsURL, m_ExampleURL_article, 50, true, true, true, "OnExtractKeywordsURL");
      while (!m_GetKeywordExtractionURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting ExtractKeywords Text");
      m_AlchemyAPI.ExtractKeywords(OnExtractKeywordsText, m_ExampleText_article, 50, true, true, true, "OnExtractKeywordsText");
      while (!m_GetKeywordExtractionTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting ExtractKeywords HTML");
      m_AlchemyAPI.ExtractKeywords(OnExtractKeywordsHTML, example_article_html, 50, true, true, true, "OnExtractKeywordsHTML");
      while (!m_GetKeywordExtractionHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetLanguages URL");
      m_AlchemyAPI.GetLanguages(OnGetLanguagesURL, m_ExampleURL_article, true, "OnGetLanguagesURL");
      while (!m_GetLanguageURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetLanguages Text");
      m_AlchemyAPI.GetLanguages(OnGetLanguagesText, m_ExampleText_article, true, "OnGetLanguagesText");
      while (!m_GetLanguageTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetLanguages HTML");
      m_AlchemyAPI.GetLanguages(OnGetLanguagesHTML, example_article_html, true, "OnGetLanguagesHTML");
      while (!m_GetLanguageHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetMicroformats URL");
      m_AlchemyAPI.GetMicroformats(OnGetMicroformatsURL, m_ExampleURL_microformats, "OnGetMicroformatsURL");
      while (!m_GetMicroformatURLTested)
        yield return null;

      //            Log.Debug("TestAlchemyAPI", "Attempting GetMicroformats HTML");
      //            m_AlchemyLanguage.GetMicroformats(OnGetMicroformatsHTML, example_microformats_html, "OnGetMicroformatsHTML");
      //            while(!m_GetMicroformatHTMLTested)
      //                yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetPublicationDate URL");
      m_AlchemyAPI.GetPublicationDate(OnGetPublicationDateURL, m_ExampleURL_article, "OnGetPublicationDateURL");
      while (!m_GetPubDateURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetPublicationDate HTML");
      m_AlchemyAPI.GetPublicationDate(OnGetPublicationDateHTML, example_article_html, "OnGetPublicationDateHTML");
      while (!m_GetPubldateHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetRelations URL");
      m_AlchemyAPI.GetRelations(OnGetRelationsURL, m_ExampleURL_article, 50, true, true, true, true, true, true, true, true, true, true, "OnGetRelationsURL");
      while (!m_GetRelationsURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetRelations Text");
      m_AlchemyAPI.GetRelations(OnGetRelationsText, m_ExampleText_article, 50, true, true, true, true, true, true, true, true, true, true, "OnGetRelationsText");
      while (!m_GetRelationsTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetRelations HTML");
      m_AlchemyAPI.GetRelations(OnGetRelationsHTML, example_article_html, 50, true, true, true, true, true, true, true, true, true, true, "OnGetRelationsHTML");
      while (!m_GetRelationsHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetTextSentiment URL");
      m_AlchemyAPI.GetTextSentiment(OnGetTextSentimentURL, m_ExampleURL_article, true, "OnGetTextSentimentURL");
      while (!m_GetTextSentimentURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetTextSentiment Text");
      m_AlchemyAPI.GetTextSentiment(OnGetTextSentimentText, m_ExampleText_article, true, "OnGetTextSentimentText");
      while (!m_GetTextSentimentTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetTextSentiment HTML");
      m_AlchemyAPI.GetTextSentiment(OnGetTextSentimentHTML, example_article_html, true, "OnGetTextSentimentHTML");
      while (!m_GetTextSentimentHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetTargetedSentiment URL");
      m_AlchemyAPI.GetTargetedSentiment(OnGetTargetedSentimentURL, m_ExampleURL_article, "Jeopardy|Jennings|Watson", true, "OnGetTargetedSentimentURL");
      while (!m_GetTargetedSentimentURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetTargetedSentiment Text");
      m_AlchemyAPI.GetTargetedSentiment(OnGetTargetedSentimentText, m_ExampleText_article, "Jeopardy|Jennings|Watson", true, "OnGetTargetedSentimentText");
      while (!m_GetTargetedSentimentTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetTargetedSentiment HTML");
      m_AlchemyAPI.GetTargetedSentiment(OnGetTargetedSentimentHTML, example_article_html, "Jeopardy|Jennings|Watson", true, "OnGetTargetedSentimentHTML");
      while (!m_GetTargetedSentimentHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetRankedTaxonomy URL");
      m_AlchemyAPI.GetRankedTaxonomy(OnGetRankedTaxonomyURL, m_ExampleURL_article, true, "OnGetRankedTaxonomyURL");
      while (!m_GetRankedTaxonomyURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetRankedTaxonomy Text");
      m_AlchemyAPI.GetRankedTaxonomy(OnGetRankedTaxonomyText, m_ExampleText_article, true, "OnGetRankedTaxonomyText");
      while (!m_GetRankedTaxonomyTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetRankedTaxonomy HTML");
      m_AlchemyAPI.GetRankedTaxonomy(OnGetRankedTaxonomyHTML, example_article_html, true, "OnGetRankedTaxonomyHTML");
      while (!m_GetRankedTaxonomyHTMLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetText HTML");
      m_AlchemyAPI.GetText(OnGetTextHTML, example_article_html, true, true, "OnGetTextHTML");
      while (!m_GetTextHTMLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetText URL");
      m_AlchemyAPI.GetText(OnGetTextURL, m_ExampleURL_article, true, true, "OnGetTextURL");
      while (!m_GetTextURLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetRawText HTML");
      m_AlchemyAPI.GetRawText(OnGetRawTextHTML, example_article_html, "OnGetTextHTML");
      while (!m_GetRawTextHTMLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetRawText URL");
      m_AlchemyAPI.GetRawText(OnGetRawTextURL, m_ExampleURL_article, "OnGetTextURL");
      while (!m_GetRawTextURLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetTitle HTML");
      m_AlchemyAPI.GetTitle(OnGetTitleHTML, example_article_html, true, "OnGetTitleHTML");
      while (!m_GetTitleHTMLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetTitle URL");
      m_AlchemyAPI.GetTitle(OnGetTitleURL, m_ExampleURL_article, true, "OnGetTitleURL");
      while (!m_GetTitleURLTested)
        yield return null;


      Log.Debug("TestAlchemyAPI", "Attempting GetCombinedData URL");
      m_AlchemyAPI.GetCombinedData(OnGetCombinedDataURL, m_ExampleURL_article, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, "OnGetCombinedDataURL");
      while (!m_GetCombinedDataURLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetCombinedData Text");
      m_AlchemyAPI.GetCombinedData(OnGetCombinedDataText, m_ExampleText_article, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, "OnGetCombinedDataText");
      while (!m_GetCombinedDataTextTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting GetCombinedData HTML");
      m_AlchemyAPI.GetCombinedData(OnGetCombinedDataHTML, example_article_html, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, "OnGetCombinedDataHTML");
      while (!m_GetCombinedDataHTMLTested)
        yield return null;

      Log.Debug("TestAlchemyAPI", "Attempting to GetNews");
      string[] returnFields = { Fields.ENRICHED_URL_ENTITIES, Fields.ENRICHED_URL_KEYWORDS };
      Dictionary<string, string> queryFields = new Dictionary<string, string>();
      queryFields.Add(Fields.ENRICHED_URL_RELATIONS_RELATION_SUBJECT_TEXT, "Obama");
      queryFields.Add(Fields.ENRICHED_URL_CLEANEDTITLE, "Washington");

      m_AlchemyAPI.GetNews(OnGetNews, returnFields, queryFields);
      while (!m_GetNewsTested)
        yield return null;

      yield break;
    }

    #region GetAuthors
    private void OnGetAuthorsURL(AuthorsData authors, string data)
    {
      Test(authors.status == "OK");
      if (authors != null)
      {
        m_GetAuthorsURLTested = true;
        LogAuthorsData(authors, data);
      }
    }

    private void OnGetAuthorsHTML(AuthorsData authors, string data)
    {
      Test(authors.status == "OK");
      if (authors != null)
      {
        m_GetAuthorsHTMLTested = true;
        LogAuthorsData(authors, data);
      }
    }

    private void LogAuthorsData(AuthorsData authors, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (authors != null)
      {
        if (authors.authors.names.Length == 0)
          Log.Debug("TestAlchemyAPI", "No authors found!");

        foreach (string name in authors.authors.names)
          Log.Debug("TestAlchemyAPI", "Author " + name + " found!");
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Author!");
      }
    }
    #endregion

    #region GetRankedConcepts
    private void OnGetRankedConceptsURL(ConceptsData concepts, string data)
    {
      Test(concepts.status == "OK");
      if (concepts != null)
      {
        m_GetRankedConceptsURLTested = true;
        LogConceptsData(concepts, data);
      }
    }

    private void OnGetRankedConceptsHTML(ConceptsData concepts, string data)
    {
      Test(concepts.status == "OK");
      if (concepts != null)
      {
        m_GetRankedConceptsHTMLTested = true;
        LogConceptsData(concepts, data);
      }
    }

    private void OnGetRankedConceptsText(ConceptsData concepts, string data)
    {
      Test(concepts.status == "OK");
      if (concepts != null)
      {
        m_GetRankedConceptsTextTested = true;
        LogConceptsData(concepts, data);
      }
    }

    private void LogConceptsData(ConceptsData concepts, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (concepts != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", concepts.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", concepts.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", concepts.language);
        if (concepts.concepts.Length == 0)
          Log.Debug("TestAlchemyAPI", "No concepts found!");

        foreach (Concept concept in concepts.concepts)
          Log.Debug("TestAlchemyAPI", "Concept: {0}, Relevance: {1}", concept.text, concept.relevance);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Concepts!");
      }
    }
    #endregion

    #region GetDates
    private void OnGetDatesURL(DateData dates, string data)
    {
      Test(dates.status == "OK");
      if (dates != null)
      {
        m_GetDatesURLTested = true;
        LogDatesData(dates, data);
      }
    }

    private void OnGetDatesHTML(DateData dates, string data)
    {
      Test(dates.status == "OK");
      if (dates != null)
      {
        m_GetDatesHTMLTested = true;
        LogDatesData(dates, data);
      }
    }

    private void OnGetDatesText(DateData dates, string data)
    {
      Test(dates.status == "OK");
      if (dates != null)
      {
        m_GetDatesTextTested = true;
        LogDatesData(dates, data);
      }
    }

    private void LogDatesData(DateData dates, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (dates != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", dates.status);
        Log.Debug("TestAlchemyAPI", "language: {0}", dates.language);
        Log.Debug("TestAlchemyAPI", "url: {0}", dates.url);
        if (dates.dates == null || dates.dates.Length == 0)
          Log.Debug("TestAlchemyAPI", "No dates found!");
        else
          foreach (Date date in dates.dates)
            Log.Debug("TestAlchemyAPI", "Text: {0}, Date: {1}", date.text, date.date);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Dates!");
      }
    }
    #endregion

    #region GetEmotion
    private void OnGetEmotionsURL(EmotionData emotions, string data)
    {
      Test(emotions.status == "OK");
      if (emotions != null)
      {
        m_GetEmotionURLTested = true;
        LogEmotionsData(emotions, data);
      }
    }

    private void OnGetEmotionsHTML(EmotionData emotions, string data)
    {
      Test(emotions.status == "OK");
      if (emotions != null)
      {
        m_GetEmotionHTMLTested = true;
        LogEmotionsData(emotions, data);
      }
    }

    private void OnGetEmotionsText(EmotionData emotions, string data)
    {
      Test(emotions.status == "OK");
      if (emotions != null)
      {
        m_GetEmotionTextTested = true;
        LogEmotionsData(emotions, data);
      }
    }

    private void LogEmotionsData(EmotionData emotions, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (emotions != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", emotions.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", emotions.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", emotions.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", emotions.text);
        if (emotions.docEmotions == null)
          Log.Debug("TestAlchemyAPI", "No emotions found!");
        else
        {
          Log.Debug("TestAlchemyAPI", "anger: {0}", emotions.docEmotions.anger);
          Log.Debug("TestAlchemyAPI", "disgust: {0}", emotions.docEmotions.disgust);
          Log.Debug("TestAlchemyAPI", "fear: {0}", emotions.docEmotions.fear);
          Log.Debug("TestAlchemyAPI", "joy: {0}", emotions.docEmotions.joy);
          Log.Debug("TestAlchemyAPI", "sadness: {0}", emotions.docEmotions.sadness);
        }
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Emotions!");
      }
    }
    #endregion

    #region GetEntityExtraction
    private void OnExtractEntitiesURL(EntityData entityData, string data)
    {
      Test(entityData.status == "OK");
      if (entityData != null)
      {
        m_GetEntityExtractionURLTested = true;
        LogEntityData(entityData, data);
      }
    }

    private void OnExtractEntitiesHTML(EntityData entityData, string data)
    {
      Test(entityData.status == "OK");
      if (entityData != null)
      {
        m_GetEntityExtractionHTMLTested = true;
        LogEntityData(entityData, data);
      }
    }

    private void OnExtractEntitiesText(EntityData entityData, string data)
    {
      Test(entityData.status == "OK");
      if (entityData != null)
      {
        m_GetEntityExtractionTextTested = true;
        LogEntityData(entityData, data);
      }
    }

    private void LogEntityData(EntityData entityData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (entityData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", entityData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", entityData.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", entityData.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", entityData.text);
        if (entityData == null || entityData.entities.Length == 0)
          Log.Debug("TestAlchemyAPI", "No entities found!");
        else
          foreach (Entity entity in entityData.entities)
            Log.Debug("TestAlchemyAPI", "text: {0}, type: {1}", entity.text, entity.type);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Emotions!");
      }
    }
    #endregion

    #region DetectFeeds
    private void OnDetectFeedsURL(FeedData feedData, string data)
    {
      Test(feedData.status == "OK");
      if (feedData != null)
      {
        m_DetectFeedsURLTested = true;
        LogFeedData(feedData, data);
      }
    }

    private void OnDetectFeedsHTML(FeedData feedData, string data)
    {
      Test(feedData.status == "OK");
      if (feedData != null)
      {
        m_DetectFeedsHTMLTested = true;
        LogFeedData(feedData, data);
      }
    }

    private void LogFeedData(FeedData feedData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (feedData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", feedData.status);
        if (feedData == null || feedData.feeds.Length == 0)
          Log.Debug("TestAlchemyAPI", "No feeds found!");
        else
          foreach (Feed feed in feedData.feeds)
            Log.Debug("TestAlchemyAPI", "text: {0}", feed.feed);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Feeds!");
      }
    }
    #endregion

    #region GetKeyword
    private void OnExtractKeywordsURL(KeywordData keywordData, string data)
    {
      Test(keywordData.status == "OK");
      if (keywordData != null)
      {
        m_GetKeywordExtractionURLTested = true;
        LogKeywordData(keywordData, data);
      }
    }

    private void OnExtractKeywordsHTML(KeywordData keywordData, string data)
    {
      Test(keywordData.status == "OK");
      if (keywordData != null)
      {
        m_GetKeywordExtractionHTMLTested = true;
        LogKeywordData(keywordData, data);
      }
    }

    private void OnExtractKeywordsText(KeywordData keywordData, string data)
    {
      Test(keywordData.status == "OK");
      if (keywordData != null)
      {
        m_GetKeywordExtractionTextTested = true;
        LogKeywordData(keywordData, data);
      }
    }

    private void LogKeywordData(KeywordData keywordData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (keywordData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", keywordData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", keywordData.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", keywordData.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", keywordData.text);
        if (keywordData == null || keywordData.keywords.Length == 0)
          Log.Debug("TestAlchemyAPI", "No keywords found!");
        else
          foreach (Keyword keyword in keywordData.keywords)
            Log.Debug("TestAlchemyAPI", "text: {0}, relevance: {1}", keyword.text, keyword.relevance);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Keywords!");
      }
    }
    #endregion

    #region GetLanguage
    private void OnGetLanguagesURL(LanguageData languages, string data)
    {
      Test(languages.status == "OK");
      if (languages != null)
      {
        m_GetLanguageURLTested = true;
        LogLanguagesData(languages, data);
      }
    }

    private void OnGetLanguagesHTML(LanguageData languages, string data)
    {
      Test(languages.status == "OK");
      if (languages != null)
      {
        m_GetLanguageHTMLTested = true;
        LogLanguagesData(languages, data);
      }
    }

    private void OnGetLanguagesText(LanguageData languages, string data)
    {
      Test(languages.status == "OK");
      if (languages != null)
      {
        m_GetLanguageTextTested = true;
        LogLanguagesData(languages, data);
      }
    }

    private void LogLanguagesData(LanguageData languages, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (languages != null)
      {
        if (string.IsNullOrEmpty(languages.language))
          Log.Debug("TestAlchemyAPI", "No languages detected!");
        else
        {
          Log.Debug("TestAlchemyAPI", "status: {0}", languages.status);
          Log.Debug("TestAlchemyAPI", "url: {0}", languages.url);
          Log.Debug("TestAlchemyAPI", "language: {0}", languages.language);
          Log.Debug("TestAlchemyAPI", "ethnologue: {0}", languages.ethnologue);
          Log.Debug("TestAlchemyAPI", "iso_639_1: {0}", languages.iso_639_1);
          Log.Debug("TestAlchemyAPI", "iso_639_2: {0}", languages.iso_639_2);
          Log.Debug("TestAlchemyAPI", "iso_639_3: {0}", languages.iso_639_3);
          Log.Debug("TestAlchemyAPI", "native_speakers: {0}", languages.native_speakers);
          Log.Debug("TestAlchemyAPI", "wikipedia: {0}", languages.wikipedia);
        }
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Dates!");
      }
    }
    #endregion

    #region GetMicroformats
    private void OnGetMicroformatsURL(MicroformatData microformats, string data)
    {
      Test(microformats.status == "OK");
      if (microformats != null)
      {
        m_GetMicroformatURLTested = true;
        LogMicroformatData(microformats, data);
      }
    }

    private void OnGetMicroformatsHTML(MicroformatData microformats, string data)
    {
      Test(microformats.status == "OK");
      if (microformats != null)
      {
        m_GetMicroformatHTMLTested = true;
        LogMicroformatData(microformats, data);
      }
    }

    private void LogMicroformatData(MicroformatData microformats, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (microformats != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", microformats.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", microformats.url);
        if (microformats.microformats.Length == 0)
          Log.Warning("TestAlchemyAPI", "No microformats found!");
        else
        {
          foreach (Microformat microformat in microformats.microformats)
            Log.Debug("TestAlchemyAPI", "field: {0}, data: {1}.", microformat.field, microformat.data);
        }
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Microformats!");
      }
    }
    #endregion

    #region GetPubDate
    private void OnGetPublicationDateURL(PubDateData pubDates, string data)
    {
      Test(pubDates.status == "OK");
      if (pubDates != null)
      {
        m_GetPubDateURLTested = true;
        LogPublicationDateData(pubDates, data);
      }
    }

    private void OnGetPublicationDateHTML(PubDateData pubDates, string data)
    {
      Test(pubDates.status == "OK");
      if (pubDates != null)
      {
        m_GetPubldateHTMLTested = true;
        LogPublicationDateData(pubDates, data);
      }
    }

    private void LogPublicationDateData(PubDateData pubDates, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (pubDates != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", pubDates.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", pubDates.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", pubDates.language);
        if (pubDates.publicationDate != null)
          Log.Debug("TestAlchemyAPI", "date: {0}, confident: {1}", pubDates.publicationDate.date, pubDates.publicationDate.confident);
        else
          Log.Debug("TestAlchemyAPI", "Failed to find Publication Dates!");
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Publication Dates!");
      }
    }
    #endregion

    #region GetRelations
    private void OnGetRelationsURL(RelationsData relationsData, string data)
    {
      Test(relationsData.status == "OK");
      if (relationsData != null)
      {
        m_GetRelationsURLTested = true;
        LogRelationsData(relationsData, data);
      }
    }

    private void OnGetRelationsHTML(RelationsData relationsData, string data)
    {
      Test(relationsData.status == "OK");
      if (relationsData != null)
      {
        m_GetRelationsHTMLTested = true;
        LogRelationsData(relationsData, data);
      }
    }

    private void OnGetRelationsText(RelationsData relationsData, string data)
    {
      Test(relationsData.status == "OK");
      if (relationsData != null)
      {
        m_GetRelationsTextTested = true;
        LogRelationsData(relationsData, data);
      }
    }

    private void LogRelationsData(RelationsData relationsData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (relationsData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", relationsData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", relationsData.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", relationsData.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", relationsData.text);
        if (relationsData.relations == null || relationsData.relations.Length == 0)
          Log.Debug("TestAlchemyAPI", "No relations found!");
        else
          foreach (Relation relation in relationsData.relations)
            if (relation.subject != null && !string.IsNullOrEmpty(relation.subject.text))
              Log.Debug("TestAlchemyAPI", "Text: {0}, Date: {1}", relation.sentence, relation.subject.text);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Relations!");
      }
    }
    #endregion

    #region GetTextSentiment
    private void OnGetTextSentimentURL(SentimentData sentimentData, string data)
    {
      Test(sentimentData.status == "OK");
      if (sentimentData != null)
      {
        m_GetTextSentimentURLTested = true;
        LogTextSentiment(sentimentData, data);
      }
    }

    private void OnGetTextSentimentHTML(SentimentData sentimentData, string data)
    {
      Test(sentimentData.status == "OK");
      if (sentimentData != null)
      {
        m_GetTextSentimentHTMLTested = true;
        LogTextSentiment(sentimentData, data);
      }
    }

    private void OnGetTextSentimentText(SentimentData sentimentData, string data)
    {
      Test(sentimentData.status == "OK");
      if (sentimentData != null)
      {
        m_GetTextSentimentTextTested = true;
        LogTextSentiment(sentimentData, data);
      }
    }

    private void LogTextSentiment(SentimentData sentimentData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (sentimentData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", sentimentData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", sentimentData.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", sentimentData.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", sentimentData.text);
        if (sentimentData.docSentiment == null)
          Log.Debug("TestAlchemyAPI", "No sentiment found!");
        else
            if (sentimentData.docSentiment != null && !string.IsNullOrEmpty(sentimentData.docSentiment.type))
          Log.Debug("TestAlchemyAPI", "Sentiment: {0}, Score: {1}", sentimentData.docSentiment.type, sentimentData.docSentiment.score);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Relations!");
      }
    }
    #endregion

    #region GetTargetedSentiment
    private void OnGetTargetedSentimentURL(TargetedSentimentData sentimentData, string data)
    {
      Test(sentimentData.status == "OK");
      if (sentimentData != null)
      {
        m_GetTargetedSentimentURLTested = true;
        LogTargetedSentiment(sentimentData, data);
      }
    }

    private void OnGetTargetedSentimentHTML(TargetedSentimentData sentimentData, string data)
    {
      Test(sentimentData.status == "OK");
      if (sentimentData != null)
      {
        m_GetTargetedSentimentHTMLTested = true;
        LogTargetedSentiment(sentimentData, data);
      }
    }

    private void OnGetTargetedSentimentText(TargetedSentimentData sentimentData, string data)
    {
      Test(sentimentData.status == "OK");
      if (sentimentData != null)
      {
        m_GetTargetedSentimentTextTested = true;
        LogTargetedSentiment(sentimentData, data);
      }
    }

    private void LogTargetedSentiment(TargetedSentimentData sentimentData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (sentimentData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", sentimentData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", sentimentData.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", sentimentData.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", sentimentData.text);
        if (sentimentData.results == null)
          Log.Debug("TestAlchemyAPI", "No sentiment found!");
        else
            if (sentimentData.results == null || sentimentData.results.Length == 0)
          Log.Warning("TestAlchemyAPI", "No sentiment results!");
        else
          foreach (TargetedSentiment result in sentimentData.results)
            Log.Debug("TestAlchemyAPI", "text: {0}, sentiment: {1}, score: {2}", result.text, result.sentiment.score, result.sentiment.type);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Relations!");
      }
    }
    #endregion

    #region GetRankedTaxonomy
    private void OnGetRankedTaxonomyURL(TaxonomyData taxonomyData, string data)
    {
      Test(taxonomyData.status == "OK");
      if (taxonomyData != null)
      {
        m_GetRankedTaxonomyURLTested = true;
        LogRankedTaxonomy(taxonomyData, data);
      }
    }

    private void OnGetRankedTaxonomyHTML(TaxonomyData taxonomyData, string data)
    {
      Test(taxonomyData.status == "OK");
      if (taxonomyData != null)
      {
        m_GetRankedTaxonomyHTMLTested = true;
        LogRankedTaxonomy(taxonomyData, data);
      }
    }

    private void OnGetRankedTaxonomyText(TaxonomyData taxonomyData, string data)
    {
      Test(taxonomyData.status == "OK");
      if (taxonomyData != null)
      {
        m_GetRankedTaxonomyTextTested = true;
        LogRankedTaxonomy(taxonomyData, data);
      }
    }

    private void LogRankedTaxonomy(TaxonomyData taxonomyData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (taxonomyData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", taxonomyData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", taxonomyData.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", taxonomyData.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", taxonomyData.text);
        if (taxonomyData.taxonomy == null)
          Log.Debug("TestAlchemyAPI", "No taxonomy found!");
        else
            if (taxonomyData.taxonomy == null || taxonomyData.taxonomy.Length == 0)
          Log.Warning("TestAlchemyAPI", "No taxonomy results!");
        else
          foreach (Taxonomy taxonomy in taxonomyData.taxonomy)
            Log.Debug("TestAlchemyAPI", "label: {0}, score: {1}", taxonomy.label, taxonomy.score);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find Relations!");
      }
    }
    #endregion

    #region GetText
    private void OnGetTextURL(TextData textData, string data)
    {
      Test(textData.status == "OK");
      if (textData != null)
      {
        m_GetTextURLTested = true;
        LogTextData(textData, data);
      }
    }

    private void OnGetTextHTML(TextData textData, string data)
    {
      Test(textData.status == "OK");
      if (textData != null)
      {
        m_GetTextHTMLTested = true;
        LogTextData(textData, data);
      }
    }

    private void LogTextData(TextData textData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (textData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", textData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", textData.url);
        Log.Debug("TestAlchemyAPI", "text: {0}", textData.text);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find text!");
      }

    }
    #endregion

    #region GetRawText
    private void OnGetRawTextURL(TextData textData, string data)
    {
      Test(textData.status == "OK");
      if (textData != null)
      {
        m_GetRawTextURLTested = true;
        LogRawTextData(textData, data);
      }
    }

    private void OnGetRawTextHTML(TextData textData, string data)
    {
      Test(textData.status == "OK");
      if (textData != null)
      {
        m_GetRawTextHTMLTested = true;
        LogRawTextData(textData, data);
      }
    }

    private void LogRawTextData(TextData textData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (textData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", textData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", textData.url);
        Log.Debug("TestAlchemyAPI", "text: {0}", textData.text);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find text!");
      }

    }
    #endregion

    #region GetTitle
    private void OnGetTitleURL(Title titleData, string data)
    {
      Test(titleData.status == "OK");
      if (titleData != null)
      {
        m_GetTitleURLTested = true;
        LogTitleData(titleData, data);
      }
    }

    private void OnGetTitleHTML(Title titleData, string data)
    {
      Test(titleData.status == "OK");
      if (titleData != null)
      {
        m_GetTitleHTMLTested = true;
        LogTitleData(titleData, data);
      }
    }

    private void LogTitleData(Title titleData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (titleData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", titleData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", titleData.url);
        Log.Debug("TestAlchemyAPI", "text: {0}", titleData.title);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to find title!");
      }

    }
    #endregion

    #region GetCombinedData
    private void OnGetCombinedDataURL(CombinedCallData combinedData, string data)
    {
      Test(combinedData.status == "OK");
      if (combinedData != null)
      {
        m_GetCombinedDataURLTested = true;
        LogCombinedData(combinedData, data);
      }
    }

    private void OnGetCombinedDataHTML(CombinedCallData combinedData, string data)
    {
      Test(combinedData.status == "OK");
      if (combinedData != null)
      {
        m_GetCombinedDataHTMLTested = true;
        LogCombinedData(combinedData, data);
      }
    }

    private void OnGetCombinedDataText(CombinedCallData combinedData, string data)
    {
      Test(combinedData.status == "OK");
      if (combinedData != null)
      {
        m_GetCombinedDataTextTested = true;
        LogCombinedData(combinedData, data);
      }
    }

    private void LogCombinedData(CombinedCallData combinedData, string data)
    {
      Log.Debug("TestAlchemyAPI", "data: {0}", data);
      if (combinedData != null)
      {
        Log.Debug("TestAlchemyAPI", "status: {0}", combinedData.status);
        Log.Debug("TestAlchemyAPI", "url: {0}", combinedData.url);
        Log.Debug("TestAlchemyAPI", "language: {0}", combinedData.language);
        Log.Debug("TestAlchemyAPI", "text: {0}", combinedData.text);
        Log.Debug("TestAlchemyAPI", "image: {0}", combinedData.image);

        if (combinedData.imageKeywords != null && combinedData.imageKeywords.Length > 0)
          foreach (ImageKeyword imageKeyword in combinedData.imageKeywords)
            Log.Debug("TestAlchemyAPI", "ImageKeyword: {0}, Score: {1}", imageKeyword.text, imageKeyword.score);

        if (combinedData.publicationDate != null)
          Log.Debug("TestAlchemyAPI", "publicationDate: {0}, Score: {1}", combinedData.publicationDate.date, combinedData.publicationDate.confident);

        if (combinedData.authors != null && combinedData.authors.names.Length > 0)
          foreach (string authors in combinedData.authors.names)
            Log.Debug("TestAlchemyAPI", "Authors: {0}", authors);

        if (combinedData.docSentiment != null)
          Log.Debug("TestAlchemyAPI", "DocSentiment: {0}, Score: {1}, Mixed: {2}", combinedData.docSentiment.type, combinedData.docSentiment.score, combinedData.docSentiment.mixed);

        if (combinedData.feeds != null && combinedData.feeds.Length > 0)
          foreach (Feed feed in combinedData.feeds)
            Log.Debug("TestAlchemyAPI", "Feeds: {0}", feed.feed);

        if (combinedData.keywords != null && combinedData.keywords.Length > 0)
          foreach (Keyword keyword in combinedData.keywords)
            Log.Debug("TestAlchemyAPI", "Keyword: {0}, relevance: {1}", keyword.text, keyword.relevance);

        if (combinedData.concepts != null && combinedData.concepts.Length > 0)
          foreach (Concept concept in combinedData.concepts)
            Log.Debug("TestAlchemyAPI", "Concept: {0}, Relevance: {1}", concept.text, concept.relevance);

        if (combinedData.entities != null && combinedData.entities.Length > 0)
          foreach (Entity entity in combinedData.entities)
            Log.Debug("TestAlchemyAPI", "Entity: {0}, Type: {1}, Relevance: {2}", entity.text, entity.type, entity.relevance);

        if (combinedData.relations != null && combinedData.relations.Length > 0)
          foreach (Relation relation in combinedData.relations)
            Log.Debug("TestAlchemyAPI", "Relations: {0}", relation.subject.text);

        if (combinedData.taxonomy != null && combinedData.taxonomy.Length > 0)
          foreach (Taxonomy taxonomy in combinedData.taxonomy)
            Log.Debug("TestAlchemyAPI", "Taxonomy: {0}, Score: {1}, Confident: {2}", taxonomy.label, taxonomy.score, taxonomy.confident);

        if (combinedData.dates != null && combinedData.dates.Length > 0)
          foreach (Date date in combinedData.dates)
            Log.Debug("TestAlchemyAPI", "Dates", date.text, date.date);

        if (combinedData.docEmotions != null && combinedData.docEmotions.Length > 0)
          foreach (DocEmotions emotions in combinedData.docEmotions)
            Log.Debug("TestAlchemyAPI", "Doc Emotions: anger: {0}, disgust: {1}, fear: {2}, joy: {3}, sadness: {4}", emotions.anger, emotions.disgust, emotions.fear, emotions.joy, emotions.sadness);
      }
      else
      {
        Log.Debug("TestAlchemyAPI", "Failed to get combined data!");
      }
    }
    #endregion

    #region GetNews
    private void OnGetNews(NewsResponse newsData, string data)
    {
      Test(newsData.status == "OK");
      if (newsData != null)
      {
        m_GetNewsTested = true;
        LogNewsData(newsData, data);
      }
    }

    private void LogNewsData(NewsResponse newsData, string data)
    {
      Log.Debug("TestAlchemyNews", "data: {0}", data);
      if (newsData != null)
      {
        Log.Debug("TestAlchemyNews", "status: {0}", newsData.status);

      }
      else
      {
        Log.Debug("TestAlchemyNews", "Failed to get news data!");
      }
    }
    #endregion
  }
}
