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
        private const float altitudeBoundary = 100000.0f;

        /// <summary>
        /// The current 'up' vector for the ship
        /// </summary>
        public Vector3 up;

        /// <summary>
        /// The direction to the right of the enemy's ship
        /// </summary>
        public Vector3 right;

        /// <summary>
        /// How quickly the ship rotates
        /// </summary>
        private const float rotationRate = 1.5f;

        /// <summary>
        /// The current position of the enemy's ship
        /// </summary>
        Vector3 pos;
        public Vector3 Pos { get; set; }

        /// <summary>
        /// The current direction the enemy's ship should face in
        /// </summary>
        Vector3 dir;
        public Vector3 Dir { get; set; }

        float speedFactor;

        /// <summary>
        /// The current velocity of the ship
        /// </summary>
        public Vector3 velocity;

        /// <summary>
        /// The mass of the ship
        /// </summary>
        private const float mass = 1.0f;
        
        /// <summary>
        /// The force of moving the ship forward
        /// </summary>
        private const float thrustForce = 12000.0f;
        
        /// <summary>
        /// What is this? <- Mitch
        /// It's the drag force factor to simulate drag so that the ship can slow down, Mitch.
        /// </summary>
        private const float dragForce = 0.97f;

        /// <summary>
        /// The world matrix of the ship
        /// </summary>
        private Matrix world;
        public Matrix World { get { return world; } }

        public Enemy(Vector3 p, Vector3 d, float s) {
            pos = p;
            dir = Vector3.Forward;
            up = Vector3.Up;
            right = Vector3.Right;
            velocity = Vector3.Zero;
            speedFactor = s;
        }

        public void Update(GameTime gameTime, Player player)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 rotationAmount = new Vector3(0, 0, 0);

            //To-do: Have the enemy face the player and go forwards the player until they enter a certain radius around the player, then stop rotating until it's outside of the radius.
            float distanceFromPlayer = Vector3.Distance(this.Pos, player.pos);

            rotationAmount = rotationAmount * rotationRate * elapsed;

            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(right, rotationAmount.Y) * Matrix.CreateFromAxisAngle(up, rotationAmount.X) * Matrix.CreateFromAxisAngle(dir, rotationAmount.Z);
            dir = Vector3.TransformNormal(dir, rotationMatrix);
            up = Vector3.TransformNormal(up, rotationMatrix);

            dir.Normalize();
            up.Normalize();

            right = Vector3.Cross(dir, up);

            Vector3 force = dir * (1f * speedFactor) * thrustForce;

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

            world = Matrix.Identity;
            world.Forward = dir;
            world.Up = up;
            world.Right = right;
            world *= Matrix.CreateScale(500f);
            world.Translation = pos;
        }

        public void Die() {
            
        }
    }
}
