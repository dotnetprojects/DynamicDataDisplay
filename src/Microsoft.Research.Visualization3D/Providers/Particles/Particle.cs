using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Microsoft.Research.Visualization3D.Particles
{
    class Particle
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Color3 Color;
        public float LifeBar;
        public ParticleType ParticleType;

        public Particle(Vector3 Position, Vector3 Velocity, Color3 Color, float LifeBar)
        {
            this.Position = Position;
            this.Velocity = Velocity;
            this.Color = Color;
            this.LifeBar = LifeBar;
            this.ParticleType = ParticleType.Cloud;
        }

        public Particle(Vector3 Position, Vector3 Velocity, Color3 Color, float LifeBar, ParticleType type)
        {
            this.Position = Position;
            this.Velocity = Velocity;
            this.Color = Color;
            this.LifeBar = LifeBar;
            this.ParticleType = type;
        }

        public long ColorARGB
        {
            get 
            {
                byte red = (byte)(Color.Red * 255);
                byte green = (byte)(Color.Green * 255);
                byte blue = (byte)(Color.Blue * 255);
                byte alpha = (byte)0;
                {
                    return (long)(((ulong)((((red << 0x10) | (green << 8)) | blue) | (alpha << 0x18))) & 0xffffffffL);
                }

            }
        }
    }

    enum ParticleType
    {
        Launcher,
        Cloud
    }
}
