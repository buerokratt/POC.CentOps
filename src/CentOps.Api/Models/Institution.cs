using CentOps.Api.Services;

namespace CentOps.Api.Models
{
    public class Institution : IModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public InstitutionStatus Status { get; set; }
    }
}
