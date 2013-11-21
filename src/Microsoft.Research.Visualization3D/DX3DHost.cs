using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;

using sdx = SlimDX;
using SlimDX.Direct3D9;
using System.Windows.Threading;
using Microsoft.Research.Visualization3D.Particles;
using SlimDX;
using Microsoft.Research.Visualization3D.MainLoops;
using Microsoft.Research.Visualization3D.VertexStructures;
using Microsoft.Research.Visualization3D.BorderCube;
using Microsoft.Research.Visualization3D.CameraUtilities;
using Microsoft.Research.Visualization3D.RayCasting;
using Microsoft.Research.Visualization3D.Isosurfaces;
using System.Diagnostics;

namespace Microsoft.Research.Visualization3D
{
    /// <summary>
    /// Description of ControlWPF.
    /// </summary>
    public class DX3DHost : ContentControl
    {
        // we use it for 3D
        Direct3D direct3D;
        Direct3DEx direct3DEx;
        Device device;
        DeviceEx deviceEx;
        PresentParameters pp;

        //timer
        TimeManager timeManager;

        //Some inner fields
        DrawableComponentsManager components;
        Point oldPosition;
        Camera camera;

        /// <summary>
        /// Represents All Visualization3D providers
        /// </summary>
        public DrawableComponentsManager Components
        {
            get { return components; }
        }

        /// <summary>
        /// Represents current scene Camera properties
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }

        // this is our Content
        Image image;
        D3DImage d3dimage;
        bool StartThread = false;
        bool sizeChanged = false;

        // some public properties
        public bool useDeviceEx
        {
            get;
            private set;
        }

        public Direct3D Direct3D
        {
            get
            {
                if (useDeviceEx)
                    return direct3DEx;
                else
                    return direct3D;
            }
        }

        public Device Device
        {
            get
            {
                if (useDeviceEx)
                    return deviceEx;
                else
                    return device;
            }
        }

        #region Events

        /// <summary>
        /// Occurs once per iteration of the main loop.
        /// </summary>
        public event EventHandler MainLoop;

        /// <summary>
        /// Occurs when the device is created.
        /// </summary>
        public event EventHandler DeviceCreated;

        /// <summary>
        /// Occurs when the device is destroyed.
        /// </summary>
        public event EventHandler DeviceDestroyed;

        /// <summary>
        /// Occurs when the device is lost.
        /// </summary>
        public event EventHandler DeviceLost;

        /// <summary>
        /// Occurs when the device is reset.
        /// </summary>
        public event EventHandler DeviceReset;

        /// <summary>
        /// Raises the OnInitialize event.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:MainLoop"/> event.
        /// </summary>
        protected virtual void OnMainLoop(EventArgs e)
        {
            if (MainLoop != null)
                MainLoop(this, e);
        }

        /// <summary>
        /// Raises the DeviceCreated event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceCreated(EventArgs e)
        {
            if (DeviceCreated != null)
                DeviceCreated(this, e);
        }

        /// <summary>
        /// Raises the DeviceDestroyed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceDestroyed(EventArgs e)
        {
            if (DeviceDestroyed != null)
                DeviceDestroyed(this, e);
        }

        /// <summary>
        /// Raises the DeviceLost event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceLost(EventArgs e)
        {
            if (DeviceLost != null)
                DeviceLost(this, e);
        }

        /// <summary>
        /// Raises the DeviceReset event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceReset(EventArgs e)
        {
            if (DeviceReset != null)
                DeviceReset(this, e);
        }

        #endregion

        public DX3DHost()
        {
            image = new Image();
            d3dimage = new D3DImage();
            image.Source = d3dimage;

            this.Content = image;
            this.components = new DrawableComponentsManager();
            this.camera = new Camera();

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InitializeDirect3D();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            sizeChanged = true;
        }

        void InitializeDirect3D()
        {
            try
            {
                direct3DEx = new Direct3DEx();
                useDeviceEx = true;
            }
            catch
            {
                direct3D = new Direct3D();
                useDeviceEx = false;
            }
        }

