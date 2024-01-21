using BepInEx.Logging;

namespace EnemiesScannerMod
{
    internal static class ModLogger
    {
        public static ManualLogSource Instance { get; private set; }

        public static void SetInstance(ManualLogSource instance)
        {
            Instance = instance;
        }
    }
}