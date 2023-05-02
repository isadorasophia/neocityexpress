using Murder;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LDGame.Services;

public static class ImmutableArrayExtensions
{

    public static ImmutableArray<Guid> Shuffle(this ImmutableArray<Guid> array)
    {
        // Copy the elements to a mutable list
        List<Guid> list = new List<Guid>(array);

        // Shuffle the list using Fisher-Yates algorithm
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Game.Random.Next(i + 1);
            Guid temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        // Create a new ImmutableArray<Guid> from the shuffled list
        return ImmutableArray.CreateRange(list);
    }
}
