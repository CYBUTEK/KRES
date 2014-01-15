using System;
using UnityEngine;
using KRES.Defaults;

namespace KRES.MainMenu
{
    public class PackSelector : MonoBehaviour
    {
        #region Static Properties
        private static bool isBeingDisplayed = false;
        public static bool IsBeingDisplayed
        {
            get { return isBeingDisplayed; }
        }
        #endregion

        #region Fields
        private Rect windowPosition = new Rect(0, 0, 500f, 300f);
        private int windowID = Guid.NewGuid().GetHashCode();
        private GUIStyle headingStyle, infoStyle, buttonStyle;
        #endregion

        #region Initialisation
        private void Awake()
        {
            isBeingDisplayed = true;

            this.windowPosition.x = (Screen.width / 2) - (this.windowPosition.width / 2);
            this.windowPosition.y = (Screen.height / 2) - (this.windowPosition.height / 2);
        }
        private void Start()
        {
            this.headingStyle = new GUIStyle(HighLogic.Skin.label);
            this.headingStyle.fontStyle = FontStyle.Bold;
            this.headingStyle.stretchWidth = true;

            this.infoStyle = new GUIStyle(HighLogic.Skin.label);
            this.infoStyle.normal.textColor = Color.white;

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button);
            this.buttonStyle.normal.textColor = this.buttonStyle.hover.textColor = this.buttonStyle.active.textColor = Color.white;
        }
        #endregion

        #region Drawing
        private void OnGUI()
        {
            this.windowPosition = KSPUtil.ClampRectToScreen(GUILayout.Window(this.windowID, this.windowPosition, Window, "Resource Pack Selector", HighLogic.Skin.window));
        }

        private void Window(int windowID)
        {
            GUILayout.BeginHorizontal();

            // Call methods to draw the main sections.
            DrawSelectionList();
            DrawSelectedInfo();

            GUILayout.EndHorizontal();

            // Close button
            if (GUILayout.Button("Close Pack Selector", HighLogic.Skin.button))
            {
                Destroy(this);
            }

            GUI.DragWindow();
        }

        // Draws the selection list and allows pack selection.
        private void DrawSelectionList()
        {
            GUI.skin = HighLogic.Skin;
            GUILayout.BeginScrollView(Vector2.zero, false, true, new GUILayoutOption[] { });
            GUI.skin = null;

            foreach (string defaultName in DefaultLibrary.GetDefaultNames())
            {
                bool isSelected = (DefaultLibrary.GetSelectedDefault().Name == defaultName);

                if (GUILayout.Toggle(isSelected, defaultName, HighLogic.Skin.button) != isSelected && isSelected == false)
                {
                    DefaultLibrary.SetSelectedDefault(DefaultLibrary.GetDefault(defaultName));
                }
            }

            GUILayout.EndScrollView();
        }

        // Draws the information for the currently selected pack.
        private void DrawSelectedInfo()
        {
            GUILayout.BeginVertical(GUILayout.Width(200f));
            GUILayout.Label("Pack Name", this.headingStyle);
            GUILayout.Label(DefaultLibrary.GetSelectedDefault().Name, this.infoStyle);
            GUILayout.Space(15f);
            GUILayout.Label("Description", this.headingStyle);
            GUILayout.Label(DefaultLibrary.GetSelectedDefault().Description, this.infoStyle);
            GUILayout.EndVertical();
        }
        #endregion

        #region Destroying
        private void OnDestroy()
        {
            isBeingDisplayed = false;
        }
        #endregion
    }
}
