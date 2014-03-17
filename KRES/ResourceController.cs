using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KRES.Data;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class ResourceController : MonoBehaviour
    {
        #region Instance
        public static ResourceController Instance { get; private set; }
        #endregion

        #region Properties
        private List<ResourceBody> resourceBodies = new List<ResourceBody>();
        /// <summary>
        /// Gets and sets the global list of resource bodies.
        /// </summary>
        public List<ResourceBody> ResourceBodies
        {
            get { return this.resourceBodies; }
            set { this.resourceBodies = value; }
        }

        public bool DataSet
        {
            get
            {
                if (DataManager.Current == null) { return false; }
                return DataManager.Current.data.Count > 0;
            }
        }
        #endregion

        #region Initialisation
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Shows all resource map textures for the specified resource type.
        /// </summary>
        public void ShowResource(string resourceName)
        {
            if (ResourceLoader.Loaded)
            {
                DebugWindow.Instance.Print("Showing: " + resourceName);
                GetCurrentBody().ResourceItems.Find(i => i.HasMap && i.Name == resourceName).Map.ShowTexture(GetCurrentBody().Name);
            }
        }

        /// <summary>
        /// Hides all resource map textures for the specified resource type.
        /// </summary>
        public void HideResource(string resourceName)
        {
            if (ResourceLoader.Loaded)
            {
                DebugWindow.Instance.Print("Hiding: " + resourceName);
                GetCurrentBody().ResourceItems.Find(i => i.HasMap && i.Name == resourceName).Map.HideTexture(GetCurrentBody().Name);
            }
        }

        /// <summary>
        /// Hides all resource map textures.
        /// </summary>
        public void HideAllResources()
        {
            if (ResourceLoader.Loaded)
            {
                DebugWindow.Instance.Print("Hiding: All Resources");
                foreach (ResourceBody body in this.resourceBodies)
                {
                    foreach (ResourceItem item in body.ResourceItems.Where(i => i.HasMap))
                    {
                        item.Map.HideTexture(body.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads all the resource maps.
        /// </summary>
        public void UnloadResources()
        {
            if (ResourceLoader.Loaded)
            {
                foreach (Transform transform in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    List<Material> materials = new List<Material>(transform.renderer.materials);
                    materials.RemoveAll(m => m.name.Contains("KRESResourceMap"));
                    transform.renderer.materials = materials.ToArray();
                }
            }

            this.resourceBodies.Clear();
        }

        /// <summary>
        /// Returns the ResourceBody of the given name
        /// </summary>
        /// <param name="name">Name of the body to find</param>
        public ResourceBody GetBody(string name)
        {
            return ResourceBodies.Find(b => b.Name == name);
        }

        /// <summary>
        /// Returns the ResourceBody associated to the current main body
        /// </summary>
        public ResourceBody GetCurrentBody()
        {
            return ResourceBodies.Find(b => b.Name == FlightGlobals.currentMainBody.bodyName);
        }

        public DataBody GetDataBody(ModuleKresScanner scanner)
        {
            if (DataSet) { return DataManager.Current.data.Find(d => d.Type == scanner.scannerType).GetBody(scanner.body.Name); }
            else
            {
                return new DataType(scanner.type).GetBody(scanner.body.Name);
            }
        }
        #endregion
    }
}
