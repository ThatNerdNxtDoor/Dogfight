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
        public Vector3 pos;
        Vector3 dir;
        float velocity = 50f;
        const float altitudeBoundary = 100000f;

        public Projectile(Vector3 p, Vector3 d) {
            pos = p;
            dir = d;

        }

        public void Update(GameTime gameTime) {
            pos += dir * velocity;
        }
    }
}
