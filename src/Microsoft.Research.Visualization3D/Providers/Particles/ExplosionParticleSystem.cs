using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using Microsoft.Research.Visualization3D.CameraUtilities;

namespace Microsoft.Research.Visualization3D.Particles
{
    class ExplosionParticleSystem : ParticleSystem
    {
        public ExplosionParticleSystem(DX3DHost host, Visualization3DDataSource dataSource) : 
            base(host, dataSource)
        {

        }
        
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "smoke.png";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(2.5);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 20;
            settings.MaxHorizontalVelocity = 30;

            settings.MinVerticalVelocity = -20;
            settings.MaxVerticalVelocity = 20;

            settings.EndVelocity = 0;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 10;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;

        }
    }
}
