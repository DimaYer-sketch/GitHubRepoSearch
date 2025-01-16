using GitHubRepoSearchApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace GitHubRepoSearchApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookmarksController : ControllerBase
    {
        // In-memory storage for bookmarks, keyed by user ID
        private static readonly ConcurrentDictionary<string, List<RepositoryModel>> BookmarksStorage = new();

        /// <summary>
        /// Adds a repository to the user's bookmarks.
        /// </summary>
        /// <param name="repository">The repository to bookmark.</param>
        /// <returns>A confirmation message.</returns>
        [Authorize]
        [HttpPost]
        public IActionResult AddBookmark([FromBody] RepositoryModel repository)
        {
            if (repository == null || string.IsNullOrWhiteSpace(repository.Id))
            {
                return BadRequest("Invalid repository data.");
            }

            // Retrieve the user's unique identifier from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Initialize the user's bookmark list if not present
            if (!BookmarksStorage.ContainsKey(userId))
            {
                BookmarksStorage[userId] = new List<RepositoryModel>();
            }

            // Check if the repository is already bookmarked
            if (BookmarksStorage[userId].Any(b => b.Id == repository.Id))
            {
                return Conflict(new { message = "Bookmark already exists." });
            }

            // Add the repository to the user's bookmarks
            BookmarksStorage[userId].Add(repository);

            return Ok(new { message = "Bookmark added successfully." });
        }

        /// <summary>
        /// Retrieves all bookmarks for the current user.
        /// </summary>
        /// <returns>A list of bookmarked repositories.</returns>
        [Authorize]
        [HttpGet]
        public IActionResult GetBookmarks()
        {
            // Retrieve the user's unique identifier from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Return the user's bookmarks or an empty list if none exist
            if (!BookmarksStorage.TryGetValue(userId, out var userBookmarks) || userBookmarks.Count == 0)
            {
                return Ok(new List<RepositoryModel>());
            }

            return Ok(userBookmarks);
        }

        /// <summary>
        /// Deletes a repository from the user's bookmarks.
        /// </summary>
        /// <param name="id">The ID of the repository to remove.</param>
        /// <returns>A confirmation message.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult RemoveBookmark([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Repository ID cannot be null or empty.");
            }

            // Retrieve the user's unique identifier from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            if (!BookmarksStorage.TryGetValue(userId, out var userBookmarks))
            {
                return NotFound("No bookmarks found for the user.");
            }

            // Find the bookmark to remove
            var bookmarkToRemove = userBookmarks.FirstOrDefault(b => b.Id == id);

            if (bookmarkToRemove == null)
            {
                return NotFound("Bookmark with the specified ID not found.");
            }

            // Remove the bookmark
            userBookmarks.Remove(bookmarkToRemove);

            return Ok(new { message = "Bookmark removed successfully." });
        }
    }
}
