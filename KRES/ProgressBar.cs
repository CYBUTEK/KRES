using System;
using UnityEngine;

namespace KRES
{
    public class ProgressBar
    {
        #region Fields
        private Texture2D background = new Texture2D(1, 1);
        private Texture2D bar = new Texture2D(1, 1);
        private Rect bgPosition = new Rect();
        private Rect barPosition = new Rect();
        private double filled = 0d;
        #endregion

        #region Initiation
        /// <summary>
        /// Generates a new ProgressBar
        /// </summary>
        public ProgressBar() { }

        /// <summary>
        /// Generates a new progressbar with the specified parameters
        /// </summary>
        /// <param name="bgPosition">Size and position of the background of the bar</param>
        /// <param name="barPosition">Size and position of the progress bar</param>
        /// <param name="background">Texture of the background of the bar</param>
        /// <param name="bar">Texture for the progressbar</param>
        public ProgressBar(Rect bgPosition, Rect barPosition, Texture2D background, Texture2D bar)
        {
            this.bgPosition = bgPosition;
            this.barPosition = barPosition;
            this.background = background;
            this.bar = bar;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the value of the bar between 0 and 1
        /// </summary>
        /// <param name="value">Percentage of the bar filled</param>
        public void SetValue(double value)
        {
            if (value > 1d) { this.filled = 1d; return; }
            if (value < 0d) { this.filled = 0d; return; }
            this.filled = value;
        }

        /// <summary>
        /// Sets the value of the bar within a given range
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public void SetValue(double value, double min, double max)
        {
            if (value > max) { this.filled = max; return; }
            if (value < min) { this.filled = min; return; }
            if (min >= max) { this.filled = 1d; return; }
            this.filled = (value - min) / (max - min);
        }

        /// <summary>
        /// Changes the GUIStyle of the background box
        /// </summary>
        /// <param name="background">GUIStyle of the box</param>
        public void SetBackGround(Texture2D background)
        {
            this.background = background;
        }

        /// <summary>
        /// Changes the texture of the progress bar
        /// </summary>
        /// <param name="bar">Texture of the progress bar</param>
        public void SetBar(Texture2D bar)
        {
            this.bar = bar;
        }

        /// <summary>
        /// Changes the size and position of the bar
        /// </summary>
        /// <param name="bgPosition">Size and position of the background of the bar</param>
        /// <param name="barPosition">Size and position of the progress bar</param>
        public void SetPosition(Rect bgPosition, Rect barPosition)
        {
            this.bgPosition = bgPosition;
            this.barPosition = barPosition;
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Call this in OnGUI() to draw the ProgressBar
        /// </summary>
        public void Draw()
        {
            GUI.DrawTexture(bgPosition, background);
            GUI.DrawTexture(new Rect(barPosition.x, barPosition.y, (float)(barPosition.width * filled), barPosition.height), bar);
        }
        #endregion
    }
}
