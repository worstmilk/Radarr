﻿using System.Collections.Generic;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;
using Marr.Data.QGen;
using NzbDrone.Core.Tv;

namespace NzbDrone.Core.Blacklisting
{
    public interface IBlacklistRepository : IBasicRepository<Blacklist>
    {
        List<Blacklist> BlacklistedByTitle(int seriesId, string sourceTitle);
        List<Blacklist> BlacklistedByTorrentInfoHash(int seriesId, string torrentInfoHash);
        List<Blacklist> BlacklistedByMovie(int seriesId);
    }

    public class BlacklistRepository : BasicRepository<Blacklist>, IBlacklistRepository
    {
        public BlacklistRepository(IMainDatabase database, IEventAggregator eventAggregator) :
            base(database, eventAggregator)
        {
        }

        public List<Blacklist> BlacklistedByTitle(int movieId, string sourceTitle)
        {
            return Query.Where(e => e.MovieId == movieId)
                        .AndWhere(e => e.SourceTitle.Contains(sourceTitle));
        }

        public List<Blacklist> BlacklistedByTorrentInfoHash(int movieId, string torrentInfoHash)
        {
            return Query.Where(e => e.MovieId == movieId)
                        .AndWhere(e => e.TorrentInfoHash.Contains(torrentInfoHash));
        }

        public List<Blacklist> BlacklistedBySeries(int seriesId)
        {
            return Query.Where(b => b.SeriesId == seriesId);
        }

        public List<Blacklist> BlacklistedByMovie(int movieId)
        {
            return Query.Where(b => b.MovieId == movieId);
        }

        protected override SortBuilder<Blacklist> GetPagedQuery(QueryBuilder<Blacklist> query, PagingSpec<Blacklist> pagingSpec)
        {
            var baseQuery = query.Join<Blacklist, Movie>(JoinType.Inner, h => h.Movie, (h, s) => h.MovieId == s.Id);

            return base.GetPagedQuery(baseQuery, pagingSpec);
        }
    }
}
