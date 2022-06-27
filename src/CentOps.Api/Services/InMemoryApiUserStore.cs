using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Collections.Concurrent;

namespace CentOps.Api.Services
{
    public sealed partial class InMemoryStore : IApiUserStore
    {
        private readonly ConcurrentDictionary<string, ApiUserDto> _apiUsers = new();

        public Task<ApiUserDto> Create(ApiUserDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            var existingName = _apiUsers.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<ApiUserDto>(model.Name);
            }

            model.Id = Guid.NewGuid().ToString();
            _ = _apiUsers.TryAdd(model.Id, model);
            return Task.FromResult(model);
        }

        async Task<bool> IModelStore<ApiUserDto>.DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"{nameof(id)} not specified.");
            }

            if (_apiUsers.ContainsKey(id))
            {
                await CheckAssociatedParticipantAsync(id).ConfigureAwait(false);
                _ = _apiUsers.Remove(id, out _);
                return true;
            }

            return false;
        }

        Task<IEnumerable<ApiUserDto>> IModelStore<ApiUserDto>.GetAll()
        {
            return Task.FromResult(_apiUsers.Values.AsEnumerable());
        }

        Task<ApiUserDto?> IModelStore<ApiUserDto>.GetById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : _apiUsers.ContainsKey(id)
                    ? Task.FromResult<ApiUserDto?>(_apiUsers[id])
                    : Task.FromResult<ApiUserDto?>(null);
        }

        Task<ApiUserDto> IModelStore<ApiUserDto>.Update(ApiUserDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                throw new ArgumentException(nameof(model.Id));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            if (!_apiUsers.ContainsKey(model.Id))
            {
                throw new ModelNotFoundException<ApiUserDto>(model.Id);
            }

            var existingName = _apiUsers.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<ApiUserDto>(model.Name!);
            }

            _apiUsers[model.Id] = model;

            return Task.FromResult(model);
        }

        public Task<ApiUserDto?> GetByKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var model = _apiUsers.Values.FirstOrDefault(i => i.ApiKey == apiKey);

            return model != null
                ? Task.FromResult<ApiUserDto?>(model)
                : Task.FromResult((ApiUserDto?)null);
        }
    }
}