        /// <summary>
        /// Initializes the various Direct3D objects we'll be using.
        /// </summary>
        public bool Initialize(bool startThread)
        {
            try
            {
                StartThread = startThread;

                ReleaseD3D();
                HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "test", IntPtr.Zero);

                pp = new PresentParameters();
                pp.SwapEffect = SwapEffect.Discard;
                pp.DeviceWindowHandle = hwnd.Handle;
                pp.Windowed = true;
                pp.BackBufferWidth = (int)ActualWidth;
                pp.BackBufferHeight = (int)ActualHeight;
                pp.BackBufferFormat = Format.X8R8G8B8;



                if (useDeviceEx)
                {
                    deviceEx = new DeviceEx((Direct3DEx)Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        pp);
                }
                else
                {
                    device = new Device(Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        pp);
                }

                // call the users one
                OnDeviceCreated(EventArgs.Empty);
                OnDeviceReset(EventArgs.Empty);

                // only if startThread is true
                if (StartThread)
                {
                    CompositionTarget.Rendering += OnRendering;
                    d3dimage.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(OnIsFrontBufferAvailableChanged);
                }
                d3dimage.Lock();
                d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).ComPointer);
                d3dimage.Unlock();

                CustomInitialize();

                return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return false;
            }
        }



        public void ReleaseD3D()
        {
            if (device != null)
            {
                if (!device.Disposed)
                {
                    device.Dispose();
                    device = null;
                }
            }
            d3dimage.Lock();
            d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            d3dimage.Unlock();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            Result result;

            try
            {
                if (Device == null)
                    Initialize(StartThread);

                if (sizeChanged)
                {
                    pp.BackBufferWidth = (int)ActualWidth;
                    pp.BackBufferHeight = (int)ActualHeight;
                    Device.Reset(pp);
                    OnDeviceReset(EventArgs.Empty);
                }

                if (d3dimage.IsFrontBufferAvailable)
                {
                    result = Device.TestCooperativeLevel();
                    if (result.IsFailure)
                    {
                        throw new Direct3D9Exception();
                    }
                    d3dimage.Lock();

                    Device.SetRenderState(RenderState.CullMode, Cull.None);
                    Device.SetRenderState(RenderState.ZEnable, true);
                    Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0, 0, 0, 0), 1.0f, 0);

                    Device.BeginScene();

                    components.Draw(timeManager.Current);

                    OnMainLoop(EventArgs.Empty);


                    Device.EndScene();

                    Device.Present();

                    d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).ComPointer);
                    d3dimage.AddDirtyRect(new Int32Rect(0, 0, d3dimage.PixelWidth, d3dimage.PixelHeight));
                    d3dimage.Unlock();
                }
            }
            catch (Direct3D9Exception ex)
            {
                string msg = ex.Message;
                Initialize(StartThread);
            }
            sizeChanged = false;
        }


        private void CustomInitialize()
        {
            Components.Initialize();

            timeManager = new TimeManager();
            timeManager.Update += Update;
            timeManager.Start();
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            oldPosition = e.GetPosition(this);

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point newPosition = e.GetPosition(this);
                camera.RotateAroundTargetX((float)(newPosition.X - oldPosition.X) / 100);
                camera.RotateAroundTargetY((float)(newPosition.Y - oldPosition.Y) / 100);

                oldPosition = newPosition;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            camera.CameraMoveToTarget((float)e.Delta / 120);

            base.OnMouseWheel(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            //camera.Up = new Vector3(camera.Up.X, -camera.Up.Y, camera.Up.Z);
            base.OnMouseRightButtonDown(e);
        }



        private void Update(object sender, TimedEventArgs e)
        {
            components.Update(e.TimeEntity);
        }


        void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3dimage.IsFrontBufferAvailable)
            {
                Initialize(StartThread);
            }
            else
            {
                CompositionTarget.Rendering -= OnRendering;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != image)
            {
                this.Content = image;
            }

            base.OnContentChanged(oldContent, newContent);
        }

    }

}

