using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SlimDX;

namespace Microsoft.Research.Visualization3D.Auxilaries
{
    static class DDS
    {
        static readonly uint DDSD_CAPS = 0x00000001;
        static readonly uint DDSD_HEIGHT = 0x00000002;
        static readonly uint DDSD_WIDTH = 0x00000004;
        static readonly uint DDSD_PITCH = 0x00000008;
        static readonly uint DDSD_PIXELFORMAT = 0x00001000;
        static readonly uint DDSD_MIPMAPCOUNT = 0x00020000;
        static readonly uint DDSD_LINEARSIZE = 0x00080000;
        static readonly uint DDSD_DEPTH = 0x00800000;
        static readonly uint DDPF_ALPHAPIXELS = 0x00000001;
        static readonly uint DDPF_FOURCC = 0x00000004;
        static readonly uint DDPF_RGB = 0x00000040;
        static readonly uint DDSCAPS_COMPLEX = 0x00000008;
        static readonly uint DDSCAPS_TEXTURE = 0x00001000;
        static readonly uint DDSCAPS_MIPMAP = 0x00400000;
        static readonly uint DDSCAPS2_CUBEMAP = 0x00000200;
        static readonly uint DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400;
        static readonly uint DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800;
        static readonly uint DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000;
        static readonly uint DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000;
        static readonly uint DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000;
        static readonly uint DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000;
        static readonly uint DDSCAPS2_VOLUME = 0x00200000;

        public static byte[] CreateHeader(uint width, uint height, uint depth)
        {
            uint[] header = new uint[32];
            header[0] = 542326876;
            header[1] = 124;
            header[2] = DDSD_CAPS | DDSD_PIXELFORMAT | DDSD_WIDTH | DDSD_HEIGHT | DDSD_DEPTH | DDSD_PITCH;
            header[3] = height;
            header[4] = width;
            header[5] = width * 4;
            header[6] = depth;
            header[19] = 32;
            header[20] = DDPF_RGB | DDPF_ALPHAPIXELS;
            header[22] = 8;
            header[23] = 0xff0000;
            header[24] = 0xff00;
            header[25] = 0xff;
            header[26] = 0xff000000;
            header[27] = DDSCAPS_TEXTURE | DDSCAPS_COMPLEX;
            header[28] = DDSCAPS2_VOLUME;

            return ConvertToByteArray(header);
        }

        private static byte[] ConvertToByteArray(uint[] array)
        {
            
            byte[] result = new byte[array.Length * 4];
            using (DataStream ds = new DataStream(result, true, true))
            {
                ds.WriteRange(array);
            }
            /*
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (byte)(array[i] >> (8 * 3));
                result[i + 1] = (byte)(array[i] >> (8 * 2));
                result[i + 2] = (byte)(array[i] >> 8);
                result[i + 3] = (byte)(array[i]);

            }*/
            return result;
        }
    }
}


