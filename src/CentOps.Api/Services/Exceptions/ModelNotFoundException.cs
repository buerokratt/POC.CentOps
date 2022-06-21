namespace CentOps.Api.Services.Exceptions
{
    public class ModelNotFoundException<TModel> : Exception where TModel : class
    {
        public ModelNotFoundException()
            : base($"{typeof(TModel).Name} not found.")
        {
        }

        public ModelNotFoundException(string model)
            : base($"{typeof(TModel).Name} '{model}' not found.")
        {
        }

        public ModelNotFoundException(string model, Exception innerException)
            : base($"{typeof(TModel).Name} '{model}' not found.", innerException)
        {
        }
    }
}
