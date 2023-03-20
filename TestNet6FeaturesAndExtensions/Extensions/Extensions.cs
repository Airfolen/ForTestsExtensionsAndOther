using System.Collections.Concurrent;

namespace ForTests.Extensions;

public static class Extensions
{
    public static Task ParallelForEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> body,
        int degreeOfParallelization = 100)
    {
        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await body(partition.Current);
                }
            }
        }

        return Task.WhenAll(
            Partitioner
                .Create(source)
                .GetPartitions(degreeOfParallelization)
                .AsParallel()
                .Select(AwaitPartition));
    }
}