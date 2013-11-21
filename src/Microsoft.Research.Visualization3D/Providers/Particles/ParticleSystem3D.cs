using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using Microsoft.Research.Visualization3D.VertexStructures;
using Microsoft.Research.Visualization3D.Auxilaries;
using Microsoft.Research.Visualization3D.CameraUtilities;

namespace Microsoft.Research.Visualization3D.Particles
{
    public class ParticleSystem3D
    {
        List<Particle> particles;
        Device device;
        Effect effect;
        double[, ,] array;
        Random r = new Random();
        double missingValue;
        float min, max;
        Camera camera;
        float particleSize = 1.0f;

        float positionScaleParam = 1f;
        private bool isEnabled = true;
        private int particleNum;


        float gradientStep = 0.01f;
        float numericalStep = 0.01f;

        IndexBuffer ib;
        VertexBuffer vb;

        List<int> indices = new List<int>();
        int vertexOffset = 0;

        public float CurrentValue { get; set; }
        /// <summary>
        /// Data to display
        /// </summary>
        public object Data
        {
            get
            {
                return array;
            }
            set
            {
                if (value is double[, ,])
                {
                    array = value as double[, ,];
                    Initialize();
                }
            }
        }

        public ParticleSystem3D(Device device, Effect effect, int particleNum, double[, ,] array, double missingValue, Camera camera)
        {
            this.device = device;
            this.array = array;
            this.particles = new List<Particle>();
            this.missingValue = missingValue;
            this.camera = camera;
            this.particleNum = particleNum;


            min = -0.1f;//(float)MathHelper.FindMin(array, missingValue);
            max = 0.1f;// (float)MathHelper.FindMax(array, missingValue);

           
        }

        public void Update(float delta)
        {
            int result = 0;
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].LifeBar -= delta * 100;

