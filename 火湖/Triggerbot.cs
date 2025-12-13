namespace UnityUniversals
{
    public static class Triggerbot
    {
        /// <summary>
        /// Implements a universal triggerbot functionality for target acquisition and automatic firing.
        /// </summary>
        /// <param name="isTargetInCrosshair">A bool to check if target is in crosshair. </param>
        /// <param name="clickDelay">The delay before, between and after clicks. 0.075f total with default parameter.</param>
        /// <remarks>
        /// This method performs the following actions:
        /// 1. Checks if the crosshair is over a valid target using the boolean value.
        /// 2. If a valid target is detected, it simulates a left mouse click using the LeftClick method.
        /// 
        /// The method is wrapped in a try-catch block to handle any exceptions that may occur during execution.
        /// If an exception is caught, it is silently ignored.
        /// 
        /// Note: This method relies on other methods in the Universal class, such as LeftClick.
        /// </remarks>
        public static void UniversalTriggerbot(bool isTargetInCrosshair, float clickDelay = 0.025f)
        {
            try
            {
                if (isTargetInCrosshair)
                {
                    Universal.LeftClick(clickDelay);
                }
            }
            catch { }
        }
    }
}