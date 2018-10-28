# IdentityServer4.ImplicitFlow.InMemory.MinimalBootsrapUi.NgCient
Shows how the [IdentityServer4](http://docs.identityserver.io/en/dev/) with [Identity](https://docs.asp.net/en/latest/security/authentication/identity.html) can be combined with Web API and [Angular7](https://blog.angular.io/) client inside a single [ASP.NET Core](https://docs.asp.net/en/latest/) project using [OpenID Connect](http://openid.net/connect/) Implicit Flow with [JWT](https://jwt.io/) access token and [angular-oauth2-oidc](https://github.com/manfredsteyer/angular-oauth2-oidc) npm package.
Uses a minimalistic login form based on [Quickstart.UI](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI).

Inspired by this [article by damienbod](https://damienbod.com/2016/10/01/identityserver4-webapi-and-angular2-in-a-single-asp-net-core-project/).

Try it live: [https://binckbamexantecosts2.azurewebsites.net/](https://binckbamexantecosts2.azurewebsites.net/).

This example exposes three protected API methods with `api_level` [`basic_api`, `advanced_api`, `administration_api`] and one public method which is accessible to all authenticated users.

In-memory test users are:

| username | password | api_level                  |
|----------|----------|----------------------------|
| admin    | a        | `administration_api`       |
| user     | u        | `basic_api` `advanced_api` |
| guest    | g        | `basic_api`                |

This example uses two separate self-signed X509 certificates for `id` and `access` tokens.
The Poweshell scripts used to generate them were adapted from [Localhost SSL and IdentityServer4 Token Certificates](https://mcguirev10.com/2018/01/04/localhost-ssl-identityserver-certificates.html).
