using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;

namespace CentOps.Api.Services
{
    public sealed class CosmosDbService : IInstitutionStore, IParticipantStore
    {
        private readonly CosmosClient _dbClient;
        private readonly Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _dbClient = dbClient ?? throw new ArgumentNullException(databaseName);
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        async Task<InstitutionDto> IModelStore<InstitutionDto>.Create(InstitutionDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            var queryString = @"SELECT *
                              FROM c
                              WHERE c.name = @name
                              AND STARTSWITH(c.PartitionKey, 'institution', false)";

            var query = new QueryDefinition(queryString)
                .WithParameter("@name", model.Name);
            var existingName = await RunQueryAsync<InstitutionDto>(query).ConfigureAwait(false);
            if (existingName != null && existingName.Any())
            {
                throw new ModelExistsException<InstitutionDto>(model.Name);
            }

            model.Id = Guid.NewGuid().ToString();
            model.PartitionKey = $"institution::{model.Id}";
            _ = await _container.CreateItemAsync(model, new PartitionKey(model.PartitionKey)).ConfigureAwait(false);
            return model;
        }

        async Task<bool> IModelStore<InstitutionDto>.DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            using (ResponseMessage responseMessage = await _container.ReadItemStreamAsync(
                partitionKey: new PartitionKey($"institution::{id}"),
                id: id).ConfigureAwait(false))
            {
                if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                if (responseMessage.IsSuccessStatusCode)
                {
                    await CheckAssociatedParticipantAsync(id).ConfigureAwait(false);
                    _ = await _container.DeleteItemAsync<InstitutionDto>(id, new PartitionKey($"institution::{id}")).ConfigureAwait(false);
                    return true;
                }
            }

            return false;
        }

        async Task<IEnumerable<InstitutionDto>> IModelStore<InstitutionDto>.GetAll()
        {
            var query = _container.GetItemQueryIterator<InstitutionDto>(new QueryDefinition("SELECT * FROM c WHERE STARTSWITH(c.pk, \"institution\", false)"));
            List<InstitutionDto> results = new();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync().ConfigureAwait(false);

                results.AddRange(response.ToList());
            }

