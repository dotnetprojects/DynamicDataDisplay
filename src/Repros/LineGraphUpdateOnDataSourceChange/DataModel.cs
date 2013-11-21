using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace LineGraphUpdateOnDataSourceChange
{
    public class DataModel
    {
        public IPointDataSource DataSource { get; set; }
    }
}
