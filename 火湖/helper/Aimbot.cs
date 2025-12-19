using PerfectRandom.Sulfur.Core.Items;
using PerfectRandom.Sulfur.Core;
using PerfectRandom.Sulfur.Core.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using PerfectRandom.Sulfur.Core.LevelGeneration;

namespace UnityUniversals
{
    public struct HeadPositionResult
    {
        public Vector3 HeadPos;
        public Npc Npc;
        public Player player;
    }

    public static class Aimbot
    {
        /// <summary>
        /// Implements a universal aimbot functionality for automatic target acquisition and aiming.
        /// </summary>
        /// <param name="targets">The targets to loop through.</param>
        /// <param name="findHeadPosition">The function check current target to get their head position.</param>
        /// <param name="localPlayerCamera">The camera associated with the local player.</param>
        /// <param name="mouseSmoothing">A factor to smooth mouse movement. Higher values result in slower, smoother aiming.</param>
        /// <param name="aimButton">A boolean indicating whether the aim button is pressed.</param>
        /// <remarks>
        /// This method performs the following actions:
        /// 1. Calculates the screen position of each target's head.
        /// 2. Determines the closest target within a specified field of view.
        /// 3. If a valid target is found and the aim button is pressed, it moves the mouse towards the target.
        /// 
        /// The method uses mouse_event to simulate mouse movement for aiming.
        /// It's wrapped in a try-catch block to handle any exceptions that may occur during execution.
        /// If an exception is caught, it is silently ignored.
        /// 
        /// </remarks>
        public static void UniversalAimbot<T>(IEnumerable<T> targets, Func<T, HeadPositionResult> findHeadPosition, Camera localPlayerCamera, double mouseSmoothing, bool aimButton)
        {
            try
            {
                float closestTargetDistance = float.MaxValue;
                Vector2 closestTargetScreenPosition = Vector2.zero;
                Npc npc = null;
                Player player = null;


                foreach (T target in targets)
                {
                    var headPosition = findHeadPosition(target);

                    Vector3 targetScreenPosition = localPlayerCamera.WorldToScreenPoint(headPosition.HeadPos);

                    if (targetScreenPosition.z > -8)
                    {
                        Vector2 centerScreenPosition = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
                        float distanceToCenter = Vector2.Distance(new Vector2(targetScreenPosition.x, Screen.height - targetScreenPosition.y), centerScreenPosition);

                        float fieldOfViewRadius = 300f;

                        if (distanceToCenter < fieldOfViewRadius && distanceToCenter < closestTargetDistance)
                        {
                            closestTargetDistance = distanceToCenter;
                            closestTargetScreenPosition = new Vector2(targetScreenPosition.x, Screen.height - targetScreenPosition.y);
                            npc = headPosition.Npc;
                            player = headPosition.player;
                        }
                    }
                }

                if (closestTargetScreenPosition != Vector2.zero)
                {
                    double distanceToMoveX = closestTargetScreenPosition.x - Screen.width / 2.0f;
                    double distanceToMoveY = closestTargetScreenPosition.y - Screen.height / 2.0f;

                    distanceToMoveX /= mouseSmoothing;
                    distanceToMoveY /= mouseSmoothing;

                    if (aimButton)
                    {
                        if (npc != null && npc.IsHostileTo(FactionIds.Player) && !npc.name.Contains("Trader") && !npc.name.Contains("Arthur") )
                        {
                            
                            //npc.Die();
                            foreach(var hitbox in npc.Hitboxes)
                            {
                                if (hitbox == null )
                                {
                                    continue;
                                }
                                if (player != null)
                                {
                                    hitbox.TakeHit(player.playerUnit.lastUsedWeapon.Damage, player.playerUnit.lastUsedWeapon.GetDamageType(), player.playerUnit, hitbox.transform.position);

                                    //StaticInstance<LootManager>.Instance.SpawnLootFrom(StaticInstance<LootManager>.Instance.LootSettings.valuablesLootTable, player.transform.position,npc.currentRoom);
                                    
                                }
                            }
                            //for (int i = 0; i < 100; i++)
                            //{
                            //    StaticInstance<LootManager>.Instance.SpawnGlobalLoot(player.transform.position, npc);
                            //}
                            Universal.mouse_event(0x0001, (int)distanceToMoveX, (int)distanceToMoveY, 0, 0);
                            
                        }
                       
                        
                    }
                }
            }
            catch { }
        }
    }
}