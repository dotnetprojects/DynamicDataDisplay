using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Visualization3D.CameraUtilities;
using SlimDX.Direct3D9;
using Microsoft.Research.Visualization3D.VertexStructures;
using SlimDX;
using Microsoft.Research.Visualization3D.Auxilaries;
using Microsoft.Research.Visualization3D.MainLoops;

namespace Microsoft.Research.Visualization3D.Particles
{
    /// <summary>
    /// Base class for advanced particle system
    /// </summary>
    public abstract class ParticleSystem : DrawableComponent
    {
        ParticleSettings settings;
        VertexBuffer vertexBuffer;

        List<ParticleVertex> particlesList;
        List<Vector3> startPositions;
        List<float> times;

        float currentTime;
        int drawCounter;

        Texture texture;
        Random random;

        public ParticleSystem(DX3DHost host, Visualization3DDataSource dataSource) :
            base(host, dataSource)
        {
            this.random = new Random();
            this.settings = new ParticleSettings();
        }

        protected abstract void InitializeSettings(ParticleSettings settings);


        public override void Initialize()
        {
            InitializeSettings(settings);

            if (effect == null)
            {
                effect = Effect.FromStream(device, this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.Visualization3D.Shaders.ParticleEffect.fx"), ShaderFlags.None);
            }

            particlesList = new List<ParticleVertex>();
            startPositions = new List<Vector3>();
            times = new List<float>();

            effect.SetValue("Duration", (float)settings.Duration.TotalSeconds);
            effect.SetValue("DurationRandomness", settings.DurationRandomness);
            effect.SetValue("Gravity", settings.Gravity);
            effect.SetValue("EndVelocity", settings.EndVelocity);

            effect.SetValue("RotateSpeed", new Vector2(settings.MinRotateSpeed, settings.MaxRotateSpeed));
            effect.SetValue("StartSize", new Vector2(settings.MinStartSize, settings.MaxStartSize));
            effect.SetValue("EndSize", new Vector2(settings.MinEndSize, settings.MaxEndSize));

            texture = Texture.FromStream(device, this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.Visualization3D.Textures." + settings.TextureName), Usage.Dynamic, Pool.Default);

            effect.SetTexture(new EffectHandle("Texture"), texture);
            effect.Technique = new EffectHandle("RotatingParticles");

            base.Initialize();

            dataSource.Maximum = 0.1f;
            dataSource.Minimum = -0.1f;

        }

        public override void Draw(TimeEntity timeEntity)
        {
            if (particlesList.Count > 0)
            {
                SetupDevice();

                // Set the particle vertex buffer and vertex declaration.
                device.SetStreamSource(0, vertexBuffer, 0, ParticleVertex.SizeInBytes);
                device.VertexFormat = ParticleVertex.Format;


                // Activate the particle effect.
                int numPasses = effect.Begin();

                for (int i = 0; i < numPasses; i++)
                {
                    effect.BeginPass(i);

                    device.DrawPrimitives(PrimitiveType.PointList, 0, particlesList.Count);

                    effect.EndPass();
                }



                effect.End();

                // Reset a couple of the more unusual renderstates that we changed,
                // so as not to mess up any other subsequent drawing.
                device.SetRenderState(RenderState.AlphaFunc, Compare.Always);
                device.SetRenderState(RenderState.PointSpriteEnable, false);
                device.SetRenderState(RenderState.ZWriteEnable, true);
            }

            drawCounter++;
        }

        private void SetupDevice()
        {
            // Enable point sprites.
            device.SetRenderState(RenderState.PointSpriteEnable, true);
            device.SetRenderState(RenderState.PointSizeMax, 256f);

            // Set the alpha blend mode.
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            device.SetRenderState(RenderState.SourceBlend, settings.SourceBlend);
            device.SetRenderState(RenderState.DestinationBlend, settings.DestinationBlend);

            // Set the alpha test mode.
            device.SetRenderState(RenderState.AlphaTestEnable, true);
            device.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
            device.SetRenderState(RenderState.AlphaRef, 0);

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            device.SetRenderState(RenderState.ZEnable, true);
            device.SetRenderState(RenderState.ZWriteEnable, false);


            // Set an effect parameter describing the viewport size. This is needed
            // to convert particle sizes into screen space point sprite sizes.
            effect.SetValue("ViewportHeight", device.Viewport.Height);

            // Set an effect parameter describing the current time. All the vertex
            // shader particle animation is keyed off this value.
            effect.SetValue("CurrentTime", currentTime);
        }


        protected override void SetCamera()
        {
            camera.CameraScale = 12.0f;

            float cameraScale = 2.0f;
            camera.Location = cameraScale * new Vector3(dataSource.DisplayData.GetLength(0), dataSource.DisplayData.GetLength(1), dataSource.DisplayData.GetLength(2));
            camera.Target = new Vector3(dataSource.DisplayData.GetLength(0) / 2f, dataSource.DisplayData.GetLength(1) / 2f, dataSource.DisplayData.GetLength(2) / 2f);
            camera.Up = new Vector3(0, 1, 0);

            effect.SetValue("View", camera.ViewMatrix);
            effect.SetValue("Projection", camera.ProjectionMatrix);
        }

        public override void Update(TimeEntity timeEntity)
        {
            currentTime = (float)timeEntity.TotalTime.TotalMilliseconds / 10000.0f;

            //if (particlesList.Count > settings.MaxParticles)
            //    particlesList.Clear();

            int oldCount = particlesList.Count;
            for (int i = 0; i < oldCount; i++)
            {
                Vector3 newPosition = ComputeNewPosition(startPositions[i], particlesList[i].Velocity, (currentTime - times[i]) * (1 + settings.DurationRandomness));
                if (MathHelper.CheckPosition(newPosition, dataSource.DisplayData, dataSource.MissingValue) && (particlesList[i].Time < settings.Duration.TotalSeconds + times[i]))
                {
                    float value = MathHelper.GetValue(newPosition, dataSource.DisplayData, dataSource.MissingValue);
                    particlesList.Add(new ParticleVertex(newPosition, particlesList[i].Velocity, (int)RgbPalette.ColorARGB(RgbPalette.GetColor(value, dataSource.Maximum, dataSource.Minimum, dataSource.MissingValue)), particlesList[i].Time + (float)timeEntity.ElapsedTime.TotalSeconds));
                    startPositions.Add(startPositions[i]);
                    times.Add(times[i]);
                }
            }
            particlesList.RemoveRange(0, oldCount);
            startPositions.RemoveRange(0, oldCount);
            times.RemoveRange(0, oldCount);

            if (particlesList.Count > 0)
            {
                if (vertexBuffer != null)
                    vertexBuffer.Dispose();

                vertexBuffer = new VertexBuffer(device, ParticleVertex.SizeInBytes * Math.Max(10, particlesList.Count), Usage.WriteOnly | Usage.Points, ParticleVertex.Format, Pool.Default);
                
                using (DataStream ds = vertexBuffer.Lock(0, 0, LockFlags.None))
                {
                    ds.WriteRange(particlesList.ToArray());
                }
                vertexBuffer.Unlock();
            }

            effect.SetValue("View", camera.ViewMatrix);
            effect.SetValue("Projection", camera.ProjectionMatrix);

        }

        public void QuerieNewParticle(Vector3 position, Vector3 baseVelocity)
        {
            Vector3 velocity = new Vector3(baseVelocity.X, baseVelocity.Y, baseVelocity.Z);
            
            // Adjust the input velocity based on how much
            // this particle system wants to be affected by it.
            velocity *= settings.EmitterVelocitySensitivity;

            // Add in some random amount of horizontal velocity.
            float horizontalVelocity = MathHelper.Lerp(settings.MinHorizontalVelocity,
                                                       settings.MaxHorizontalVelocity,
                                                       (float)random.NextDouble());

            double horizontalAngle = random.NextDouble() * 2.0 * (float)Math.PI;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            // Add in some random amount of vertical velocity.
            velocity.Y += MathHelper.Lerp(settings.MinVerticalVelocity,
                                          settings.MaxVerticalVelocity,
                                          (float)random.NextDouble());
            
            if (MathHelper.CheckPosition(position, dataSource.DisplayData, dataSource.MissingValue))
            {
                this.particlesList.Add(new ParticleVertex(position, velocity, (int)RgbPalette.ColorARGB(RgbPalette.GetColor(MathHelper.GetValue(position, dataSource.DisplayData, dataSource.MissingValue), dataSource.Maximum, dataSource.Minimum, dataSource.MissingValue)), currentTime));
                this.startPositions.Add(position);
                this.times.Add(currentTime);
            }
            //else
            //{
                //this.particlesList.Add(new ParticleVertex(position, velocity, (int)RgbPalette.ColorARGB(new Color3(1,1,1)), currentTime));
                //this.startPositions.Add(position);
            //}
        }

        public override float CurrentValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private Vector3 ComputeNewPosition(Vector3 position, Vector3 velocity, float age)
        {
            Vector3 result = new Vector3(position.X, position.Y, position.Z);
            float startVelocity = velocity.Length();
            float endVelocity = settings.EndVelocity * startVelocity;
            float normalizedAge = MathHelper.Saturate(age / (float)settings.Duration.TotalSeconds);
            float velocityIntegral = startVelocity * normalizedAge + (endVelocity - startVelocity) * normalizedAge * normalizedAge / 2.0f;
            result += Vector3.Normalize(velocity) * velocityIntegral * (float)settings.Duration.TotalSeconds;
            result += settings.Gravity * age * normalizedAge;
            return result;
        }
    }
}
