import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/services/api.service';
import { RepositoryModel } from '../../models/repository.model';

@Component({
  selector: 'app-repo-search',
  templateUrl: './repo-search.component.html',
  styleUrls: ['./repo-search.component.scss'],
})
export class RepoSearchComponent implements OnInit {
  keyword: string = ''; // Holds the search keyword
  repositories: RepositoryModel[] = []; // Holds the search results
  searchPerformed: boolean = false; // Indicates if a search has been performed
  loading: boolean = false; // Tracks the loading state

  constructor(private apiService: ApiService) {}

  /**
   * Initializes the component.
   * Fetches the JWT token if not already stored and restores search state.
   */
  ngOnInit(): void {
    // Retrieve and store the token if not already present
    const token = localStorage.getItem('jwtToken');
    if (!token) {
      this.apiService.getToken().subscribe({
        next: (response) => {
          localStorage.setItem('jwtToken', response.token);
          console.log('JWT token retrieved and saved.');
        },
        error: (error) => {
          console.error('Failed to retrieve JWT token:', error);
          alert('Authentication failed. Some features may not work.');
        },
      });
    }

    // Restore search state from localStorage if available
    const savedState = localStorage.getItem('searchState');
    if (savedState) {
      const { keyword, repositories } = JSON.parse(savedState);
      this.keyword = keyword;
      this.repositories = repositories;
      this.searchPerformed = true;
      this.updateBookmarksStatus();
    }
  }

  /**
   * Searches for repositories based on the entered keyword.
   */
  searchRepos(): void {
    if (!this.keyword.trim()) {
      alert('Please enter a valid keyword.');
      return;
    }

    this.loading = true;

    this.apiService.searchRepositories(this.keyword).subscribe({
      next: (data: { items: RepositoryModel[] }) => {
        this.repositories = data.items;
        this.loading = false;
        this.searchPerformed = true;

        // Save the search state to localStorage
        localStorage.setItem(
          'searchState',
          JSON.stringify({
            keyword: this.keyword,
            repositories: this.repositories,
          })
        );

        this.updateBookmarksStatus();
        console.log('Repositories loaded successfully.');
      },
      error: (error: any) => {
        this.loading = false;
        console.error('Failed to load repositories:', error);
        alert('Failed to load repositories. Please try again.');
      },
    });
  }

  /**
   * Adds a repository to bookmarks if it's not already added.
   * @param repo The repository to bookmark.
   */
  addBookmark(repo: RepositoryModel): void {
    const token = localStorage.getItem('jwtToken');
    if (!token) {
      alert('Authentication required. Please refresh the page.');
      return;
    }

    this.apiService.addBookmark(repo).subscribe({
      next: () => {
        console.log(`"${repo.name}" added to bookmarks.`);
        repo.isBookmarked = true;
        alert('Bookmark added successfully.');
      },
      error: (err) => {
        console.error('Error adding bookmark:', err);
        alert('Failed to add bookmark. Please try again.');
      },
    });
  }

  /**
   * Updates the "isBookmarked" status for each repository.
   */
  private updateBookmarksStatus(): void {
    this.apiService.getBookmarks().subscribe({
      next: (bookmarks) => {
        const bookmarkedIds = new Set(bookmarks.map(b => b.id));
        this.repositories.forEach(repo => {
          repo.isBookmarked = bookmarkedIds.has(repo.id);
        });
      },
      error: (error) => {
        console.error('Failed to update bookmarks status:', error);
      },
    });
  }
}
