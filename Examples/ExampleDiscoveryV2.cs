using IBM.Watson.Discovery.V2;
using IBM.Watson.Discovery.V2.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Bearer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;

public class ExampleDiscoveryV2 : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The Bearer Token.")]
    [SerializeField]
    private string bearerToken;
    [Tooltip("The service URL (optional). This defaults to \"https://api.us-south.discovery.watson.cloud.ibm.com\"")]
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
        if (string.IsNullOrEmpty(bearerToken))
        {
            throw new IBMException("Plesae provide Bearer Token for the service.");
        }

        // Option 1
        //  Create credential and instantiate service using bearer token
        BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(bearerToken: bearerToken);

        // Option 2
        //  Create credential and instantiate service using username/password
        // var authenticator = new CloudPakForDataAuthenticator(
        //     url: "https://{cpd_cluster_host}{:port}",
        //     username: "{username}",
        //     password: "{password}"
        // );

        //  Wait for tokendata
        while (!authenticator.CanAuthenticate())
            yield return null;

        service = new DiscoveryService(versionDate, authenticator);
        service.SetServiceUrl("service_url");
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }

        Runnable.Run(ExampleListCollections());
    }

    private IEnumerator ExampleListCollections()
    {
        Log.Debug("DiscoveryServiceV2", "ListCollections");
        ListCollectionsResponse listCollectionsResponse = null;
        service.ListCollections(
            callback: (DetailedResponse<ListCollectionsResponse> response, IBMError error) =>
            {
                Log.Debug("DiscoveryServiceV2", "ListCollections result: {0}", response.Response);
                listCollectionsResponse = response.Result;
            },
            projectId: "{project_id}"
        );

        while (listCollectionsResponse == null)
            yield return null;
    }
}
