using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services.ModelStore.Interfaces
{
    public interface IApiUserStore : IModelStore<ApiUserDto>
    {
        Task<ApiUserDto?> GetByKeyAsync(string apiKey);
    }
}