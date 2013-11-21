using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.IO;
using Microsoft.Research.Visualization3D.Auxilaries;
using Microsoft.Research.Visualization3D.CameraUtilities;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Research.Visualization3D.MainLoops;
using Microsoft.Research.Visualization3D.VertexStructures;

namespace Microsoft.Research.Visualization3D.RayCasting
{
    public class RayCastingProvider : DrawableComponent
    {
        int textureSizeX = 64;
        int textureSizeY = 64;
        int textureSizeZ = 64;
        float step = 0.03f;
        int resolutionX = 1024;
        int resolutionY = 768;
        float dataLengthMax = 0;
        float intensitiveScale = 5.0f;
        float intensitiveInterval = 0.05f;
        float densityMod = 0.75f;
        int volumeShorterring = 2;
        int shortedWidth = 0;
        int shortedHeight = 0;
        
        VolumeTexture texture;
        Matrix world, view, projection, tex;
        Texture solidDepthRT, volumeDepthRT, volumeAccuRT, denModTex;
        VertexBuffer fulScreenTriangle, cube;
        IndexBuffer cubeIndices;
        Surface solidDepthSurface, volumeDepthSurface, volumeAccuSurface, backBufferSurface;

        public bool EnableSlicing { get; set; }
        public override float CurrentValue { get; set; }

        public RayCastingProvider(DX3DHost host, Visualization3DDataSource dataSource) :
            base(host, dataSource)
        {
        }


