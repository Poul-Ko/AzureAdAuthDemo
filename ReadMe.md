# Azure Authentication Example
The example consists of two applications:
* a single-page client application (SPA)
* a WebApi backed application

The project demonstrates:
* authentication for public applications using the Auth Code Flow
* calling protected Web API from a public application, both Graph and custom
* authentication and authorization using Azure AD on the Web Api side
* WebApi - WebApi call on behalf of the authenticated user (the On Behalf Of Flow)

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
* in the WebApi registration, a Application ID URI should be generated, and **Profile.View** scope should be added
* the client application registration should be given an API permission for Profile.View, admin consent isn't required
* the WebApi registration should be given API permissions to call Graph (Directory.Read.All, User.Read)
* admin consent should be granted for WebApi as the current implementation doesn't support dynamic consent

Limitations
* the solution works well for the users directly registered in the Azure AD instance (i.e. the single-tenant scenario)
* dynamic consent requests originated from the WebApi is not supported
* in the multi-tenant scenario, including personal accounts usage, WebApi - WebAPi calls doesn't work because admin consent can't be granted for such cases, the WebApi fails to call Graph with a challenge for additional consent  