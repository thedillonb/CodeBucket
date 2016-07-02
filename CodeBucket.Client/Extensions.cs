using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBucket.Client;
using CodeBucket.Client.Models;

public static class Extensions
{
    public static async Task<IEnumerable<T>> AllItems<T>(this BitbucketClient client, Func<BitbucketClient, Task<Collection<T>>> operation)
    {
        var ret = await operation(client);
        var items = new List<T>(ret.Values);
        var next = ret.Next;

        while (!string.IsNullOrEmpty(next))
        {
            var t = await client.Get<Collection<T>>(next);
            items.AddRange(t.Values);
            next = t.Next;
        }

        return items;
    }

    public static async Task ForAllItems<T>(this BitbucketClient client, Func<BitbucketClient, Task<Collection<T>>> operation, Action<IEnumerable<T>> addAction)
    {
        var ret = await operation(client);
        addAction(ret.Values);
        var next = ret.Next;

        while (!string.IsNullOrEmpty(next))
        {
            var t = await client.Get<Collection<T>>(next);
            addAction(t.Values);
            next = t.Next;
        }
    }
}

