using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityUniversals
{
    public static class Universal
    {
        /// <summary>
        /// Synthesizes mouse motion and button clicks.
        /// </summary>
        /// <param name="dwFlags">A set of bit flags that specify various aspects of mouse motion and button clicking.</param>
        /// <param name="dx">The mouse's absolute position along the x-axis or its amount of motion since the last mouse event.</param>
        /// <param name="dy">The mouse's absolute position along the y-axis or its amount of motion since the last mouse event.</param>
        /// <param name="dwData">If dwFlags contains MOUSEEVENTF_WHEEL, then dwData specifies the amount of wheel movement. Otherwise, dwData should be zero.</param>
        /// <param name="dwExtraInfo">An additional value associated with the mouse event.</param>
        /// <remarks>
        /// This is a platform invoke (P/Invoke) declaration for the Windows API function mouse_event.
        /// It allows direct calls to this low-level Windows function from managed code.
        /// 
        /// Common dwFlags values:
        /// - 0x0001: MOUSEEVENTF_MOVE
        /// - 0x0002: MOUSEEVENTF_LEFTDOWN
        /// - 0x0004: MOUSEEVENTF_LEFTUP
        /// - 0x0008: MOUSEEVENTF_RIGHTDOWN
        /// - 0x0010: MOUSEEVENTF_RIGHTUP
        /// - 0x0020: MOUSEEVENTF_MIDDLEDOWN
        /// - 0x0040: MOUSEEVENTF_MIDDLEUP
        /// - 0x0800: MOUSEEVENTF_WHEEL
        /// 
        /// Note: This function is specific to Windows and will not work on other platforms.
        /// </remarks>
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        /// <summary>
        /// Creates a coroutine that waits for a specified amount of real time.
        /// </summary>
        /// <param name="waitTime">The time to wait, in seconds.</param>
        /// <returns>An IEnumerator that can be used in a coroutine to pause execution for the specified time.</returns>
        /// <remarks>
        /// This method uses WaitForSecondsRealtime, which is not affected by Time.timeScale.
        /// It's useful for creating delays that are consistent regardless of the game's time scale.
        /// </remarks>
        public static IEnumerator Wait(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
        }

        /// <summary>
        /// Simulates a left mouse button click with a small delay between press and release.
        /// </summary>
        /// <remarks>
        /// This method performs the following actions:
        /// 1. Waits for a short duration (25ms)
        /// 2. Simulates a left mouse button press
        /// 3. Waits for another short duration (25ms)
        /// 4. Simulates a left mouse button release
        /// 5. Waits for a final short duration (25ms)
        /// 
        /// Note: This method uses the Windows API mouse_event function and may not work on all platforms.
        /// It's designed for use in Windows environments only.
        /// </remarks>
        public static void LeftClick(float delay)
        {
            Wait(delay);
            mouse_event(0x0002, 0, 0, 0, 0);
            Wait(delay);
            mouse_event(0x0004, 0, 0, 0, 0);
            Wait(delay);
        }

        /// <summary>
        /// Determines if the mouse cursor is over a valid target player's specified body parts.
        /// </summary>
        /// <typeparam name="T">The type of the player object. Must be a class.</typeparam>
        /// <param name="target">A target player.</param>
        /// <param name="localPlayer">The local player object to exclude from targeting.</param>
        /// <param name="localPlayerCamera">The camera associated with the local player.</param>
        /// <param name="targetTransformNames">An array of transform names representing valid target body parts.</param>
        /// <returns>
        /// True if the mouse is over a valid target player's specified body part; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method casts a ray from the mouse position through the scene and checks if it hits
        /// any of the specified body parts of target player. It excludes the local player from being targeted.
        /// </remarks>
        public static bool IsInCross<T>(T target, Camera localPlayerCamera, string[] targetTransformNames)
        {
            Ray ray = localPlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, float.MaxValue))
            {
                foreach (string targetTransformName in targetTransformNames)
                {
                    if (raycastHit.transform.name == targetTransformName)
                    {
                        // Find the player associated with this body part
                        var hitPlayer = FindPlayerByBodyPart(target, raycastHit.transform);

                        if (hitPlayer != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Finds and returns the player object from a collection of targets that contains the specified body part.
        /// </summary>
        /// <typeparam name="T">The type of the target objects. Must be a class and either a Component or MonoBehaviour.</typeparam>
        /// <param name="target">A target object to search through.</param>
        /// <param name="bodyPart">The Transform of the body part to find the corresponding player for.</param>
        /// <returns>The player object of type T that contains the specified body part, or null if not found.</returns>
        /// <remarks>
        /// This method checks the provided target, checking if the given body part is a child of the target transform.
        /// It works with objects that are either Components or MonoBehaviours.
        /// </remarks>
        public static T FindPlayerByBodyPart<T>(T target, Transform bodyPart)
        {
            Transform targetTransform;

            if (target is Component component)
            {
                targetTransform = component.transform;
            }
            else if (target is MonoBehaviour monoBehaviour)
            {
                targetTransform = monoBehaviour.transform;
            }
            else
            {
                targetTransform = null;
            }

            // Check if the body part belongs to this player
            if (bodyPart.IsChildOf(targetTransform))
            {
                return target;
            }

            return target;
        }

        /// <summary>
        /// Finds the position of a specified head bone for a given target object.
        /// </summary>
        /// <typeparam name="T">The type of the target object. Must be a class and either a Component or MonoBehaviour.</typeparam>
        /// <param name="target">The target object to search for the head bone.</param>
        /// <param name="headBoneName">The name of the head bone to find.</param>
        /// <returns>
        /// The Vector3 position of the head bone if found; otherwise, Vector3.zero.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Determines the root transform of the target object.
        /// 2. Recursively searches for a child transform matching the specified head bone name.
        /// 3. If found, returns the world position of the head bone transform.
        /// 4. If not found, returns Vector3.zero.
        /// 
        /// The method works with objects that are either Components or MonoBehaviours.
        /// If the target is neither, Vector3.zero is returned.
        /// 
        /// Note: This method relies on the FindChildTransform method for the recursive search.
        /// </remarks>
        public static Vector3 FindHeadBonePosition<T>(T target, string headBoneName)
        {

            Transform targetTransform;

            if (target is Component component)
            {
                targetTransform = component.transform;
            }
            else if (target is MonoBehaviour monoBehaviour)
            {
                targetTransform = monoBehaviour.transform;
            }
            else
            {
                return Vector3.zero;
            }

            // Attempt to find the head bone transform recursively
            Transform headTransform = FindChildTransform(targetTransform, headBoneName);

            if (headTransform != null)
            {
                // Debug.Log($"Found Head Bone: {headTransform.name}");
                return headTransform.position; // Return the position of the found transform
            }
            else
            {
                // Debug.LogWarning($"Head bone '{headBoneName}' not found in target.");
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Recursively searches for a child transform with a specified name within a given parent transform's hierarchy.
        /// </summary>
        /// <param name="parent">The parent transform to start the search from.</param>
        /// <param name="childName">The name of the child transform to find.</param>
        /// <returns>
        /// The Transform of the child if found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method performs a depth-first search through the transform hierarchy:
        /// 1. It checks all immediate children of the parent transform.
        /// 2. If a match is found, it returns that transform immediately.
        /// 3. If not found among immediate children, it recursively searches each child's hierarchy.
        /// 4. The search continues until the transform is found or all children have been checked.
        /// 5. If the transform is not found in the entire hierarchy, the method returns null.
        /// 
        /// This method is useful for finding deeply nested child objects in complex transform hierarchies.
        /// </remarks>
        public static Transform FindChildTransform(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    // Return the transform if it matches the name
                    return child; 
                }

                // Recursively search in this child's children
                Transform found = FindChildTransform(child, childName);
                if (found != null)
                {
                    // Return if found in deeper hierarchy
                    return found;
                }
            }

            // Return null if not found at all levels
            return null;
        }

        /// <summary>
        /// Retrieves a list of all Transform names associated with the given target object.
        /// </summary>
        /// <typeparam name="T">The type of the target object. Must be a class and either a Component or MonoBehaviour.</typeparam>
        /// <param name="target">The target object to gather Transform names from.</param>
        /// <returns>A List of strings containing all Transform names associated with the target object.</returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Checks if the target is null and returns an empty list if so.
        /// 2. Determines the root Transform of the target object.
        /// 3. Recursively gathers all Transform names starting from the root.
        /// 4. Returns the list of gathered Transform names.
        /// 
        /// If the target is neither a Component nor a MonoBehaviour, an empty list is returned.
        /// </remarks>
        public static List<string> GetAllTransformNames<T>(T target)
        {
            List<string> transformNames = new List<string>();

            if (target == null)
            {
                // Return an empty list if the target is null
                return transformNames;
            }

            Transform targetTransform;

            if (target is Component component)
            {
                targetTransform = component.transform;
            }
            else if (target is MonoBehaviour monoBehaviour)
            {
                targetTransform = monoBehaviour.transform;
            }
            else
            {
                return transformNames;
            }

            // Get the root transform of the target
            Transform rootTransform = targetTransform;

            // Call the recursive method to gather all names
            GatherTransformNames(rootTransform, transformNames);

            return transformNames;
        }

        /// <summary>
        /// Recursively gathers the names of a Transform and all its children.
        /// </summary>
        /// <param name="currentTransform">The starting Transform to gather names from.</param>
        /// <param name="transformNames">A List to store the gathered Transform names.</param>
        /// <remarks>
        /// This method performs a depth-first traversal of the Transform hierarchy,
        /// adding the name of each Transform encountered to the provided list.
        /// The list is modified in-place, and the order of names will reflect
        /// the depth-first traversal order.
        /// </remarks>
        public static void GatherTransformNames(Transform currentTransform, List<string> transformNames)
        {
            // Add the name of the current transform to the list
            transformNames.Add(currentTransform.name);

            // Recursively call this method for each child transform
            foreach (Transform child in currentTransform)
            {
                GatherTransformNames(child, transformNames);
            }
        }
    }
}