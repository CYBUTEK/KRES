﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class DebugWindow : MonoBehaviour
    {
        #region Instance
        public static DebugWindow Instance { get; private set; }
        #endregion

        #region Fields
        private Vector2 windowSize = new Vector2(400f, 0f);
        private Rect windowPosition = new Rect();
        private int windowID = Guid.NewGuid().GetHashCode();
        private int numberOfEntries = 5;
        private Queue<string> logEntries = new Queue<string>();
        private bool showTexture = false;
        private string textureName = string.Empty;
        private Texture textureImage = null;
        private float textureScale = 1f;
        private int i = 0;
        private string body = string.Empty;
        private ResourceItem[] items = null;
        #endregion

        #region Properties
        private bool visible = false;
        /// <summary>
        /// Gets and sets whether the window is visible.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        #endregion

        #region Initialisation
        private void Awake()
        {
            // Check for current instance.
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

        private void Start()
        {
            this.windowPosition = new Rect(this.windowPosition.x, this.windowPosition.y, this.windowSize.x, this.windowSize.y);
            Print("Debug window started.");
        }
        #endregion

        #region Update and Drawing
        private void Update()
        {
            // Toggle the window visibility with the F11 key.
            if (Input.GetKeyDown(KeyCode.F11))
            {
                this.visible = !this.visible;
            }

            if (ResourceLoader.Loaded && HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.mainBody.bodyName != body)
            {
                if (items != null)
                {
                    items[i].Map.HideTexture(body);
                    Print("Hid " + items[i].Name + " around " + body);
                    ClearTexture();
                    i = 0;
                }
                if (FlightGlobals.ActiveVessel.mainBody.bodyName == "Sun")
                {
                    body = string.Empty;
                    Print("Body is now Sun");
                    items = null;
                }
                else
                {
                    body = FlightGlobals.ActiveVessel.mainBody.bodyName;
                    Print("Body is now " + body);
                    items = ResourceController.Instance.ResourceBodies.Find(b => b.Name == body).ResourceItems.Where(i => i.HasMap).ToArray();
                }
            }

            else if (!HighLogic.LoadedSceneIsFlight && this.textureImage != null)
            {
                ResourceController.Instance.HideAllResources();
                ClearTexture();
                i = 0;
                items = null;
            }
        }

        private void OnGUI()
        {
            if (this.visible)
            {
                this.windowPosition = GUILayout.Window(this.windowID, this.windowPosition, Window, "KRES (Debug)", HighLogic.Skin.window);
            }
        }

        private void Window(int windowID)
        {
            // Draw the log entries.
            GUILayout.BeginVertical(HighLogic.Skin.box);
            foreach (string entry in logEntries)
            {
                GUILayout.Label(entry);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Show next map", HighLogic.Skin.button))
            {
                if (ResourceLoader.Loaded)
                {
                    if (HighLogic.LoadedSceneIsFlight)
                    {
                        if (body != string.Empty && items.Length > 0)
                        {
                            items[i].Map.HideTexture(body);
                            if (textureImage != null) { i++; }
                            if (i > items.Length - 1) { i = 0; }
                            items[i].Map.ShowTexture(body);
                            SetTexture(items[i]);
                            Print("Showing " + items[i].Resource.name + " around " + body);
                        }
                    }
                    else { Print("Cannot display map, not in flight mode"); }
                }
                else { Print("Cannot display map, resources are not loaded"); }
            }

            if (GUILayout.Button("Show previous map", HighLogic.Skin.button))
            {
                if (ResourceLoader.Loaded)
                {
                    if (HighLogic.LoadedSceneIsFlight)
                    {
                        if (body != string.Empty && items.Length > 0)
                        {
                            items[i].Map.HideTexture(body);
                            if (textureImage != null) { i--; }
                            if (i < 0) { i = items.Length - 1; }
                            items[i].Map.ShowTexture(body);
                            SetTexture(items[i]);
                            Print("Showing " + items[i].Resource.name + " around " + body);
                        }
                    }
                    else { Print("Cannot display map, not in flight mode"); }
                }
                else { Print("Cannot display map, resources are not loaded"); }
            } 

            if (GUILayout.Button("Hide map", HighLogic.Skin.button))
            {
                if (ResourceLoader.Loaded)
                {
                    if (HighLogic.LoadedSceneIsFlight && items.Length > 0)
                    {
                        if (body != string.Empty)
                        {
                            items[i].Map.HideTexture(body);
                            Print("Hid " + items[i].Resource.name + " around " + body);
                            ClearTexture();
                            i = 0;
                        }
                    }
                    else { Print("Cannot hide map, not in flight mode"); }
                }
                else { Print("Cannot hide map, resources are not loaded"); }
            }

            // If a texture has been set allow it to be displayed.
            if (this.textureImage != null)
            {
                // Draw the toggle button to display the texture.
                if (GUILayout.Toggle(this.showTexture, "Show Texture", HighLogic.Skin.button) != this.showTexture)
                {
                    this.showTexture = !this.showTexture;

                    // If toggled reset the window size.
                    this.windowPosition.width = this.windowSize.x;
                    this.windowPosition.height = this.windowSize.y;
                }

                if (this.showTexture)
                {
                    // Draw the texture name if one has been supplied.
                    if (this.textureName.Length > 0)
                    {
                        GUILayout.Label(this.textureName);
                    }

                    GUILayout.BeginHorizontal();
                    float previousScale = this.textureScale;
                    this.textureScale = GUILayout.HorizontalSlider(this.textureScale, 0.1f, 1f, GUILayout.ExpandWidth(true));
                    GUILayout.Label((this.textureScale * 100f).ToString("F0") + "%");
                    GUILayout.EndHorizontal();

                    if (this.textureScale != previousScale)
                    {
                        this.windowPosition.width = this.windowSize.x;
                        this.windowPosition.height = this.windowSize.y;
                    }
                    GUILayoutOption[] boxOptions = 
                    {
                        GUILayout.Width(this.textureImage.width * this.textureScale),
                        GUILayout.Height(this.textureImage.height * this.textureScale)
                    };
                    GUILayout.Box(this.textureImage, boxOptions);
                }
            }

            GUI.DragWindow();
        }
        #endregion

        #region Print
        /// <summary>
        /// Prints a log entry to the debug window and KSP log file.
        /// </summary>
        public void Print(string entry)
        {
            if (this.logEntries.Count == this.numberOfEntries)
            {
                this.logEntries.Dequeue();
            }

            this.logEntries.Enqueue(entry);
            print("[KRES Debug]: " + entry);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Integer)
        /// </summary>
        public void Print(int value)
        {
            Print("Integer: " + value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Float)
        /// </summary>
        public void Print(float value)
        {
            Print("Float: " + value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Double)
        /// </summary>
        public void Print(double value)
        {
            Print("Double: " + value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Boolean)
        /// </summary>
        public void Print(bool value)
        {
            Print("Boolean: " + value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Byte)
        /// </summary>
        public void Print(byte value)
        {
            Print("Byte: " + value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector2)
        /// </summary>
        public void Print(Vector2 value)
        {
            Print("Vector2: XY(" + value.x + ", " + value.y + ")");
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector2d)
        /// </summary>
        public void Print(Vector2d value)
        {
            Print("Vector2d: XY(" + value.x + ", " + value.y + ")");
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector3)
        /// </summary>
        public void Print(Vector3 value)
        {
            Print("Vector3: XYZ(" + value.x + ", " + value.y + ", " + value.z + ")");
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector3d)
        /// </summary>
        public void Print(Vector3d value)
        {
            Print("Vector3d: XYZ(" + value.x + ", " + value.y + ", " + value.z + ")");
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector4)
        /// </summary>
        public void Print(Vector4 value)
        {
            Print("Vector4: XYZW(" + value.x + ", " + value.y + ", " + value.z + ", " + value.w + ")");
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector4d)
        /// </summary>
        public void Print(Vector4d value)
        {
            Print("Vector4d: XYZW(" + value.x + ", " + value.y + ", " + value.z + ", " + value.w + ")");
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Color)
        /// </summary>
        public void Print(Color value)
        {
            Print("Color: " + KRESUtils.ColourToString(value));
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Rect)
        /// </summary>
        public void Print(Rect value)
        {
            Print("Rect: XYWH(" + value.x + ", " + value.y + ", " + value.width + ", " + value.height + ")");
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (String[])
        /// </summary>
        public void Print(string[] value)
        {
            Print("String[]: " + string.Join(", ", value));
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Array)
        /// </summary>
        public void Print(Array value)
        {
            Print(value.GetType().Name + ": " + value.Length);
        }
        #endregion

        #region Texture
        /// <summary>
        /// Sets a texture that can be viewed from within the debug window.
        /// </summary>
        public void SetTexture(ResourceItem item)
        {
            this.textureImage = item.Map.GetTexture();
            this.textureName = item.Resource.name;
        }

        /// <summary>
        /// Clears the currently displayed texture.
        /// </summary>
        public void ClearTexture()
        {
            this.textureImage = null;
            this.textureName = string.Empty;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Prints a log entry to the debug window and KSP log file.
        /// </summary>
        public static void Log(string entry)
        {
            Instance.Print(entry);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Integer)
        /// </summary>
        public static void Log(int value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Float)
        /// </summary>
        public static void Log(float value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Double)
        /// </summary>
        public static void Log(double value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Boolean)
        /// </summary>
        public static void Log(bool value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Byte)
        /// </summary>
        public static void Log(byte value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector2)
        /// </summary>
        public static void Log(Vector2 value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector2d)
        /// </summary>
        public static void Log(Vector2d value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector3)
        /// </summary>
        public static void Log(Vector3 value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector3d)
        /// </summary>
        public static void Log(Vector3d value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector4)
        /// </summary>
        public static void Log(Vector4 value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Vector4d)
        /// </summary>
        public static void Log(Vector4d value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Color)
        /// </summary>
        public static void Log(Color value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Rect)
        /// </summary>
        public static void Log(Rect value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (String[])
        /// </summary>
        public static void Log(string[] value)
        {
            Instance.Print(value);
        }

        /// <summary>
        /// Prints a log entry to the debug window and KSP log file. (Array)
        /// </summary>
        public static void Log(Array value)
        {
            Instance.Print(value);
        }
        #endregion
    }
}
