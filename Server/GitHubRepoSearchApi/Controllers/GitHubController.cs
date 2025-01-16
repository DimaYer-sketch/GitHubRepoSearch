using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using GitHubRepoSearchApi.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubRepoSearchApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GitHubController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GitHubController(IHttpClientFactory httpClientFactory)
        {
            // Create an instance of HttpClient with default headers.
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHubRepoSearchAPI");
        }

        /// <summary>
        /// Searches for GitHub repositories based on the provided keyword.
        /// </summary>
        /// <param name="keyword">Search keyword for GitHub repositories.</param>
        /// <returns>A list of repositories matching the keyword.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchRepositories([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Search keyword cannot be null or empty." });
            }

            try
            {
                // Construct the GitHub API URL for searching repositories.
                var url = $"https://api.github.com/search/repositories?q={Uri.EscapeDataString(keyword)}";

                // Send the GET request to the GitHub API.
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // Log the status code and reason for debugging.
                    Console.Error.WriteLine($"GitHub API request failed with status: {response.StatusCode}, reason: {response.ReasonPhrase}");
                    return StatusCode((int)response.StatusCode, new { message = "GitHub API request failed.", details = response.ReasonPhrase });
                }

                // Read the response content as a string.
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into GitHubApiResponse.
                var apiResponse = JsonConvert.DeserializeObject<GitHubApiResponse>(content);

                // Map the response items to RepositoryModel.
                var items = apiResponse?.Items.Select(item => new RepositoryModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    OwnerAvatarUrl = item.Owner?.AvatarUrl ?? string.Empty
                }).ToList();

                // Return the mapped repository models.
                return Ok(new { items });
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging.
                Console.Error.WriteLine($"Exception occurred while processing the request: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
