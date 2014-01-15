using System;
using System.IO;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace KRES.MainMenu
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MenuOverlay : MonoBehaviour
    {
        private Rect windowPosition = new Rect(Screen.width - 500f, 10f, 500f, 0);
        private int windowID = Guid.NewGuid().GetHashCode();
        private GUIStyle windowStyle, headingStyle, normalStyle, buttonStyle;

        private string version = string.Empty;

        private Texture2D buttonTextureNormal = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D buttonTextureHover = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D buttonTextureActive = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        private void Start()
        {
            GUIStyle dottyFontStyle = KRESUtils.GetDottyFontStyle();

            this.buttonTextureNormal.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetDLLPath(), "GUI/Button/Normal.png")));
            this.buttonTextureHover.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetDLLPath(), "GUI/Button/Hover.png")));
            this.buttonTextureActive.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetDLLPath(), "GUI/Button/Active.png")));

            this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.windowStyle = new GUIStyle();

            this.headingStyle = new GUIStyle(dottyFontStyle);
            this.headingStyle.normal.textColor = Color.yellow;
            this.headingStyle.alignment = TextAnchor.UpperCenter;
            this.headingStyle.fontSize = 50;
            this.headingStyle.stretchWidth = true;

            this.normalStyle = new GUIStyle(dottyFontStyle);
            this.normalStyle.normal.textColor = Color.white;
            this.normalStyle.alignment = TextAnchor.UpperCenter;
            this.normalStyle.fontSize = 24;
            this.normalStyle.stretchWidth = true;

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button);
            this.buttonStyle.normal.textColor = Color.yellow;
            this.buttonStyle.normal.background = this.buttonTextureNormal;
            this.buttonStyle.hover.textColor = Color.yellow;
            this.buttonStyle.hover.background = this.buttonTextureHover;
            this.buttonStyle.active.textColor = Color.yellow;
            this.buttonStyle.active.background = this.buttonTextureActive;
        }

        private void OnGUI()
        {
            this.windowPosition = GUILayout.Window(this.windowID, this.windowPosition, Window, string.Empty, this.windowStyle);
        }

        private void Window(int windowID)
        {
            GUILayout.Label("KSP Resource Expansion System", this.headingStyle);
            GUILayout.Label("Currently Selected Resource Pack: Default", this.normalStyle);
            if (GUILayout.Button("Select Resource Pack", this.buttonStyle) && !PackSelector.IsBeingDisplayed)
            {
                new GameObject("PackSelector", typeof(PackSelector));
            }
        }
    }
}