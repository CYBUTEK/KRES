using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using KRES.Defaults;
using KRES.Extensions;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class MapGenerator : MonoBehaviour
    {
        #region MapGenerator
        /// <summary>
        /// Creates and save texture using Simplex noise
        /// </summary>
        /// <param name="seed">Seed for the simplex generator</param>
        /// <param name="octaves">Octaves for the simplex function</param>
        /// <param name="persistence">Persistence for the simplex function</param>
        /// <param name="frequency">Frequency for the simplex function</param>
        /// <param name="path">Path to save the texture to</param>
        /// <param name="name">Name of the resource</param>
        /// <param name="colour">Colour of the resource map</param>
        /// <param name="limit">Percentage of the map that will be covered</param>
        private void GenerateMap(int seed, double octaves, double persistence, double frequency, string path, string name, Color colour, double limit)
        {
            if (limit <= 0) { return; }
            Simplex simplex = new Simplex(seed, octaves, persistence, frequency);
            Texture2D map = new Texture2D(360, 180, TextureFormat.ARGB32, false);

            print("[KRES]: Generating...");
            var timer = System.Diagnostics.Stopwatch.StartNew();
            for (int x = 0; x < 360; x++)
            {
                for (int y = 0; y < 180; y++)
                {
                    float a = 0;
                    double density = 0;
                    Vector3d position = KRESUtils.SphericalToCartesian(10d, x, y);
                    density = simplex.noiseNormalized(position) - 1d + limit;
                    if (density > 0)
                    {
                        a = Mathf.Lerp(0.2f, 1f, (float)(density / limit)) * colour.a;
                        if (a > 0.8f) { a = 0.8f; }
                        map.SetPixel(x, y, new Color(colour.r, colour.g, colour.b, a));
                    }
                    else { map.SetPixel(x, y, KRESUtils.BlankColour); }
                }
            }
            map.Apply();

            //Save texture
            File.WriteAllBytes(Path.Combine(path, name + ".png"), map.EncodeToPNG());
            print("[KRES]: Saved texture for " + name + " in " + path);
            timer.Stop();
            print(String.Format("[KRES]: Texture created in {0}ms", timer.ElapsedMilliseconds));
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            ConfigNode settings = new ConfigNode("settings"), test = null, cfg = new ConfigNode("KRES");
            string settingsPath = Path.Combine(KRESUtils.GetSavePath(), "KRESSettings.cfg");
            string texturePath = Path.Combine(KRESUtils.GetSavePath(), "KRESTextures");
            string name = string.Empty;
            bool generated = false;

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
                var timer = System.Diagnostics.Stopwatch.StartNew();
                foreach (ConfigNode body in cfg.nodes)
                {
                    if (KRESUtils.IsCelestialBody(body.name))
                    {
                        if (!Directory.Exists(Path.Combine(texturePath, body.name)))
                        {
                            Debug.LogWarning("[KRES]: Texture folder for " + body.name + " is missing");
                            Directory.CreateDirectory(Path.Combine(texturePath, body.name));
                            print("[KRES]: Created texture folder for " + body.name);   
                        }
                        string path = Path.Combine(texturePath, body.name);
                        foreach (ConfigNode resource in body.GetNodes("KRES_RESOURCE"))
                        {
                            int seed = 0;
                            if (resource.TryGetValue("seed", ref seed))
                            {
                                string resourceName = string.Empty;
                                Color colour = KRESUtils.BlankColour;
                                double density = 0, octaves = 0, persistence = 0, frequency = 0;

                                if (resource.TryGetValue("name", ref resourceName)) { }
                                else
                                {
                                    Debug.LogWarning("[KRES]: Nameless resource for " + body.name);
                                    continue;
                                }
                                print("[KRES]: Creating map for " + resourceName + " on " + body.name);

                                resource.TryGetValue("octaves", ref octaves);
                                resource.TryGetValue("persistence", ref persistence);
                                resource.TryGetValue("frequency", ref frequency);
                                resource.TryGetValue("colour", ref colour);
                                resource.TryGetValue("density", ref density);
                                if (colour.a == 0)
                                {
                                    Debug.LogWarning("[KRES]: Invalid colour for node " + resourceName + " for " + body.name);
                                    continue;
                                }
                                if (density == 0 || octaves == 0 || persistence == 0 || frequency == 0)
                                {
                                    Debug.LogWarning("[KRES]: Invalid values for node " + resourceName + "for " + body.name);
                                    continue;
                                }
                                GenerateMap(seed, octaves, persistence, frequency, path, resourceName, colour, density);
                            }
                        }
                    }
                }
                timer.Stop();
                print("[KRES]: Map generation complete");
                print(String.Format("[KRES]: Map generation took a total of {0}ms", timer.ElapsedMilliseconds));
                generated = true;
                cfg.SetValue("generated", generated.ToString());
                settings.ClearNodes();
                settings.AddNode(cfg);
                settings.Save(settingsPath);
            }
        }
        #endregion
    }
}
