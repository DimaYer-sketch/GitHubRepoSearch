import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RepositoryModel } from '../models/repository.model';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiBaseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Retrieves a new JWT token from the server.
   * @returns {Observable<{ token: string }>} The token response.
   */
  getToken(): Observable<{ token: string }> {
    return this.http.get<{ token: string }>(`${this.apiBaseUrl}/auth/token`);
  }

  /**
   * Searches for repositories based on a keyword.
   * @param {string} keyword The search keyword.
   * @returns {Observable<{ items: RepositoryModel[] }>} The search results.
   */
  searchRepositories(
    keyword: string
  ): Observable<{ items: RepositoryModel[] }> {
    return this.http.get<{ items: RepositoryModel[] }>(
      `${this.apiBaseUrl}/github/search?keyword=${encodeURIComponent(keyword)}`
    );
  }

  /**
   * Adds a repository to bookmarks.
   * @param {RepositoryModel} bookmark The repository to bookmark.
   * @returns {Observable<void>} The server response.
   */
  addBookmark(bookmark: RepositoryModel): Observable<void> {
    return this.http.post<void>(`${this.apiBaseUrl}/bookmarks`, bookmark);
  }

  /**
   * Retrieves all bookmarks.
   * @returns {Observable<RepositoryModel[]>} The list of bookmarks.
   */
  getBookmarks(): Observable<RepositoryModel[]> {
    return this.http.get<RepositoryModel[]>(`${this.apiBaseUrl}/bookmarks`);
  }

  /**
   * Removes a bookmark by its ID.
   * @param {string} bookmarkId The ID of the bookmark to remove.
   * @returns {Observable<void>} The server response.
   */
  removeBookmark(bookmarkId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/bookmarks/${bookmarkId}`);
  }
}
