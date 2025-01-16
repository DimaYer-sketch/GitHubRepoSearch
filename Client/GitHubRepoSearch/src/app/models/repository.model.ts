export interface RepositoryModel {
  id: string;
  name: string;
  ownerAvatarUrl: string;

  /**
   * Indicates whether the repository is bookmarked.
   */
  isBookmarked?: boolean;
}
