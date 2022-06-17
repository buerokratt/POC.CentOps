namespace CentOps.Api.Services
{
    public interface IModelStore<TModel> where TModel : IModel
    {
        Task<TModel> Create(TModel participant);

        Task<TModel> GetById(string id);

        Task<IEnumerable<TModel>> GetAll();

        Task<TModel> Update(TModel participant);

        Task<TModel> DeleteById(string id);
    }
}
