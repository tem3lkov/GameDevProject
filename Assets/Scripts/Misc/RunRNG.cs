using UnityEngine;

public static class RunRNG
{
    private static Unity.Mathematics.Random gameRng;
    private static bool isInitialized = false;

    public static void InitializeSeed(uint seed)
    {
        if (seed == 0) seed = 1;

        gameRng = new Unity.Mathematics.Random(seed);
        isInitialized = true;
        Debug.Log($"[RunRNG] Initialized Gameplay RNG with seed: {seed}");
    }

    public static int Range(int minInclusive, int maxExclusive)
    {
        if (!isInitialized) InitializeSeed((uint)UnityEngine.Random.Range(1, int.MaxValue));
        return gameRng.NextInt(minInclusive, maxExclusive);
    }

    public static float Range(float minInclusive, float maxInclusive)
    {
        if (!isInitialized) InitializeSeed((uint)UnityEngine.Random.Range(1, int.MaxValue));
        return gameRng.NextFloat(minInclusive, maxInclusive);
    }

    public static float Value()
    {
        if (!isInitialized) InitializeSeed((uint)UnityEngine.Random.Range(1, int.MaxValue));
        return gameRng.NextFloat(0f, 1f);
    }
}