using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace MultipleDataPlotting
{
	class AnimatedDataSource
	{
		static Random rnd = new Random();

		const int length = 100;

		public double[] x = new double[length];
		public double[] y = new double[length];

		private EnumerableDataSource<double> xDS;
		private CompositeDataSource ds;
		public CompositeDataSource DataSource
		{
			get { return ds; }
		}

		double phase = 0;
		/// <summary>
		/// Should be called to update phase of sine and therefore all the data.
		/// </summary>
		public void Update()
		{
			for (int i = 0; i < x.Length; i++)
			{
				x[i] = 2 * Math.PI * i / x.Length + phase;
				y[i] = Math.Sin(x[i]);
			}

			xDS.RaiseDataChanged();

			phase += 0.03;
		}

		public AnimatedDataSource()
		{
			// randomize start phase
			phase = rnd.NextDouble() * 3;

			// create data source
			xDS = new EnumerableDataSource<double>(x);
			xDS.SetXMapping(X => X);
			EnumerableDataSource<double> yDS = new EnumerableDataSource<double>(y);
			yDS.SetYMapping(Y => Y);
			Update();

			ds = new CompositeDataSource(xDS, yDS);
		}
	}
}
