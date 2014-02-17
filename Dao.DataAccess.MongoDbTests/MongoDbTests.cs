using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dao.DataAccess.MongoDbTests
{
    [TestClass()]
    public class MongoDbTests
    {
        UserRepository repository;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Initialization.Setup();
        }
        //ClassCleanupAttribute

        [TestInitialize]
        public void Setup()
        {
            repository = new UserRepository();
        }
        [TestCleanup]
        public void Cleanup()
        {
            repository.Dispose();
        }

        [TestMethod]
        public void CreateUser()
        {
            List<User> users = new List<User>();
            for (int i = 0; i < 10; i++)
            {
                users.Add(new User()
                {
                    Name = "dao-" + i.ToString()
                });
            }

            repository.Insert(users);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void CreateNullUser()
        {
            User user = null;
            repository.Insert(user);
        }

        //[TestMethod]
        //public void GetUser2()
        //{
        //    User user = repository.GetItem(x => x.Name.Equals("dao0"));

        //    Assert.IsNotNull(user);
        //}

        //[TestMethod]
        //public void GetUsers2()
        //{
        //    IList<User> users = repository.GetItems(x => x.Name.Contains("ao"));

        //    //var query = new Dictionary<string, object>();
        //    //query.Add("Name", "dao0");
        //    //IList<User> users = repository.ExecuteQuery(query);

        //    Assert.IsTrue(users.Count >= 15);
        //}

        [TestMethod]
        public void GetUserById()
        {
            User user = repository.GetById("5301b9b364ea7f44b0fa8b0f");
            //User user = repository.GetItem(x => x.Name.Equals("dao-0"));

            Assert.IsTrue(user != null && user.Name.Equals("dao-0"));
        }

        [TestMethod]
        public void GetUser()
        {
            User user = repository.GetUserByName("dao-0");

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void GetUsers()
        {
            IList<User> users = repository.GetUsersByName("dao");

            Assert.IsTrue(users.Count >= 10);
        }

        [TestMethod]
        public void GetPagedUsers()
        {
            IList<User> users = repository.GetPagedUsers(0, 10);

            Assert.IsTrue(users.Count == 10 && users[0].Name == "dao-0");
        }

        [TestMethod]
        public void UpdateUser()
        {
            User user = new User()
            {
                Name = "dao-updated"
            };
            Assert.IsTrue(repository.Update(user) == 0);

            user.Id = "5301b9b364ea7f44b0fa8b0f";
            Assert.IsTrue(repository.Update(user) > 0);
        }

        [TestMethod]
        public void RemoveUsers()
        {
            bool success = repository.RemoveAll();
            Assert.IsTrue(success);
        }

    }
}
