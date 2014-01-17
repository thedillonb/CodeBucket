using System;
using CodeFramework.Core.Services;

namespace CodeFramework.iOS
{
	public class JsonSerializationService : IJsonSerializationService
    {
		public string Serialize(object o)
		{
			return BitbucketSharp;
		}

		public TData Deserialize<TData>(string data)
		{
			throw new NotImplementedException();
		}
    }
}

