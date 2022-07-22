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
            _ = CreateMap<ParticipantDto, AdminParticipantResponseModel>();
            _ = CreateMap<CreateUpdateInsitutionModel, InstitutionDto>();
            _ = CreateMap<InstitutionDto, InstitutionResponseModel>();
            _ = CreateMap<ParticipantDto, ParticipantStateReponseModel>();
            _ = CreateMap<ParticipantStatus, ParticipantStatusDto>();
        }
    }
}
