using UnityEngine;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ResourceLoader : MonoBehaviour
    {
        private static bool loaded = false;
        /// <summary>
        /// Gets the loaded state of the resource system.
        /// </summary>
        public static bool Loaded
        {
            get { return loaded; }
        }

        private void Awake()
        {
            if (!loaded)
            {
                // TODO: Load from save or create using config settings.

                ResourceBody kerbin = new ResourceBody("Kerbin", new ResourceMap[] { new ResourceMap("LiquidFuel", Color.yellow, 0, 0, 0) });
                ResourceController.Instance.ResourceBodies.Add(kerbin);

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
