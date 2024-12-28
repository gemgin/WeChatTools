using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace WeChatTools.Core
{

    //监测专用
    public class RedisCacheToolsYUN1
    {
        private static string strErrorInfo = "{0}:{1}发生异常!key={2},异常信息={3}";
        private static readonly PooledRedisClientManager pool = null;
        private static readonly string[] redisHosts = null;

        #region 配置
        public static int RedisMaxReadPool = int.Parse(ConfigurationManager.AppSettings["redis_max_read_pool"]);
        public static int RedisMaxWritePool = int.Parse(ConfigurationManager.AppSettings["redis_max_write_pool"]);
        public static long RedisDefaultDb = long.Parse(ConfigurationManager.AppSettings["redis_default_db_YUN_1"]);
        static RedisCacheToolsYUN1()
        {
            var redisHostStr = ConfigurationManager.AppSettings["redis_server_session"];

            if (!string.IsNullOrEmpty(redisHostStr))
            {
                redisHosts = redisHostStr.Split(',');

                if (redisHosts.Length > 0)
                {
                    RedisConfig.VerifyMasterConnections = false;//阿里云的 redis 服务目前不支持 redis 的 role 命令。
                    pool = new PooledRedisClientManager(redisHosts, redisHosts,
                        new RedisClientManagerConfig()
                        {
                            MaxWritePoolSize = RedisMaxWritePool,
                            MaxReadPoolSize = RedisMaxReadPool,
                            DefaultDb =  RedisDefaultDb,
                            AutoStart = true
                        });
                }
            }
        }
        #endregion

        #region 添加
        public static void Add<T>(string key, T value)
        {
            if (value == null)
            {
                return;
            }
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Set(key, value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "存储", key, ex.Message);
                LogTools.WriteLine("Add Key-->" + msg);
            }

        }

        public static void Add<T>(string key, T value, DateTime expiry)
        {
            if (value == null)
            {
                return;
            }

            if (expiry <= DateTime.Now)
            {
                Remove(key);

                return;
            }

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Set(key, value, expiry - DateTime.Now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "存储", key, ex.Message);
                LogTools.WriteLine("Add Key DateTime-->" + msg);
            }

        }

        public static void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (value == null)
            {
                return;
            }

            if (slidingExpiration.TotalSeconds <= 0)
            {
                Remove(key);

                return;
            }

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Set(key, value, slidingExpiration);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "存储", key, ex.Message);
                LogTools.WriteLine("Add Key TimeSpan-->" + msg);
            }

        }
        #endregion
        #region 计数
        public static void Incr(string key)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.IncrementValue(key);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "存储", key, ex.Message);
                LogTools.WriteLine("Add Key DateTime-->" + msg);
            }

        }
        #endregion

        #region 计算设置过期时间
        public static void Expire(string key, DateTime expiry)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.ExpireEntryAt(key, expiry);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "存储", key, ex.Message);
                LogTools.WriteLine("Add Key DateTime-->" + msg);
            }

        }
        #endregion
        #region 获取
        public static IDictionary<string, T> Get<T>(out string errorMsg)
        {
            IDictionary<string, T> obj = new Dictionary<string, T>();
            errorMsg = null;
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            List<string> allKeys = r.SearchKeys("*");
                            if (allKeys.Count > 0)
                            {
                                obj = r.GetAll<T>(allKeys);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "获取", "GetAll", ex.Message);
                LogTools.WriteLine("Get All-->" + msg);
                errorMsg = ex.Message;
            }

            return obj;
        }
        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            T obj = default(T);

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            obj = r.Get<T>(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "获取", key, ex.Message);
                LogTools.WriteLine("Get Key-->" + msg);
            }

            return obj;
        }

        public static string GetRandomKey()
        {
            string obj = "";
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            obj = r.GetRandomKey();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "获取", obj, ex.Message);
                LogTools.WriteLine("Get RandomKey-->" + msg);
            }

            return obj;
        }
        #endregion

        #region 获取key存活时间
        public static TimeSpan? GetKeyTimeToLive(string key)
        {
            TimeSpan? obj = default(TimeSpan?);
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            obj = r.GetTimeToLive(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "获取", key, ex.Message);
                LogTools.WriteLine("Get Key-->" + msg);
            }

            return obj;
        }
        #endregion
        #region 移除
        public static void Remove(string key)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Remove(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "删除", key, ex.Message);
                LogTools.WriteLine("Remove Key-->" + msg);
            }

        }
        #endregion

        #region 判断是否存在
        public static bool Exists(string key)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            return r.ContainsKey(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(strErrorInfo, "cache", "是否存在", key, ex.Message);
                LogTools.WriteLine("Exists Key-->" + msg);
            }

            return false;
        }
        #endregion
    }
}