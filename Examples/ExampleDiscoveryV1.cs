using IBM.Watson.Discovery.V1;
using IBM.Watson.Discovery.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
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
            throw new IBMException("Plesae provide IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

        //  Wait for tokendata
        while (!authenticator.CanAuthenticate())
            yield return null;

        service = new DiscoveryService(versionDate, authenticator);
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }

        Runnable.Run(ExampleCreateEnvironment());
        Runnable.Run(ExampleListEnvironments());
    }

    private IEnumerator ExampleCreateEnvironment()
    {
        ModelEnvironment createEnvironmentResponse = null;
        service.CreateEnvironment(
            callback: (DetailedResponse<ModelEnvironment> response, IBMError error) =>
            {
                Log.Debug("DiscoveryServiceV1", "CreateEnvironment result: {0}", response.Response);
                createEnvironmentResponse = response.Result;
                // environmentId = createEnvironmentResponse.EnvironmentId;
            },
            name: "my_environment",
            description: "My environment"
        );

        while (createEnvironmentResponse == null)
            yield return null;
    }

    private IEnumerator ExampleListEnvironments()
    {
        Log.Debug("DiscoveryServiceV1", "ListEnvironments");
        ListEnvironmentsResponse listEnvironmentsResponse = null;
        service.ListEnvironments(
            callback: (DetailedResponse<ListEnvironmentsResponse> response, IBMError error) =>
            {
                Log.Debug("DiscoveryServiceV1", "ListEnvironments result: {0}", response.Response);
                listEnvironmentsResponse = response.Result;
            }
        );

        while (listEnvironmentsResponse == null)
            yield return null;
    }
}