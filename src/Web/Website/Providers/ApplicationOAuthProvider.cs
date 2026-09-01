using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Website.App_Start;
using Website.Models;

namespace Website.Providers
{
	public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
	{
		private readonly string _publicClientId;

		public ApplicationOAuthProvider(string publicClientId)
		{
			if (publicClientId == null)
			{
				throw new ArgumentNullException("publicClientId");
			}

			_publicClientId = publicClientId;
		}


        /// <summary>
        /// Added from Website.OLD (unsure is it needed)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        /// <summary>
        /// Added from Website.OLD (unsure is it needed)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Added from Website.OLD (unsure is it needed)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

		public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
		{
			if (context.ClientId == _publicClientId)
			{
				// Workaround code, to solve redirect to ROOT problem via returnUrl appendix
				// Added analysis of returnUrl parameter
				Uri expectedUri;
				if (!string.IsNullOrEmpty(context.Request.Query["returnUrl"]))
				{
					expectedUri = new Uri(context.Request.Query["returnUrl"]);
				}
				else
				{
					expectedUri = new Uri(context.Request.Uri, "/");
				}

				if (expectedUri.AbsoluteUri == context.RedirectUri)
				{
					context.Validated();
				}
				else if (context.ClientId == "web")
				{
					//var expectedUri = new Uri(context.Request.Uri, "/");
					context.Validated(expectedUri.AbsoluteUri);
				}
			}

			return Task.FromResult<object>(null);
		}

        /// <summary>
        /// /// Added from Website.OLD (unsure is it needed)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
	}
}