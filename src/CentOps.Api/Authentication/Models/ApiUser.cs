namespace CentOps.Api.Authentication.Models
{
    public class ApiUser
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public bool IsAdmin { get; set; }
    }
}
