using PerfectRandom.Sulfur.Core;
using PerfectRandom.Sulfur.Core.DevTools;
using PerfectRandom.Sulfur.Core.Items;
using PerfectRandom.Sulfur.Core.LevelGeneration;
using PerfectRandom.Sulfur.Core.Units;
using PerfectRandom.Sulfur.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityUniversals;
using 火湖;
using static UnityEngine.GraphicsBuffer;

namespace injectDll
{
    internal class Hack : MonoBehaviour
    {
        static GameManager gameMgr = StaticInstance<GameManager>.Instance;
        LocalPlayer player = new LocalPlayer();
        List<Npc> npcs = new List<Npc>();
        static Texture2D redTex;
        Npc targetNpc;
        Texture2D aimCircleTex;
        float aimCircleRadius = 50;

        public void Start()
        {
            
        }
        public void Update()
        {
            this.npcs = gameMgr.aliveNpcs;
            this.player.SetPlayer(gameMgr.PlayerScript);

            Camera cam = gameMgr.currentCamera;
            double mouseSmoothing = 2.0;
            bool aimButton = Input.GetMouseButton(1);

            Aimbot.UniversalAimbot<Npc>(
                npcs,                       // IEnumerable<Npc>
                npc => FindNpcHead(npc),     // Func<Npc, Vector3>
                cam,                         // Camera
                mouseSmoothing,                         // mouseSmoothing
                Input.GetMouseButton(1)      // aimButton
            );

        }

        void OnGUI()
        {
      
            Camera cam = gameMgr.currentCamera;
            


            if (cam == null)
                return;
            
            if (targetNpc != null)
            {
                GUI.Label(new Rect(10, 10, 300, 20),
                    $"Target: {targetNpc.name}");
            }
            CreateAimCircleTexture((int)aimCircleRadius);
            if (aimCircleTex != null)
            {
                float size = aimCircleRadius * 2f;

                Rect r = new Rect(
                    Screen.width * 0.5f - aimCircleRadius,
                    Screen.height * 0.5f - aimCircleRadius,
                    size,
                    size
                );

                GUI.DrawTexture(r, aimCircleTex);
            }
            foreach (var npc in npcs)
            {

                float distance = Vector3.Distance(
                this.player.GetPlayer().transform.position,
                npc.transform.position
                );
                Draw(npc.transform.position,this.player,cam, $"NPC:{npc.name} Dis:{distance}",Color.white);
                
                foreach (var hitbox in npc.Hitboxes)
                {
                    if (npc.IsHostileTo(FactionIds.Player) && !npc.name.Contains("Trader") && !npc.name.Contains("Arthur"))
                    {
                        if (hitbox == null || !hitbox.LabelShort.Contains("Head") )
                        {
                            continue;
                        }
                        ESP.UniversalESP(hitbox.transform.position, cam, true, true, 2);
                        UpdateTargetNpcByMouse(cam, npc);
                    }
                    
                    
                    
                }

            }

            //Pickup[] allPickups = GameObject.FindObjectsOfType<Pickup>();
            //foreach (var pickup in allPickups)
            //{
            //    float distance = Vector3.Distance(
            //    this.player.GetPlayer().transform.position,
            //    pickup.transform.position
            //    );
            //    Draw(pickup.transform.position, this.player, cam, $"pickup:{pickup.itemText} Dis:{distance}", Color.yellow);
            //}


            var rooms = gameMgr.graphContext.orderedRooms;

            foreach (var room in rooms)
            {
                if (room == null)
                    return;
                foreach (var hiddenChests in room.hiddenChests)
                {
                    float distance = Vector3.Distance(
                    this.player.GetPlayer().transform.position,
                    hiddenChests.transform.position
                    );
                    Draw(hiddenChests.transform.position, this.player, cam, $"Chests:{hiddenChests.name} Dis:{distance}", Color.red);


                }

                foreach (var containers in room.containers)
                {
                    float distance = Vector3.Distance(
                    this.player.GetPlayer().transform.position,
                    containers.transform.position
                    );
                    Draw(containers.transform.position, this.player, cam, $"Containers:{containers.name} Dis:{distance}", Color.green);
                }

                //foreach (var connectors in room.connectors)
                //{
                //    float distance = Vector3.Distance(
                //    this.player.GetPlayer().transform.position,
                //    connectors.transform.position
                //    );
                //    Draw(connectors.transform.position, this.player, cam, $"connectors:{connectors.name} Dis:{distance}", Color.green);
                //}

                //foreach (var pickup in room.roomLOD.gameObjects)
                //{
                //    float distance = Vector3.Distance(
                //    this.player.GetPlayer().transform.position,
                //    pickup.transform.position
                //    );
                //    Draw(pickup.transform.position, this.player, cam, $"pickup:{pickup.name} Dis:{distance}", Color.yellow);
                //}

            }


        }

