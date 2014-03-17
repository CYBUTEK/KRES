using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KRES.Extensions;
using KRES.Data;

namespace KRES
{
    public class ModuleKresScanner : PartModule
    {
        #region KSPFields
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Optimal alt"), UI_FloatRange(minValue = 100000f, maxValue = 2000000f, stepIncrement = 5000f)]
        public float optimalAltitude = 100000f;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Optimal pressure"), UI_FloatRange(minValue = 0.1f, maxValue = 1f, stepIncrement = 0.05f)]
        public float optimalPressure = 0.1f;
        [KSPField]
        public float scaleFactor = 0.02f;
        [KSPField]
        public float maxPrecision = 0.05f;
        [KSPField]
        public float scanningSpeed = 3600f;
        [KSPField]
        public bool isTweakable = true;
        [KSPField]
        public string type = "ore";
        #endregion

        #region Misc KSPFields
        [KSPField(isPersistant = true)]
        public bool scanning = false;
        [KSPField(guiActive = true, guiName = "Status")]
        public string status = "Idle";
        [KSPField(guiActive = true, guiFormat = "0.000", guiName = "Pressure")]
        public float pressure = 0f;
        #endregion

        #region Propreties
        internal double ASL
        {
            get { return FlightGlobals.getAltitudeAtPos(this.vessel.findWorldCenterOfMass()); }
        }

        internal double AtmosphericPressure
        {
            get
            {
                double alt = FlightGlobals.getAltitudeAtPos(this.part.transform.position);
                double pressure = FlightGlobals.getStaticPressure(alt, this.vessel.mainBody);
                return pressure > 1E-6d ? pressure : 0d;
            }
        }

        internal bool ResourceValid
        {
            get { return resource != string.Empty && PartResourceLibrary.Instance.resourceDefinitions.Contains(resource) && rate > 0f; }
        }

        private bool DataVisible
        {
            get { return (currentError - maxPrecision) / (1d - maxPrecision) <= 0.95d; }
        }

        private bool PercentageVisible
        {
            get { return (currentError - maxPrecision) / (1d - maxPrecision) <= 0.25d; }
        }
        #endregion

        #region Fields
        //Scanning
        internal ResourceBody body = new ResourceBody();
        internal ResourceType scannerType = ResourceType.ORE;
        internal List<ResourceItem> items = new List<ResourceItem>();
        internal double currentError = 1d;
        internal bool scannedFlag = false;
        internal IScanner scanner = null;
        internal string resource = "ElectricCharge";
        internal float rate = 10f;

        //GUI
        private int ID = Guid.NewGuid().GetHashCode();
        internal bool visible = false;
        private Rect window = new Rect();
        internal string location = string.Empty;
        internal string presence = string.Empty;
        private GUISkin skins = HighLogic.Skin;
        private Vector2 scroll = new Vector2();
        #endregion

        #region Part GUI
        [KSPEvent(guiName = "Start scanning", active = true, guiActive = true, externalToEVAOnly = false, guiActiveUnfocused = true, unfocusedRange = 5f)]
        public void GUIToggleScanning()
        {
            ToggleScan();
        }

        [KSPEvent(guiName = "Toggle window", active = true, guiActive = true)]
        public void GUIToggleWindow()
        {
            List<ModuleKresScanner> modules = new List<ModuleKresScanner>(this.vessel.FindPartModulesImplementing<ModuleKresScanner>());
            if (modules.Count > 1) { modules.Find(m => m.visible).visible = false; }
            this.visible = !this.visible;
        }
        #endregion