        private void GenerateTexture(double[, ,] array)
        {
            int index = 0;
            byte[] buffer = new byte[4 * textureSizeX * textureSizeY * textureSizeZ];
            
            for (int i = 0; i < textureSizeX; i++)
            {
                for (int j = 0; j < textureSizeY; j++)
                {
                    for (int k = 0; k < textureSizeZ; k++)
                    {
                        float i1 = (float)(i * (float)(array.GetLength(0) - 1) / textureSizeX);
                        float j1 = (float)(j * (float)(array.GetLength(1) - 1) / textureSizeY);
                        float k1 = (float)(k * (float)(array.GetLength(2) - 1) / textureSizeZ);

                        float value = MathHelper.GetValue(new Vector3(i1, j1, k1), array);
                        byte[] color = RgbPalette.GetColorBytes(value, dataSource.Maximum, dataSource.Minimum, dataSource.MissingValue);

                        if (value > dataSource.Maximum) value = dataSource.Maximum;
                        if (value < dataSource.Minimum) value = dataSource.Minimum;
                        
                        buffer[index++] = color[3];
                        buffer[index++] = color[2];
                        buffer[index++] = color[1];
                        buffer[index++] = (byte)((value - dataSource.Minimum) / (dataSource.Maximum - dataSource.Minimum) * 255);

                    }
                }
            }

            texture = new VolumeTexture(device, textureSizeX, textureSizeY, textureSizeZ, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

            DataBox dbox = texture.LockBox(0, LockFlags.None);
            dbox.Data.WriteRange(buffer);
            texture.UnlockBox(0);
        }

        private void DrawRayCasting(TimeEntity timeEntity)
        {
            effect.SetValue("g_ObjectToClip", world * camera.ViewMatrix * camera.ProjectionMatrix);
            effect.SetValue("g_ObjectToWorld", world);
            effect.SetValue("g_ObjectToView", world * camera.ViewMatrix);

            effect.SetValue("g_ViewToTex", Matrix.Invert(tex * world * camera.ViewMatrix));
            effect.SetValue("g_SamplingParams", new Vector4(intensitiveInterval, intensitiveScale, (float)1.0f / camera.ProjectionMatrix.M11, (float)1.0f / camera.ProjectionMatrix.M22));
            effect.SetValue("g_TextureSize", new Vector4(textureSizeX, textureSizeY, textureSizeZ, 0));
            effect.SetValue("g_TexCoordOffset", new Vector4((float)timeEntity.TotalTime.TotalMilliseconds * 0.002f, (float)timeEntity.TotalTime.TotalMilliseconds * 0.001f, densityMod, 0));
            effect.SetValue("denValueRange", new Vector4((CurrentValue - dataSource.Minimum) / (dataSource.Maximum - dataSource.Minimum), 0, 0, step));
            effect.SetValue("EnableSlicing", EnableSlicing);

            Vector4 pixelOffset = new Vector4(0.5f / resolutionX, -0.5f / resolutionY, 0f, 0f);
            pixelOffset *= (float)volumeShorterring;

            effect.SetValue("g_PixelOffset", pixelOffset);
            effect.SetTexture(new EffectHandle("g_EmiAbsTexture"), texture);
            effect.SetTexture(new EffectHandle("g_DensityModTexture"), denModTex);
            effect.SetTexture(new EffectHandle("g_DepthBufferTexture"), solidDepthRT);
            effect.SetTexture(new EffectHandle("g_EmiAbsAccuBufferTexture"), volumeAccuRT);
            effect.SetTexture(new EffectHandle("g_EmiAbsDepthBufferTexture"), volumeDepthRT);

            device.VertexFormat = VertexFormat.Position;
            int numPasses = effect.Begin();
            for (int i = 0; i < numPasses; i++)
            {
                effect.BeginPass(i);
                device.SetStreamSource(0, cube, 0, Vertex.SizeInBytes);
                device.Indices = cubeIndices;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
                effect.EndPass();
            }
            effect.End();

        }

        private void ClearRenderTarget(Vector4 vec)
        {
            effect.Technique = new EffectHandle("ClearRenderTarget");
            effect.SetValue("g_ClearValue", vec);

            effect.SetValue("g_PixelOffset", volumeShorterring * new Vector4(1.0f / resolutionX, -1.0f / resolutionY, 0, 0));

            int numPasses = effect.Begin();
            for (int i = 0; i < numPasses; i++)
            {
                effect.BeginPass(i);
                device.VertexFormat = VertexFormat.Position;
                device.SetStreamSource(0, fulScreenTriangle, 0, sizeof(float) * 3);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
                effect.EndPass();
            }
            effect.End();
        }

        protected override void SetCamera()
        {
            float cameraScale = 1.5f;
            this.camera.Location = cameraScale * new Vector3(-dataSource.DisplayData.GetLength(0) / dataLengthMax, dataSource.DisplayData.GetLength(1) / dataLengthMax, dataSource.DisplayData.GetLength(2) / dataLengthMax);
            this.camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
            camera.CameraScale = 1.0f;
        }

        public override void Initialize()
        {
            
            //hack (dlya etih dannih tak kontrastnee bydet:)
            dataSource.Maximum = 0.1f;// (float)MathHelper.FindMax(dataSource.DisplayData, dataSource.MissingValue);
            dataSource.Minimum = -0.1f;//(float)MathHelper.FindMin(dataSource.DisplayData, dataSource.MissingValue); 
            CurrentValue = (dataSource.Maximum + dataSource.Minimum) / 2.0f;

            dataLengthMax = Math.Max(dataSource.DisplayData.GetLength(0), Math.Max(dataSource.DisplayData.GetLength(1), dataSource.DisplayData.GetLength(2)));

            this.world = Matrix.Identity;
            this.view = camera.ViewMatrix;
            this.projection = camera.ProjectionMatrix;

            OnDataSourceChanged();

            tex = Matrix.Identity;
            tex.M22 = -1;
            tex.M42 = 1;
            
            if (effect == null)
            {
                effect = Effect.FromStream(device, this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.Visualization3D.Shaders.RayCasting.fx"), ShaderFlags.None);
            }

            shortedWidth = this.resolutionX / volumeShorterring;
            shortedHeight = this.resolutionY / volumeShorterring;

            solidDepthRT = new Texture(device, shortedWidth, shortedHeight, 1, Usage.RenderTarget, Format.R16F, Pool.Default);
            volumeDepthRT = new Texture(device, shortedWidth, shortedHeight, 1, Usage.RenderTarget, Format.R16F, Pool.Default);
            volumeAccuRT = new Texture(device, shortedWidth, shortedHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            denModTex = Texture.FromStream(device, this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.Visualization3D.Textures.denmod.dds"), Usage.Dynamic, Pool.Default);

            solidDepthSurface = solidDepthRT.GetSurfaceLevel(0);
            volumeDepthSurface = volumeDepthRT.GetSurfaceLevel(0);
            volumeAccuSurface = volumeAccuRT.GetSurfaceLevel(0);
            backBufferSurface = device.GetRenderTarget(0);


            fulScreenTriangle = new VertexBuffer(device, 3 * Vertex.SizeInBytes, Usage.WriteOnly, VertexFormat.Position, Pool.Default);
            using (DataStream ds = fulScreenTriangle.Lock(0, 0, LockFlags.None))
            {
                ds.WriteRange(
                    new Vertex[] 
                    { 
                        new Vertex() { Position=new Vector3( -1.0f, -3.0f, 0.0f)},
                        new Vertex() { Position=new Vector3(-1.0f, 1.0f, 0.0f)},
                        new Vertex() { Position=new Vector3(3.0f, 1.0f, 0.0f)}
                    });
            }
            fulScreenTriangle.Unlock();


            cube = new VertexBuffer(device, Vertex.SizeInBytes * 8, Usage.WriteOnly, VertexFormat.Position, Pool.Default);
            using (DataStream ds = cube.Lock(0, 0, LockFlags.None))
            {
                ds.WriteRange(
                    new Vertex[] 
                    { 
                        new Vertex() { Position=new Vector3(0.0f, 0.0f, 0.0f)},
                        new Vertex() { Position=new Vector3(0.0f, 1.0f, 0.0f)},
                        new Vertex() { Position=new Vector3(1.0f, 1.0f, 0.0f)},
                        new Vertex() { Position=new Vector3(1.0f, 0.0f, 0.0f)},

                        new Vertex() { Position=new Vector3(0.0f, 0.0f, 1.0f)},
                        new Vertex() { Position=new Vector3(0.0f, 1.0f, 1.0f)},
                        new Vertex() { Position=new Vector3(1.0f, 1.0f, 1.0f)},
                        new Vertex() { Position=new Vector3(1.0f, 0.0f, 1.0f)}

                    });
            }
            cube.Unlock();

            cubeIndices = new IndexBuffer(device, sizeof(int) * 36, Usage.WriteOnly, Pool.Default, false);
            using (DataStream ds = cubeIndices.Lock(0, 0, LockFlags.None))
            {
                ds.WriteRange(new int[] 
                {
                    0, 1, 2, 0, 2, 3,
				    0+4, 2+4, 1+4, 0+4, 3+4, 2+4,
				    3, 2, 2+4, 3, 2+4, 3+4,
				    0+4, 1+4, 1, 0+4, 1, 0,
				    1, 1+4, 2+4, 1, 2+4, 2,
				    0, 3+4, 0+4, 0, 3, 3+4
                });

            }
            cubeIndices.Unlock();

            base.Initialize();
        }


        public override void Draw(TimeEntity timeEntity)
        {
            device.SetRenderTarget(0, solidDepthSurface);
            ClearRenderTarget(new Vector4(1000, 0, 0, 0));

            

            device.SetRenderTarget(0, volumeDepthSurface);
            ClearRenderTarget(new Vector4(camera.NearPlane, 0, 0, 0));
            effect.Technique = new EffectHandle("Depth");
            DrawRayCasting(timeEntity);

            device.SetRenderTarget(0, volumeAccuSurface);
            device.Clear(ClearFlags.Target, new Color4(1.0f, 0, 0, 0), 1.0f, 0);
            effect.Technique = new EffectHandle("EmiAbs_Accu");
            DrawRayCasting(timeEntity);


            device.SetRenderTarget(0, backBufferSurface);
            effect.Technique = new EffectHandle("EmiAbs_Compose");
            DrawRayCasting(timeEntity);
        }

        public override void Update(TimeEntity timeEntity)
        {
            //Nothing to do here
        }

        protected override void OnDeviceReset(object sender, EventArgs e)
        {
            backBufferSurface = device.GetRenderTarget(0);

            resolutionX = device.Viewport.Width;
            resolutionY = device.Viewport.Height;
            shortedWidth = this.resolutionX / volumeShorterring;
            shortedHeight = this.resolutionY / volumeShorterring;

            camera.AspectRatio = (float)resolutionX / (float)resolutionY;
            camera.NearPlane = 0.1f;
            camera.FarPlane = 1000.0f;
        }

        protected override void OnDataSourceChanged()
        {
            GenerateTexture(dataSource.DisplayData);

            this.world = new Matrix
            {
                M11 = dataSource.DisplayData.GetLength(0) / dataLengthMax,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = dataSource.DisplayData.GetLength(1) / dataLengthMax,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = dataSource.DisplayData.GetLength(2) / dataLengthMax,
                M34 = 0,
                M41 = -dataSource.DisplayData.GetLength(0) / (2 * dataLengthMax),
                M42 = -dataSource.DisplayData.GetLength(1) / (2 * dataLengthMax),
                M43 = -dataSource.DisplayData.GetLength(2) / (2 * dataLengthMax),
                M44 = 1
            } * Matrix.RotationZ((float)Math.PI);
        }
    }

    


}
