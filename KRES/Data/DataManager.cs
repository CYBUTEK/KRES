using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KRES.Extensions;

namespace KRES.Data
{
    public class DataManager : ScenarioModule
    {
        #region Propreties
        /// <summary>
        /// Gets the current KRESDataManager for this save
        /// </summary>
        public static DataManager Current
        {
            get
            {
                //Borrowed from https://github.com/Majiir/Kethane/blob/master/Plugin/KethaneData.cs#L10-L28
                Game game = HighLogic.CurrentGame;
                if (game == null) { return null; }
                if (!game.scenarios.Any(p => p.moduleName == typeof(DataManager).Name))
                {
                    ProtoScenarioModule scenario = game.AddProtoScenarioModule(typeof(DataManager), GameScenes.FLIGHT);
                    if (scenario.targetScenes.Contains(HighLogic.LoadedScene)) { scenario.Load(ScenarioRunner.fetch); }
                }
                return game.scenarios.Select(s => s.moduleRef).OfType<DataManager>().SingleOrDefault();
            }
        }
        #endregion

        #region Fields
        internal List<DataType> data = new List<DataType>();
        #endregion

        #region Overrides
        public override void OnSave(ConfigNode node)
        {
            print("OnSave()");
            if (!HighLogic.LoadedSceneIsFlight) { return; }
            foreach(DataType type in data)
            {
                ConfigNode t = node.AddNode(KRESUtils.GetTypeString(type.Type));
                foreach (DataBody body in type.Bodies)
                {
                    ConfigNode b = t.AddNode(body.Name);
                    b.AddValue("currentError", body.CurrentError.ToString("0.00000"));
                }
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            print("OnLoad()");
            if (!HighLogic.LoadedSceneIsFlight) { return; }
            CheckForDataNodes(node);
            GetNodes(node);
        }
        #endregion

        #region Methods
        private bool CheckForDataNodes(ConfigNode node)
        {
            foreach (string type in KRESUtils.types.Values)
            {
                if (!node.HasNode(type)) { goto Incomplete; }
                ConfigNode t = node.GetNode(type);
                foreach (CelestialBody body in KRESUtils.GetRelevantBodies(type))
                {
                    if (!t.HasNode(body.bodyName)) { goto Incomplete; }
                }
            }
            return true;

            Incomplete:
            {
                print("no nodes");
                node.ClearNodes();
                foreach(string type in KRESUtils.types.Values)
                {
                    ConfigNode t = node.AddNode(type);
                    foreach(CelestialBody body in KRESUtils.GetRelevantBodies(type))
                    {
                        ConfigNode b = t.AddNode(body.bodyName);
                        b.AddValue("currentError", 1d);
                    }
                }
                return false;
            }
        }

        private void GetNodes(ConfigNode node)
        {
            data.Clear();
            foreach (ConfigNode type in node.nodes)
            {
                data.Add(new DataType(type));
            }
        }
        #endregion
    }
}
