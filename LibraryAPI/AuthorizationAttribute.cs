using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationAttribute : Attribute, IResourceFilter
    {
        private const string AuthorizationHeaderKey = "Authorization";
        private const string AuthorizationKeyValue = "mulonavain";

        private readonly string conditionalKey;

        /// <summary>
        /// Uses default authorization
        /// </summary>
        public AuthorizationAttribute()
        {

        }

        /// <summary>
        /// Override deafult authorization and give your own authorization key to validate access
        /// </summary>
        /// <param name="conditionalKey"></param>
        public AuthorizationAttribute(string conditionalKey)
        {
            this.conditionalKey = conditionalKey;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {

            if (!context.HttpContext.Request.Headers.TryGetValue(AuthorizationHeaderKey, out var secretKey))
            {
                context.Result = new UnauthorizedResult();
            }

            if (!string.IsNullOrEmpty(conditionalKey))
            {
                if (secretKey != conditionalKey)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                if (secretKey != AuthorizationKeyValue)
                {
                    context.Result = new UnauthorizedResult();
                }
            }

        }
    }
}