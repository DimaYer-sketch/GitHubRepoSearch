import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RepoSearchComponent } from './components/repo-search/repo-search.component';
import { BookmarkListComponent } from './components/bookmark-list/bookmark-list.component';

const routes: Routes = [
  { path: '', redirectTo: '/search', pathMatch: 'full' }, // Default route
  { path: 'search', component: RepoSearchComponent }, // Search page
  { path: 'bookmarks', component: BookmarkListComponent }, // Bookmarks page
  { path: '**', redirectTo: '/search' }, // Wildcard route for invalid paths
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
