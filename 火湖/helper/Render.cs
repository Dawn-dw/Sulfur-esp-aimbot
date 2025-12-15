using UnityEngine;

namespace UnityUniversals
{
    public class Render : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the GUIStyle used for rendering strings in GUI methods.
        /// </summary>
        /// <remarks>
        /// This style determines the appearance of text drawn using methods like DrawString.
        /// It's initialized with the default GUI skin's label style but can be customized.
        /// Modifying this style will affect all text drawn using methods that rely on it.
        /// </remarks>
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

        /// <summary>
        /// A shared Texture2D used for drawing lines and potentially other primitive shapes.
        /// </summary>
        /// <remarks>
        /// This texture is typically initialized as a 1x1 white texture and used in methods like DrawLine.
        /// It should be initialized before use, usually in a method like DrawLine if it's null.
        /// </remarks>
        public static Texture2D lineTex;

        /// <summary>
        /// Gets or sets the current GUI color used for rendering GUI elements.
        /// </summary>
        /// <remarks>
        /// This property is a wrapper around GUI.color, providing an easier way to access and modify the current GUI color.
        /// Changes to this color will affect subsequent GUI drawing operations until it's changed again.
        /// It's commonly used to tint GUI elements like lines, shapes, and text.
        /// </remarks>
        public static Color Color
        {
            get { return GUI.color; }
            set { GUI.color = value; }
        }

        /// <summary>
        /// Draws a string on the GUI at the specified position.
        /// </summary>
        /// <param name="position">The position to draw the string. If centered, this will be the center of the string.</param>
        /// <param name="label">The text to be drawn.</param>
        /// <param name="centered">If true, the text will be centered on the position. If false, the position will be the upper-left corner of the text. Default is true.</param>
        /// <remarks>
        /// This method uses Unity's GUI system to draw text:
        /// 1. It creates a GUIContent with the provided label.
        /// 2. Calculates the size of the text using a predefined StringStyle.
        /// 3. Determines the upper-left position of the text based on whether it should be centered.
        /// 4. Draws the text using GUI.Label.
        /// 
        /// Note: 
        /// - This method should be called from OnGUI() or a similar GUI event function.
        /// - It uses Unity-specific classes like GUI, GUIContent, and assumes the existence of a StringStyle (likely a GUIStyle) for text formatting.
        /// - The StringStyle determines the font, size, color, and other text properties. Ensure it's properly defined elsewhere in your code.
        /// </remarks>
        public static void DrawString(Vector2 position, string label, bool centered = true)
        {
            var content = new GUIContent(label);
            var size = StringStyle.CalcSize(content);
            var upperLeft = centered ? position - size / 2f : position;
            GUI.Label(new Rect(upperLeft, size), content);
        }


        /// <summary>
        /// Draws a line between two points on the GUI.
        /// </summary>
        /// <param name="pointA">The starting point of the line.</param>
        /// <param name="pointB">The ending point of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="width">The width (thickness) of the line.</param>
        /// <remarks>
        /// This method uses Unity's GUI system to draw a line:
        /// 1. It saves the current GUI matrix and color.
        /// 2. Creates a 1x1 white texture if it doesn't exist.
        /// 3. Calculates the angle between the line and the horizontal axis.
        /// 4. Scales and rotates the GUI around the starting point to position the line correctly.
        /// 5. Draws the line using GUI.DrawTexture.
        /// 6. Restores the original GUI matrix and color.
        /// 
        /// Note: 
        /// - This method should be called from OnGUI() or a similar GUI event function.
        /// - It uses Unity-specific classes like GUI, GUIUtility, and Texture2D.
        /// - The line is drawn using a stretched 1x1 texture, which may not be ideal for all scenarios.
        /// </remarks>
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Matrix4x4 matrix = GUI.matrix;
            if (!lineTex)
                lineTex = new Texture2D(1, 1);

            Color color2 = GUI.color;
            GUI.color = color;
            float num = Vector3.Angle(pointB - pointA, Vector2.right);

            if (pointA.y > pointB.y)
                num = -num;

            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);
            GUI.matrix = matrix;
            GUI.color = color2;
        }

        /// <summary>
        /// Draws a rectangular box on the screen using line primitives.
        /// </summary>
        /// <param name="x">The x-coordinate of the top-left corner of the box.</param>
        /// <param name="y">The y-coordinate of the top-left corner of the box.</param>
        /// <param name="w">The width of the box.</param>
        /// <param name="h">The height of the box.</param>
        /// <param name="color">The color of the box lines.</param>
        /// <param name="thickness">The thickness of the box lines.</param>
        /// <remarks>
        /// This method draws a box by rendering four lines:
        /// 1. Top horizontal line
        /// 2. Left vertical line
        /// 3. Right vertical line
        /// 4. Bottom horizontal line
        /// 
        /// It uses the DrawLine method (not shown here) to render each side of the box.
        /// The box is drawn in screen space coordinates, where (0,0) is typically the top-left corner of the screen.
        /// 
        /// Note: This method assumes the existence of a DrawLine method that can render a line between two Vector2 points.
        /// Ensure that the DrawLine method is properly implemented and available in the same context.
        /// </remarks>
        public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
        {
            DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
            DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
            DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
            DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
        }
    }
}