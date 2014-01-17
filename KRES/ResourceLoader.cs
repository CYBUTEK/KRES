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

        private void Awake()
        {
            if (!loaded)
            {
                // TODO: Load from save or create using config settings.

                foreach (CelestialBody planet in FlightGlobals.Bodies)
                {
                    if (planet.bodyName != "Sun")
                    {
                        ResourceBody body = new ResourceBody(planet.bodyName);
                        ResourceController.Instance.ResourceBodies.Add(body);
                    }
                }
                loaded = true;
                DebugWindow.Instance.Print("- Loaded Resources -");
            }
        }

        private void OnDestroy()
        {
            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                // TODO: Save to file associated with player's game save.

                ResourceController.Instance.UnloadResources();

                loaded = false;

                DebugWindow.Instance.Print("- Unloaded Resources -");
            }
        }
    }
}
