# IBM Watson SDK for Unity
Use this SDK to build Watson-powered applications in Unity. It comes with a set of prefabs that you can use to develop a simple Watson application in just one minute.

## Before you begin
Ensure that you have the following prerequisites:
* An IBM Bluemix account. If you don't have one, [sign up](https://apps.admin.ibmcloud.com/manage/trial/bluemix.html?cm_mmc=WatsonDeveloperCloud-_-LandingSiteGetStarted-_-x-_-CreateAnAccountOnBluemixCLI).
* [Unity](https://unity3d.com/get-unity). You win! You can use the **free** Personal edition.

## Getting the Watson SDK and adding it to Unity
You can get the SDK either by cloning the the repository from GitHub or by downloading it from the Unity Asset Store.
### Getting the SDK from GitHub and adding it to Unity
1. Clone the Watson SDK repository from GitHub by issuing one of the following commands in your terminal:
  ```
  git clone https://
  ```
  ```
  git clone git@github.com:
  ```
2. Open Unity.
3. Add the Watson SDK package by selecting **Assets -> Import Package -> Custom Package**.
4. Navigate to the location of the `WatsonUnitySDK.unitypackage`, and click **Open**. The Importing package window is displayed.
5. Click **Import**. The asset is imported, and the prompt to configure your Watson service credentials is displayed.
6. Click **Yes**. The Config Editor is displayed, and the Watson services to be configured are in the list.
7. Go to [Configuring Watson service credentials](#configuring-Watson-service-credentials).
### Getting the SDK from the Unity Asset Store
1. Open Unity.
2. Click **Window -> Asset Store**.
3. Search for Watson SDK.
4. Click **Open in Unity**.
5. Click **Download**. The Terms of Service agreement is displayed.
6. Read the Terms of Service agreement, and click **Accept**. The Importing package window is displayed.
7. Click **Import**. The asset is imported, and the prompt to configure your Watson service credentials is displayed.
8. Click **Yes**. The Config Editor is displayed, and the Watson services to be configured are in the list.
9. Go to [Configuring Watson service credentials](#configuring-Watson-service-credentials).

## Configuring your service credentials
1. Determine which services to configure.
2. If you have configured the services already, complete the following steps. Otherwise, go to step 3.
      1. Navigate to the Dashboard on your Bluemix account.
      2. Click the tile for a service.
      3. Click **Service Credentials**.
      4. Copy the content in the Service Credentials field, and paste it in the credentials field in the Config Editor in Unity.
      5. Click **Apply Credentials**.
      6. Repeat steps 1 - 5 for each service you want to use.
3. If you need to configure the services that you want to use, complete the following steps.
      1. In the Config Editor, click the **Configure** button beside the service to register. The service window is displayed.
      2. Click **Create**.
      3. Click **Service Credentials**.
      4. Copy the content in the Service Credentials field, and paste it in the empty credentials field in the Config Editor in Unity.
      5. Click **Apply Credentials**.
      6. Repeat steps 1 - 5 for each service you want to use.
4. Click **Save**, and close the Config Editor.

## Preparing the test data for developing a basic application
The SDK contains a TestNLC classifier, which contains classes for temperature and conditions. Before you develop a sample application in the next section, train the classifier on the test data.

1. Open the NLC Editor by clicking **Watson -> NLC Editor**.
2. Locate the TestNLC classifier, and click **Train**. The training process begins. The process lasts a few minutes.
3. To check the status of the training process, click **Refresh**. When the status changes from Training to Available, the process is finished, and you can begin to develop a basic application that uses the Natural Language Classifier, as described in the next section.

## Developing a basic application in one minute
You can quickly develop a basic application that uses the Speech to Text service and the Natural Language Classifier service by using the prefabs that come with the SDK. Ensure that you prepare the test data before you complete the the following steps:
1. Drag the following prefabs from `Assets -> Watson -> Prefabs`, and drop them in the Hierarchy tab:
  * MicWidget
  * SpeechToTextWidget
  * NlcWidget
2. Select the **NlcWidget**.
5. In the **Classifier Name** field in the Inspector tab, specify `TestNLC/`.
6. In the NLC Editor, expand the **TestNLC** classifier, expand the classes, and determine which questions about the weather to ask to test the classifier.
7. Run the application.
8. Say your questions into the microphone to test the MicWidget, the SpeechToTextWidget, and the NlcWidget.

## Dialog and classifier management
The SDK contains editors for managing your dialogs and classifiers.

### Uploading dialogs
You can upload dialogs by using the Dialog Editor.
1. Click **Watson -> Dialog Editor**. The Dialog Editor window is displayed.
2. Specify a name for the dialog in the **Name** field.
3. Click **Upload**.
4. Navigate to the dialog file to be uploaded, and click **Open**.

### Managing classifiers
You can use the NLC Editor to import and export classifier files, and create new classifiers and edit them.
#### Importing files for existing classifiers
1. Click **Watson -> NLC Editor**. The NLC Editor window is displayed.
2. Click **Import**.
3. Navigate to the `.csv` file to import, and click **Open**. The file is imported.
4. Click **Train**.
#### Creating new classifiers
1. Click **Watson -> NLC Editor**. The NLC Editor window is displayed.
2. In the **Name** field, specify a name for the classifier.
3. Click **Create**.
#### Editing and training classifiers
1. Click **Watson -> NLC Editor**. The NLC Editor window is displayed.
2. Expand the classifier.
3. To create a new class, specity a name for the new class in the empty field, and click **Add Class**.
4. To add a phrase to the class, specify a phrase in the empty field, and click **Add Phrase**.
5. Click **Train**.