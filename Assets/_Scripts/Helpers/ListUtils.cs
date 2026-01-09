using System.Collections.Generic;
using UnityEngine;

public static class ListUtils
{
    public static List<T> GetShuffled<T>(IList<T> list)
    {
        List<T> shuffled = new List<T>(list);
        int n = shuffled.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        return shuffled;
    }
}