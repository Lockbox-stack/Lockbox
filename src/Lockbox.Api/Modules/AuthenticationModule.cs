using System;
using System.Collections.Generic;
using System.Linq;
using Lockbox.Api.Domain;
using Lockbox.Api.Extensions;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;


namespace Lockbox.Api.Modules
{
    public class AuthenticationModule : ModuleBase
    {
        public AuthenticationModule(IAuthenticationService authenticationService)
        {
            Post("authenticate", async args =>
            {
                var request = BindRequest<Authenticate>();
                var token = await authenticationService.AuthenticateAsync(request.Username, request.Password);

                return new 
                {
                    token = token.Token, 
                    expiry = token.Expiry.ToTimestamp()
                };
            });
        }
    }
}