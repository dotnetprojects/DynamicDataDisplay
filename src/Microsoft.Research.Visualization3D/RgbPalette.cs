using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Microsoft.Research.Visualization3D
{
    class RgbPalette
    {
        public static Color3 GetColor(double value, double max, double min, double missingValue)
        {
            //Color3 black = new Color3(0, 0, 0);
            //Color3 blue = new Color3(0, 0, 1);
            //Color3 green = new Color3(0, 1, 0);
            //Color3 red = new Color3(1, 0, 0);

            //Color sdBlue = Color.Blue;
            //Color sdRed = Color.Red;
            //Color sdGreen = Color.Green;

            //if (value == missingValue)
            //    return black;

            //if (value > max) value = max;
            //if (value < min) value = min;


            //float alpha = (float)((2 * value - min - max) / (max - min));

            //if (alpha < 0)
            //{
            //    byte r = (byte)(Color.Blue.R * Math.Abs(alpha) + Color.Green.R * (1 - Math.Abs(alpha)));
            //    byte g = (byte)(Color.Blue.G * Math.Abs(alpha) + Color.Green.G * (1 - Math.Abs(alpha)));
            //    byte b = (byte)(Color.Blue.B * Math.Abs(alpha) + Color.Green.B * (1 - Math.Abs(alpha)));

            //    return new Color3(r / 255f, g / 255f, b / 255f);

            //    /*return new Color3(
            //        (blue.Red * Math.Abs(alpha) + green.Red * (1 - Math.Abs(alpha))),
            //        (blue.Green * Math.Abs(alpha) + green.Green * (1 - Math.Abs(alpha))),
            //        (blue.Blue * Math.Abs(alpha) + green.Blue * (1 - Math.Abs(alpha))));*/
            //}
            //else
            //{
            //    byte r = (byte)(Color.Red.R * alpha + Color.Green.R * (1 - alpha));
            //    byte g = (byte)(Color.Red.G * alpha + Color.Green.G * (1 - alpha));
            //    byte b = (byte)(Color.Red.B * alpha + Color.Green.B * (1 - alpha));

            //    return new Color3(r / 255f, g / 255f, b / 255f);
            //    /*return new Color3(
            //                   (red.Red * alpha + green.Red * (1 - alpha)),
            //                   (red.Green * alpha + green.Green * (1 - alpha)),
            //                   (red.Blue * alpha + green.Blue * (1 - alpha)));*/

            //}
            var bytes = GetColorBytes(value, max, min, missingValue);
            return new Color3(bytes[1] / 255f, bytes[2] / 255f, bytes[3] / 255f);
        }

        public static byte[] GetColorBytes(double value, double max, double min, double missingValue)
        {
            Color red = Color.Red;
            Color green = Color.Green;
            Color blue = Color.Blue;
            
            
            byte[] result = new byte[] { 0, 0, 0, 0 };

            if (value == missingValue || value < min )
            {
                return result;
            }

            if (value > max) value = max;
            if (value < min) value = min;

            //float alpha = (float)((2 * value - min - max) / (max - min));
            result[0] = 255;// (byte)(255 * (alpha - min) / (max - min));

            double middle = (max + min) / 2.0;
            if (value > middle)
            {
                double alpha = (value - middle) / (max - middle);
                result[1] = (byte)(Color.Red.R * alpha + green.R * (1 - alpha));
                result[2] = (byte)(Color.Red.G * alpha + green.G * (1 - alpha));
                result[3] = (byte)(Color.Red.B * alpha + green.B * (1 - alpha));

                return result;
            }
            else
            {
                double alpha = (middle - value) / (middle - min);

                result[1] = (byte)(Color.Blue.R * alpha + green.R * (1 - alpha));
                result[2] = (byte)(Color.Blue.G * alpha + green.G * (1 - alpha));
                result[3] = (byte)(Color.Blue.B * alpha + green.B * (1 - alpha));

                return result;
            }


            /*
            if (alpha < 0)
            {
                result[1] = (byte)(Color.Blue.R * Math.Abs(alpha) + Color.Green.R * (1 - Math.Abs(alpha)));
                result[2] = (byte)(Color.Blue.G * Math.Abs(alpha) + Color.Green.G * (1 - Math.Abs(alpha)));
                result[3] = (byte)(Color.Blue.B * Math.Abs(alpha) + Color.Green.B * (1 - Math.Abs(alpha)));

                return result;

            }
            else
            {
                result[1] = (byte)(Color.Red.R * alpha + Color.Green.R * (1 - alpha));
                result[2] = (byte)(Color.Red.G * alpha + Color.Green.G * (1 - alpha));
                result[3] = (byte)(Color.Red.B * alpha + Color.Green.B * (1 - alpha));

                return result;

            }*/
        }

        public static long ColorARGB(Color3 Color)
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
