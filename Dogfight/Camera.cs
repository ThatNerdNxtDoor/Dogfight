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
        //postition
        private Vector3 chasePos;
        public Vector3 ChasePos { get { return chasePos; } set { chasePos = value; } }

        //direction
        private Vector3 chaseDir;
        public Vector3 ChaseDir { get { return chaseDir; } set { chaseDir = value; } }

        //up vector
        private Vector3 up = Vector3.Up;
        public Vector3 Up { get { return up; } set { up = value; } }

        #region CameraDesiredPosition

        //Desired camera position
        private Vector3 desiredPos;
        public Vector3 DesiredPos { get { return desiredPos; } set { desiredPos = value; } }

        private Vector3 desiredPositionOffset = new Vector3(0, 2.0f, 2.0f);
        public Vector3 DesiredPositionOffset { get { return desiredPositionOffset; } set { desiredPositionOffset = value; } }

        private Vector3 lookAt;
        public Vector3 LookAt { get { return lookAt; } }

        private Vector3 lookAtOffset = new Vector3(0, 3.0f, 0);
        public Vector3 LookAtOffset { get { return lookAtOffset; } set { lookAtOffset = value; } }

        #endregion

        #region CameraPhysics

        private float stiffness = 1500f;
        public float Stiffness { get { return stiffness; } set { stiffness = value; } }

        private float damping = 600f;
        public float Damping { get { return damping; } set { damping = value; } }

        private float mass = 50.0f;
        public float Mass { get { return mass; } set { mass = value; } }

        #endregion

        #region CameraCurrentProperties
        private Vector3 pos;
        public Vector3 Pos { get { return pos; } set { pos = value; } }

        private Vector3 velocity;
        public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

        private float aspectRatio = 4.0f / 3.0f;
        public float AspectRatio { get { return aspectRatio; } set { aspectRatio = value; } }

        private float fieldOfView = MathHelper.ToRadians(45f);
        public float FieldOfView { get { return fieldOfView; } set { fieldOfView = value; } }

        private float nearPlaneDist = 1.0f;
        public float NearPlaneDist { get { return nearPlaneDist; } set { nearPlaneDist = value; } }

        private float farPlaneDist = 1000.0f;
        public float FarPlaneDist { get { return farPlaneDist; } set { farPlaneDist = value; } }

        #endregion

        private Matrix view;
        public Matrix View { get { return view; } set { view = value; } }

        private Matrix projection;
        public Matrix projectionView { get { return projection; } set { projection = value; } }

        private void UpdateWorldPosition()
        {
            Matrix transform = Matrix.Identity;
            transform.Forward = ChaseDir;
            transform.Up = Up;
            transform.Right = Vector3.Cross(Up, chaseDir);

            desiredPos = chasePos + Vector3.TransformNormal(DesiredPositionOffset, transform);

            lookAt = ChasePos + Vector3.TransformNormal(lookAtOffset, transform);
        }

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
