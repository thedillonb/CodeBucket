using System;
using MonoTouch;
using CodeBucket.Core.Services;

namespace CodeBucket.Services
{
    public class DefaultValueService : IDefaultValueService
    {
        public T Get<T>(string key)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)Utilities.Defaults.IntForKey(key);
            if (typeof(T) == typeof(bool))
                return (T)(object)Utilities.Defaults.BoolForKey(key);
            throw new Exception("Key does not exist in Default database.");
        }

        public bool TryGet<T>(string key, out T value)
        {
            try
            {
                value = Get<T>(key);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }

//            var val = Utilities.Defaults.ValueForKey(new MonoTouch.Foundation.NSString(key));
//            if (val == null)
//            {
//                value = default(T);
//                return false;
//            }
//            value = Get<T>(key);
//            return true;
        }

        public void Set(string key, object value)
        {
            if (value == null)
                Utilities.Defaults.RemoveObject(key);
            else if (value is int)
                Utilities.Defaults.SetInt((int)value, key);
            else if (value is bool)
                Utilities.Defaults.SetBool((bool)value, key);
            Utilities.Defaults.Synchronize();
        }
    }
}
