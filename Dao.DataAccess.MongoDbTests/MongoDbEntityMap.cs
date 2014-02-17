using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dao.DataAccess.MongoDbTests
{
    /// <summary>
    /// Bson entity contract for MongoDB Bson Serializer
    /// refer to http://docs.mongodb.org/ecosystem/tutorial/serialize-documents-with-the-csharp-driver/#serialize-documents-with-the-csharp-driver
    /// </summary>
    internal static class MongoDbEntityMap
    {
        public static void Init()
        {
            // set global IdGenerator
            //BsonSerializer.RegisterIdGenerator(typeof(Guid), CombGuidGenerator.Instance);

            BsonClassMap.RegisterClassMap<User>(x =>
            {
                x.AutoMap();
                x.SetIdMember(x.GetMemberMap(m => m.Id));
                x.IdMemberMap.SetRepresentation(BsonType.ObjectId);
                x.GetMemberMap(m => m.Id).SetIgnoreIfNull(true);
            });

            // set other type ...

        }
    }
}
