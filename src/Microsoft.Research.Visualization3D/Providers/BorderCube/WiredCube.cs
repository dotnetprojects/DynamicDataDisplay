using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using Microsoft.Research.Visualization3D.VertexStructures;
using SlimDX;
using Microsoft.Research.Visualization3D.CameraUtilities;

namespace Microsoft.Research.Visualization3D.BorderCube
{
    public class WiredCube
    {
        Device device;
        Effect effect;

        VertexBuffer vb;
        IndexBuffer ib;

        Camera camera;

        VertexPositionNormalColor[] vertices;
        int[] indices;

        float width, height, depth;

        int color;

        public float Depth
        {
            get { return depth; }
            set { 
                    depth = value;
                    SetUpBuffers();
                }
        }

        public float Height
        {
            get { return height; }
            set { 
                    height = value;
                    SetUpBuffers();
                }
        }

        public float Width
        {
            get { return width; }
            set { 
                    width = value;
                    SetUpBuffers();
                }
        }

        Matrix world;

        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }

        public WiredCube(Device device, Effect effect, float width, float height, float depth, Color3 cubeColor, Camera camera)
        {
            this.device = device;
            this.effect = effect;

            this.width = width;
            this.height = height;
            this.depth = depth;

            this.color = (int)RgbPalette.ColorARGB(cubeColor);

            this.camera = camera;
            world = Matrix.Identity;
            SetUpBuffers();
        }

        private void SetUpBuffers()
        {
            vertices = new VertexPositionNormalColor[]
            {
                new VertexPositionNormalColor(new Vector3(0,0,0),Vector3.Zero, color),
                new VertexPositionNormalColor(new Vector3(0,height,0),Vector3.Zero, color),
                new VertexPositionNormalColor(new Vector3(width,height,0),Vector3.Zero, color),
                new VertexPositionNormalColor(new Vector3(width,0,0),Vector3.Zero, color),

                new VertexPositionNormalColor(new Vector3(0,0,depth),Vector3.Zero, color),
                new VertexPositionNormalColor(new Vector3(0,height,depth),Vector3.Zero, color),
                new VertexPositionNormalColor(new Vector3(width,height,depth),Vector3.Zero, color),
                new VertexPositionNormalColor(new Vector3(width,0,depth),Vector3.Zero, color),
            };

            indices = new int[] { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 };

            vb = new VertexBuffer(device, VertexPositionNormalColor.SizeInBytes * vertices.Length, Usage.WriteOnly, VertexPositionNormalColor.Format, Pool.Default);
            using (DataStream stream = vb.Lock(0, 0, LockFlags.None))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    stream.Write(vertices[i]);
                }
                vb.Unlock();
            }

            ib = new IndexBuffer(device, sizeof(int) * indices.Length, Usage.WriteOnly, Pool.Default, false);
            using (DataStream stream = ib.Lock(0, 0, LockFlags.None))
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    stream.Write(indices[i]);
                }
                ib.Unlock();
            }
        }

        public void Draw()
        {
            device.VertexFormat = VertexPositionNormalColor.Format;
            device.SetStreamSource(0, vb, 0, VertexPositionNormalColor.SizeInBytes);
            device.Indices = ib;

            int passes = effect.Begin(FX.None);
            for (int i = 0; i < passes; i++)
            {

                effect.BeginPass(i);
                effect.Device.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertices.Length, 0, indices.Length / 2);
                effect.EndPass();
            }
        }

        #region IVisualization3DProvider Members

        public void Initialize()
        {
            float cameraScale = 2.0f;
            camera.Location = cameraScale * new Vector3(width, height, depth);
            camera.Target = new Vector3(width / 2f, height / 2f, depth / 2f);
            camera.Up = new Vector3(0, 1, 0);

            camera.NearPlane = 0.1f;
            camera.FarPlane = 1000.0f;
            camera.FieldOfView = (float)Math.PI / 4.0f;
            camera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;

            effect.SetValue("world", world);
            effect.SetValue("view", camera.ViewMatrix);
            effect.SetValue("projection", camera.ProjectionMatrix);

            effect.SetValue("cameraPosition", camera.Location);
            effect.SetValue("lightPosition", camera.Location);
            effect.SetValue("ambientLightColor", new Vector4(1.0f, 1.0f, 1.0f, 1));
            effect.SetValue("diffuseLightColor", new Vector4(1.0f, 1.0f, 1.0f, 1));
            effect.SetValue("specularLightColor", new Vector4(1.0f, 1.0f, 1.0f, 1));

            effect.SetValue("specularPower", 1.0f);
            effect.SetValue("specularIntensity", 1.0f);
        }

        public void Draw(float time)
        {
            this.Draw();
        }

        public void Update(float time)
        {
            effect.SetValue("world", world);
            effect.SetValue("view", camera.ViewMatrix);
            effect.SetValue("projection", camera.ProjectionMatrix);

            effect.SetValue("cameraPosition", camera.Location);
            effect.SetValue("lightPosition", camera.Location);
            effect.SetValue("ambientLightColor", new Vector4(1.0f, 1.0f, 1.0f, 1));
            effect.SetValue("diffuseLightColor", new Vector4(1.0f, 1.0f, 1.0f, 1));
            effect.SetValue("specularLightColor", new Vector4(1.0f, 1.0f, 1.0f, 1));

            effect.SetValue("specularPower", 1.0f);
            effect.SetValue("specularIntensity", 1.0f);
        }

        public bool IsEnabled
        {
            get;
            set;

        }

        public Camera Camera
        {
            get { return camera; }
        }

        public float Max
        {
            get { return float.MaxValue; }
        }

        public float Min
        {
            get { return float.MinValue; }
        }

        public float CurrentValue
        {
            get
            {
                return 0; ;
            }
            set
            {
                //nothing
            }
        }

        public object Data
        {
            get
            {
                return null;
            }
            set
            {
               //nothing
            }
        }

        #endregion
    }
}
