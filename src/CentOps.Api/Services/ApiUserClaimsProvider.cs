﻿using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Authentication.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using System.Security.Claims;

namespace CentOps.Api.Services
{
    public class ApiUserClaimsProvider : IApiUserClaimsProvider
    {
        private readonly IApiUserStore _userStore;

        public ApiUserClaimsProvider(IApiUserStore userStore)
        {
            _userStore = userStore;
        }

        public async Task<ApiUser?> GetUserClaimsAsync(string apiKey)
        {
            var user = await _userStore.GetByKeyAsync(apiKey).ConfigureAwait(false);

            ApiUser? result = null;

            if (user != null && !string.IsNullOrEmpty(user.Id)) // Code analysis requires the 'Id' null check
            {
                result = new ApiUser(new[]
                {
                    new Claim("id", user.Id),
                    new Claim("isAdmin", user.IsAdmin.ToString().ToUpperInvariant())
                });
            }

            return result;
        }
    }
}
