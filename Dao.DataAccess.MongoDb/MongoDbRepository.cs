using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using DbQuery = MongoDB.Driver.Builders.Query;
using DbUpdate = MongoDB.Driver.Builders.Update;

namespace Dao.DataAccess.MongoDb
{
    public class MongoDbRepository<T> : IDisposable where T : class
    {
        const string ObjectIdKey = "_id";

        internal MongoDbContext<T> DbContext { get; private set; }

        public MongoDbRepository(string connectionStringName = "")
        {
            this.DbContext = new MongoDbContext<T>(connectionStringName);
        }

        #region Create

        public virtual void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.DbContext.Collection.Insert<T>(entity);
        }

        public virtual void Insert(IList<T> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                throw new ArgumentNullException("entities");
            }

            List<BsonDocument> list = new List<BsonDocument>();
            foreach (var item in entities)
            {
                list.Add(item.ToBsonDocument());
            }
            this.DbContext.Collection.InsertBatch(list, WriteConcern.Acknowledged);
        }

        #endregion

        #region Retrieve

        public virtual T GetById(string id)
        {
            var query = DbQuery.EQ(ObjectIdKey, new ObjectId(id));

            return this.GetItem(query);
        }

        [Obsolete("Please try to use the MongoDB primitive version")]
        public virtual T GetItem(Func<T, bool> predicate)
        {
            var result = this.DbContext.Collection.AsQueryable<T>().Where(predicate);

            return result.FirstOrDefault();
        }

        [Obsolete("Please try to use the MongoDB primitive version")]
        public virtual List<T> GetItems(Func<T, bool> predicate)
        {
            return this.DbContext.Collection.AsQueryable<T>()
                .Where(predicate).ToList();
        }

        public virtual T GetItem(IMongoQuery query)
        {
            return this.DbContext.Collection.FindOne(query);
        }

        public virtual List<T> GetItems(IMongoQuery query)
        {
            MongoCursor<T> cursor = this.DbContext.Collection.Find(query);

            return cursor.ToList<T>();
        }

        /// <summary>
        /// Get paged items
        /// </summary>
        /// <param name="query"></param>
        /// <param name="startIndex">start from 0</param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public virtual List<T> GetItems(IMongoQuery query, int startIndex, int pageSize, SortByDocument sortBy)
        {
            if (startIndex <= 0)
            {
                startIndex = 0;
            }

            sortBy = InitSortBy(sortBy);

            MongoCursor<T> cursor = this.DbContext.Collection.Find(query)
                .SetSortOrder(sortBy)
                .SetSkip(startIndex).SetLimit(pageSize);

            return cursor.ToList();
        }

        public virtual long GetCounter(IMongoQuery query)
        {
            return this.DbContext.Collection.Count(query);
        }

        #endregion

        #region Update

        public virtual long Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                BsonDocument bson = entity.ToBsonDocument();
                ObjectId id = bson[ObjectIdKey].AsObjectId;

                if (id != null)
                {
                    var query = DbQuery.EQ(ObjectIdKey, id);
                    var update = DbUpdate.Replace<T>(entity);
                    return this.Update(query, update);
                }
            }
            catch (KeyNotFoundException) { }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="update"></param>
        /// <param name="muti">true -- update multiple records, false -- update the first record</param>
        /// <returns></returns>
        public virtual long Update(IMongoQuery query, IMongoUpdate update, bool muti = false)
        {
            UpdateFlags flags = new UpdateFlags();
            if (muti)
            {
                flags = UpdateFlags.Multi;
            }
            else
            {
                flags = UpdateFlags.None;
            }

            var result = this.DbContext.Collection.Update(query, update, flags, WriteConcern.Acknowledged);
            return result.DocumentsAffected;
        }

        #endregion

        #region Delete

        public virtual long Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return 0;
            }

            var query = DbQuery.EQ(ObjectIdKey, new ObjectId(id));
            return this.Delete(query);
        }

        public virtual long Delete(IMongoQuery query)
        {
            return this.DbContext.Collection.Remove(query).DocumentsAffected;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Check & set default sorting
        /// </summary>
        /// <param name="sortBy">-1 -- decsending, 1 -- ascending</param>
        /// <returns></returns>
        private static SortByDocument InitSortBy(SortByDocument sortBy)
        {
            if (sortBy == null)
            {
                sortBy = new SortByDocument(ObjectIdKey, 1);
            }

            return sortBy;
        }

        #endregion

        public void Dispose()
        {
            if (this.DbContext != null)
            {
                this.DbContext.Dispose();
                this.DbContext = null;
            }
        }
    }
}
