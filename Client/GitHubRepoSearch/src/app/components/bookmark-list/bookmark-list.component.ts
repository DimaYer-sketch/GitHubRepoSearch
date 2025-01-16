import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/services/api.service';
import { RepositoryModel } from '../../models/repository.model';

@Component({
  selector: 'app-bookmark-list',
  templateUrl: './bookmark-list.component.html',
  styleUrls: ['./bookmark-list.component.scss'],
})
export class BookmarkListComponent implements OnInit {
  bookmarks: RepositoryModel[] = []; // Stores the list of bookmarks
  loading: boolean = false; // Tracks the loading state

  constructor(private apiService: ApiService) {}

  /**
   * Initializes the component.
   * Fetches the list of bookmarks from the server and handles errors.
   */
  ngOnInit(): void {
    this.loadBookmarks();
  }

  /**
   * Loads bookmarks from the server.
   */
  loadBookmarks(): void {
    this.loading = true;

    this.apiService.getBookmarks().subscribe({
      next: (bookmarks: RepositoryModel[]) => {
        this.bookmarks = bookmarks;
        this.loading = false;
        console.log('Bookmarks loaded successfully.');
      },
      error: (error: any) => {
        this.loading = false;
        console.error('Failed to load bookmarks:', error);
        alert('Failed to load bookmarks. Please try again.');
      },
    });
  }

  /**
   * Removes a bookmark by its ID.
   * @param bookmarkId The ID of the bookmark to remove.
   */
  removeBookmark(bookmarkId: string): void {
    if (!confirm('Are you sure you want to remove this bookmark?')) {
      return;
    }

    this.apiService.removeBookmark(bookmarkId).subscribe({
      next: () => {
        // Remove the bookmark from the local list
        this.bookmarks = this.bookmarks.filter((b) => b.id !== bookmarkId);
        console.log(`Bookmark with ID ${bookmarkId} removed successfully.`);
      },
      error: (error: any) => {
        console.error('Failed to remove bookmark:', error);
        alert('Failed to remove bookmark. Please try again.');
      },
    });
  }
}
