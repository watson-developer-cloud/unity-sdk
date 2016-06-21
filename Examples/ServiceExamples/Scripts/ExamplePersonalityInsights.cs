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
using IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v2;
using IBM.Watson.DeveloperCloud.Logging;

public class ExamplePersonalityInsights : MonoBehaviour {
    PersonalityInsights m_personalityInsights = new PersonalityInsights();
    private string testString = "Columbus, Ohio (CNN)Hillary Clinton is narrowing her choices for a running mate, intently focusing on a handful of potential candidates as her team closes in on the final weeks of vetting before she makes a decision in less than a month, several Democrats watching the process tell CNN.\n\nWith her long Democratic primary fight now over, Clinton has privately signaled she is less concerned about choosing someone who fills a specific liberal or progressive void, rather than selecting a partner who is fully prepared for the job and has a strong camaraderie with her.\nThe list of serious vice presidential candidates is believed to be smaller rather than larger, with Democrats close to the campaign placing it at no more than five contenders. But several aides acknowledged they were not sure, considering the secrecy imposed on the process by Clinton.\nClinton has not yet conducted formal interviews, but has devoted hours studying the records and backgrounds of several Democrats on a list that includes Sen. Tim Kaine of Virginia, Sen. Elizabeth Warren of Massachusetts and Housing and Urban Development Secretary Julian Castro of Texas.\nPlot Clinton's path to 270\nBut those three should not be seen as absolute finalists, several Democrats said, only as active contenders. The roster also may include Labor Secretary Tom Perez, Sen. Sherrod Brown of Ohio and Rep. Xavier Beccera of California.\nAsked about his prospects, Kaine smiled and winked Tuesday as he stepped into an elevator in the Capitol.\nIt does not include Sen. Bernie Sanders of Vermont, her primary rival who has yet to endorse her candidacy but has pledged to help defeat Donald Trump. He was not expecting to be considered, aides said, and her aides say he is not.\nJohn Podesta, chairman of the Clinton campaign and a trusted confidante, is leading the effort, according to Democrats who spoke on the condition of anonymity because they are not authorized to speak about the highly-secretive process. Cheryl Mills, Clinton's longtime adviser and lawyer, is also helping Clinton with the decision.\nBoth were seen leaving Clinton's home in Washington on June 10, hours after the former secretary of state met with Warren. The topic of the meeting was not the vice presidency, aides said, but it was an opportunity for the two whose relationship has not always been warm to have a face-to-face conversation about the direction of the party.\nTrump speech to attack Clinton amid campaign turmoil\nAs Clinton has repeatedly said in interviews, her top consideration is someone who would be able to step into the presidency should anything happen to her. And, by extension, someone who Republicans could not credibly cast as ill-prepared.\n\"I want to be sure that whoever I pick could be president immediately if something were to happen,\" Clinton told CNN earlier this month. \"That's the most important qualification.\"\nAnother top consideration for Clinton and her aides, Democrats said, is finding someone she actually wants to work with, not necessarily someone who checks regional or specific electorate boxes. She, perhaps more than most presumptive nominees in recent history, knows the inner-workings of the West Wing intimately.\nThis could bode well for several Democrats, who aides say Clinton enjoyed campaigning with this year, including Kaine, Perez, Castro and Sen. Cory Booker of New Jersey.\nFor all the calculations about who would make a better running mate, the list of actual candidates is believed to be fairly small. Clinton is not expected to make a decision before Trump reveals his choice at the Republican convention, but aides say she is almost certain to have her decision made privately by then.\nClinton to cast Trump as dangerous — this time, on the economy\n CNN Politics app\nEach Democrat being considered offers a variety of pros and cons that Podesta, Mills and other aides are currently weighing. A veteran Washington lawyer, James Hamilton, is also overseeing the vetting of the candidates.\nThe real scrutiny, though, comes through the work of Democratic lawyers and researchers who are assigned specific candidates and are walled-off from others. They start by studying public records, searching for anything embarrassing, distracting or otherwise problematic.\nOne area of inquiry, for instance, is a batch of legal files in Richmond, Virginia, where death penalty cases of a young civil rights lawyer named Tim Kaine are being reviewed. Kaine was vetted by the Obama campaign eight years ago and people close to that process say nothing was discovered that would disqualify him.\nKaine, a former governor and chairman of the Democratic National Committee, is one of the few prospects with executive experience. He also speaks fluent Spanish, often conducting interviews on the campaign trail or on Capitol Hill in his second language. He is not a progressive firebrand, but that may be less of a demand than once thought during the heat of the Clinton-Sanders fight.\nCastro is seen as young, vibrant and would further cement the Latino vote. But his experience is far less than anyone else on the list and some Democrats fear he could be cast as a lightweight.\nPoll: Clinton tops Trump, but neither prompts excitement\nPerez is seen as someone ready and willing to attack Trump and whose long history in labor politics could excite voters in labor strongholds like Ohio, Pennsylvania and Wisconsin. Yet he has spent most of his life as a political appointee, only successfully running for county council of Montgomery County, Maryland, in 2002.\nWhile Warren is being actively considered, several Democrats close to both women are skeptical she will be selected. She has aggressively attacked Trump in recent weeks -- much to the delight of the Clinton campaign -- but the two do not have a personal relationship and Warren has, at times, been outspoken against some of the Clinton White House's policies.\nLast week, Warren dropped by Clinton's headquarters and fired up the troops, leading one top Democrat to say: \"Never say never. She's good.\"";

