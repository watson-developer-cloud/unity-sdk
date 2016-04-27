# Watson Developer Cloud Unity SDK

Use this SDK to build Watson-powered applications in Unity. It comes with a set of prefabs that you can use to develop a simple Watson application in just one minute.

## Before you begin
Ensure that you have the following prerequisites:
* An IBM Bluemix account. If you don't have one, [sign up](https://apps.admin.ibmcloud.com/manage/trial/bluemix.html?cm_mmc=WatsonDeveloperCloud-_-LandingSiteGetStarted-_-x-_-CreateAnAccountOnBluemixCLI).
* [Unity](https://unity3d.com/get-unity). You win! You can use the **free** Personal edition.
* Change the build settings to PC, Mac & Linux Standalone. Click File > Build Settings > PC, Mac & Linux Standalone and click the Switch Platform button.

## Getting the Watson SDK and adding it to Unity
You can get the SDK by cloning the the repository from GitHub.

### Installing the SDK source into your Unity project
1. Clone the following GIT repository into the Assets directory within your current Unity project.
      * git clone git@github.com:watson-developer-cloud/unity-sdk.git Assets/Watson/
       * OR
      * git submodule add https://github.com/watson-developer-cloud/unity-sdk.git Assets/Watson/
	     * OR
      * Click on the "Download Zip" button at the following URL https://github.com/watson-developer-cloud/unity-sdk, extract the zip file into your Assets directory and **rename the directory from "unity-sdk" to "Watson".**
	  
2. Go to [Configuring Watson service credentials](#configuring-Watson-service-credentials).

## Configuring your service credentials
1. Determine which services to configure.
2. If you have configured the services already, complete the following steps. Otherwise, go to step 3.
      1. Navigate to the Dashboard on your Bluemix account.
      2. Click the tile for a service.
      3. Click **Service Credentials**.
      4. Copy the content in the Service Credentials field, and paste it in the credentials field in the Config Editor (Watson -> Config Editor) in Unity.
      5. Click **Apply Credentials**.
      6. Repeat steps 1 - 5 for each service you want to use.
3. If you need to configure the services that you want to use, complete the following steps.
      1. In the Config Editor (Watson -> Config Editor), click the **Configure** button beside the service to register. The service window is displayed.
      2. Click **Create**.
      3. Click **Service Credentials**.
      4. Copy the content in the Service Credentials field, and paste it in the empty credentials field in the Config Editor in Unity.
      5. Click **Apply Credentials**.
      6. Repeat steps 1 - 5 for each service you want to use.
4. Click **Save**, and close the Config Editor.

## Preparing the test data for developing a basic application
The SDK contains a Test Natural Language Classifier, which contains classes for temperature and conditions. Before you develop a sample application in the next section, train the classifier on the test data.

1. Open the Natural Language Classifier Editor by clicking **Watson -> Natural Language Classifier Editor**.
2. Locate the Test Natural Language Classifier, and click **Train**. The training process begins. The process lasts a few minutes.
3. To check the status of the training process, click **Refresh**. When the status changes from Training to Available, the process is finished, and you can begin to develop a basic application that uses the Natural Language Classifier, as described in the next section.

## Developing a basic application in one minute
You can quickly develop a basic application that uses the Speech to Text service and the Natural Language Classifier service by using the prefabs that come with the SDK. Ensure that you prepare the test data before you complete the the following steps:
1. Create a new scene and drag the following prefabs from 'Assets -> Watson -> Prefabs', and drop them in the Hierarchy tab:
  * MicWidget
  * SpeechToTextWidget
  * Natural Language Classifier Widget
  * ClassDisplayWidget
2. Select the **Natural Language Classifier Widget**.
5. In the **Classifier Name** field in the Inspector tab, specify 'TestNaturalLanguageClassifier/'.
6. In the Natural Language Classifier Editor, expand the **Test Natural Languge Classifier** , expand the classes, and determine which questions about the weather to ask to test the classifier.
7. Run the application.
8. Say your questions into the microphone to test the MicWidget, the SpeechToTextWidget, and the NaturalLanguageClassifierWidget.

## Dialog and classifier management
The SDK contains editors for managing your dialogs and classifiers.

### Uploading dialogs
You can upload dialogs by using the Dialog Editor.
1. Click **Watson -> Dialog Editor**. The Dialog Editor window is displayed.
2. Specify a **unique** name for the dialog in the **Name** field.
3. Click **Upload**.
4. Navigate to the dialog file to be uploaded, and click **Open**.

### Managing classifiers
You can use the Natural Language Classifier Editor to import and export classifier files, and create new classifiers and edit them.
#### Importing files for existing classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. Click **Import**.
3. Navigate to the '.csv' file to import, and click **Open**. The file is imported.
4. Click **Train**.
#### Creating new classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. In the **Name** field, specify a name for the classifier.
3. Click **Create**.
#### Editing and training classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. Expand the classifier.
3. To create a new class, specity a name for the new class in the empty field, and click **Add Class**.
4. To add a phrase to the class, specify a phrase in the empty field, and click **Add Phrase**.
5. Click **Train**.

## Documentation
To read the documentation you need to have a **chm reader** installed. Open the documentation by selcting API Reference the Watson menu (Watson -> API Reference).
