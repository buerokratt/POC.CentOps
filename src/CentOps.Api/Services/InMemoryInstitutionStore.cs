﻿using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Collections.Concurrent;

namespace CentOps.Api.Services
{
    public sealed partial class InMemoryStore : IInstitutionStore
    {
        private readonly ConcurrentDictionary<string, InstitutionDto> _institutions = new();

        public Task<InstitutionDto> Create(InstitutionDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            var existingName = _institutions.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<InstitutionDto>(model.Name);
            }

            model.Id = Guid.NewGuid().ToString();
            _ = _institutions.TryAdd(model.Id, model);
            return Task.FromResult(model);
        }

        public async Task<bool> DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"{nameof(id)} not specified.");
            }

            if (_institutions.ContainsKey(id))
            {
                await CheckAssociatedParticipantAsync(id).ConfigureAwait(false);
                _ = _institutions.Remove(id, out _);
                return true;
            }

            return false;
        }

        public Task<IEnumerable<InstitutionDto>> GetAll()
        {
            return Task.FromResult(_institutions.Values.AsEnumerable());
        }

        public Task<InstitutionDto?> GetById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : _institutions.ContainsKey(id)
                    ? Task.FromResult<InstitutionDto?>(_institutions[id])
                    : Task.FromResult<InstitutionDto?>(null);
        }

        public Task<InstitutionDto> Update(InstitutionDto model)
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

            if (!_institutions.ContainsKey(model.Id))
            {
                throw new ModelNotFoundException<InstitutionDto>(model.Id);
            }

            var existingName = _institutions.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<InstitutionDto>(model.Name!);
            }

            _institutions[model.Id] = model;

            return Task.FromResult(model);
        }
    }
}