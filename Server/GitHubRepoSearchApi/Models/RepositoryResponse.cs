namespace GitHubRepoSearchApi.Models
{
    public class RepositoryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Owner Owner { get; set; }
    }
}
