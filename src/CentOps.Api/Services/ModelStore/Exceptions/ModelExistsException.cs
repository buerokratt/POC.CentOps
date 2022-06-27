using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Services.ModelStore.Exceptions
{
    // Excluded as exception contains no logic.
    [ExcludeFromCodeCoverage]
    public class ModelExistsException<TModel> : Exception where TModel : class
    {
        public ModelExistsException()
            : base($"{typeof(TModel).Name} already exists.")
        {
        }

        public ModelExistsException(string model)
            : base($"{typeof(TModel).Name} '{model}' already exists.")
        {
        }

        public ModelExistsException(string model, Exception innerException)
            : base($"{typeof(TModel).Name} '{model}' already exists.", innerException)
        {
        }
    }
}
