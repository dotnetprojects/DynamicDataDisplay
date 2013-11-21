using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Microsoft.Research.Visualization3D.MainLoops;

namespace Microsoft.Research.Visualization3D.Particles
{
    public class Projectile
    {
        const float trailParticlesPerSecond = 200;
        const int numExplosionParticles = 30;
        const int numExplosionSmokeParticles = 50;
        float projectileLifespan = 0.5f;
        const float sidewaysVelocityRange = 60;
        const float verticalVelocityRange = 40;
        const float gravity = 15;

        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;
        ParticleSystem projectileParticleSystem;

        ParticleEmitter trailEmitter;

        Vector3 position;
        Vector3 velocity;
        float age;

        float elapsedTime = 0;

        static Random random = new Random();

        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Projectile(ParticleSystem explosionParticles,
                          ParticleSystem explosionSmokeParticles,
                          ParticleSystem projectileTrailParticles,
                          Vector3 startPosition,
                          float lifeSpan)
        {
            this.explosionParticles = explosionParticles;
            this.explosionSmokeParticles = explosionSmokeParticles;
            this.projectileParticleSystem = projectileTrailParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            position = startPosition;

            projectileLifespan = lifeSpan;

            velocity.X = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            velocity.Y = (float)(random.NextDouble() - 0.5) * verticalVelocityRange;
            velocity.Z = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;

            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(projectileTrailParticles,
                                               trailParticlesPerSecond, position);
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(TimeEntity timeEntity)
        {
            elapsedTime = (float)timeEntity.ElapsedTime.TotalSeconds;

            // Simple projectile physics.
            position += velocity * elapsedTime;
            velocity.Y -= elapsedTime * gravity;
            age += elapsedTime;

            // Update the particle emitter, which will create our particle trail.
            trailEmitter.Update(timeEntity, position);

            // If enough time has passed, explode! Note how we pass our velocity
            // in to the AddParticle method: this lets the explosion be influenced
            // by the speed and direction of the projectile which created it.
            if (age > projectileLifespan)
            {
                for (int i = 0; i < numExplosionParticles; i++)
                    explosionParticles.QuerieNewParticle(position, velocity);

                for (int i = 0; i < numExplosionSmokeParticles; i++)
                    explosionSmokeParticles.QuerieNewParticle(position, velocity);

                return false;
            }

            return true;
        }
    }
}
