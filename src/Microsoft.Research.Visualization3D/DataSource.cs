using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Visualization3D.Auxilaries;

namespace Microsoft.Research.Visualization3D
{
    public class Visualization3DDataSource
    {
        public double[, ,] DisplayData
        {
            get;
            private set;
        }
        public double MissingValue
        {
            get;
            private set;
        }
        public float Maximum
        {
            get;
            set;
        }
        public float Minimum
        {
            get;
            set;
        }
        public float CurrentValue
        {
            get;
            set;
        }
        public Visualization3DDataSource(double[,,] data, double missingValue)
        {
            DisplayData = data;
            MissingValue = missingValue;
            Maximum = (float)MathHelper.FindMax(data, missingValue);
            Minimum = (float)MathHelper.FindMin(data, missingValue);
            CurrentValue = (Maximum + Minimum) / 2.0f;
        }

    }
}
