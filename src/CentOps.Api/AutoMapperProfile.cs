using AutoMapper;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            _ = CreateMap<CreateUpdateParticipantModel, ParticipantDto>();
            _ = CreateMap<ParticipantDto, ParticipantResponseModel>();
            _ = CreateMap<CreateUpdateInsitutionModel, InstitutionDto>();
            _ = CreateMap<InstitutionDto, InstitutionResponseModel>();
        }
    }
}