    void Start () 
    {
        LogSystem.InstallDefaultReactors();
        string dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";

        if(!m_personalityInsights.GetProfile(OnGetProfile, dataPath, DataModels.ContentType.TEXT_PLAIN, DataModels.Language.ENGLISH))
            Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
	}

    private void OnGetProfile(DataModels.Profile profile, string data)
    {
        Log.Debug("ExamplePersonalityInsights", "data: {0}", data);
        if(profile != null)
        {
            if(!string.IsNullOrEmpty(profile.id))
                Log.Debug("ExamplePersonalityInsights", "id: {0}", profile.id);
            if(!string.IsNullOrEmpty(profile.source))
                Log.Debug("ExamplePersonalityInsights", "source: {0}", profile.source);
            if(!string.IsNullOrEmpty(profile.processed_lang))
                Log.Debug("ExamplePersonalityInsights", "proccessed_lang: {0}", profile.processed_lang);
            if(!string.IsNullOrEmpty(profile.word_count))
                Log.Debug("ExamplePersonalityInsights", "word_count: {0}", profile.word_count);
            if(!string.IsNullOrEmpty(profile.word_count_message))
                Log.Debug("ExamplePersonalityInsights", "word_count_message: {0}", profile.word_count_message);

            if(profile.tree != null)
            {
                LogTraitTree(profile.tree);
            }
        }
        else
        {
            Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
        }
    }

    private void LogTraitTree(DataModels.TraitTreeNode traitTreeNode)
    {
        if(!string.IsNullOrEmpty(traitTreeNode.id))
            Log.Debug("ExamplePersonalityInsights", "id: {0}", traitTreeNode.id);
        if(!string.IsNullOrEmpty(traitTreeNode.name))
            Log.Debug("ExamplePersonalityInsights", "name: {0}", traitTreeNode.name);
        if(!string.IsNullOrEmpty(traitTreeNode.category))
            Log.Debug("ExamplePersonalityInsights", "category: {0}", traitTreeNode.category);
        if(!string.IsNullOrEmpty(traitTreeNode.percentage))
            Log.Debug("ExamplePersonalityInsights", "percentage: {0}", traitTreeNode.percentage);
        if(!string.IsNullOrEmpty(traitTreeNode.sampling_error))
            Log.Debug("ExamplePersonalityInsights", "sampling_error: {0}", traitTreeNode.sampling_error);
        if(!string.IsNullOrEmpty(traitTreeNode.raw_score))
            Log.Debug("ExamplePersonalityInsights", "raw_score: {0}", traitTreeNode.raw_score);
        if(!string.IsNullOrEmpty(traitTreeNode.raw_sampling_error))
            Log.Debug("ExamplePersonalityInsights", "raw_sampling_error: {0}", traitTreeNode.raw_sampling_error);
        if(traitTreeNode.children != null && traitTreeNode.children.Length > 0)
            foreach(DataModels.TraitTreeNode childNode in traitTreeNode.children)
                LogTraitTree(childNode);
    }
}
