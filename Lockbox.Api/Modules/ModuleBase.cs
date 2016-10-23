using System;
using System.Security.Authentication;
using System.Text;
using Lockbox.Api.Extensions;
using Lockbox.Api.Requests;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;

namespace Lockbox.Api.Modules
{
    public abstract class ModuleBase : NancyModule
    {
        protected ModuleBase(string modulePath = "") : base(modulePath)
        {
        }

        protected T BindRequest<T>() => this.Bind<T>();

        protected T BindBasicAuthenticationRequest<T>() where T : BasicAuthenticationRequest
        {
            var request = BindRequest<T>();
            var authorizationTypeAndToken = Context.Request.Headers.Authorization.ParseAuthorzationHeader();
            if (authorizationTypeAndToken.Key != "basic")
                throw new AuthenticationException("Basic authentication header has not been found.");

            var encodedToken = Convert.FromBase64String(authorizationTypeAndToken.Value);
            var decodedToken = Encoding.UTF8.GetString(encodedToken);
            if (!decodedToken.Contains(":"))
                throw new AuthenticationException("Invalid authentication header.");

            var tokenValues = decodedToken.Split(':');
            request.Username = tokenValues[0];
            request.Password = tokenValues[1];

            return request;
        }

        protected string CurrentUsername => Context.CurrentUser.Identity.Name;

        protected Negotiator Created(string endpoint)
            => Negotiate.WithHeader("Location", $"{Request.Url.SiteBase}{endpoint}")
                .WithStatusCode(HttpStatusCode.Created);
    }
}