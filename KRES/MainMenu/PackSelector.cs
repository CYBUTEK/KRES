using System;
using UnityEngine;

namespace KRES.MainMenu
{
    public class PackSelector : MonoBehaviour
    {
        private static bool isBeingDisplayed = false;
        public static bool IsBeingDisplayed
        {
            get { return isBeingDisplayed; }
        }

        private Rect windowPosition = new Rect(0, 0, 600f, 300f);
        private int windowID = Guid.NewGuid().GetHashCode();

        private void Awake()
        {
            isBeingDisplayed = true;

            this.windowPosition.x = (Screen.width / 2) - (this.windowPosition.width / 2);
            this.windowPosition.y = (Screen.height / 2) - (this.windowPosition.height / 2);
        }

        private void OnGUI()
        {
            this.windowPosition = KSPUtil.ClampRectToScreen(GUILayout.Window(this.windowID, this.windowPosition, Window, "Resource Pack Selector", HighLogic.Skin.window));
        }

        private void Window(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUI.skin = HighLogic.Skin;
            GUILayout.BeginScrollView(Vector2.zero, false, true, new GUILayoutOption[] { });
            GUI.skin = null;

            for (int i = 0; i < 3; i++)
            {
                GUILayout.Button("Pack " + (i + 1), HighLogic.Skin.button);
            }
            GUILayout.EndScrollView();
            GUILayout.BeginVertical(GUILayout.Width(200f));
            GUILayout.Label("Pack Name: Default");
            GUILayout.Label("Description: Some description");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Button("Use Selected Pack", HighLogic.Skin.button, GUILayout.Width(150f));
            if (GUILayout.Button("Cancel", HighLogic.Skin.button, GUILayout.Width(150f)))
                Destroy(this);
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void OnDestroy()
        {
            isBeingDisplayed = false;
        }
    }
}
