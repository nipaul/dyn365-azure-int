
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;

/// <summary>
/// Create a new Connection to Dynamics CRM Online using a Client Id, Client Secret, OrganizationService Url.
/// Returns an instance of OrganizationWebProxyClient or IOrganizationService
/// Use the OrganizationWebProxyClient for impersonation scenarios to set the CallerId
/// </summary>
/// <typeparam name="T">OrganizationWebProxyClient or IOrganizationService</typeparam>
/// <returns>OrganizationWebProxyClient or IOrganizationService</returns>
public static T Connect<T>() where T: OrganizationWebProxyClient, IOrganizationService
{
    if (organizationService == null)
    {
        string resource = ConfigurationManager.AppSettings["CrmBaseUrl"].ToString();
        string clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
        string clientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
        string authority = ConfigurationManager.AppSettings["Authority"].ToString();

        //Ininitiate the Client Credential
        ClientCredential clientcred = new ClientCredential(clientId, clientSecret);

        // Create a AuthenticationContext using the Authenticating Authority
        AuthenticationContext authenticationContext = new AuthenticationContext(authority);
        AuthenticationResult authenticationResult = authenticationContext.AcquireTokenAsync(resource, clientcred).Result;
        string crmAccessToken = authenticationResult.AccessToken;

        string orgServiceRelativeUrl = ConfigurationManager.AppSettings["OrganizationServiceRelativeUrl"].ToString();
        Uri orgServiceUri = new Uri(new Uri(resource), orgServiceRelativeUrl);

        ServiceProxyClient = new OrganizationWebProxyClient(orgServiceUri, false)
        {
            //Set the Auth Token 
            HeaderToken = crmAccessToken
        };

        organizationService = ServiceProxyClient as IOrganizationService;
        LogHelper.LogInfo($"CrmConnection.Connect :: Initiated a new Connection to CRM");
    }

    if (typeof(T) == typeof(OrganizationWebProxyClient))
        return (T)ServiceProxyClient;

    else if (typeof(T) is IOrganizationService)
        return (T)organizationService;
    else
    {
        return default(T);
    }

}


/*
Add a config section on the client app/web config file

<add key="Authority" value="https://login.microsoftonline.com/<crm-org-name>.onmicrosoft.com" />
<add key="CrmBaseUrl" value="https://<crm-org-name>.crm.dynamics.com" />
<add key="OrganizationServiceRelativeUrl" value="/XRMServices/2011/Organization.svc/web?SdkClientVersion=9.1" />
<!-- 
     // Substitute your app registration values that can be obtained after you  
     // register the app in Active Directory on the Microsoft Azure portal.  
-->
<add key="ClientId" value="00000000-0000-0000-0000-000000000000" />
<add key="ClientSecret" value="{W]2^}J;QxB{)ij;+na<O.!]C#J^e.*:.@" /> 

*/
