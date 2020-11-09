using IBM.Watson.LanguageTranslator.V3;
using IBM.Watson.LanguageTranslator.V3.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;

public class ExampleLanguageTranslatorV3 : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string iamApikey;
    [Tooltip("The service URL (optional). This defaults to \"https://api.us-south.language-translator.watson.cloud.ibm.com\"")]
    [SerializeField]
    private string serviceUrl;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string versionDate;
    #endregion

    private LanguageTranslatorService service;
    // Start is called before the first frame update
    void Start()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());
    }

    // Update is called once per frame
    public IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamApikey))
        {
            throw new IBMException("Plesae provide IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

        //  Wait for tokendata
        while (!authenticator.CanAuthenticate())
            yield return null;

        service = new LanguageTranslatorService(versionDate, authenticator);
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }

        Log.Debug("LanguageTranslatorServiceV3", "ListModels result");
    }

    private IEnumerator ExampleListModels()
    {
        TranslationModels listModelsResponse = null;
        service.ListModels(
            callback: (DetailedResponse<TranslationModels> response, IBMError error) =>
            {
                Log.Debug("LanguageTranslatorServiceV3", "ListModels result: {0}", response.Response);
                listModelsResponse = response.Result;
            },
            source: "en",
            target: "fr"
        );

        while (listModelsResponse == null)
            yield return null;
    }
}
