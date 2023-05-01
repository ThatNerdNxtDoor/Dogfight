using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Dogfight
{
    internal class Player
    {
        /// <summary>
        /// The limit on how much the player can move away from it's starting position
        /// </summary>
        private const float altitudeBoundary = 100000.0f;

        /// <summary>
        /// The current position of the player's ship
        /// </summary>
        public Vector3 pos;

        /// <summary>
        /// The current direction the player's ship should face in
        /// </summary>
        public Vector3 dir;

        /// <summary>
        /// The current 'up' vector for the ship
        /// </summary>
        public Vector3 up;

        /// <summary>
        /// The current velocity of the ship
        /// </summary>
        public Vector3 velocity;

        /// <summary>
        /// The direction to the right of the player's ship
        /// </summary>
        private Vector3 right;
        public Vector3 Right { get { return right; } }

        /// <summary>
        /// The mouse state
        /// </summary>
        MouseState mState;

        /// <summary>
        /// The mouse's position
        /// </summary>
        public Vector2 mousePos;

        /// <summary>
        /// The position the portion of the crosshair that is drawn within the circle is located
        /// </summary>
        public Vector2 mousePosInCircle;

        /// <summary>
        /// How quickly the ship rotates
        /// </summary>
        private const float rotationRate = 1.5f;
        
        /// <summary>
        /// The mass of the ship
        /// </summary>
        private const float mass = 1.0f;
        
        /// <summary>
        /// The force of moving the ship forward
        /// </summary>
        private const float thrustForce = 12000.0f;
        
        /// <summary>
        /// The drag force factor to simulate drag so that the ship can slow down.
        /// </summary>
        private const float dragForce = 0.97f;

        /// <summary>
        /// The world matrix of the ship
        /// </summary>
        private Matrix world;
        /// <summary>
        /// The world matrix of the ship
        /// </summary>
        public Matrix World { get { return world; } }

        /// <summary>
        /// Resets the ship's values to defaults
        /// </summary>
        public void Reset()
        {
            pos = new Vector3(0, 0, 0);
            dir = Vector3.Forward;
            up = Vector3.Up;
            right = Vector3.Right;
            velocity = Vector3.Zero;
        }

        public Player() { Reset(); }

        /// <summary>
        /// Calculate the position to draw the portion of the crosshair that stays inside the circle
        /// </summary>
        /// <param name="center">The center of the circle</param>
        /// <param name="mousePos">The mouse's position</param>
        /// <param name="radius">The radius of the circle</param>
        private void _calculateMousePosInCircle(Vector2 center, Vector2 mousePos, float radius)
        {
            this.mousePosInCircle = Vector2.Normalize(mousePos - center) * radius + center;
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphicsDevice, List<Enemy> enemyList, Viewport view2D, Matrix proj, Matrix view, Matrix globalWorld)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //rotation
            Vector3 rotationAmount = new Vector3(0, 0, 0);
            if (keyboardState.IsKeyDown(Keys.A))
            {
                rotationAmount.Z = -1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                rotationAmount.Z = 1.0f;
            }

            //Todo: Add rotation with mouse
            mState = Mouse.GetState();
            mousePos = new Vector2(mState.X, mState.Y);
            Vector2 center = new Vector2(graphicsDevice.PreferredBackBufferWidth / 2, graphicsDevice.PreferredBackBufferHeight / 2);
            float distanceFromCenter = Vector2.Distance(center, mousePos);
            if (distanceFromCenter < 150) //Firing Range
            {
                mousePosInCircle = mousePos;
                for (int i = 0; i < enemyList.Count; i++)
                {
                    Enemy enemy = enemyList[i];
                    float distanceToPlayer = Vector3.Distance(pos, enemy.Pos);
                    Vector2 enemyPos2D = new Vector2(view2D.Project(enemy.Pos, proj, view, globalWorld).X, view2D.Project(enemy.Pos, proj, view, globalWorld).Y);
                    float distanceToCursor = Vector2.Distance(enemyPos2D, mousePos);
                    if (distanceToCursor <= 10000/(distanceToPlayer/10)) {
                        enemyList.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
            else if (new Rectangle(0, 0, graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight).Contains(mState.Position)){ //Rotating 
                _calculateMousePosInCircle(center, mousePos, 150);
                rotationAmount.X = -(mState.X - center.X) / 100;
                rotationAmount.Y = -(mState.Y - center.Y) / 100;
            }

            rotationAmount = rotationAmount * rotationRate * elapsed;

            //Matrix rotationMatrix = Matrix.CreateFromAxisAngle(right, rotationAmount.Y) * Matrix.CreateRotationY(rotationAmount.X) * Matrix.CreateRotationZ(rotationAmount.Z);
            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(right, rotationAmount.Y) * Matrix.CreateFromAxisAngle(up, rotationAmount.X) * Matrix.CreateFromAxisAngle(dir, rotationAmount.Z);
            dir = Vector3.TransformNormal(dir, rotationMatrix);
            up = Vector3.TransformNormal(up, rotationMatrix);

            dir.Normalize();
            up.Normalize();

            right = Vector3.Cross(dir, up);
            //up = Vector3.Cross(right, dir);

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
            pos.Y = Math.Max(pos.Y, -altitudeBoundary);
            pos.Y = Math.Min(pos.Y, altitudeBoundary);
            pos.X = Math.Max(pos.X, -altitudeBoundary);
            pos.X = Math.Min(pos.X, altitudeBoundary);
            pos.Z = Math.Max(pos.Z, -altitudeBoundary);
            pos.Z = Math.Min(pos.Z, altitudeBoundary);

            //world matrix
            world = Matrix.Identity;
            world.Forward = dir;
            world.Up = up;
            world.Right = right;
            world *= Matrix.CreateScale(500f);
            world.Translation = pos;
        }
    }
}
