using System;
using System.IO;

namespace MonoTouch
{
    public class Configurations
    {
        private static readonly string BaseDir = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

        public static T Load<T>(string domain, string key) where T : new()
        {
            var path = Path.Combine(BaseDir, domain, key);
            if (!File.Exists(path))
                return new T();

            try
            {
                var data = File.ReadAllText(path);
                return RestSharp.SimpleJson.DeserializeObject<T>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to load configuration object: " + e.Message);
            }

            return new T();
        }

        public static void Save(string domain, string key, object obj)
        {
            var path = Path.Combine(BaseDir, domain);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                path = Path.Combine(path, key);
                var data = RestSharp.SimpleJson.SerializeObject(obj);
                File.WriteAllText(path, data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to save configuration object: " + e.Message);
            }
        }

        public static void Delete(string domain)
        {
            var path = Path.Combine(BaseDir, domain);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void Delete(string domain, string key)
        {
            var path = Path.Combine(BaseDir, domain, key);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}

