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
    internal class Camera
    {
        /// <summary>
        /// The position the camera is trying to follow
        /// </summary>
        private Vector3 chasePos;

        /// <summary>
        /// The position the camera is trying to follow
        /// </summary>
        public Vector3 ChasePos { get { return chasePos; } set { chasePos = value; } }

        /// <summary>
        /// The direction the camera is currently moving in
        /// </summary>
        private Vector3 chaseDir;

        /// <summary>
        /// The direction the camera is currently moving in
        /// </summary>
        public Vector3 ChaseDir { get { return chaseDir; } set { chaseDir = value; } }

        /// <summary>
        /// The up direction for the camera
        /// </summary>
        private Vector3 up = Vector3.Up;

        /// <summary>
        /// The up direction for the camera
        /// </summary>
        public Vector3 Up { get { return up; } set { up = value; } }

        #region CameraDesiredPosition

        /// <summary>
        /// The position the camera attempts to go toward
        /// </summary>
        private Vector3 desiredPos;

        /// <summary>
        /// The position the camera attempts to go toward
        /// </summary>
        public Vector3 DesiredPos { get { return desiredPos; } set { desiredPos = value; } }

        /// <summary>
        /// The offset that the desired position moves away from the target's position
        /// </summary>
        private Vector3 desiredPositionOffset = new Vector3(0, .1f, .1f);

        /// <summary>
        /// The offset that the desired position moves away from the target's position
        /// </summary>
        public Vector3 DesiredPositionOffset { get { return desiredPositionOffset; } set { desiredPositionOffset = value; } }

        /// <summary>
        /// The position the camera is looking at
        /// </summary>
        private Vector3 lookAt;

        /// <summary>
        /// The position the camera is looking at
        /// </summary>
        public Vector3 LookAt { get { return lookAt; } }

        /// <summary>
        /// The offset that the lookAt position moves away from the target's position
        /// </summary>
        private Vector3 lookAtOffset = new Vector3(0, 1.0f, 0);

        /// <summary>
        /// The offset that the lookAt position moves away from the target's position
        /// </summary>
        public Vector3 LookAtOffset { get { return lookAtOffset; } set { lookAtOffset = value; } }

        #endregion

        #region CameraPhysics

        /// <summary>
        /// Stiffness factor of spring
        /// </summary>
        private float stiffness = 3000f;
        public float Stiffness { get { return stiffness; } set { stiffness = value; } }

        /// <summary>
        /// The damping factor on the spring
        /// </summary>
        private float damping = 600f;
        public float Damping { get { return damping; } set { damping = value; } }

        /// <summary>
        /// Mass of the camera
        /// </summary>
        private float mass = 50.0f;
        public float Mass { get { return mass; } set { mass = value; } }

        #endregion

        #region CameraCurrentProperties
        /// <summary>
        /// Current position of the camera
        /// </summary>
        private Vector3 pos;

        /// <summary>
        /// Current position of the camera
        /// </summary>
        public Vector3 Pos { get { return pos; } set { pos = value; } }

        /// <summary>
        /// Current velocity of the camera
        /// </summary>
        private Vector3 velocity;

        /// <summary>
        /// Current velocity of the camera
        /// </summary>
        public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

        /// <summary>
        /// The aspect ratio of the screen
        /// </summary>
        private float aspectRatio;

        /// <summary>
        /// The aspect ratio of the screen
        /// </summary>
        public float AspectRatio { get { return aspectRatio; } set { aspectRatio = value; } }

        /// <summary>
        /// The field of view of the camera
        /// </summary>
        private float fieldOfView = MathHelper.ToRadians(45f);
        public float FieldOfView { get { return fieldOfView; } set { fieldOfView = value; } }

        /// <summary>
        /// The distance to the close clipping plane of the camera
        /// </summary>
        private float nearPlaneDist = 1.0f;

        /// <summary>
        /// The distance to the close clipping plane of the camera
        /// </summary>
        public float NearPlaneDist { get { return nearPlaneDist; } set { nearPlaneDist = value; } }

        /// <summary>
        /// The distance to the far clipping plane of the camera
        /// </summary>
        private float farPlaneDist = 1000.0f;

        /// <summary>
        /// The distance to the far clipping plane of the camera
        /// </summary>
        public float FarPlaneDist { get { return farPlaneDist; } set { farPlaneDist = value; } }

        #endregion

        /// <summary>
        /// The computed view matrix of the camera
        /// </summary>
        private Matrix view;

        /// <summary>
        /// The computed view matrix of the camera
        /// </summary>
        public Matrix View { get { return view; } set { view = value; } }

        /// <summary>
        /// The computed projection matrix of the camera
        /// </summary>
        private Matrix projection;

        /// <summary>
        /// The computed projection matrix of the camera
        /// </summary>
        public Matrix projectionView { get { return projection; } set { projection = value; } }
        
        /// <summary>
        /// Find and set the position the camera is trying to go toward and the position it is looking at
        /// </summary>
        private void UpdateWorldPosition()
        {
            Matrix transform = Matrix.Identity;
            transform.Forward = ChaseDir;
            transform.Up = Up;
            transform.Right = Vector3.Cross(Up, chaseDir);

            desiredPos = chasePos + Vector3.TransformNormal(DesiredPositionOffset, transform);

            lookAt = chasePos + Vector3.TransformNormal(lookAtOffset, transform);
        }

        /// <summary>
        /// Completely reset the camera values
        /// </summary>
        public void Reset()
        {
            UpdateWorldPosition();

            velocity = Vector3.Zero;
            pos = desiredPos;

            view = Matrix.CreateLookAt(this.Pos, this.lookAt, this.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, nearPlaneDist, farPlaneDist);
        }

        public void Update(GameTime gameTime)
        {
            UpdateWorldPosition();

            //How much time has gone by
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Spring Force
            Vector3 stretch = pos - desiredPos;
            Vector3 force = -stiffness * stretch - damping * velocity;
            Vector3 acceleration = force / mass;
            velocity += acceleration * elapsed;

            pos += velocity * elapsed;

            view = Matrix.CreateLookAt(this.Pos, this.lookAt, this.Up);
        }
    }
}
