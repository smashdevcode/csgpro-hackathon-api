﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Net;
using System.Security.Principal;
using CSGProHackathonAPI.Shared.Data;
using CSGProHackathonAPI.Shared.Infrastructure;

namespace CSGProHackathonAPI.Infrastructure
{
    /// <summary>
    /// This class taken from: http://bitoftech.net/2013/12/03/enforce-https-asp-net-web-api-basic-authentication/
    /// </summary>
    public class BasicAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public BasicAuthorizeAttribute()
        {
        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            // Case that user is authenticated using forms authentication
            // so no need to check header for basic authentication.
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return;
            }

            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    var credArray = GetCredentials(authHeader);
                    var userName = credArray[0];
                    var password = credArray[1];

                    // TODO get the context and repository from the DI container
                    var context = new Context();
                    var repository = new Repository(context);
                    var user = repository.LoginUser(userName, password);
                    if (user != null)
                    {
                        var userIdentity = new UserIdentity(user);
                        var currentPrincipal = new GenericPrincipal(userIdentity, null);
                        Thread.CurrentPrincipal = currentPrincipal;
                        return;
                    }
                }
            }

            HandleUnauthorizedRequest(actionContext);
        }

        private string[] GetCredentials(System.Net.Http.Headers.AuthenticationHeaderValue authHeader)
        {
            // Base 64 encoded string
            var rawCred = authHeader.Parameter;
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var cred = encoding.GetString(Convert.FromBase64String(rawCred));

            var credArray = cred.Split(':');

            return credArray;
        }

        private void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='CSG Pro Hackathon API' location='http://csgprohackathonapi.azurewebsites.net'");
        }
    }
}
