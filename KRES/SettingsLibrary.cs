using System.IO;
using UnityEngine;

namespace KRES
{
    public class SettingsLibrary
    {
        #region Instance
        private static SettingsLibrary instance;
        public static SettingsLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingsLibrary();
                }
                return instance;
            }
        }
        #endregion

        #region Fields
        private ConfigNode configNode = new ConfigNode();
        #endregion

        #region Properties
        private string fileFullName = Path.Combine(KRESUtils.GetDLLPath(), "KRESSettings.txt");
        public string FileFullName
        {
            get { return this.fileFullName; }
            set { this.fileFullName = value; }
        }
        #endregion

        #region Static Properties
        public static ConfigNode ConfigNode
        {
            get { return Instance.configNode; }
            set { Instance.configNode = value; }
        }
        #endregion

        #region Initialisation
        private SettingsLibrary()
        {
            Load();
        }
        #endregion

        #region Static Methods
        public static void Save()
        {
            ConfigNode.Save(Instance.FileFullName);
        }

        public static void Load()
        {
            ConfigNode = ConfigNode.Load(Instance.FileFullName);
        }
        #endregion
    }
}
