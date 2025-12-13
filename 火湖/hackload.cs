using injectDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace 火湖
{
    public class HackLoad
    {


        public static void Init()
        {
            HackLoad.Load = new GameObject();
            HackLoad.Load.AddComponent<Hack>();
            UnityEngine.Object.DontDestroyOnLoad(HackLoad.Load);
        }

        public static void Unload()
        {
            _Unload();
        }

        public static void _Unload()
        {
            GameObject.Destroy(Load);

        }

        public static void OnDestroy()
        {

        }
        private static GameObject Load;
    }
}
