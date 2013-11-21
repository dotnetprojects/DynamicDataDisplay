using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Microsoft.Research.Visualization3D.Auxilaries;

namespace Microsoft.Research.Visualization3D.CameraUtilities
{
    /// <summary>
    /// Represents a view onto a 3D scene.
    /// </summary>
    public class Camera
    {
        private const float MaxRadius = 300.0f;
        
        Vector3 location;
        Vector3 target;
        Vector3 up = Vector3.UnitY;
        float fieldOfView;
        float aspectRatio;
        float nearPlane;
        float farPlane;

        float cameraScale = 10.0f;

        public float CameraScale
        {
            get { return cameraScale; }
            set { cameraScale = value; }
        }

        Matrix viewMatrix;
        Matrix projectionMatrix;
        bool viewDirty = true;
        bool projectionDirty = true;

        /// <summary>
        /// Gets or sets the location of the camera eye point.
        /// </summary>
        /// <value>The location of the camera eye point.</value>
        public Vector3 Location
        {
            get { return location; }
            set
            {
                if (location == value)
                    return;

                location = value;
                viewDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the view target point.
        /// </summary>
        /// <value>The view target point.</value>
        public Vector3 Target
        {
            get { return target; }
            set
            {
                if (target == value)
                    return;

                target = value;
                viewDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the view up vector.
        /// </summary>
        /// <value>The view up vector.</value>
        public Vector3 Up
        {
            get { return up; }
            set
            {
                if (up == value)
                    return;

                up = value;
                viewDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>The field of view.</value>
        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                if (fieldOfView == value)
                    return;

                fieldOfView = value;
                projectionDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                if (aspectRatio == value)
                    return;

                aspectRatio = value;
                projectionDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the near plane.
        /// </summary>
        /// <value>The near plane.</value>
        public float NearPlane
        {
            get { return nearPlane; }
            set
            {
                if (nearPlane == value)
                    return;

                nearPlane = value;
                projectionDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the far plane.
        /// </summary>
        /// <value>The far plane.</value>
        public float FarPlane
        {
            get { return farPlane; }
            set
            {
                if (farPlane == value)
                    return;

                farPlane = value;
                projectionDirty = true;
            }
        }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>The view matrix.</value>
        public Matrix ViewMatrix
        {
            get
            {
                if (viewDirty)
                    RebuildViewMatrix();
                return viewMatrix;
            }
            protected set { viewMatrix = value; }
        }

        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        /// <value>The projection matrix.</value>
        public Matrix ProjectionMatrix
        {
            get
            {
                if (projectionDirty)
                    RebuildProjectionMatrix();
                return projectionMatrix;
            }
            protected set { projectionMatrix = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        public Camera()
        {
        }

        /// <summary>
        /// Rebuilds the view matrix.
        /// </summary>
        protected virtual void RebuildViewMatrix()
        {
            viewMatrix = Matrix.LookAtLH(Location, Target, Up);
            viewDirty = false;
        }

        /// <summary>
        /// Rebuilds the projection matrix.
        /// </summary>
        protected virtual void RebuildProjectionMatrix()
        {
            projectionMatrix = Matrix.PerspectiveFovLH(FieldOfView, AspectRatio, NearPlane, FarPlane);
            projectionDirty = false;
        }

        public void RotateAroundTargetX(float angle)
        {
            Vector3 direction = this.target - this.location;
            direction.Normalize();
            direction = Vector3.TransformCoordinate(direction, Matrix.RotationY(up.Y * angle));
            this.location = this.target - direction * (this.target - this.location).Length();

            this.viewDirty = true;
        }

        public void RotateAroundTargetY(float angle)
        {
            Vector3 normal = MathHelper.CalculateNormal(this.location, this.target, new Vector3(this.target.X, this.target.Y + 0.1f, this.target.Z));
            Vector3 direction = this.target - this.location;
            direction.Normalize();
            direction = Vector3.TransformCoordinate(direction, Matrix.RotationAxis(normal, -angle));


            float dot = Vector3.Dot(up, direction); 
            if ((dot > -0.95 && angle>0) || (dot < 0.95 && angle<0) )
            {
                
                Vector3 newLocation = this.target - direction * (this.target - this.location).Length();

                this.location = newLocation;

                this.viewDirty = true;
            }
        }

        public void CameraMoveToTarget(float distance)
        {
            Vector3 direction = this.target - this.location;
            direction.Normalize();

            Vector3 newLocation = this.location + cameraScale * distance * direction;
            Vector3 newDirection = this.target - newLocation;
            newDirection.Normalize();

            float dot = Vector3.Dot(direction, newDirection);
            float dst = Vector3.Distance(newLocation, this.target);
            if (dot > 0 && dst<MaxRadius)
            {
                this.location = newLocation;
                this.viewDirty = true;

            }


        }
    }
}
