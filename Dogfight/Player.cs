using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogfight
{
    internal class Player
    {
        private const float minimumAltitude = 350.0f;

        public Vector3 pos;
        public Vector3 dir;
        public Vector3 up;
        public Vector3 velocity;
        private Vector3 right;
        public Vector3 Right { get { return right; } }

        private const float rotationRate = 1.5f;
        private const float mass = 1.0f;
        private const float thrustForce = 24000.0f;
        private const float dragForce = 0.97f;

        private Matrix world;
        public Matrix World { get { return world; } }

        public void Reset()
        {
            pos = new Vector3(0, minimumAltitude, 0);
            dir = Vector3.Forward;
            up = Vector3.Up;
            right = Vector3.Right;
            velocity = Vector3.Zero;
        }

        public Player() { Reset(); }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //rotation
            Vector2 rotationAmount = new Vector2(0, 0);
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                rotationAmount.X = 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                rotationAmount.X = -1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                rotationAmount.Y = 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                rotationAmount.Y = -1.0f;
            }

            rotationAmount = rotationAmount * rotationRate * elapsed;

            if (up.Y < 0)
            {
                rotationAmount.X = -rotationAmount.X;
            }

            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(right, rotationAmount.Y) * Matrix.CreateRotationY(rotationAmount.X);
            dir = Vector3.TransformNormal(dir, rotationMatrix);
            up = Vector3.TransformNormal(up, rotationMatrix);

            dir.Normalize();
            up.Normalize();

            right = Vector3.Cross(dir, up);
            up = Vector3.Cross(right, dir);

            //ThrustAmount
            float thrustAmount = 0;
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                thrustAmount = 1.0f;
            }

            Vector3 force = dir * thrustAmount * thrustForce;

            Vector3 acceleration = force / mass;
            velocity += acceleration * elapsed;
            velocity *= dragForce;

            pos += velocity * elapsed;
            pos.Y = Math.Max(pos.Y, minimumAltitude);

            //world matrix
            world = Matrix.Identity;
            world.Forward = dir;
            world.Up = up;
            world.Right = right;
            world.Translation = pos;
        }
    }
}
