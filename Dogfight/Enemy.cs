using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogfight
{
    internal class Enemy
    {
        Vector3 pos;
        public Vector3 Pos { get; set; }
        Vector3 dir;
        public Vector3 Dir { get; set; }
        float speed;

        public Enemy(Vector3 p, Vector3 d, float s) {
            pos = p;
            dir = d;
            speed = s;
        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}
