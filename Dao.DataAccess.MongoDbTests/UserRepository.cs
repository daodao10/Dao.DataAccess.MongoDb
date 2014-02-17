using System.Collections.Generic;
using Dao.DataAccess.MongoDb;
using MongoDB.Driver;
using DbQuery = MongoDB.Driver.Builders.Query;

namespace Dao.DataAccess.MongoDbTests
{
    class UserRepository : MongoDbRepository<User>
    {
        public User GetUserByName(string name)
        {
            IMongoQuery query = DbQuery.EQ("Name", name);
            return this.GetItem(query);
        }

        public List<User> GetUsersByName(string name)
        {
            IMongoQuery query = DbQuery.Matches("Name", name);
            return this.GetItems(query);
        }

        public List<User> GetPagedUsers(int startIndex, int pageSize)
        {
            IMongoQuery query = null;
            SortByDocument sortBy = new SortByDocument(new MongoDB.Bson.BsonElement("Name", 1));//ascending
            //SortByDocument sortBy = new SortByDocument() { { "Name", -1 } };//desceding
            //SortByDocument sortBy = null;

            return this.GetItems(query, startIndex, pageSize, sortBy);
        }

        public bool RemoveAll()
        {
            IMongoQuery query = null;
            this.Delete(query);

            return this.GetCounter(query) == 0;
        }

    }
}