            return results;
        }

        async Task<InstitutionDto?> IModelStore<InstitutionDto>.GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                ItemResponse<InstitutionDto> response = await _container.ReadItemAsync<InstitutionDto>(id, new PartitionKey($"institution::{id}")).ConfigureAwait(false);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        async Task<IEnumerable<ParticipantDto>> IInstitutionStore.GetParticipantsByInstitutionId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), $"{nameof(id)} not specified.");
            }
            else
            {
                var queryString = @"SELECT *
                              FROM c
                              WHERE c.institutionId = @id
                              AND STARTSWITH(c.pk, 'participant', false)";
                var query = new QueryDefinition(queryString)
                    .WithParameter("@id", id);
                return await RunQueryAsync<ParticipantDto>(query).ConfigureAwait(false);
            }
        }

        async Task<InstitutionDto> IModelStore<InstitutionDto>.Update(InstitutionDto model)
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

            var queryString = @"SELECT *
                              FROM c
                              WHERE c.name = @name AND NOT c.id = @id";
            var query = new QueryDefinition(queryString)
                .WithParameter("@name", model.Name)
                .WithParameter("@id", model.Id);

            var existingName = await RunQueryAsync<InstitutionDto>(query).ConfigureAwait(false);
            if (existingName != null && existingName.Any())
            {
                throw new ModelExistsException<InstitutionDto>(model.Name);
            }

            var response = await _container.ReplaceItemAsync(model, model.Id, new PartitionKey(model.PartitionKey)).ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.NotFound
                ? throw new ModelNotFoundException<InstitutionDto>(model.Id)
                : model;
        }

        async Task<ParticipantDto> IModelStore<ParticipantDto>.Create(ParticipantDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.InstitutionId))
            {
                throw new ArgumentException(nameof(model.InstitutionId));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            await CheckInstitution(model.InstitutionId).ConfigureAwait(false);

            var queryString = @"SELECT *
                              FROM c
                              WHERE c.name = @name
                              AND STARTSWITH(c.pk, 'participant', false)";
            var query = new QueryDefinition(queryString)
                .WithParameter("@name", model.Name);
            var existingName = await RunQueryAsync<ParticipantDto>(query).ConfigureAwait(false);
            if (existingName != null && existingName.Any())
            {
                throw new ModelExistsException<ParticipantDto>(model.Name);
            }

            model.Id = Guid.NewGuid().ToString();
            model.PartitionKey = $"participant::{model.Id}";
            _ = await _container.CreateItemAsync(model, new PartitionKey(model.PartitionKey)).ConfigureAwait(false);
            return model;
        }

        async Task<bool> IModelStore<ParticipantDto>.DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            using (ResponseMessage responseMessage = await _container.ReadItemStreamAsync(
                partitionKey: new PartitionKey($"participant::{id}"),
                id: id).ConfigureAwait(false))
            {
                if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                if (responseMessage.IsSuccessStatusCode)
                {
                    await CheckAssociatedParticipantAsync(id).ConfigureAwait(false);
                    _ = await _container.DeleteItemAsync<ParticipantDto>(id, new PartitionKey($"participant::{id}")).ConfigureAwait(false);
                    return true;
                }
            }

            return false;
        }

        async Task<IEnumerable<ParticipantDto>> IModelStore<ParticipantDto>.GetAll()
        {
            var query = _container.GetItemQueryIterator<ParticipantDto>(new QueryDefinition("SELECT * FROM c WHERE STARTSWITH(c.pk, \"participant\", false)"));
            List<ParticipantDto> results = new();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync().ConfigureAwait(false);

                results.AddRange(response.ToList());
            }

            return results;
        }

        async Task<ParticipantDto?> IModelStore<ParticipantDto>.GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                ItemResponse<ParticipantDto> response = await _container.ReadItemAsync<ParticipantDto>(id, new PartitionKey($"participant::{id}")).ConfigureAwait(false);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        async Task<ParticipantDto> IModelStore<ParticipantDto>.Update(ParticipantDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                throw new ArgumentException(nameof(model.Id));
            }

            if (string.IsNullOrEmpty(model.InstitutionId))
            {
                throw new ArgumentException(nameof(model.InstitutionId));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            await CheckInstitution(model.InstitutionId).ConfigureAwait(false);

            var queryString = @"SELECT *
                              FROM c
                              WHERE c.name = @name AND NOT c.id = @id";
            var query = new QueryDefinition(queryString)
                .WithParameter("@name", model.Name);
            var existingName = await RunQueryAsync<ParticipantDto>(query).ConfigureAwait(false);
            if (existingName != null && existingName.Any())
            {
                throw new ModelExistsException<ParticipantDto>(model.Name);
            }

            var response = await _container.ReplaceItemAsync(model, model.Id, new PartitionKey(model.PartitionKey)).ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.NotFound
                ? throw new ModelNotFoundException<ParticipantDto>(model.Id)
                : model;
        }

        async Task<ParticipantDto?> IParticipantStore.GetByApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey), $"{nameof(apiKey)} not specified.");
            }

            var query = BuildQueryDefinition(
                @"SELECT * FROM c
                    WHERE STARTSWITH(c.pk, 'participant', false)
                        AND c.apiKey = @apiKey
                        AND c.status != @status",
                ("@apiKey", apiKey),
                ("@status", ParticipantStatusDto.Disabled));

            var results = await RunQueryAsync<ParticipantDto>(query).ConfigureAwait(false);
            return results?.FirstOrDefault();
        }

        private async Task CheckInstitution(string institutionId)
        {
            var institutionStore = this as IInstitutionStore;

            var institution = await institutionStore.GetById(institutionId).ConfigureAwait(false);
            if (institution == null)
            {
                throw new ModelNotFoundException<InstitutionDto>(institutionId);
            }
        }

        private async Task CheckAssociatedParticipantAsync(string institutionId)
        {
            var participantStore = this as IParticipantStore;

            var participants = await participantStore.GetAll().ConfigureAwait(false);
            var associatedParticipant = participants.FirstOrDefault(x => x.InstitutionId == institutionId);

            if (associatedParticipant != null)
            {
                throw new ModelExistsException<ParticipantDto>(associatedParticipant.Id!);
            }
        }

        private async Task<IEnumerable<TModel>> RunQueryAsync<TModel>(QueryDefinition queryString)
            where TModel : class, IModel
        {
            var query = _container.GetItemQueryIterator<TModel>(queryString);
            List<TModel> results = new();
            while (query.HasMoreResults)
            {
                try
                {
                    var response = await query.ReadNextAsync().ConfigureAwait(false);
                    results.AddRange(response.ToList());
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    continue;
                }
            }

            return results;
        }

        private static QueryDefinition BuildQueryDefinition(string queryString, params (string paramName, object paramValue)[] parameters)
        {
            var query = new QueryDefinition(queryString);

            foreach (var (paramName, paramValue) in parameters)
            {
                query = query.WithParameter(paramName, paramValue);
            }

            return query;
        }

        async Task<IEnumerable<ParticipantDto>> IParticipantStore.GetAll(IEnumerable<ParticipantTypeDto> types, bool includeInactive)
        {
            var results = new List<ParticipantDto>();
            var queryable = _container
                .GetItemLinqQueryable<ParticipantDto>()
                .Where(p => p.PartitionKey!.StartsWith("participant", StringComparison.OrdinalIgnoreCase));

            if (types != null && types.Any())
            {
                queryable = queryable.Where(p => types.Contains(p.Type));
            }

            if (includeInactive == false)
            {
                queryable = queryable.Where(p => p.Status == ParticipantStatusDto.Active);
            }

            using (FeedIterator<ParticipantDto> setIterator = queryable.ToFeedIterator())
            {
                // Asynchronous query execution
                while (setIterator.HasMoreResults)
                {
                    foreach (var item in await setIterator.ReadNextAsync().ConfigureAwait(false))
                    {
                        results.Add(item);
                    }
                }
            }

            return results;
        }
    }
}
