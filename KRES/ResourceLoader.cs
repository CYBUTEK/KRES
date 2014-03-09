using UnityEngine;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ResourceLoader : MonoBehaviour
    {
        #region Propreties
        private static bool loaded = false;
        /// <summary>
        /// Gets the loaded state of the resource system.
        /// </summary>
        public static bool Loaded
        {
            get { return loaded; }
        }
        #endregion

        #region Initiation
        private void Awake()
        {
            if (!loaded && MapGenerator.generated)
            {
                Load();
            }
        }
        #endregion

        #region Static Methods
        public static void Load()
        {
            ConfigNode settings = ConfigNode.Load(System.IO.Path.Combine(KRESUtils.GetSavePath(), "KRESSettings.cfg")).GetNode("KRES");
            foreach (ConfigNode planet in settings.nodes)
            {
                if (KRESUtils.IsCelestialBody(planet.name))
                {
                    ResourceBody body = new ResourceBody(planet);
                    ResourceController.Instance.ResourceBodies.Add(body);
                }
            }
            loaded = true;
            DebugWindow.Instance.Print("- Loaded Resources -");
        }
        #endregion

        #region Unloading
        private void OnDestroy()
        {
            if (HighLogic.LoadedScene == GameScenes.MAINMENU && loaded)
            {
                ResourceController.Instance.UnloadResources();
                loaded = false;
                DebugWindow.Instance.Print("- Unloaded Resources -");
            }
        }
        #endregion
    }
}
