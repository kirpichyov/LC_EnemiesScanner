using System;
using System.Collections.Concurrent;

namespace EnemiesScannerMod
{
    public static class DynamicCache
    {
        public static ConcurrentDictionary<Type, string> EnemyTypeToNormalizedNames { get; } =
            new ConcurrentDictionary<Type, string>();
    }
}