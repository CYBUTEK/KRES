using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using KRES.Defaults;
using KRES.Extensions;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class MapGenerator : MonoBehaviour
    {
        #region Fields
        private string texturePath = Path.Combine(KRESUtils.GetSavePath(), "KRESTextures");
        private string currentBody = string.Empty;
        private string currentResource = string.Empty;
        private System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        private double amountComplete = 0f, max = 0f;
        private ConfigNode settings = new ConfigNode("settings"), cfg = new ConfigNode("KRES");
        private int ID = Guid.NewGuid().GetHashCode();
        private bool visible = false;
        private Rect window = new Rect();
        private GUISkin skins = HighLogic.Skin;
        private Texture2D background = new Texture2D(1, 1);
        private Texture2D bar = new Texture2D(1, 1);
        private Rect bgPos = new Rect();
        private Rect barPos = new Rect();
        private double time = 0d;
        private ProgressBar progressBar = new ProgressBar();
        private List<CelestialBody> relevantBodies = new List<CelestialBody>();
        #endregion

        #region Propreties
        private static bool generated = false;
        public static bool Generated
        {
            get { return generated; }
        }

        private static string defaultName = string.Empty;
        public static string DefaultName
        {
            get { return defaultName; }
        }
        #endregion

        #region Methods
        private IEnumerator<YieldInstruction> GenerateAllMaps()
        {
            double doing = -1d;
            foreach (ConfigNode body in cfg.GetNode("ore").nodes)
            {
                CelestialBody planet = null;
                if (KRESUtils.TryParseCelestialBody(body.name, out planet) && relevantBodies.Contains(planet))
                {
                    currentResource = string.Empty;
                    doing++;
                    currentBody = planet.name;
                    DefaultBody defaultBody = DefaultLibrary.GetSelectedDefault().GetBody(planet.bodyName);
                    if (!Directory.Exists(Path.Combine(texturePath, body.name)))
                    {
                        Debug.LogWarning("[KRES]: Created texture folder for " + body.name);
                        Directory.CreateDirectory(Path.Combine(texturePath, body.name));
                    }
                    string path = Path.Combine(texturePath, body.name);
                    double current = -1d;
                    double res = (1d / body.GetNodes("KRES_DATA").Count());
                    foreach (ConfigNode resource in body.GetNodes("KRES_DATA"))
                    {
                        current++;
                        amountComplete = (doing + (current * res)) / max;
                        string resourceName = string.Empty;
                        if (!resource.TryGetValue("name", ref resourceName))
                        {
                            Debug.LogWarning("[KRES]: Nameless resource for " + body.name);
                            continue;
                        }
                        currentResource = resourceName;
                        yield return null;
                        DefaultResource defaultResource = defaultBody.GetResourceOfType(resourceName, "ore");
                        Color colour = ResourceInfoLibrary.Instance.GetResource(resourceName).Colour;

                        if (colour.a == 0)
                        {
                            Debug.LogWarning("[KRES]: Invalid colour for node " + resourceName + " for " + body.name);
                            continue;
                        }
                        if (defaultResource.Density == 0 || defaultResource.Octaves == 0 || defaultResource.Persistence == 0 || defaultResource.Frequency == 0)
                        {
                            Debug.LogWarning("[KRES]: Invalid values for node " + resourceName + "for " + body.name);
                            continue;
                        }

                        //Map generation
                        Simplex simplex = new Simplex(defaultResource.Seed, defaultResource.Octaves, defaultResource.Persistence, defaultResource.Frequency);
                        Texture2D map = new Texture2D(1440, 720, TextureFormat.ARGB32, false);

                        var timer = System.Diagnostics.Stopwatch.StartNew();
                        for (int x = 0; x < 1440; x++)
                        {
                            double lon = 90d - (x / 4d);
                            for (int y = 0; y < 720; y++)
                            {
                                double lat = (y / 4d) - 90d;

                                //Takes too much time to process ~50s per texture, will have to do on scanning.
                                /*
                                if (!double.IsInfinity(defaultResource.MinAltitude) || !double.IsInfinity(defaultResource.MaxAltitude))
                                {
                                    double alt = planet.TerrainAltitude(lat, lon);
                                    if (alt > defaultResource.MaxAltitude || alt < defaultResource.MinAltitude)
                                    {
                                        map.SetPixel(x, y, KRESUtils.BlankColour);
                                        continue;
                                    }
                                }
                                */

                                if (defaultResource.Biomes.Length > 0 || defaultResource.ExcludedBiomes.Length > 0)
                                {
                                    string biome = planet.GetBiome(lat, lon);
                                    if ((defaultResource.Biomes.Length > 0 && !defaultResource.Biomes.Contains(biome)) || (defaultResource.ExcludedBiomes.Length > 0 && defaultResource.ExcludedBiomes.Contains(biome)))
                                    {
                                        map.SetPixel(x, y, KRESUtils.BlankColour);
                                        continue;
                                    }
                                }

                                Vector3d position = KRESUtils.SphericalToCartesian(10d, x / 4d, y / 4d);
                                double density = simplex.noiseNormalized(position) - 1d + defaultResource.Density;
                                if (density > 0)
                                {
                                    float a = Mathf.Lerp(0.2f, 1f, (float)(density / defaultResource.Density)) * colour.a;
                                    if (a > 0.8f) { a = 0.8f; }
                                    map.SetPixel(x, y, new Color(colour.r, colour.g, colour.b, a));
                                }
                                else { map.SetPixel(x, y, KRESUtils.BlankColour); }
                            }
                            if (x % 360 == 0) { yield return null; }
                        }
                        map.Apply();

                        //Save texture
                        File.WriteAllBytes(Path.Combine(path, resourceName + ".png"), map.EncodeToPNG());
                        Destroy(map);
                        timer.Stop();
                        print(String.Format("[KRES]: Texture for {0} on {1} created in {2}ms", resourceName, currentBody, timer.ElapsedMilliseconds));
                    }
                }
            }
            amountComplete = 1d;
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            if (!Generated)
            {
                ConfigNode test = null;

                if (!File.Exists(KRESUtils.DataURL)) { Debug.LogWarning("[KRES]: The KRESData.cfg file is missing"); }
                else
                {
                    test = ConfigNode.Load(KRESUtils.DataURL);
                    if (!DefaultLibrary.HasComponents(test))
                    {
                        Debug.LogWarning("[KRES]: The KRESData.cfg file is missing components");
                        test = null;
                    }
                }

                if (test == null)
                {
                    test = new ConfigNode("test");
                    print("[KRES]: Creating new KRESSettings.cfg");
                    DefaultLibrary.SaveSelectedDefault(test, KRESUtils.DataURL);
                    settings = ConfigNode.Load(KRESUtils.DataURL);
                    cfg = settings.GetNode("KRES");
                    print("[KRES]: Successfully created and saved new KRESData.cfg");
                    cfg.TryGetValue("name", ref defaultName);
                    cfg.TryGetValue("generated", ref generated);
                }
                else
                {
                    print("[KRES]: Successfully loaded KRESData.cfg");
                    settings = test;
                    cfg = settings.GetNode("KRES");
                    cfg.TryGetValue("name", ref defaultName);
                    cfg.TryGetValue("generated", ref generated);
                }

                if (!Directory.Exists(texturePath))
                {
                    Debug.LogWarning("[KRES]: KRESTextures directory does not exist in the save file");
                    Directory.CreateDirectory(texturePath);
                    generated = false;
                    print("[KRES]: Successfully created texture directory");
                }

                if (!Generated)
                {
                    if (defaultName != DefaultLibrary.GetSelectedDefault().Name)
                    {
                        print("[KRES]: Generating from new defaults: " + DefaultLibrary.GetSelectedDefault().Name);
                        settings.ClearNodes();
                        DefaultLibrary.SaveSelectedDefault(settings, KRESUtils.DataURL);
                        settings = ConfigNode.Load(KRESUtils.DataURL);
                        cfg = settings.GetNode("KRES");
                        print("[KRES]: Generated new files from new defaults");
                    }

                    print("[KRES]: Generating resource maps");
                    timer.Start();

                    relevantBodies = new List<CelestialBody>(KRESUtils.GetRelevantBodies("ore").Where(b => cfg.GetNode("ore").GetNode(b.bodyName).nodes.Count > 0));
                    max = relevantBodies.Count;
                    StartCoroutine(GenerateAllMaps());
                    bgPos = new Rect(0, 0, 374, 19);
                    barPos = new Rect(2, 2, 370, 15);
                    background.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetDLLPath(), "GUI/ProgressBar/progressBackground.png")));
                    bar.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetDLLPath(), "GUI/ProgressBar/progressBar.png")));
                    amountComplete = 0d;
                    progressBar = new ProgressBar(bgPos, barPos, background, bar);
                    progressBar.SetValue(0d);
                    this.window = new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 60, 400, 120);
                    this.visible = true;
                }
                else if (!ResourceLoader.Loaded)
                {
                    timer.Start();
                    bgPos = new Rect(0, 0, 374, 19);
                    barPos = new Rect(2, 2, 370, 15);
                    background.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetDLLPath(), "GUI/ProgressBar/progressBackground.png")));
                    bar.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetDLLPath(), "GUI/ProgressBar/progressBar.png")));
                    amountComplete = 0d;
                    progressBar = new ProgressBar(bgPos, barPos, background, bar);
                    progressBar.SetValue(0d);
                    this.window = new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 60, 400, 120);
                    this.visible = true;
                    amountComplete = 1d;
                }
            }
        }
        #endregion

        #region Update
        private void Update()
        {
            if (amountComplete == 1d && !generated)
            {
                print("[KRES]: Map generation complete");
                print(String.Format("[KRES]: Map generation took a total of {0:0.000}s", timer.Elapsed.TotalSeconds));
                generated = true;
                cfg.SetValue("generated", bool.TrueString);
                settings.ClearNodes();
                settings.AddNode(cfg);
                settings.Save(KRESUtils.DataURL);
                ResourceLoader.Instance.Load();
            }
        }
        #endregion

        #region GUI
        private void OnGUI()
        {
            if (this.visible)
            {
                this.window = GUI.Window(this.ID, this.window, Window, "KRES Resource Loader", skins.window);
            }
        }

        private void Window(int id)
        {
            GUI.BeginGroup(new Rect(10, 10, 380, 120));
            if (amountComplete == 1d && ResourceLoader.Loaded) { GUI.Label(new Rect(0, 20, 380, 15), "Complete", skins.label); }
            else if (amountComplete != 1d) { GUI.Label(new Rect(0, 20, 380, 15), String.Concat("Currently generating: ", currentBody, " - ", currentResource), skins.label); }
            else if (!ResourceLoader.Loaded) { GUI.Label(new Rect(0, 20, 380, 15), "Loading resources", skins.label); }
            GUI.BeginGroup(new Rect(5, 50, 380, 30));
            if (amountComplete != 1d || amountComplete == 1d && ResourceLoader.Loaded) { progressBar.SetValue(amountComplete); }
            else if (amountComplete == 1d && !ResourceLoader.Loaded) { progressBar.SetValue(ResourceLoader.Instance.LoadPercent); }
            progressBar.Draw();
            GUI.EndGroup();
            if (amountComplete == 1d && ResourceLoader.Loaded)
            {
                if (GUI.Button(new Rect(155, 80, 80, 25), "Close", skins.button))
                {
                    this.visible = false;
                }
                if (timer.IsRunning)
                {
                    timer.Stop();
                    time = timer.Elapsed.TotalSeconds;
                }
                GUI.Label(new Rect(240, 80, 140, 15), String.Format("Elapsed time: {0:0.000}s", time), skins.label);
            }
            else
            {
                if (amountComplete == 1d)
                {
                    GUI.Label(new Rect(0, 80, 380, 15), "Please wait for resources to load. Do not leave the scene.", skins.label);
                }
                else { GUI.Label(new Rect(0, 80, 380, 15), "Please wait for map generation to finish. Do not leave the scene.", skins.label); }
            }
            GUI.EndGroup();
        }
        #endregion

        #region Unloading
        private void OnDestroy()
        {
            if (HighLogic.LoadedScene == GameScenes.MAINMENU && Generated)
            {
                generated = false;
            }
        }
        #endregion
    }
}
