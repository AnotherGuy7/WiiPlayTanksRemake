﻿using System;
using System.Collections.Generic;

namespace TanksRebirth.Internals.Common.Utilities;

public static class RandomUtils
{
    public static Random DefaultSeed = new();
    public static float NextFloat(this Random random, float min, float max)
    => (float)(random.NextDouble() * (max - min) + min);
    public static double NextFloat(this Random random, double min, double max)
        => random.NextDouble() * (max - min) + min;
    public static short Next(this Random random, short min, short max)
        => (short)random.Next(min, max);
    public static byte Next(this Random random, byte min, byte max)
        => (byte)random.Next(min, max);
    public static T PickRandom<T>(T[] input) => input[DefaultSeed.Next(0, input.Length)];
    public static List<T> PickRandom<T>(T[] input, int amount)
    {
        List<T> values = new();
        List<int> chosenTs = new();
        for (int i = 0; i < amount; i++)
        {
        ReRoll:
            int rand = new Random().Next(0, input.Length);

            if (!chosenTs.Contains(rand))
            {
                chosenTs.Add(rand);
                values.Add(input[rand]);
            }
            else
                goto ReRoll;
        }
        chosenTs.Clear();
        return values;
    }
    public static TEnum PickRandom<TEnum>() where TEnum : struct, Enum => (TEnum)(object)DefaultSeed.Next(0, Enum.GetNames<TEnum>().Length);
}