        static Vector3 WorldToScreenPoint(Vector3 worldPos, Transform cameraTrans, float fov, Rect renderRect)
        {
            Vector3 vector = Quaternion.Inverse(cameraTrans.rotation) * (worldPos - cameraTrans.position);
            float num = fov * 0.017453292f;
            float num2 = 1f / Mathf.Tan(num * 0.5f);
            int num3 = Mathf.RoundToInt((float)Screen.width * renderRect.width);
            int num4 = Mathf.RoundToInt((float)Screen.height * renderRect.height);
            float num5 = (float)num3 / (float)num4;
            Vector3 vector2 = new Vector3(vector.x * num2 / num5 / vector.z, vector.y * num2 / vector.z, vector.z);
            return new Vector3((vector2.x * 0.5f + 0.5f) * (float)num3, (vector2.y * 0.5f + 0.5f) * (float)num4, vector2.z);
        }

        static void Draw(Vector3 target,LocalPlayer localPlayer,Camera cam,string text,Color color)
        {
            Vector3 sp = cam.WorldToScreenPoint(target);

            if (sp.z <= 0f)
                return;

            float guiX = sp.x;
            float guiY = Screen.height - sp.y;

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = color;

            GUI.Label(
                new Rect(guiX, guiY, 400, 20),
               text,
               style
            );
        }
        void DrawHitboxGUI(Hitbox hb, Camera cam)
        {
            Bounds b = hb.GetBounds();
            Vector3[] corners = GetBoundsCorners(b);

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            bool visible = false;

            foreach (var p in corners)
            {
                Vector3 sp = WorldToScreenPoint(
                    p,
                    cam.transform,
                    cam.fieldOfView,
                    cam.rect
                );

                if (sp.z <= 0f)
                    continue;

                visible = true;

                minX = Mathf.Min(minX, sp.x);
                minY = Mathf.Min(minY, sp.y);
                maxX = Mathf.Max(maxX, sp.x);
                maxY = Mathf.Max(maxY, sp.y);
            }

            if (!visible)
                return;

            Rect r = new Rect(
                minX,
                Screen.height - maxY,
                maxX - minX,
                maxY - minY
            );

            // === 样式（保持你原来的逻辑，不做优化） ===
            var yellowBoxStyle = new GUIStyle(GUI.skin.box);
            yellowBoxStyle.normal.textColor = Color.black;

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.yellow);
            tex.Apply();

            yellowBoxStyle.normal.background = tex;

            GUI.Box(r, hb.LabelShort, yellowBoxStyle);
        }


