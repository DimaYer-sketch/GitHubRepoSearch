using Newtonsoft.Json;

namespace GitHubRepoSearchApi.Models
{
    public class Owner
    {
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
    }
}
