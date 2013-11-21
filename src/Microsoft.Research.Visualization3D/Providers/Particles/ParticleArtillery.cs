using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Visualization3D.CameraUtilities;
using SlimDX.Direct3D9;
using SlimDX;
using Microsoft.Research.Visualization3D.MainLoops;

namespace Microsoft.Research.Visualization3D.Particles
{
    public class ParticleArtillery : DrawableComponent
    {
        DX3DHost host;
        Random r = new Random();

        List<Projectile> projectiles = new List<Projectile>();
        int numProjectiles = 150;

        ExplosionParticleSystem explosionParticleSystem;
        SmokeParticleSystem smokeParticleSystem;
        ProjectileParticleSystem projectileParticleSystem;

        public ParticleArtillery(DX3DHost host, Visualization3DDataSource dataSource) :
            base(host, dataSource)
        {
            this.host = host;
            explosionParticleSystem = new ExplosionParticleSystem(host, dataSource);
            smokeParticleSystem = new SmokeParticleSystem(host, dataSource);
            projectileParticleSystem = new ProjectileParticleSystem(host, dataSource);
        }


        public override void Initialize()
        {
            if (effect == null)
            {
                effect = Effect.FromStream(device, this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.Visualization3D.Shaders.ParticleEffect.fx"), ShaderFlags.None);
            }

            this.projectileParticleSystem.Initialize();
            this.smokeParticleSystem.Initialize();
            this.explosionParticleSystem.Initialize();

            for (int i = 0; i < numProjectiles; i++)
            {
                projectiles.Add(new Projectile(
                    explosionParticleSystem,
                    smokeParticleSystem,
                    projectileParticleSystem,
                    GeneratePosition(),
                    (float)r.NextDouble()));
            }

            base.Initialize();
        }

        protected override void SetCamera()
        {
            float cameraScale = 2.0f;
            camera.Location = cameraScale * new Vector3(dataSource.DisplayData.GetLength(0), dataSource.DisplayData.GetLength(1), dataSource.DisplayData.GetLength(2));
            camera.Target = new Vector3(dataSource.DisplayData.GetLength(0) / 2f, dataSource.DisplayData.GetLength(1) / 2f, dataSource.DisplayData.GetLength(2) / 2f);
            camera.Up = new Vector3(0, 1, 0);

            camera.NearPlane = 0.1f;
            camera.FarPlane = 1000.0f;
            camera.FieldOfView = (float)Math.PI / 4.0f;
            camera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;
        }


        public override void Draw(TimeEntity timeEntity)
        {
            this.projectileParticleSystem.Draw(timeEntity);
            this.explosionParticleSystem.Draw(timeEntity);
            this.smokeParticleSystem.Draw(timeEntity);
        }

        public override void Update(Microsoft.Research.Visualization3D.MainLoops.TimeEntity timeEntity)
        {
            for (int i = 0; i < numProjectiles; i++)
            {
                if (!projectiles[i].Update(timeEntity))
                {
                    projectiles[i] = new Projectile(
                    explosionParticleSystem,
                    smokeParticleSystem,
                    projectileParticleSystem,
                    GeneratePosition(),
                    (float)r.NextDouble());
                }

            }
            this.projectileParticleSystem.Update(timeEntity);
            this.explosionParticleSystem.Update(timeEntity);
            this.smokeParticleSystem.Update(timeEntity);

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

        private Vector3 GeneratePosition()
        {
            //return new Vector3(dataSource.DisplayData.GetLength(0) / 2.0f, dataSource.DisplayData.GetLength(1) / 2.0f, dataSource.DisplayData.GetLength(2) / 2.0f);
            return new Vector3((float)r.NextDouble() * dataSource.DisplayData.GetLength(0), (float)r.NextDouble() * dataSource.DisplayData.GetLength(1), (float)r.NextDouble() * dataSource.DisplayData.GetLength(2));
        }

    }
}
