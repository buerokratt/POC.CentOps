using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.Azure.Cosmos;
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
            var existingName = await GetExistingInstitutions(query).ConfigureAwait(false);
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
                throw new ArgumentException($"{nameof(id)} not specified.");
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
            var query = _container.GetItemQueryIterator<InstitutionDto>(new QueryDefinition("SELECT * FROM c WHERE STARTSWITH(c.PartitionKey, \"institution\", false)"));
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
                throw new ArgumentException($"{nameof(id)} not specified.");
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

        Task<IEnumerable<ParticipantDto>> IInstitutionStore.GetParticipantsByInstitutionId(string id)
        {
            // return string.IsNullOrEmpty(id)
            //     ? throw new ArgumentException($"{nameof(id)} not specified.")
            //     : Task.FromResult(_participants.Values.Where(key => id.Equals(key.InstitutionId, StringComparison.Ordinal)).AsEnumerable());
            throw new NotImplementedException();
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

            var existingName = await GetExistingInstitutions(query).ConfigureAwait(false);
            if (existingName != null && existingName.Any())
            {
                throw new ModelExistsException<InstitutionDto>(model.Name);
            }

            var response = await _container.UpsertItemAsync(model, new PartitionKey(model.PartitionKey)).ConfigureAwait(false);

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
                              AND STARTSWITH(c.PartitionKey, 'participant', false)";
            var query = new QueryDefinition(queryString)
                .WithParameter("@name", model.Name);
            var existingName = await GetExistingParticipants(query).ConfigureAwait(false);
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
                throw new ArgumentException($"{nameof(id)} not specified.");
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
            var query = _container.GetItemQueryIterator<ParticipantDto>(new QueryDefinition("SELECT * FROM c WHERE STARTSWITH(c.PartitionKey, \"participant\", false)"));
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
                throw new ArgumentException($"{nameof(id)} not specified.");
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
            var existingName = await GetExistingParticipants(query).ConfigureAwait(false);
            if (existingName != null && existingName.Any())
            {
                throw new ModelExistsException<ParticipantDto>(model.Name);
            }

            _ = await _container.UpsertItemAsync(model, new PartitionKey(model.PartitionKey)).ConfigureAwait(false);

            return model;
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

        private async Task<IEnumerable<InstitutionDto>> GetExistingInstitutions(QueryDefinition queryString)
        {
            var query = _container.GetItemQueryIterator<InstitutionDto>(queryString);
            List<InstitutionDto> results = new();
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

        private async Task<IEnumerable<ParticipantDto>> GetExistingParticipants(QueryDefinition queryString)
        {
            var query = _container.GetItemQueryIterator<ParticipantDto>(queryString);
            List<ParticipantDto> results = new();
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
    }
}
