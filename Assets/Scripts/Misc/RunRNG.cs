using UnityEngine;

public static class RunRNG
{
    private static Unity.Mathematics.Random gameRng;
    private static bool isInitialized = false;

    public static void InitializeSeed(int seed)
    {
        if (seed == 0) seed = 1;

        UnityEngine.Random.InitState(seed);

        uint useed = (uint)Mathf.Abs(seed);
        gameRng = new Unity.Mathematics.Random(useed);
        isInitialized = true;
        Debug.Log($"[RunRNG] Initialized Gameplay RNG with seed: {useed}");
    }

    public static int Range(int minInclusive, int maxExclusive)
    {
        if (!isInitialized) InitializeSeed(UnityEngine.Random.Range(1, int.MaxValue));
        return gameRng.NextInt(minInclusive, maxExclusive);
    }

    public static float Range(float minInclusive, float maxInclusive)
    {
        if (!isInitialized) InitializeSeed(UnityEngine.Random.Range(1, int.MaxValue));
        return gameRng.NextFloat(minInclusive, maxInclusive);
    }

    public static float Value()
    {
        if (!isInitialized) InitializeSeed(UnityEngine.Random.Range(1, int.MaxValue));
        return gameRng.NextFloat(0f, 1f);
    }
}