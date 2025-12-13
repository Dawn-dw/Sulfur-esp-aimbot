using PerfectRandom.Sulfur.Core;
using PerfectRandom.Sulfur.Core.DevTools;
using PerfectRandom.Sulfur.Core.Items;
using PerfectRandom.Sulfur.Core.Units;
using PerfectRandom.Sulfur.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using 火湖;
using static UnityEngine.GraphicsBuffer;

namespace injectDll
{
    internal class Hack : MonoBehaviour
    {
        static GameManager gameMgr = StaticInstance<GameManager>.Instance;
        LocalPlayer player = new LocalPlayer();
        List<Npc> npcs = new List<Npc>();
        public void Start()
        {
           
        }
        public void Update()
        {
            this.npcs = gameMgr.aliveNpcs;
            this.player.SetPlayer(gameMgr.PlayerScript);
        }


        void OnGUI()
        {
      
            Camera cam = gameMgr.currentCamera;
            


            if (cam == null)
                return;

            foreach (var npc in npcs)
            {
                float distance = Vector3.Distance(
                this.player.GetPlayer().transform.position,
                npc.transform.position
                );
                Draw(npc.transform.position,this.player,cam, $"NPC:{npc.name} Dis:{distance}");
            }
            
            foreach (var hiddenChests in gameMgr.CurrentRoom.hiddenChests)
            {
                float distance = Vector3.Distance(
                this.player.GetPlayer().transform.position,
                hiddenChests.transform.position
                );
                Draw(hiddenChests.transform.position, this.player, cam, $"Chests:{hiddenChests.name} Dis:{distance}");
            }

            foreach(var containers  in gameMgr.CurrentRoom.containers)
            {
                float distance = Vector3.Distance(
                this.player.GetPlayer().transform.position,
                containers.transform.position
                );
                Draw(containers.transform.position, this.player, cam, $"Containers:{containers.name} Dis:{distance}");
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

        static void Draw(Vector3 target,LocalPlayer localPlayer,Camera cam,string text)
        {
            Vector3 sp = cam.WorldToScreenPoint(target);

            if (sp.z <= 0f)
                return;

            float guiX = sp.x;
            float guiY = Screen.height - sp.y;

            

            GUI.Label(
                new Rect(guiX, guiY, 400, 20),
               text
            );
        }

    }
}
