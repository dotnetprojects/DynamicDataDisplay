using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Test.Common
{
	internal class PropertySetSystem
	{
		public PropertySetSystem()
		{
		}

		public void TrySetValue(object target, PropertyInfo property)
		{
			bool set = false;
			Debug.Indent();
			foreach (var value in GetValues(property))
			{
				set = true;
				try
				{
					property.SetValue(target, value, null);
				}
				catch (TargetInvocationException exc)
				{
					if (!(exc.InnerException is ArgumentException))
						throw;
				}
				Debug.WriteLine(target.GetType().Name + "." + property.Name + " = " + ((value != null ? value.Equals("") ? "\"\"" : value : "null") ?? "null").ToString());
			}
			Debug.Unindent();

			if (!set)
			{
				Debug.WriteLine(target.GetType().Name + "." + property.Name + " - not set!");
			}
		}

		private IEnumerable<object> GetValues(PropertyInfo property)
		{
			Type type = property.PropertyType;
			if (type.Is<double>())
			{
				yield return -1.0;
				yield return 0.0;
				yield return 1.0;
			}
			if (type.Is<bool>())
			{
				yield return false;
				yield return true;
			}
			if (type.Is<int>())
			{
				yield return -1;
				yield return 0;
				yield return 1;
			}
			if (type.Is<string>())
			{
				yield return "";
				yield return "sample string";
			}
			if (type.Is<Point>())
			{
				yield return new Point();
				yield return new Point(-1, 1);
			}
			if (type.Is<Brush>())
			{
				yield return Brushes.AliceBlue;
			}
			if (type.Is<AxisPlacement>())
			{
				yield return AxisPlacement.Bottom;
				yield return AxisPlacement.Left;
				yield return AxisPlacement.Right;
				yield return AxisPlacement.Top;
			}
			if (type.Is<Vector>())
			{
				yield return new Vector();
				yield return new Vector(1, 2);
			}
			if (type.Is<object>() && type.IsClass)
			{
				bool notNull = property.IsDefined(typeof(NotNullAttribute), false);
				if (!notNull)
				{
					yield return null;
				}
			}
		}
	}

	internal static class TypeExtensions
	{
		public static bool Is<T>(this Type type)
		{
			return typeof(T).IsAssignableFrom(type);
		}
	}
}
