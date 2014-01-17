using System;
using CodeFramework.Core.Services;

namespace CodeBucket.Core
{
	public class JsonSerializationService : IJsonSerializationService
    {
		private readonly BitbucketSharp.SimpleJsonSerializer _serializer = new BitbucketSharp.SimpleJsonSerializer();

		public string Serialize(object o)
		{
			return _serializer.Serialize(o);
		}

		public TData Deserialize<TData>(string data)
		{
			return _serializer.Deserialize<TData>(data);
		}
    }
}

