using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public struct Degree : IComparable, IComparable<Degree>, IEquatable<Degree>
	{
		private readonly CoordinateType coordinateType;
		public CoordinateType CoordinateType
		{
			get { return coordinateType; }
		}

		private readonly double value;
		public double TotalDegrees
		{
			get { return value; }
		}

		public Degree(double totalDegrees)
		{
			if (Double.IsNaN(totalDegrees))
				throw new ArgumentException("Value cannot be NaN.", "totalDegrees");
			if (Math.Abs(totalDegrees) >= Int32.MaxValue)
				throw new ArgumentOutOfRangeException("totalDegrees", "Value cannot be greater than Int32.MaxValue.");

			this.value = totalDegrees;
			coordinateType = CoordinateType.Latitude;
		}

		public Degree(int degrees, int minutes, double seconds, CoordinateType coordinateType)
			: this(degrees + minutes / 60.0 + seconds / 3600.0)
		{
			if (Math.Abs(minutes) >= 60)
				throw new ArgumentOutOfRangeException("minutes");
			if (Math.Abs(seconds) >= 60.0)
				throw new ArgumentOutOfRangeException("seconds");

			this.coordinateType = coordinateType;
		}

		public Degree(double value, CoordinateType coodinateType)
			: this(value)
		{
			this.coordinateType = coodinateType;
		}

		public static Degree CreateLatitude(int degrees, int minutes)
		{
			return Degree.CreateLatitude(degrees, minutes, 0);
		}

		public static Degree CreateLongitude(int degrees, int minutes)
		{
			return Degree.CreateLongitude(degrees, minutes, 0);
		}

		public static Degree CreateLatitude(int degrees, int minutes, double seconds)
		{
			int sign = Math.Sign(degrees);
			return new Degree(degrees, sign * minutes, sign * seconds, CoordinateType.Latitude);
		}

		public static Degree CreateLongitude(int degrees, int minutes, double seconds)
		{
			int sign = Math.Sign(degrees);
			return new Degree(degrees, sign * minutes, sign * seconds, CoordinateType.Longitude);
		}

		public static Degree CreateLatitude(double value)
		{
			return new Degree(value, CoordinateType.Latitude);
		}

		public static Degree CreateLongitude(double value)
		{
			return new Degree(value, CoordinateType.Longitude);
		}

		public int Degrees
		{
			get { return (int)Math.Sign(value) * (int)Math.Floor(Math.Abs(value)); }
		}

		public int Minutes
		{
			get { return (int)Math.Floor(TotalMinutes); }
		}

		public double TotalMinutes
		{
			get
			{
				double abs = Math.Abs(value);
				double frac = abs - Math.Floor(abs);
				return frac * 60;
			}
		}

		public int Seconds
		{
			get
			{
				return (int)Math.Floor(TotalSeconds);
			}
		}

		public double TotalSeconds
		{
			get
			{
				double mins = TotalMinutes;
				double frac = mins - Math.Floor(mins);
				return frac * 60;
			}
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (!(obj is Degree))
				throw new ArgumentException("Argument must be Degree.", "obj");
			double otherValue = ((Degree)obj).value;

			return value.CompareTo(otherValue);
		}

		#endregion

		#region IComparable<Degree> Members

		public int CompareTo(Degree other)
		{
			return value.CompareTo(other.value);
		}

		#endregion

		#region Operators

		public static Degree operator -(Degree degree)
		{
			return new Degree(-degree.value);
		}

		public static bool operator ==(Degree left, Degree right)
		{
			return left.value == right.value;
		}

		public static bool operator !=(Degree left, Degree right)
		{
			return left.value != right.value;
		}

		#endregion

		#region IEquatable<Degree> Members

		public override bool Equals(object obj)
		{
			return (obj is Degree) && (((Degree)obj).value == value);
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public bool Equals(Degree other)
		{
			return value == other.value;
		}

		#endregion

		public override string ToString()
		{
			return ToString("{0}° {1:0#}' {2:00.00}\" {3}");
		}

		public string ToString(string format)
		{
			if (String.IsNullOrEmpty(format))
				throw new ArgumentException("Format string cannot be null or empty.", "format");

			string typeStr = CreateTypeString();
			string str = String.Format(format, Math.Abs(Degrees), Minutes, TotalSeconds, typeStr);
			return str;
		}

		private string CreateTypeString()
		{
			CultureInfo culture = CultureInfo.CurrentCulture;

			// russian language
			if (culture.TwoLetterISOLanguageName == "ru")
			{
				if (coordinateType == CoordinateType.Latitude)
					return value >= 0 ? "с.ш." : "ю.ш.";
				else
					return value >= 0 ? "в.д." : "з.д.";
			}

			// other languages - taking English translation
			if (coordinateType == CoordinateType.Latitude)
				return value >= 0 ? "N" : "S";
			else
				return value >= 0 ? "E" : "W";
		}
	}
}
