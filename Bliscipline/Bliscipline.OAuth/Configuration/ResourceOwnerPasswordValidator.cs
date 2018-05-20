using Bliscipline.Data.Helpers;
using Bliscipline.Data.Repositories;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bliscipline.OAuth.Configuration
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private IUserRepository repository;

        public ResourceOwnerPasswordValidator(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await repository.GetAsync(context.UserName, HashHelper.Sha512(context.Password + context.UserName));

            if (user != null)
            {
                context.Result = new GrantValidationResult(user.Id.ToString(),
                    authenticationMethod: "custom");
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,
                    "Invalid Credentials");
            }
        }
    }
}
