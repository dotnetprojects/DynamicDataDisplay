using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace AxesApp
{
    public class FiveMinutesMaxTicksProvider : DateTimeTicksProvider
    {
        protected override DateTime[] ModifyTicks(DateTime[] ticks, object info)
        {
            if (info.Equals(DifferenceIn.Minute))
            {
                var result = ticks.Where(t => t.Minute % 5 == 0).ToArray();
                var largerResult = new DateTime[result.Length + 2];
                largerResult[0] = result[0].AddMinutes(-5);
                result.CopyTo(largerResult, 1);
                largerResult[largerResult.Length - 1] = result[result.Length - 1].AddMinutes(5);

                return largerResult;
            }

            return base.ModifyTicks(ticks, info);
        }
    }
}
