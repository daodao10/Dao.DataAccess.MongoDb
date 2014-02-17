using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Dao.DataAccess.MongoDb
{
    public static class MongoDbHelper
    {
        private static readonly Dictionary<string, DbSetting> cache = new Dictionary<string, DbSetting>();

        public static void Register(string connectionStringName)
        {
            DbSetting setting;
            if (!cache.TryGetValue(connectionStringName, out setting))
            {
                setting = new DbSetting();

                setting.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                int index = setting.ConnectionString.LastIndexOf('/');
                if (index < 0)
                {
                    throw new ArgumentException("wrong connection string setting of mongodb!");
                }
                setting.DbName = setting.ConnectionString.Substring(index + 1);

                cache[connectionStringName] = setting;
            }
        }

        internal static bool GetDbSetting(string connectionStringName, out DbSetting setting)
        {
            setting = null;
            if (string.IsNullOrEmpty(connectionStringName))
            {
                if (cache.Count > 0)
                {
                    setting = cache.First().Value;
                }
                else
                {
                    return false;
                }
            }
            else if (!cache.TryGetValue(connectionStringName, out setting))
            {
                return false;
            }

            return true;
        }

    }
}
