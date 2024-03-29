﻿using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Services.ModelStore.Exceptions
{
    // Excluded as exception contains no logic.
    [ExcludeFromCodeCoverage]
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