                if (particles[i].LifeBar > 0)
                {
                    result++;

                    particles[i].Position.X += particles[i].Velocity.X;
                    particles[i].Position.Y += particles[i].Velocity.Y;
                    particles[i].Position.Z += particles[i].Velocity.Z;

                    Vector3 position = particles[i].Position;

                    //while (float.IsNaN(position.X) || particles[i].Position.X - gradientStep < 0 || particles[i].Position.Y - gradientStep < 0 || particles[i].Position.Z - gradientStep < 0 || particles[i].Position.X + gradientStep > array.GetLength(0) - 1 || particles[i].Position.Y + gradientStep > array.GetLength(1) - 1 || particles[i].Position.Z + gradientStep > array.GetLength(2) - 1 || MissingCheck(position))
                    //{
                    //    particles[i].Position = GeneratePosition();
                    //    position = particles[i].Position;
                    //}

                    Vector3 backPosition = new Vector3(position.X - numericalStep, position.Y, position.Z);
                    Vector3 frontPosition = new Vector3(position.X + numericalStep, position.Y, position.Z);
                    float velocityX = MathHelper.GetPatrialDerivation(MathHelper.GetValue(frontPosition, array), MathHelper.GetValue(backPosition, array), numericalStep);

                    Vector3 bottomPosition = new Vector3(position.X, position.Y - numericalStep, position.Z);
                    Vector3 topPosition = new Vector3(position.X, position.Y + numericalStep, position.Z);
                    float velocityY = MathHelper.GetPatrialDerivation(MathHelper.GetValue(topPosition, array), MathHelper.GetValue(bottomPosition, array), numericalStep);

                    Vector3 rightPosition = new Vector3(position.X, position.Y, position.Z + numericalStep);
                    Vector3 leftPosition = new Vector3(position.X, position.Y, position.Z - numericalStep);
                    float velocityZ = MathHelper.GetPatrialDerivation(MathHelper.GetValue(rightPosition, array), MathHelper.GetValue(leftPosition, array), numericalStep);

                    Vector3 velocity = new Vector3(velocityX, velocityY, velocityZ);

                    velocity.Normalize();

                    particles[i].Velocity = -velocity;
                    particles[i].Color = RgbPalette.GetColor(MathHelper.GetValue(position, array), max, min, missingValue);
                }
                else
                {

                    Vector3 position = GeneratePosition();
                    //while (float.IsNaN(position.X) || particles[i].Position.X - gradientStep < 0 || particles[i].Position.Y - gradientStep < 0 || particles[i].Position.Z - gradientStep < 0 || particles[i].Position.X + gradientStep > array.GetLength(0) - 1 || particles[i].Position.Y + gradientStep > array.GetLength(1) - 1 || particles[i].Position.Z + gradientStep > array.GetLength(2) - 1 || MissingCheck(position))
                    //{
                    //    position = GeneratePosition();
                    //}

                    Vector3 backPosition = new Vector3(position.X - numericalStep, position.Y, position.Z);
                    Vector3 frontPosition = new Vector3(position.X + numericalStep, position.Y, position.Z);
                    float velocityX = MathHelper.GetPatrialDerivation(MathHelper.GetValue(frontPosition, array), MathHelper.GetValue(backPosition, array), numericalStep);

                    Vector3 bottomPosition = new Vector3(position.X, position.Y - numericalStep, position.Z);
                    Vector3 topPosition = new Vector3(position.X, position.Y + numericalStep, position.Z);
                    float velocityY = MathHelper.GetPatrialDerivation(MathHelper.GetValue(topPosition, array), MathHelper.GetValue(bottomPosition, array), numericalStep);

                    Vector3 rightPosition = new Vector3(position.X, position.Y, position.Z + numericalStep);
                    Vector3 leftPosition = new Vector3(position.X, position.Y, position.Z - numericalStep);
                    float velocityZ = MathHelper.GetPatrialDerivation(MathHelper.GetValue(rightPosition, array), MathHelper.GetValue(leftPosition, array), numericalStep);

                    Vector3 velocity = new Vector3(velocityX, velocityY, velocityZ);
                    velocity.Normalize();

                    particles[i] = new Particle(position, -velocity, RgbPalette.GetColor(MathHelper.GetValue(position, array), max, min, missingValue), (float)(r.NextDouble() * 100) + 50);

                }
            }
            SetUpBuffers();
            //SetCamera();
        }

        private void Draw()
        {
            if (vertexOffset > 0)
            {
                device.SetStreamSource(0, vb, 0, VertexPositionNormalColor.SizeInBytes);
                device.VertexFormat = VertexPositionNormalColor.Format;
                device.Indices = ib;

                int passes = effect.Begin(FX.None);
                for (int i = 0; i < passes; i++)
                {

                    effect.BeginPass(i);
                    effect.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexOffset, 0, indices.Count / 3);
                    effect.EndPass();
                }
            }
            effect.End();

        }


        private void SetUpBuffers()
        {
            indices.Clear();
            int indexOffset = 0;

            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i].LifeBar > 0)
                {
                    vertexOffset += 4;
                }
            }
            if (vertexOffset > 0)
            {

                if (vb!=null) vb.Dispose();
                vb = new VertexBuffer(device, VertexPositionNormalColor.SizeInBytes * vertexOffset, Usage.None, VertexPositionNormalColor.Format, Pool.Default);
                vertexOffset = 0;
                using (DataStream stream = vb.Lock(0, 0, LockFlags.None))
                {
                    for (int i = 0; i < particles.Count; i++)
                    {
                        if (particles[i].LifeBar > 0)
                        {

                            float angle = (float)Math.PI / 3.0f;

                            Matrix billBoard = Matrix.Translation(-particles[i].Position) * CreateTransform(particles[i].Velocity) * Matrix.Translation(particles[i].Position);
                            Vector3 v1 = Vector3.TransformCoordinate(
                                new Vector3(
                                    particles[i].Position.X,
                                    particles[i].Position.Y + particleSize * 2.0f,
                                    particles[i].Position.Z),
                                billBoard);
                            Vector3 v2 = Vector3.TransformCoordinate(
                                new Vector3(
                                    particles[i].Position.X + particleSize * (float)Math.Sin(angle),
                                    particles[i].Position.Y - particleSize * (float)Math.Cos(angle),
                                    particles[i].Position.Z),
                                billBoard);
                            Vector3 v3 = Vector3.TransformCoordinate(
                                new Vector3(
                                    particles[i].Position.X - particleSize * (float)Math.Sin(angle) * (float)Math.Sin(angle / 2.0f),
                                    particles[i].Position.Y - particleSize * (float)Math.Cos(angle),
                                    particles[i].Position.Z + particleSize * (float)Math.Sin(angle) * (float)Math.Cos(angle / 2.0f)),
                                billBoard);
                            Vector3 v4 = Vector3.TransformCoordinate(
                                new Vector3(
                                    particles[i].Position.X - particleSize * (float)Math.Sin(angle) * (float)Math.Sin(angle / 2.0f),
                                    particles[i].Position.Y - particleSize * (float)Math.Cos(angle),
                                    particles[i].Position.Z - particleSize * (float)Math.Sin(angle) * (float)Math.Cos(angle / 2.0f)),
                                billBoard);


                            Vector4 v11 = new Vector4(v1 * positionScaleParam, 1.0f);
                            VertexPositionNormalColor vertex1 = new VertexPositionNormalColor(v1, Vector3.TransformNormal(new Vector3(0, 1, 0), billBoard), (int)particles[i].ColorARGB);
                            stream.Write(vertex1);

                            Vector4 v22 = new Vector4(v2 * positionScaleParam, 1.0f);
                            VertexPositionNormalColor vertex2 = new VertexPositionNormalColor(v2, Vector3.TransformNormal(v2 - particles[i].Position, billBoard), (int)particles[i].ColorARGB);
                            stream.Write(vertex2);

                            Vector4 v33 = new Vector4(v3 * positionScaleParam, 1.0f);
                            VertexPositionNormalColor vertex3 = new VertexPositionNormalColor(v3, Vector3.TransformNormal(v3 - particles[i].Position, billBoard), (int)particles[i].ColorARGB);
                            stream.Write(vertex3);

                            Vector4 v44 = new Vector4(v4 * positionScaleParam, 1.0f);
                            VertexPositionNormalColor vertex4 = new VertexPositionNormalColor(v4, Vector3.TransformNormal(v4 - particles[i].Position, billBoard), (int)particles[i].ColorARGB);
                            stream.Write(vertex4);

                            indices.AddRange(new int[] { vertexOffset, vertexOffset + 1, vertexOffset + 2, vertexOffset, vertexOffset + 2, vertexOffset + 3, vertexOffset, vertexOffset + 1, vertexOffset + 3, vertexOffset + 1, vertexOffset + 2, vertexOffset + 3, });

                            vertexOffset += 4;
                            indexOffset += 12;

                        }
                    }
                    vb.Unlock();
                }
            }


            if (vertexOffset > 0)
            {
                if (ib!=null) ib.Dispose(); 
                ib = new IndexBuffer(device, sizeof(int) * indices.Count, Usage.None, Pool.Default, false);
                using (DataStream stream = ib.Lock(0, 0, LockFlags.None))
                {
                    for (int i = 0; i < indices.Count; i++)
                    {
                        stream.Write(indices[i]);
                    }
                    ib.Unlock();
                }
            }

        }

        

        private Matrix CreateTransform(Vector3 velocity)
        {
            Vector3 res = Vector3.Lerp(new Vector3(0, 1, 0), Vector3.Normalize(velocity), 0.5f);
            res.Normalize();
            return Matrix.RotationAxis(res, (float)Math.PI);
        }

        private Vector3 GeneratePosition()
        {
            Vector3 tempVector = new Vector3((float)(r.NextDouble()) * (array.GetLength(0) - 1 - numericalStep * 3f) + numericalStep * 1.5f, (float)(r.NextDouble()) * (array.GetLength(1) - 1 - numericalStep * 3f) + numericalStep * 1.5f, (float)(r.NextDouble()) * (array.GetLength(2) - 1 - numericalStep * 3f) + numericalStep * 1.5f);
            while (MathHelper.GetValue(tempVector, array) < (float)(r.NextDouble() * (max - min) + min))
            {
                tempVector = new Vector3((float)(r.NextDouble()) * (array.GetLength(0) - 1 - numericalStep * 3f) + numericalStep * 1.5f, (float)(r.NextDouble()) * (array.GetLength(1) - 1 - numericalStep * 3f) + numericalStep * 1.5f, (float)(r.NextDouble()) * (array.GetLength(2) - 1 - numericalStep * 3f) + numericalStep * 1.5f);
            }
            return tempVector;
        }

        private void SetCamera()
        {
            float cameraScale = 2.0f;
            camera.Location = cameraScale * new Vector3(array.GetLength(0), array.GetLength(1), array.GetLength(2));
            camera.Target = new Vector3(array.GetLength(0) / 2f, array.GetLength(1) / 2f, array.GetLength(2) / 2f);
            camera.Up = new Vector3(0, 1, 0);

            camera.NearPlane = 0.1f;
            camera.FarPlane = 1000.0f;
            camera.FieldOfView = (float)Math.PI / 4.0f;
            camera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;

            effect.SetValue("world", Matrix.Identity);
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

        #region IVisualization3DProvider Members

        public void Initialize()
        {
            if (effect == null)
            {
                effect = Effect.FromStream(device, this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.Visualization3D.Shaders.PerPixelLightning.fx"), ShaderFlags.None);
            }
            
            particles.Clear();
            for (int i = 0; i < particleNum; i++)
            {
                Vector3 position = GeneratePosition();

                while (float.IsNaN(position.X) || position.X - gradientStep < 0 || position.Y - gradientStep < 0 || position.Z - gradientStep < 0 || position.X + gradientStep > array.GetLength(0) - 1 || position.Y + gradientStep > array.GetLength(1) - 1 || position.Z + gradientStep > array.GetLength(2) - 1)
                {
                    position = GeneratePosition();
                }

                Vector3 backPosition = new Vector3(position.X - numericalStep, position.Y, position.Z);
                Vector3 frontPosition = new Vector3(position.X + numericalStep, position.Y, position.Z);
                float velocityX = MathHelper.GetPatrialDerivation(MathHelper.GetValue(frontPosition, array), MathHelper.GetValue(backPosition, array), numericalStep);

                Vector3 bottomPosition = new Vector3(position.X, position.Y - numericalStep, position.Z);
                Vector3 topPosition = new Vector3(position.X, position.Y + numericalStep, position.Z);
                float velocityY = MathHelper.GetPatrialDerivation(MathHelper.GetValue(topPosition, array), MathHelper.GetValue(bottomPosition, array), numericalStep);

                Vector3 rightPosition = new Vector3(position.X, position.Y, position.Z + numericalStep);
                Vector3 leftPosition = new Vector3(position.X, position.Y, position.Z - numericalStep);
                float velocityZ = MathHelper.GetPatrialDerivation(MathHelper.GetValue(rightPosition, array), MathHelper.GetValue(leftPosition, array), numericalStep);

                Vector3 velocity = new Vector3(velocityX, velocityY, velocityZ);

                velocity.Normalize();

                this.particles.Add(new Particle(position, -velocity, RgbPalette.GetColor(MathHelper.GetValue(position, array), max, min, missingValue), (float)(r.NextDouble() * 100) + 50));
            }

            SetUpBuffers();
            SetCamera();
        }

        public void Draw(float time)
        {
            this.Draw();
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                if (value)
                    Initialize();
            }
        }

        public Camera Camera
        {
            get { return this.camera; ; }
        }

        public float Max
        {
            get { return max; }
        }

        public float Min
        {
            get { return min; }
        }

        #endregion
    }
}
