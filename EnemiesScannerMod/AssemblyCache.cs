using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EnemiesScannerMod
{
    internal static class AssemblyCache
    {
        public static Dictionary<Type, string> EnemyTypeToNameInvariant { get; }

        static AssemblyCache()
        {
            EnemyTypeToNameInvariant = Assembly.GetAssembly(typeof(EnemyAI))
                .DefinedTypes
                .Where(typeInfo => typeInfo.BaseType == typeof(EnemyAI))
                .ToDictionary(
                    typeInfo => typeInfo.AsType(),
                    typeInfo => typeInfo.Name
                        .Replace("AI", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace("Enemy", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .ToLowerInvariant());
        }
    }
}