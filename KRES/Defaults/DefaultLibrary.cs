using System.Collections.Generic;
using System.Linq;
using KRES.Extensions;
using UnityEngine;

namespace KRES.Defaults
{
    public class DefaultLibrary
    {
        #region Instance
        private static DefaultLibrary instance;
        public static DefaultLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DefaultLibrary();
                }
                return instance;
            }
        }
        #endregion

        #region Properties
        private Dictionary<string, DefaultConfig> defaults = new Dictionary<string, DefaultConfig>();
        public Dictionary<string, DefaultConfig> Defaults
        {
            get { return this.defaults; }
        }

        private DefaultConfig selectedDefault = new DefaultConfig();
        public DefaultConfig SelectedDefault
        {
            get { return this.selectedDefault; }
            set { this.selectedDefault = value; }
        }
        #endregion

        #region Initialisation
        private DefaultLibrary()
        {
            Load();
        }
        #endregion

        #region Public Methods
        public void Load()
        {
            this.defaults.Clear();

            foreach (UrlDir.UrlConfig urlConfig in GameDatabase.Instance.GetConfigs("KRES_DEFAULTS"))
            {
                DefaultConfig defaultConfig = new DefaultConfig(urlConfig.config);
                this.defaults.Add(defaultConfig.Name, defaultConfig);

                if (defaultConfig.Name == "Default")
                {
                    this.selectedDefault = defaultConfig;
                }
            }
        }
        #endregion

        #region Static Methods
        public static List<DefaultConfig> GetDefaults()
        {
            return new List<DefaultConfig>(Instance.Defaults.Values);
        }

        public static List<string> GetDefaultNames()
        {
            return new List<string>(Instance.Defaults.Keys);
        }

        public static DefaultConfig GetDefault(string name)
        {
            if (instance.Defaults.ContainsKey(name))
            {
                return Instance.Defaults[name];
            }
            return null;
        }

        public static DefaultConfig GetSelectedDefault()
        {
            return Instance.SelectedDefault;
        }

        public static void SetSelectedDefault(DefaultConfig defaultConfig)
        {
            Instance.SelectedDefault = defaultConfig;
        }

        public static void SaveSelectedDefault(ConfigNode config, string path)
        {
            config.AddNode(GetSelectedDefault().CreateConfigNode(true));
            config.Save(path);
        }

        public static bool HasComponents(ConfigNode cfg)
        {
            if (cfg.HasNode("KRES"))
            {
                ConfigNode node = cfg.GetNode("KRES");
                return node.HasValue("generated") && node.nodes.Count > 0;
            }
            return false;
        }
        #endregion
    }
}
