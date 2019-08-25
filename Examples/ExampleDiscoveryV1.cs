using IBM.Watson.Discovery.V1;
using IBM.Watson.Discovery.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;

public class ExampleDiscoveryV1 : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string iamApikey;
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/discovery/api\"")]
    [SerializeField]
    private string serviceUrl;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string versionDate;
    #endregion

    private DiscoveryService service;
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
            throw new IBMException("Please add IAM ApiKey to the Iam Apikey field in the inspector.");
        }

        IamTokenOptions tokenOptions = new IamTokenOptions()
        {
            IamApiKey = iamApikey
        };
        Credentials credentials = new Credentials(tokenOptions, serviceUrl);

        while (!credentials.HasTokenData())
        {
            yield return null;
        }

        service = new DiscoveryService(versionDate, credentials);

        Runnable.Run(ExampleCreateEnvironment());
        //Runnable.Run(ExampleListModels());
        //Runnable.Run(ExampleDeleteModel());
    }

    private IEnumerator ExampleCreateEnvironment()
    {
        ModelEnvironment createEnvironmentResponse = null;
        service.CreateEnvironment(
            callback: (DetailedResponse<ModelEnvironment> response, IBMError error) =>
            {
                Log.Debug("DiscoveryServiceV1", "CreateEnvironment result: {0}", response.Response);
                //createEnvironmentResponse = response.Result;
                //createdEnvironmentId = createEnvironmentResponse.EnvironmentId;
            },
            name: "my_environment",
            description: "My environment"
        );

        while (createEnvironmentResponse == null)
            yield return null;
    }
}
