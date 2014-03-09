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
        private string settingsPath = Path.Combine(KRESUtils.GetSavePath(), "KRESSettings.cfg");
        private string texturePath = Path.Combine(KRESUtils.GetSavePath(), "KRESTextures");
        private string currentBody = string.Empty;
        private string currentResource = string.Empty;
        private System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        public static bool generated = false;
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
        double time = 0d;
        ProgressBar progressBar = new ProgressBar();
        #endregion

        #region Methods
        private IEnumerator<YieldInstruction> GenerateAllMaps()
        {
            double doing = -1d;
            foreach (ConfigNode body in cfg.nodes)
            {
                CelestialBody planet = null;
                if (KRESUtils.TryParseCelestialBody(body.name, out planet))
                {
                    doing++;
                    currentBody = planet.name;
                    if (!Directory.Exists(Path.Combine(texturePath, body.name)))
                    {
                        Debug.LogWarning("[KRES]: Texture folder for " + body.name + " is missing");
                        Directory.CreateDirectory(Path.Combine(texturePath, body.name));
                        print("[KRES]: Created texture folder for " + body.name);
                    }
                    string path = Path.Combine(texturePath, body.name);
                    double current = -1d;
                    double res = (1d / body.GetNodes("KRES_RESOURCE").Count());
                    foreach (ConfigNode resource in body.GetNodes("KRES_RESOURCE"))
                    {
                        yield return null;
                        current++;
                        amountComplete = (doing + (current * res)) / max;
                        int seed = 0;
                        if (resource.TryGetValue("seed", ref seed))
                        {
                            string resourceName = string.Empty;
                            if (!resource.TryGetValue("name", ref resourceName))
                            {
                                Debug.LogWarning("[KRES]: Nameless resource for " + body.name);
                                continue;
                            }
                            currentResource = resourceName;
                            Color colour = KRESUtils.BlankColour;
                            double limit = 0, octaves = 0, persistence = 0, frequency = 0;
                            double minAltitude = double.NegativeInfinity, maxAltitude = double.PositiveInfinity;
                            string[] biomes = new string[] { }, excludedBiomes = new string[] { };

                            print("[KRES]: Creating map for " + resourceName + " on " + body.name);

                            resource.TryGetValue("octaves", ref octaves);
                            resource.TryGetValue("persistence", ref persistence);
                            resource.TryGetValue("frequency", ref frequency);
                            resource.TryGetValue("colour", ref colour);
                            resource.TryGetValue("density", ref limit);
                            resource.TryGetValue("minAltitude", ref minAltitude);
                            resource.TryGetValue("maxAltitude", ref maxAltitude);
                            resource.TryGetValue("biomes", ref biomes);
                            resource.TryGetValue("excludedBiomes", ref excludedBiomes);
                            if (colour.a == 0)
                            {
                                Debug.LogWarning("[KRES]: Invalid colour for node " + resourceName + " for " + body.name);
                                continue;
                            }
                            if (limit == 0 || octaves == 0 || persistence == 0 || frequency == 0)
                            {
                                Debug.LogWarning("[KRES]: Invalid values for node " + resourceName + "for " + body.name);
                                continue;
                            }

                            //Map generation
                            Simplex simplex = new Simplex(seed, octaves, persistence, frequency);
                            Texture2D map = new Texture2D(1440, 720, TextureFormat.ARGB32, false);

                            print("[KRES]: Generating...");
                            var timer = System.Diagnostics.Stopwatch.StartNew();
                            for (int x = 0; x < 1440; x++)
                            {
                                double lon = (x / 4d) - 180d;
                                for (int y = 0; y < 720; y++)
                                {
                                    double lat = (y / 4d) - 90d;
                                    //Takes too much time to process ~50s per texture, will have to do on scanning.
                                    /*
                                    if (!double.IsInfinity(minAltitude) || !double.IsInfinity(maxAltitude))
                                    {
                                        double alt = planet.TerrainAltitude(lat, lon);
                                        if (alt > maxAltitude || alt < minAltitude)
                                        {
                                            map.SetPixel(x, y, KRESUtils.BlankColour);
                                            continue;
                                        }
                                    }
                                    */

                                    //And for that it appears the overlay isn't synced over correct longitude. Will be on-the-go too.
                                    /*
                                    if (biomes.Length > 0 || excludedBiomes.Length > 0)
                                    {
                                        string biome = planet.GetBiome(lat, lon);
                                        if ((biomes.Length > 0 && !biomes.Contains(biome)) || (excludedBiomes.Length > 0 && excludedBiomes.Contains(biome)))
                                        {
                                            map.SetPixel(x, y, KRESUtils.BlankColour);
                                            continue;
                                        }
                                    }
                                    */
                                    float a = 0;
                                    double density = 0;
                                    Vector3d position = KRESUtils.SphericalToCartesian(10d, x / 4d, y / 4d);
                                    density = simplex.noiseNormalized(position) - 1d + limit;
                                    if (density > 0)
                                    {
                                        a = Mathf.Lerp(0.2f, 1f, (float)(density / limit)) * colour.a;
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
                            print("[KRES]: Saved texture for " + resourceName + " in " + path);
                            timer.Stop();
                            print(String.Format("[KRES]: Texture created in {0}ms", timer.ElapsedMilliseconds));
                        }
                    }
                }
            }
            amountComplete = 1d;
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            ConfigNode test = null;
            string name = string.Empty;

            if (File.Exists(settingsPath)) { test = ConfigNode.Load(settingsPath); }

            else { Debug.LogWarning("[KRES]: KRESSettings.cfg is missing"); }

            //Gets the settings node
            if (!File.Exists(settingsPath) ||  DefaultLibrary.HasComponents(test.GetNode("KRES")))
            {
                //If simply missing components
                if (File.Exists(settingsPath))
                {
                    Debug.LogWarning("[KRES]: The KRESSettings.cfg file is missing components");
                    File.Delete(settingsPath);
                }
                //Create KRES config
                print("[KRES]: Creating new KRESSettings.cfg");
                DefaultLibrary.SaveSelectedDefault(settings, settingsPath);
                cfg = settings.GetNode("KRES");
                print("[KRES]: Successfully created and saved new KRESSettings.cfg");
            }

            else
            {
                print("[KRES]: Successfully loaded KRESSettings.cfg");
                settings = test;
                cfg = test.GetNode("KRES");
            }

            cfg.TryGetValue("name", ref name);
            cfg.TryGetValue("generated", ref generated);


            if (!Directory.Exists(texturePath))
            {
                Debug.LogWarning("[KRES]: KRESTextures directory does not exist in the save file");
                Directory.CreateDirectory(texturePath);
                generated = false;
                print("[KRES]: Successfully created texture directory");
            }

            if (!generated)
            {
                if (name != DefaultLibrary.GetSelectedDefault().Name)
                {
                    print("[KRES]: Generating from new defaults");
                    File.Delete(settingsPath);
                    settings.ClearNodes();
                    DefaultLibrary.SaveSelectedDefault(settings, settingsPath);
                    cfg = settings.GetNode("KRES");
                    print("[KRES]: Generated new files from new defaults");
                }

                print("[KRES]: Generating resource maps");
                timer.Start();

                StartCoroutine(GenerateAllMaps());
                max = cfg.nodes.DistinctNames().Count(n => KRESUtils.IsCelestialBody(n));
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
        }
        #endregion

        #region Update
        private void Update()
        {
            if (amountComplete == 1d && !generated)
            {
                print("[KRES]: Map generation complete");
                print(String.Format("[KRES]: Map generation took a total of {0:0.000}s", time));
                generated = true;
                cfg.SetValue("generated", generated.ToString());
                settings.ClearNodes();
                settings.AddNode(cfg);
                settings.Save(settingsPath);
                ResourceLoader.Load();
            }
        }
        #endregion

        #region GUI
        private void OnGUI()
        {
            if (this.visible)
            {
                this.window = GUI.Window(this.ID, this.window, Window, "KRES Map generation", skins.window);
            }
        }

        private void Window(int id)
        {
            GUI.BeginGroup(new Rect(10, 10, 380, 120));
            if (amountComplete == 1d) { GUI.Label(new Rect(0, 20, 380, 15), "Complete", skins.label); }
            else { GUI.Label(new Rect(0, 20, 380, 15), String.Concat("Currently generating: ", currentBody, " - ", currentResource), skins.label); }
            GUI.BeginGroup(new Rect(5, 50, 380, 30));
            progressBar.SetValue(amountComplete);
            progressBar.Draw();
            GUI.EndGroup();
            if (amountComplete == 1d)
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
                GUI.Label(new Rect(0, 80, 380, 15), "Please wait for map generation to finish. Do not leave the scene.", skins.label);
            }
            GUI.EndGroup();
        }
        #endregion
    }
}
