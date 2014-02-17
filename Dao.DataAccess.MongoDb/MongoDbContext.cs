using System;
using MongoDB.Driver;

namespace Dao.DataAccess.MongoDb
{
    internal class MongoDbContext<T> : IDisposable
    {
        public MongoCollection<T> Collection { get; private set; }

        /// <summary>
        /// Just for advanced DB operations or cross-collection access
        /// </summary>
        public MongoDatabase Db { get; private set; }

        public MongoDbContext(string connectionStringName = "")
        {
            DbSetting setting;
            if (MongoDbHelper.GetDbSetting(connectionStringName, out setting))
            {
                var client = new MongoClient(setting.ConnectionString);
                var server = client.GetServer();
                this.Db = server.GetDatabase(setting.DbName);
                this.Collection = this.Db.GetCollection<T>(typeof(T).Name);

                return;
            }

            throw new ArgumentException("ConnectionString is invalid");
        }

        public void Dispose()
        {
            //todo: how to implement
            if (this.Db != null)
            {
                this.Db.Server.Disconnect();
            }
            this.Collection = null;
            this.Db = null;
        }
    }
}
