using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using KRES.Defaults;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class MapGenerator : MonoBehaviour
    {
        //Simplex map generator
        private void GenerateMap(int seed, double octaves, double persistence, double frequency, string path, string name, Color colour, double limit)
        {
            if (limit == 0) { return; }
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
                    density = simplex.noiseNormalized(position) - limit;
                    if (density > 0)
                    {
                        a = Mathf.Lerp(0.2f, 1.25f, (float)(density / limit)) * colour.a;
                        if (a / colour.a > 1) { a = colour.a; }
                        map.SetPixel(x, y, new Color(colour.r, colour.g, colour.b, a));
                    }
                    else { map.SetPixel(x, y, new Color(255, 255, 255, 0)); }
                }
            }
            map.Apply();

            //Save texture
            File.WriteAllBytes(Path.Combine(path, name + ".png"), map.EncodeToPNG());
            print("[KRES]: Saved texture for " + name + " in " + path);
            timer.Stop();
            print(String.Format("[KRES]: Texture created in {0}ms", timer.ElapsedMilliseconds));
        }

        private void Awake()
        {
            ConfigNode settings = new ConfigNode("settings"), test = null, cfg = new ConfigNode("KRES");
            string saveFile = Path.Combine(KSPUtil.ApplicationRootPath, "saves/" + HighLogic.fetch.GameSaveFolder + "/KRESSettings.cfg");
            string textures = Path.Combine(KSPUtil.ApplicationRootPath, "saves/" + HighLogic.fetch.GameSaveFolder + "/KRESTextures");
            string name = string.Empty;
            bool generated = false;

            if (File.Exists(saveFile)) { test = ConfigNode.Load(saveFile); }

            else { Debug.LogWarning("[KRES]: KRESSettings.cfg is missing"); }

            //Gets the settings node
            if (!File.Exists(saveFile) ||  DefaultLibrary.HasComponents(test.GetNode("KRES")))
            {
                //If simply missing components
                if (File.Exists(saveFile))
                {
                    Debug.LogWarning("[KRES]: The KRESSettings.cfg file is missing components");
                    File.Delete(saveFile);
                }

                //Create KRES config
                print("[KRES]: Creating new KRESSettings.cfg");
                DefaultLibrary.SaveDefaults(settings, saveFile);
                cfg = settings.GetNode("KRES");
                print("[KRES]: Successfully created and saved new KRESSettings.cfg");
            }

            else
            {
                print("[KRES]: Successfully loaded KRESSettings.cfg");
                settings = test;
                cfg = test.GetNode("KRES");
            }

            if (cfg.HasValue("name")) { name = cfg.GetValue("name"); }

            if (cfg.HasValue("generated"))
            {
                bool gen;
                if (bool.TryParse(cfg.GetValue("generated"), out gen)) { generated = gen; }
            }

            if (!Directory.Exists(textures))
            {
                Debug.LogWarning("[KRES]: KRESTextures directory does not exist in the save file");
                Directory.CreateDirectory(textures);
                generated = false;
                print("[KRES]: Successfully created texture directory");
            }

            if (!generated)
            {
                if (name != DefaultLibrary.GetSelectedDefault().Name)
                {
                    print("[KRES]: Generating from new defaults");
                    File.Delete(saveFile);
                    settings.ClearNodes();
                    DefaultLibrary.SaveDefaults(settings, saveFile);
                    cfg = settings.GetNode("KRES");
                    print("[KRES]: Generated new files from new defaults");
                }
                print("[KRES]: Generating resource maps");
                foreach (ConfigNode body in cfg.nodes)
                {
                    if (KRESUtils.IsCelestialBody(body.name))
                    {
                        if (!Directory.Exists(Path.Combine(textures, body.name)))
                        {
                            Debug.LogWarning("[KRES]: Texture folder for " + body.name + " is missing");
                            Directory.CreateDirectory(Path.Combine(textures, body.name));
                            Debug.Log("[KRES]: Created texture folder for " + body.name);
                        }
                        string path = Path.Combine(textures, body.name);
                        foreach (ConfigNode resource in body.GetNodes("KRES_RESOURCE"))
                        {
                            int seed = 0;
                            if (resource.HasValue("seed") && int.TryParse(resource.GetValue("seed"), out seed))
                            {
                                string resourceName = string.Empty;
                                Color colour = new Color(0, 0, 0, 0);
                                double density = 0, octaves = 0, persistence = 0, frequency = 0;

                                if (resource.HasValue("name")) { resourceName = resource.GetValue("name"); }
                                else
                                {
                                    Debug.LogWarning("[KRES]: Invalid resource while generating node " + resource.id + " for " + body.name);
                                    continue;
                                }
                                print("[KRES]: Creating map for " + resourceName + " on " + body.name);

                                if (resource.HasValue("octaves"))
                                {
                                    double oct;
                                    if (double.TryParse(resource.GetValue("octaves"), out oct)) { octaves = oct; }
                                }

                                if (resource.HasValue("persistence"))
                                {
                                    double pers;
                                    if (double.TryParse(resource.GetValue("persistence"), out pers)) { persistence = pers; }
                                }

                                if (resource.HasValue("frequency"))
                                {
                                    double freq;
                                    if (double.TryParse(resource.GetValue("frequency"), out freq)) { frequency = freq; }
                                }

                                if (resource.HasValue("colour"))
                                {
                                    Vector4 col = KSPUtil.ParseVector4(resource.GetValue("colour"));
                                    colour = new Color(col.x, col.y, col.z, col.w);
                                }

                                if (colour.r == 0 && colour.g == 0 && colour.b == 0 && colour.a == 0)
                                {
                                    Debug.LogWarning("[KRES]: Invalid colour for node " + resource.id + " for " + body.name);
                                    continue;
                                }

                                if (resource.HasValue("density"))
                                {
                                    double den;
                                    if (double.TryParse(resource.GetValue("density"), out den)) { density = den; }
                                }

                                if (density == 0 || octaves == 0 || persistence == 0 || frequency == 0)
                                {
                                    Debug.LogWarning("[KRES]: Invalid values for node " + resource.id + "for" + body.name);
                                    continue;
                                }
                                GenerateMap(seed, octaves, persistence, frequency, path, resourceName, colour, density);
                            }
                        }
                    }
                }
                print("[KRES]: Map generation complete");
                generated = true;
                cfg.SetValue("generated", generated.ToString());
                settings.ClearNodes();
                settings.AddNode(cfg);
                settings.Save(saveFile);
            }
        }
    }
}
