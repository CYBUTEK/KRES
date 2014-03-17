using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ResourceLoader : MonoBehaviour
    {
        #region Propreties
        private static ResourceLoader instance = null;
        /// <summary>
        /// Returns the current instance
        /// </summary>
        public static ResourceLoader Instance
        {
            get { return instance; }
        }

        private static bool loaded = false;
        /// <summary>
        /// Gets the loaded state of the resource system.
        /// </summary>
        public static bool Loaded
        {
            get { return loaded; }
        }

        private double loadPercent = 0d;
        /// <summary>
        /// How much of the resources are loaded if loading
        /// </summary>
        public double LoadPercent
        {
            get { return loadPercent; }
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                if (MapGenerator.Generated && !Loaded) { Load(); }
            }
            else { Destroy(this); }
        }
        #endregion

        #region Methods
        public void Load()
        {
            StartCoroutine(LoadBodies());
        }

        private IEnumerator<YieldInstruction> LoadBodies()
        {
            ConfigNode settings = ConfigNode.Load(KRESUtils.DataURL);
            double max = FlightGlobals.Bodies.Count;
            double current = -1d;
            System.Random random = new System.Random();
            foreach (CelestialBody planet in FlightGlobals.Bodies)
            {
                current++;
                loadPercent = current / max;
                ResourceBody body = new ResourceBody(planet.bodyName);
                var b = body.LoadItems(settings.GetNode("KRES"), random);
                while (b.MoveNext()) { yield return b.Current; }
                ResourceController.Instance.ResourceBodies.Add(body);
            }
            settings.Save(KRESUtils.DataURL);
            loadPercent = 1d;
            loaded = true;
            DebugWindow.Instance.Print("- Loaded Resources -");
        }
        #endregion

        #region Unloading
        private void OnDestroy()
        {
            if (HighLogic.LoadedScene == GameScenes.MAINMENU && loaded)
            {
                instance = null;
                ResourceController.Instance.UnloadResources();
                loaded = false;
                DebugWindow.Instance.Print("- Unloaded Resources -");
            }
        }
        #endregion
    }
}
