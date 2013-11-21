using System;
using System.Collections.Generic;
using Microsoft.Research.Visualization3D.VertexStructures;
using SlimDX.Direct3D9;
using Microsoft.Research.Visualization3D.CameraUtilities;
using SlimDX;
using Microsoft.Research.Visualization3D.Auxilaries;
using System.Windows.Threading;
using System.Diagnostics;
using Microsoft.Research.Visualization3D.MainLoops;
using System.Threading;

namespace Microsoft.Research.Visualization3D.Isosurfaces
{
    public partial class Metaball : DrawableComponent
    {
        List<MetaballVertex> vertexList;
        int primitiveCount;

        int iDataSetSizeX;
        int iDataSetSizeY;
        int iDataSetSizeZ;

        VertexBuffer vb;

        public Metaball(DX3DHost host, Visualization3DDataSource dataSource) :
            base(host, dataSource)
        {
            iDataSetSizeX = dataSource.DisplayData.GetLength(0);
            iDataSetSizeY = dataSource.DisplayData.GetLength(1);
            iDataSetSizeZ = dataSource.DisplayData.GetLength(2);
            this.WpfDispatcher = Dispatcher.CurrentDispatcher;
        }

        public override void Initialize()
        {
            
            if (effect == null)
            {
                effect = Effect.FromStream(device, this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.Visualization3D.Shaders.PerPixelLightning.fx"), ShaderFlags.None);
            }
            SetCamera();
            vertexList = new List<MetaballVertex>();
            Color3 mbColor = RgbPalette.GetColor(fTargetValue, dataSource.Maximum, dataSource.Minimum, dataSource.MissingValue);
            effect.SetValue("mbColor", new Vector4(mbColor.Red, mbColor.Green, mbColor.Blue, 1.0f));

            MarchingCubes();
            Completed();

            base.Initialize();
        }

        //Calling Cube building function for each voxel
        public void MarchingCubes()
        {
            vertexList = new List<MetaballVertex>();

            for (int iX = 0; iX < iDataSetSizeX - 1; iX++)
                for (int iY = 0; iY < iDataSetSizeY - 1; iY++)
                    for (int iZ = 0; iZ < iDataSetSizeZ - 1; iZ++)
                    {
                        vMarchCube(iX, iY, iZ, 1.0f);
                    }
        }

        float fGetOffset(float fValue1, float fValue2, float fValueDesired)
        {
            double fDelta = fValue2 - fValue1;

            if (fDelta == 0.0)
            {
                return 0.5f;
            }
            return (fValueDesired - fValue1) / (float)fDelta;
        }

        float fSample(float fX, float fY, float fZ)
        {
            return MathHelper.GetValue(new Vector3(fX, fY, fZ), dataSource.DisplayData);
        }

        //vGetColor fins color via point and normal
        void vGetColor(ref Vector3 rfColor, Vector3 rfPosition, Vector3 rfNormal)
        {
            float fX = rfNormal.X;
            float fY = rfNormal.Y;
            float fZ = rfNormal.Z;
            rfColor.X = (float)((fX > 0.0 ? fX : 0.0) + (fY < 0.0 ? -0.5 * fY : 0.0) + (fZ < 0.0 ? -0.5 * fZ : 0.0));
            rfColor.Y = (float)((fY > 0.0 ? fY : 0.0) + (fZ < 0.0 ? -0.5 * fZ : 0.0) + (fX < 0.0 ? -0.5 * fX : 0.0));
            rfColor.Z = (float)((fZ > 0.0 ? fZ : 0.0) + (fX < 0.0 ? -0.5 * fX : 0.0) + (fY < 0.0 ? -0.5 * fY : 0.0));
        }

        //vGetNormal() Calculates normal via gradient to point
        void vGetNormal(ref Vector3 rfNormal, float fX, float fY, float fZ)
        {
            rfNormal.X = MathHelper.GetValue(new Vector3(fX - 0.01f, fY, fZ), dataSource.DisplayData) - MathHelper.GetValue(new Vector3(fX + 0.01f, fY, fZ), dataSource.DisplayData);
            rfNormal.Y = MathHelper.GetValue(new Vector3(fX, fY - 0.01f, fZ), dataSource.DisplayData) - MathHelper.GetValue(new Vector3(fX, fY + 0.01f, fZ), dataSource.DisplayData);
            rfNormal.Z = MathHelper.GetValue(new Vector3(fX, fY, fZ - 0.01f), dataSource.DisplayData) - MathHelper.GetValue(new Vector3(fX, fY, fZ + 0.01f), dataSource.DisplayData);
            if (rfNormal.Length() > 0)
                rfNormal.Normalize();
        }

        //Find part of Surface for current voxel
        void vMarchCube(float fX, float fY, float fZ, float fScale)
        {


            int iCorner, iVertex, iVertexTest, iEdge, iTriangle, iFlagIndex, iEdgeFlags;
            float fOffset;
            Vector3 sColor = Vector3.Zero;
            float[] afCubeValue = new float[8];
            Vector3[] asEdgeVertex = new Vector3[12];
            Vector3[] asEdgeNorm = new Vector3[12];

            //Find value in voxel's knots
            for (iVertex = 0; iVertex < 8; iVertex++)
            {
                afCubeValue[iVertex] = (float)dataSource.DisplayData[(int)(fX + a2fVertexOffset[iVertex, 0]), (int)(fY + a2fVertexOffset[iVertex, 1]), (int)(fZ + a2fVertexOffset[iVertex, 2])];
                if (afCubeValue[iVertex] == dataSource.MissingValue) return;
            }

            //Checking intersections via table
            iFlagIndex = 0;
            for (iVertexTest = 0; iVertexTest < 8; iVertexTest++)
            {
                if (afCubeValue[iVertexTest] <= fTargetValue)
                    iFlagIndex |= 1 << iVertexTest;
            }

            //Finally, get our surface configuration
            iEdgeFlags = aiCubeEdgeFlags[iFlagIndex];

            //if our surface doesn't intersect voxel
            if (iEdgeFlags == 0)
            {
                return;
            }

            //Building vertices
            for (iEdge = 0; iEdge < 12; iEdge++)
            {
                if ((iEdgeFlags & (1 << iEdge)) != 0)
                {
                    fOffset = fGetOffset(afCubeValue[a2iEdgeConnection[iEdge, 0]],
                               afCubeValue[a2iEdgeConnection[iEdge, 1]], fTargetValue);

                    asEdgeVertex[iEdge].X = (float)(fX + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 0] + fOffset * a2fEdgeDirection[iEdge, 0]) * fScale);
                    asEdgeVertex[iEdge].Y = (float)(fY + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 1] + fOffset * a2fEdgeDirection[iEdge, 1]) * fScale);
                    asEdgeVertex[iEdge].Z = (float)(fZ + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 2] + fOffset * a2fEdgeDirection[iEdge, 2]) * fScale);

                    vGetNormal(ref asEdgeNorm[iEdge], asEdgeVertex[iEdge].X, asEdgeVertex[iEdge].Y, asEdgeVertex[iEdge].Z);
                }
            }


            //Building triangles
            for (iTriangle = 0; iTriangle < 5; iTriangle++)
            {
                if (a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle] < 0)
                    break;

                for (iCorner = 0; iCorner < 3; iCorner++)
                {
                    iVertex = a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle + iCorner];

                    vertexList.Add(new MetaballVertex(
                        asEdgeVertex[iVertex],
                        asEdgeNorm[iVertex]
                        ));

                }

            }


        }

        protected override void SetCamera()
        {
            camera.CameraScale = 12.0f;
            
            float cameraScale = 2.0f;
            camera.Location = cameraScale * new Vector3(dataSource.DisplayData.GetLength(0), dataSource.DisplayData.GetLength(1), dataSource.DisplayData.GetLength(2));
            camera.Target = new Vector3(dataSource.DisplayData.GetLength(0) / 2f, dataSource.DisplayData.GetLength(1) / 2f, dataSource.DisplayData.GetLength(2) / 2f);
            camera.Up = new Vector3(0, 1, 0);

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

        public override void Draw(TimeEntity timeEntity)
        {
            if (primitiveCount > 0)
            {
                effect.Technique = new EffectHandle("PerPixelDiffuseAndPhongMetaball");
                device.SetStreamSource(0, vb, 0, MetaballVertex.SizeInBytes);
                device.VertexFormat = MetaballVertex.Format;
                int passes = effect.Begin();
                for (int i = 0; i < passes; i++)
                {
                    effect.BeginPass(i);
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, primitiveCount);
                    effect.EndPass();
                }
                effect.End();
            }
        }

        public override void Update(TimeEntity timeEntity)
        {
            effect.SetValue("world", Matrix.Identity);
            effect.SetValue("view", camera.ViewMatrix);
            effect.SetValue("projection", camera.ProjectionMatrix);

            effect.SetValue("cameraPosition", camera.Location);
            effect.SetValue("lightPosition", camera.Location);
        }
    }
}
