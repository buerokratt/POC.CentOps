namespace CentOps.Api.Services
{
    public interface IModelStore<TModel> where TModel : IModel
    {
        Task<TModel> Create(TModel participant);

        Task<TModel> ReadParticipantById(string id);

        Task<IEnumerable<TModel>> ReadParticipants();

        Task<TModel> UpdateParticipant(TModel participant);

        Task<TModel> DeleteParticipantById(string id);
    }
}
