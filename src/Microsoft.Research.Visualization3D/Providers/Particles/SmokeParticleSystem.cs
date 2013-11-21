using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using Microsoft.Research.Visualization3D.CameraUtilities;

namespace Microsoft.Research.Visualization3D.Particles
{
    class SmokeParticleSystem : ParticleSystem
    {
        public SmokeParticleSystem(DX3DHost host, Visualization3DDataSource dataSource) :
            base(host, dataSource)
        {
        }
        
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "smoke.png";

            settings.MaxParticles = 200;

            settings.Duration = TimeSpan.FromSeconds(4);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 50;

            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 50;

            settings.Gravity = new Vector3(0, -20, 0);

            settings.EndVelocity = 0;

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 10;
        }
    }
}
