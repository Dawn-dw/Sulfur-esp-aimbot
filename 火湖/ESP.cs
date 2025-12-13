using PerfectRandom.Sulfur.Core.Units;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUniversals
{
    public static class ESP
    {
        /// <summary>
        /// Implements a universal ESP (Extra Sensory Perception) system for rendering visual indicators for target objects.
        /// </summary>
        /// <param name="targetCenterPosition">The center position of target.</param>
        /// <param name="localPlayerCamera">The camera associated with the local player, used for world-to-screen coordinate conversion.</param>
        /// <param name="drawBox">If true, draws a box around each target. Default is true.</param>
        /// <param name="drawLine">If true, draws a line from the screen center to each target. Default is true.</param>
        /// <param name="widthOffset">A factor to determine the width of the ESP box relative to its height. Default is 2f.</param>
        /// <remarks>
        /// This method performs the following actions for each valid target:
        /// 1. Determines the target's transform.
        /// 2. Calculates the foot and head positions of the target in world space.
        /// 3. Converts these positions to screen space.
        /// 4. If the target is in front of the camera, calls DrawBoxAndOrLineESP to render the ESP elements.
        /// 
        /// It's wrapped in a try-catch block to handle any exceptions that may occur during execution.
        /// If an exception is caught, it is silently ignored.
        /// 
        /// Note: This method relies on the DrawBoxAndOrLineESP method for rendering ESP elements.
        /// </remarks>
        public static void UniversalESP(Vector3 targetCenterPosition, Camera localPlayerCamera, bool drawBox = true, bool drawLine = true, float widthOffset = 2f)
        {
            try
            {
                //In-Game Position
                Vector3 pivotPosition = targetCenterPosition; //Pivot point NOT at the feet, at the center
                Vector3 playerFootPosition; playerFootPosition.x = pivotPosition.x; playerFootPosition.z = pivotPosition.z; playerFootPosition.y = pivotPosition.y - 2f; //At the feet
                Vector3 playerHeadPosition; playerHeadPosition.x = pivotPosition.x; playerHeadPosition.z = pivotPosition.z; playerHeadPosition.y = pivotPosition.y + 2f; //At the head

                //Screen Position
                Vector3 w2sFootPosition = localPlayerCamera.WorldToScreenPoint(playerFootPosition);
                Vector3 w2sHeadPosition = localPlayerCamera.WorldToScreenPoint(playerHeadPosition);

                if (w2sFootPosition.z > 0f)
                {
                    DrawBoxAndOrLineESP(w2sFootPosition, w2sHeadPosition, Color.red, drawBox, drawLine, widthOffset);
                }
            }
            catch { }
        }

        /// <summary>
        /// Draws an ESP (Extra Sensory Perception) box and/or line for a target in 3D space with customizable width.
        /// </summary>
        /// <param name="footPosition">The position of the target's feet in screen coordinates.</param>
        /// <param name="headPosition">The position of the target's head in screen coordinates.</param>
        /// <param name="color">The color to use for drawing the ESP elements.</param>
        /// <param name="drawBox">If true, draws a box around the target.</param>
        /// <param name="drawLine">If true, draws a line from the screen center to the target's feet.</param>
        /// <param name="widthOffset">A factor to determine the width of the ESP box relative to its height.</param>
        /// <remarks>
        /// This method provides visual ESP functionality:
        /// - Box ESP: Draws a rectangular box around the target if drawBox is true.
        /// - Line ESP: Draws a line from the screen center to the target's feet if drawLine is true.
        /// 
        /// The box dimensions are calculated based on the height of the target (distance between head and feet).
        /// The width of the box is determined by dividing the height by the widthOffset factor.
        /// 
        /// This method relies on a Render class for actual drawing operations.
        /// </remarks>
        public static void DrawBoxAndOrLineESP(Vector3 footPosition, Vector3 headPosition, Color color, bool drawBox, bool drawLine, float widthOffset)
        {
            float height = headPosition.y - footPosition.y;
            float width = height / widthOffset;

            if (drawBox)
            {
                Render.DrawBox(footPosition.x - (width / 2), Screen.height - footPosition.y - height, width, height, color, 2f);
            }
            if (drawLine)
            {
                Render.DrawLine(new Vector2((Screen.width / 2), (Screen.height / 2)), new Vector2(footPosition.x, Screen.height - footPosition.y), color, 2f);
            }
        }
    }
}