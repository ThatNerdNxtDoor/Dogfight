using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Dogfight
{
    internal class Player
    {
        private const float minimumAltitude = 500.0f;

        public Vector3 pos;
        public Vector3 dir;
        public Vector3 up;
        public Vector3 velocity;
        private Vector3 right;
        public Vector3 Right { get { return right; } }

        MouseState mState;
        Vector2 mousePos;

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

        public void Update(GameTime gameTime, List<Enemy> enemyList, Viewport view2D, Matrix proj, Matrix view, Matrix globalWorld)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //rotation
            Vector3 rotationAmount = new Vector3(0, 0, 0);
            if (keyboardState.IsKeyDown(Keys.A))
            {
                rotationAmount.Z = 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                rotationAmount.Z = -1.0f;
            }

            //Todo: Add rotation with mouse
            mState = Mouse.GetState();
            mousePos = new Vector2(mState.X, mState.Y);
            float distanceFromCenter = Vector2.Distance(new Vector2(200, 200), mousePos);
            if (distanceFromCenter < 50) //Firing Range
            {
                foreach(Enemy enemy in enemyList)
                {
                    Vector2 enemyPos2D = new Vector2(view2D.Project(enemy.Pos, proj, view, globalWorld).X, view2D.Project(enemy.Pos, proj, view, globalWorld).Y);
                    if (Vector2.Distance(enemyPos2D, mousePos) <= 20) {
                        enemy.Die();
                    }
                }
            }
            else { //Rotating 
                rotationAmount.X = mState.X / 20;
                rotationAmount.Y = mState.Y / 20;
            }

            rotationAmount = rotationAmount * rotationRate * elapsed;

            if (up.Y < 0)
            {
                rotationAmount.X = -rotationAmount.X;
            }

            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(right, rotationAmount.Y) * Matrix.CreateRotationY(rotationAmount.X) * Matrix.CreateRotationZ(rotationAmount.Z);
            dir = Vector3.TransformNormal(dir, rotationMatrix);
            up = Vector3.TransformNormal(up, rotationMatrix);

            dir.Normalize();
            up.Normalize();

            right = Vector3.Cross(dir, up);
            up = Vector3.Cross(right, dir);

            //ThrustAmount
            float thrustAmount = 0;
            if (keyboardState.IsKeyDown(Keys.W))
            {
                thrustAmount += 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.S)) {
                thrustAmount -= 1.0f;
            }

            Vector3 force = dir * thrustAmount * thrustForce;

            Vector3 acceleration = force / mass;
            velocity += acceleration * elapsed;
            velocity *= dragForce;

            pos += velocity * elapsed;
            pos.Y = Math.Max(pos.Y, -minimumAltitude);
            pos.Y = Math.Min(pos.Y, minimumAltitude);
            pos.X = Math.Max(pos.X, -minimumAltitude);
            pos.X = Math.Min(pos.X, minimumAltitude);
            pos.Z = Math.Max(pos.Z, -minimumAltitude);
            pos.Z = Math.Min(pos.Z, minimumAltitude);

            //world matrix
            world = Matrix.Identity;
            world.Forward = dir;
            world.Up = up;
            world.Right = right;
            world.Translation = pos;
        }
    }
}
