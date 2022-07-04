namespace CentOps.Api.Services.ModelStore.Interfaces
{
    public interface IModelStore<TModel> where TModel : class, IModel
    {
        Task<TModel> Create(TModel model);

        Task<TModel?> GetById(string id);

        Task<IEnumerable<TModel>> GetAll();

        Task<TModel> Update(TModel model);

        Task<bool> DeleteById(string id);

        Task<TModel?> GetByApiKeyAsync(string apiKey);
    }
}
