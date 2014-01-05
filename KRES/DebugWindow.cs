using System;
using System.Collections.Generic;
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
        private int numberOfEntries = 10;
        private Queue<string> logEntries = new Queue<string>();
        private bool showTexture = false;
        private string textureName = string.Empty;
        private Texture textureImage = null;
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

                    GUILayout.Box(this.textureImage);
                }
            }

            GUI.DragWindow();
        }
        #endregion

        #region Public Methods
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
            print("[KRES Log]: " + entry);
        }

        /// <summary>
        /// Sets a texture that can be viewed from within the debug window.
        /// </summary>
        public void SetTexture(Texture textureImage, string textureName = "")
        {
            this.textureImage = textureImage;
            this.textureName = textureName;
        }
        #endregion
    }
}
