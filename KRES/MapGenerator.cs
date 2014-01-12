using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class MapGenerator : MonoBehaviour
    {
        private Simplex simplex;
        private int seed;
        private double octaves;
        private double persistence;
        private double frequency;
        private List<string> bodies = new List<string>();

        private void GenerateMap(int seed, double octaves, double persistence, double frequency)
        {
            simplex = new Simplex(seed, octaves, persistence, frequency);
        }

        private void Awake()
        {
            ConfigNode settings = new ConfigNode("settings");
            ConfigNode cfg = new ConfigNode("KRES");
            string saveFile = "saves/" + HighLogic.fetch.GameSaveFolder + "/KRESSettings.cfg";
            ConfigNode test = ConfigNode.Load(Path.Combine(KSPUtil.ApplicationRootPath, saveFile));

            //Gets the settings node
            if (test != null)
            {
                print("[KRES]: Successfully loaded KRESSettings.cfg");
                settings = test;
            }

            else
            {
                print("[KRES]: KRESSettings.cfg is missing");

                //Create KRES config
                print("[KRES]: Creating new KRESSettings.cfg");
                cfg.AddValue("octaves", 1d);
                cfg.AddValue("persistence", 1d);
                cfg.AddValue("frequency", 1d);

                //Generate all the planet config nodes
                KRESSettings.GenerateSettings(cfg);

                //Save settings
                settings.AddNode(cfg);
                settings.Save(Path.Combine(KSPUtil.ApplicationRootPath, saveFile));
                print("[KRES]: Successfully created and saved new KRESSettings.cfg");
            }

            print("[KRES]: Parsing simplex generator values");
            foreach (string value in settings.values)
            {
                if (value == "octaves")
                {
                    double oct;
                    if (double.TryParse(value, out oct)) { octaves = oct; }
                }

                if (value == "persistence")
                {
                    double pers;
                    if (double.TryParse(value, out pers)) { persistence = pers; }
                }

                if (value == "frequency")
                {
                    double freq;
                    if (double.TryParse(value, out freq)) { frequency = freq; }
                }
            }

            /*
            //Generate the list of bodies
            foreach (CelestialBody body in FlightGlobals.Bodies) { bodies.Add(body.bodyName); }

            foreach (ConfigNode body in settings.GetNode("KRES").nodes)
            {
                if (bodies.Contains(body.name))
                {
                    foreach (ConfigNode resource in body.GetNodes("KRES_RESOURCE"))
                    {

                    }
                }
            }*/
        }
    }
}
