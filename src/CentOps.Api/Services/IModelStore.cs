namespace CentOps.Api.Services
{
    public interface IModelStore<TModel> where TModel : IModel
    {
        Task<TModel> Create(TModel model);

        Task<TModel> GetById(string id);

        Task<IEnumerable<TModel>> GetAll();

        Task<TModel> Update(TModel model);

        Task<TModel> DeleteById(string id);
    }
}
