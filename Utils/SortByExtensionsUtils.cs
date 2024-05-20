namespace TFGMaui.Utils
{
    public static class SortByExtensionsUtils
    {
        public enum SortBy
        {
            PopularityAsc,
            PopularityDesc,
            ReleaseDateAsc,
            ReleaseDateDesc,
            RevenueAsc,
            RevenueDesc,
            PrimaryReleaseDateAsc,
            PrimaryReleaseDateDesc,
            OriginalTitleAsc,
            OriginalTitleDesc,
            VoteAverageAsc,
            VoteAverageDesc,
            VoteCountAsc,
            VoteCountDesc
        }

        public static string ToQueryString(this SortBy sortBy) => sortBy switch
        {
            SortBy.PopularityAsc => "popularity.asc",
            SortBy.PopularityDesc => "popularity.desc",
            SortBy.ReleaseDateAsc => "release_date.asc",
            SortBy.ReleaseDateDesc => "release_date.desc",
            SortBy.RevenueAsc => "revenue.asc",
            SortBy.RevenueDesc => "revenue.desc",
            SortBy.PrimaryReleaseDateAsc => "primary_release_date.asc",
            SortBy.PrimaryReleaseDateDesc => "primary_release_date.desc",
            SortBy.OriginalTitleAsc => "original_title.asc",
            SortBy.OriginalTitleDesc => "original_title.desc",
            SortBy.VoteAverageAsc => "vote_average.asc",
            SortBy.VoteAverageDesc => "vote_average.desc",
            SortBy.VoteCountAsc => "vote_count.asc",
            SortBy.VoteCountDesc => "vote_count.desc",

            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null)
        };
    }

}
