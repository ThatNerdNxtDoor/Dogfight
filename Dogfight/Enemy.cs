using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogfight
{
    internal class Enemy
    {
        private const float altitudeBoundary = 1000000.0f;

        /// <summary>
        /// The current 'up' vector for the ship
        /// </summary>
        public Vector3 up;

        /// <summary>
        /// The direction to the right of the enemy's ship
        /// </summary>
        public Vector3 right;

        /// <summary>
        /// How quickly the ship rotates (in radians / second)
        /// </summary>
        private const float rotationRate = 1.5f;

        /// <summary>
        /// The current position of the enemy's ship
        /// </summary>
        Vector3 pos;
        public Vector3 Pos { get { return pos; } set { pos = value; } }

        /// <summary>
        /// The current direction the enemy's ship should face in
        /// </summary>
        Vector3 dir;
        public Vector3 Dir { get { return dir; } set { dir = value; } }

        float speedFactor;

        float fireInterval;

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
            dir = d;
            dir.Normalize();
            up = Vector3.Up * d;
            up.Normalize();
            right = Vector3.Right * d;
            velocity = Vector3.Zero;
            speedFactor = s;
            fireInterval = 1f;
        }

        public void Update(GameTime gameTime, Player player, List<Projectile> pList)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Vector3 rotationAmount = new Vector3(0, 0, 0);

            //To-do: Have the enemy face the player and go forwards the player until they enter a certain radius around the player, then stop rotating and start firing until it's outside of the radius.
            //This should give a strafing run effect

            float distanceFromPlayer = Vector3.Distance(pos, player.pos);

            Matrix rotationMatrix;

            if (distanceFromPlayer <= 21000f) //Firing Range
            {
                if (fireInterval > 0f)
                {
                    fireInterval -= elapsed;
                }
                else {
                    pList.Add(new Projectile(pos, dir));
                    fireInterval = 1f;
                }

                rotationMatrix = Matrix.Identity;
            }
            else { //Rotating
                Vector3 rotationTarget = player.pos - this.pos;
                rotationTarget.Normalize();

                Vector3 rotationAxis = Vector3.Cross(this.dir, rotationTarget); //The axis to rotate the ship's model around

                float rotationAmount = (float)Math.Acos(d: Vector3.Dot(rotationTarget, dir)); //The amount the ship would have to rotate to point toward the player ship
                /* The angle to rotate is the angle between the direction vector and the rotation target
                 * (Dot product divided by magnitudes, which are both one anyway as these are normalized)
                 */

                //Debug.WriteLine("E: " + pos + dir + rotationAmount);

                //Now we apply the rotation rate
                float rotationRateAmount = rotationRate * elapsed; //The amount the ship should rotate, based on it's rotation speed
                //If the rotation amount is bigger (i.e. faster), then we should use the rotation rate. Otherwise, use the rotation amount so we don't overshoot
                if (rotationRateAmount < rotationAmount)
                {
                    rotationAmount = rotationRateAmount;
                }

                rotationMatrix = Matrix.CreateFromAxisAngle(rotationAxis, rotationRateAmount);
            }
            
            //rotationAmount = rotationAmount * rotationRate * elapsed;

            //Matrix rotationMatrix = Matrix.CreateFromAxisAngle(right, rotationAmount.Y) * Matrix.CreateFromAxisAngle(up, rotationAmount.X) * Matrix.CreateFromAxisAngle(dir, rotationAmount.Z);
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
    }
}
