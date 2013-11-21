using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public class AdjustablePointSelector : PointSelector
	{
		#region Properties

		#region AllowedRect property

		public DataRect AllowedRegion
		{
			get { return (DataRect)GetValue(AllowedRegionProperty); }
			set { SetValue(AllowedRegionProperty, value); }
		}

		public static readonly DependencyProperty AllowedRegionProperty = DependencyProperty.Register(
		  "AllowedRegion",
		  typeof(DataRect),
		  typeof(AdjustablePointSelector),
		  new FrameworkPropertyMetadata(DataRect.Infinite, OnAllowedRegionChanged));

		private static void OnAllowedRegionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AdjustablePointSelector owner = (AdjustablePointSelector)d;
			owner.OnAllowedRegionChanged();
		}

		private void OnAllowedRegionChanged()
		{
			// removing points that are outside of allowed region
			var region = AllowedRegion;
			var pointsToRemove = Points.Where(p => !region.Contains(p)).ToArray();

			foreach (var point in pointsToRemove)
			{
				Points.Remove(point);
			}

			addPointCommand.RaiseCanExecuteChanged();
		}

		#endregion // end of AllowedRect property

		#region MaxPointsCount property

		public int MaxPointsCount
		{
			get { return (int)GetValue(MaxPointsCountProperty); }
			set { SetValue(MaxPointsCountProperty, value); }
		}

		public static readonly DependencyProperty MaxPointsCountProperty = DependencyProperty.Register(
		  "MaxPointsCount",
		  typeof(int),
		  typeof(AdjustablePointSelector),
		  new FrameworkPropertyMetadata(Int32.MaxValue, OnMaxPointsCountChanged), ValidateMaxPointsCount);

		private static bool ValidateMaxPointsCount(object value)
		{
			int count = (int)value;

			return count >= 0;
		}

		private static void OnMaxPointsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AdjustablePointSelector owner = (AdjustablePointSelector)d;
			owner.OnMaxPointsCountChanged();
		}

		private void OnMaxPointsCountChanged()
		{
			// remove all extra points
			int maxCount = MaxPointsCount;
			int index = maxCount;
			while (index < Points.Count)
			{
				Points.RemoveAt(index);
			}

			addPointCommand.RaiseCanExecuteChanged();
			removePointCommand.RaiseCanExecuteChanged();
		}

		#endregion // end of MaxPointsCount property

		#endregion // end of Properties

		#region Commands

		protected override bool AddPointCanExecute(object parameter)
		{
			var result = base.AddPointCanExecute(parameter);

			if (parameter is Point)
			{
				Point pointToAdd = (Point)parameter;
				// point should be inside of allowed region to be allowed to be added
				result &= AllowedRegion.Contains(pointToAdd);
				result &= Points.Count < MaxPointsCount;
			}

			return result;
		}

		#endregion // end of Commands

		protected override void OnPoint_PositionChanged(object sender, PositionChangedEventArgs e)
		{
			DraggablePoint marker = (DraggablePoint)sender;

			// adjusting position to fit inside allowed region
			var region = AllowedRegion;
			var position = e.Position;
			if (position.X < region.XMin)
				position.X = region.XMin;
			if (position.Y < region.YMin)
				position.Y = region.YMin;
			if (position.X > region.XMax)
				position.X = region.XMax;
			if (position.Y > region.YMax)
				position.Y = region.YMax;

			marker.Position = position;
			marker.DataContext = position;

			var index = DevMarkerChart.GetIndex(marker);
			if (0 <= index && index < Points.Count && !ProtectedPoints.Changing)
			{
				Points[index] = position;
			}
		}
	}
}