        #region Functions
        private void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight || FlightGlobals.currentMainBody == null || FlightGlobals.ActiveVessel == null || !this.vessel.loaded || !ResourceController.Instance.DataSet) { return; }
            if (this.body.Name != this.vessel.mainBody.bodyName)
            {
                LoadData();
                this.scanning = false;
                Events["GUIToggleScanning"].active = true;
            }
        }

        private void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight || FlightGlobals.currentMainBody == null || FlightGlobals.ActiveVessel == null || !this.vessel.loaded || !ResourceController.Instance.DataSet) { return; }
            if (this.items.Count > 0 && this.scanning && currentError > maxPrecision)
            {
                currentError -= this.scanner.Scan();
                scannedFlag = false;
                ResourceController.Instance.GetDataBody(this).CurrentError = this.currentError;
            }
            else if (this.scanning && currentError <= maxPrecision)
            {
                currentError = maxPrecision;
                ResourceController.Instance.GetDataBody(this).CurrentError = this.currentError;
                this.scanning = false;
                Events["GUIToggleScanning"].active = false;
                status = "Complete";
                ScreenMessages.PostScreenMessage("Scanning complete, scanner turned off.", 5, ScreenMessageStyle.UPPER_CENTER);
            }
            else if (this.scanning && this.scannerType == ResourceType.GAS && AtmosphericPressure > 0 && items.Count <= 0)
            {
                this.scanning = false;
                Events["GUIToggleScanning"].active = false;
                status = "No resources";
                currentError = -1d;
                ScreenMessages.PostScreenMessage("No resources in the atmosphere", 5, ScreenMessageStyle.UPPER_CENTER);
            }
            else if (this.scanning && this.scannerType == ResourceType.LIQUID && this.vessel.Splashed && items.Count <= 0)
            {
                this.scanning = false;
                Events["GUIToggleScanning"].active = false;
                status = "No resources";
                currentError = -1d;
                ScreenMessages.PostScreenMessage("No resources in the ocean", 5, ScreenMessageStyle.UPPER_CENTER);
            }

            if (Fields["pressure"].guiActive) { this.pressure = (float)AtmosphericPressure; }
        }
        #endregion

        #region Overrides
        public override void OnStart(PartModule.StartState state)
        {
            if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight) { return; }
            LoadScanner();

            if (!isTweakable || scannerType == ResourceType.LIQUID)
            {
                Fields["optimalAltitude"].guiActiveEditor = false;
                Fields["optimalPressure"].guiActiveEditor = false;
            }
            else
            {
                Fields["optimalPressure"].guiActiveEditor = (scannerType == ResourceType.GAS);
                Fields["optimalAltitude"].guiActiveEditor = (scannerType == ResourceType.ORE);
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (this.scanning) { Events["GUIToggleScanning"].guiName = "Stop scanning"; }
                this.window = new Rect(200, 200, 250, 400);
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            if (node.HasNode("INPUT"))
            {
                ConfigNode input = node.GetNode("INPUT");
                if (!input.TryGetValue("name", ref resource)) { return; }
                input.TryGetValue("rate", ref rate);
            }
        }

        public override string GetInfo()
        {
            string infoList = string.Empty;
            switch (this.type)
            {
                case "ore": infoList = string.Format("Scanner type: {0}\n", "Orbital"); break;
                case "gas": infoList = string.Format("Scanner type: {0}\n", "Atmospheric"); break;
                case "liquid": infoList = string.Format("Scanner type: {0}\n", "Oceanic"); break;
                default: return string.Empty;
            }

            infoList += String.Format("Minimal scanning period: {0}\n", KRESUtils.SecondsToTime(scanningSpeed));
            infoList += String.Format("Minimum error margin: {0:0.00}%", maxPrecision * 100d);
            if (type == "ore")
            {
                infoList += String.Format("\nOptimal scanning altitude: {0}m\n", optimalAltitude);
                infoList += String.Format("Scale altitude: {0:0.000}m", scaleFactor * optimalAltitude);
            }
            else if (type == "gas")
            {
                infoList += String.Format("\nOptimal scanning pressure: {0}atm\n", optimalPressure);
                infoList += String.Format("Scale pressure: {0:0.000}atm", scaleFactor * optimalPressure);
            }

            if (ResourceValid)
            {
                infoList += "\n\n<b><color=#99ff00ff>Input:</color></b>\n";
                infoList += String.Format("Resource: {0}\n", resource);
                infoList += String.Format("Rate: {0:0.0}/m", rate);
            }

            return infoList;
        }
        #endregion

        #region Methods
        private void ToggleScan()
        {
            this.scanning = !this.scanning;
            Events["GUIToggleScanning"].guiName = this.scanning ? "Stop scanning" : "Start scanning";
            status = this.scanning ? "Scanning" : "Idle";
        }

        private void LoadScanner()
        {
            this.scannerType = KRESUtils.GetResourceType(this.type);
            switch (this.scannerType)
            {
                case ResourceType.ORE:
                    {
                        scanner = new OrbitalScanner(this);
                        break;
                    }

                case ResourceType.GAS:
                    {
                        scanner = new AtmosphericScanner(this);
                        break;
                    }

                case ResourceType.LIQUID:
                    {
                        scanner = new OceanicScanner(this);
                        break;
                    }

                default:
                    break;
            }
        }

        private void LoadData()
        {
            body = ResourceController.Instance.GetCurrentBody();
            print(body.Name);
            items.Clear();
            items.AddRange(body.GetItemsOfType(scannerType));
            print(String.Join(", ", items.Select(i => i.Name).ToArray()));
            DataBody data = ResourceController.Instance.GetDataBody(this);
            print(data.Name + " " + data.CurrentError);
            this.currentError = data.CurrentError;
        }

        private string ItemPercentage(ResourceItem item)
        {
            return (((currentError * item.ActualError) + 1d) * item.ActualDensity * 100d).ToString("0.00");
        }

        private string ItemError(ResourceItem item)
        {
            return (item.ActualDensity * currentError * 100d).ToString("0.00");
        }
        #endregion

        #region GUI
        private void OnGUI()
        {
            if (!HighLogic.LoadedSceneIsFlight) { return; }
            if (this.visible)
            {
                this.window = GUILayout.Window(this.ID, this.window, Window, "KRES Scan Data", skins.window);
            }
        }

        private void Window(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(location + " resources:", KRESUtils.BoldLabel, GUILayout.Width(150));
            scroll = GUILayout.BeginScrollView(scroll, false, false, skins.horizontalScrollbar, skins.verticalScrollbar, skins.box);
            if (DataVisible)
            {
                foreach (ResourceItem item in items)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(item.Name + presence, KRESUtils.GetLabelOfColour(item.Name), GUILayout.Width(120));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(String.Format("{0} ± {1}%", PercentageVisible ? ItemPercentage(item) : "--", PercentageVisible ? ItemError(item) : "--"));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                if (this.scanning) { GUILayout.Label(status, skins.label); }
                else { GUILayout.Label("Nothing to show.", skins.label); }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Max error:", skins.label);
            GUILayout.Label(String.Format("   ± {0}%", PercentageVisible ? (currentError * 100d).ToString("0.00") : "--"));
            GUILayout.EndHorizontal();
            if (currentError > maxPrecision) { if (GUILayout.Button((this.scanning ? "Stop" : "Start") + " scanning", skins.button)) { ToggleScan(); } }
            if (GUILayout.Button("Close", skins.button)) { this.visible = false; }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        #endregion
    }
}
