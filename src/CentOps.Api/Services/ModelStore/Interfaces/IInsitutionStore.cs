using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services.ModelStore.Interfaces
{
    public interface IInstitutionStore : IModelStore<InstitutionDto>
    {
    }

    public class CosmosDb : IModelStore<InstitutionDto>, IModelStore<ParticipantDto>
    {
        public Task<InstitutionDto> Create(InstitutionDto model)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipantDto> Create(ParticipantDto model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InstitutionDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<InstitutionDto?> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<InstitutionDto> Update(InstitutionDto model)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipantDto> Update(ParticipantDto model)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ParticipantDto>> IModelStore<ParticipantDto>.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<ParticipantDto?> IModelStore<ParticipantDto>.GetById(string id)
        {
            throw new NotImplementedException();
        }
    }

}


