using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using Microsoft.Research.Visualization3D.CameraUtilities;
using Microsoft.Research.Visualization3D.MainLoops;

namespace Microsoft.Research.Visualization3D
{
    public abstract class DrawableComponent
    {
        protected Device device;
        protected Effect effect;
        protected Camera camera;
        protected Visualization3DDataSource dataSource;

        private bool isEnabled;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                if (value)
                    this.Initialize();
            }
        }

        public abstract float CurrentValue
        {
            get;
            set;
        }

        public Camera Camera
        {
            get { return camera; }
        }

        public Visualization3DDataSource DataSource
        {
            get { return this.dataSource; }
            set
            {
                dataSource = value;
                OnDataSourceChanged();
            }
        }

        public DrawableComponent(DX3DHost host, Visualization3DDataSource dataSource)
        {
            this.device = host.Device;
            this.camera = host.Camera;
            this.dataSource = dataSource;

            this.isEnabled = false;

            host.DeviceReset += OnDeviceReset;
            host.DeviceLost += OnDeviceLost;
            host.DeviceDestroyed += OnDeviceDestroyed;
        }

        public abstract void Draw(TimeEntity timeEntity);
        public abstract void Update(TimeEntity timeEntity);
        protected virtual void OnDeviceReset(object sender, EventArgs e)
        {
 
        }
        protected virtual void OnDeviceLost(object sender, EventArgs e)
        {
 
        }

        protected virtual void OnDeviceDestroyed(object sender, EventArgs e)
        {
 
        }

        protected virtual void SetCamera()
        {
 
        }

        protected virtual void OnDataSourceChanged()
        {
 
        }

        public virtual void Initialize()
        {
            camera.NearPlane = 0.1f;
            camera.FarPlane = 1000.0f;
            camera.FieldOfView = (float)Math.PI / 4.0f;
            camera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            
            SetCamera();
        }

    }
}
