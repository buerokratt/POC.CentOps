﻿using Newtonsoft.Json;

namespace CentOps.Api.Services.ModelStore.Interfaces
{
    public interface IModel
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "pk")]
        public string? PartitionKey { get; set; }
    }
}
