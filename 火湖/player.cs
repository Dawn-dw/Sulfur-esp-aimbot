using PerfectRandom.Sulfur.Core;
using PerfectRandom.Sulfur.Core.UI;
using PerfectRandom.Sulfur.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 火湖
{
    internal class LocalPlayer

    {
        Player player;


        public Player GetPlayer()
        {
            return player;
        }

        public void SetPlayer(Player player) {
            this.player = player;
        }

        
    }
}
