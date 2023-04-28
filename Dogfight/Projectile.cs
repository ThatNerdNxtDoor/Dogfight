using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dogfight
{
    internal class Projectile
    {
        float speedFactor;
        Vector3 pos;
        Vector3 dir;
        float velocity = 50f;
        const float altitudeBoundary = 100000f;

        public Projectile(Vector3 p, Vector3 d) {
            pos = p;
            dir = d;

        }

        public void Update(GameTime gameTime, Player player) {
            pos += dir * velocity;

            //Check if projectile hits player
            if (Vector3.Distance(pos, player.pos) <= 1000f) {
                
            }

            //Check if projectile has hit boundary
            if (pos.X >= altitudeBoundary || pos.X <= -altitudeBoundary)
            {

            }
            else if (pos.Y >= altitudeBoundary || pos.Y <= -altitudeBoundary)
            {

            }
            else if (pos.Z >= altitudeBoundary || pos.Z <= -altitudeBoundary) {
            
            }
        }
    }
}
