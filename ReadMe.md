# Azure Authentication Example
The example consists of two applications:
* a single-page client application (SPA)
* a WebApi backed application

The project demonstrates:
* authentication for public applications using the Auth Code Flow
* calling protected Web API from a public application, both Graph and custom
* authentication and authorization using Azure AD on the Web Api side
* WebApi - WebApi call on behalf of the authenticated user (the On Behalf Of Flow)
* WebApi - WebApi call as application (the Client Credentials flow)

Technologies used
* plain JavaScript and MSAL.js for the SPA
* ASP.NET Core 6 and Microsoft.Identity.Web for the Web Api

Build and run
* the applications might be launched locally
* some Azure configuration required
* the WebApi client secret should be stored in the secret store for local run

Required Azure configuration
* both applications should be registered in Azure AD, their Client IDs should be specified in the configuration files / source code
* the client application should be registered with SPA platform support and a valid redirect URI
* the WebApi application registration doesn't need a platform configuration, but needs a client secret
* in the WebApi registration, a Application ID URI should be generated
* in the WebApi registration, some scopes should be added:
  * Profile.View (consent by users and admins)
  * Users.View (consent by admins only)
* the client application registration should be given API permissions for
  * Profile.View, admin consent isn't required
  * Users.View, admin consent should be granted
* the WebApi registration should be given API permissions to call Graph (Directory.Read.All, User.Read,
  User.Read.All), admin consent should be granted for all those permissions

Limitations
* the solution works well for the users directly registered in the Azure AD instance (i.e. the single-tenant scenario)
* dynamic consent requests originated from the WebApi is not supported
* in the multi-tenant scenario, including personal accounts usage, WebApi - WebAPi calls doesn't work because admin consent can't be granted for such cases, the WebApi fails to call Graph with a challenge for additional consent
* the SPA uses popup windows to authenticate users for the sake of simplicity (redirecting is the other option)

Highlights and Issues
* when asking for an access token to a Web API from client (SPA or another Web API in the case of Client Credentials flow), **.default** scope should be specified
* both the SPA and the API uses caching, in the case of any trouble, restart the API app (as it uses in-memory caching) or logout and login again on the client side (as it uses session storage to cache tokens)

Questions / further development
* what if the client application has been added as an Authorized Client Application in the Web Api registration?
* use KeyVault and/or certificates instead of the client secret approach as it is more production-like scenario