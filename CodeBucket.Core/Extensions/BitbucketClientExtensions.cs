using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using BitbucketSharp;
using BitbucketSharp.Models.V2;

public static class BitbucketClientExtensions
{
    public static IObservable<IEnumerable<T>> RetrieveItems<T>(this Client client, Func<Client, Task<Collection<T>>> operation)
    {
        return client.AllItems(operation).ToObservable();
    }
}