        static Vector3[] GetBoundsCorners(Bounds b)
        {
            Vector3 c = b.center;
            Vector3 e = b.extents;

            return new Vector3[]
            {
        c + new Vector3(-e.x, -e.y, -e.z),
        c + new Vector3( e.x, -e.y, -e.z),
        c + new Vector3( e.x,  e.y, -e.z),
        c + new Vector3(-e.x,  e.y, -e.z),

        c + new Vector3(-e.x, -e.y,  e.z),
        c + new Vector3( e.x, -e.y,  e.z),
        c + new Vector3( e.x,  e.y,  e.z),
        c + new Vector3(-e.x,  e.y,  e.z),
            };
        }
        void DrawGuiWorldLine(
            Vector3 worldFrom,
            Vector3 worldTo,
            Camera cam,
            float thickness = 2f
        )
        {
            Vector3 sp0 = cam.WorldToScreenPoint(worldFrom);
            Vector3 sp1 = cam.WorldToScreenPoint(worldTo);

            // 任意一个在相机后面就不画
            if (sp0.z <= 0f || sp1.z <= 0f)
                return;

            // GUI 坐标系 Y 翻转
            sp0.y = Screen.height - sp0.y;
            sp1.y = Screen.height - sp1.y;

            Vector2 p0 = sp0;
            Vector2 p1 = sp1;

            Vector2 dir = p1 - p0;
            float length = dir.magnitude;
            if (length < 1f)
                return;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Rect rect = new Rect(
                p0.x,
                p0.y - thickness * 0.5f,
                length,
                thickness
            );

            Matrix4x4 oldMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, p0);
            GUI.DrawTexture(rect, redTex);
            GUI.matrix = oldMatrix;
        }

        void EnsureRedTexture()
        {
            if (redTex != null)
                return;

            redTex = new Texture2D(1, 1);
            redTex.SetPixel(0, 0, Color.red);
            redTex.Apply();
        }

        HeadPositionResult FindNpcHead(Npc npc)
        {
            var result = new HeadPositionResult();
            result.Npc = npc;
            result.HeadPos = npc.transform.position;
            foreach (var hb in npc.Hitboxes)
            {
                
                
                if (hb.LabelShort == "Head" || hb.Label == "Head")
                {
                    result.HeadPos = hb.GetBounds().center;
                    return result;
                }
                
               
            }

            // 兜底：没有头就用身体中心
            return result;
        }


        void UpdateTargetNpcByMouse(Camera cam, Npc npc)
        {
            float aimRadius = aimCircleRadius; // 你的锁定圆半径
            Vector2 screenCenter = new Vector2(
                Screen.width * 0.5f,
                Screen.height * 0.5f
            );

            // 1. 取 NPC 头部世界坐标
            Vector3 worldPos = GetNpcHeadWorldPos(npc);

            // 2. 世界 → 屏幕
            Vector3 sp = WorldToScreenPoint(
                worldPos,
                cam.transform,
                cam.fieldOfView,
                cam.rect
            );

            // 在相机后面，直接忽略
            if (sp.z <= 0f)
                return;

            // 3. 转为 GUI 坐标
            Vector2 npcScreenPos = new Vector2(
                sp.x,
                Screen.height - sp.y
            );

            // 4. 到屏幕中心的距离
            float distToCenter = Vector2.Distance(npcScreenPos, screenCenter);

            // 5. 必须在圆内
            if (distToCenter > aimRadius)
                return;

            // 6. 赋值目标
            targetNpc = npc;
        }


        Vector3 GetNpcHeadWorldPos(Npc npc)
        {
            foreach (var hb in npc.Hitboxes)
            {
                if (hb.LabelShort == "Head" || hb.Label == "Head")
                {
                    return hb.GetBounds().center;
                }
            }

            // 兜底
            return npc.transform.position;
        }



        void CreateAimCircleTexture(int radius)
        {
            int size = radius * 2;
            aimCircleTex = new Texture2D(size, size, TextureFormat.ARGB32, false);

            // 完全透明
            Color clear = new Color(0f, 0f, 0f, 0f);

            // 绿色描边
            Color green = Color.green;

            Vector2 center = new Vector2(radius, radius);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);

                    // 线宽约 2~3 像素的绿色圆环
                    if (Mathf.Abs(dist - radius) <= 1.5f)
                        aimCircleTex.SetPixel(x, y, green);
                    else
                        aimCircleTex.SetPixel(x, y, clear);
                }
            }

            aimCircleTex.Apply();
        }

    }
}
