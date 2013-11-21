using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace DynamicDataDisplay.Markers.DataSources
{
	internal sealed class CompositeDataSourcePropertyDescriptor : PropertyDescriptor
	{
		public CompositeDataSourcePropertyDescriptor(string name, IEnumerator enumerator, Type propertyType)
			: base(name, null)
		{
			if (propertyType == null)
				throw new ArgumentNullException("propertyType");

			this.enumerator = enumerator;
			this.propertyType = propertyType;
		}

		private IEnumerator enumerator;
		public IEnumerator Enumerator
		{
			get { return enumerator; }
			set { enumerator = value; }
		}

		private readonly Type propertyType;

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return typeof(DynamicItem); }
		}

		public override object GetValue(object component)
		{
			return enumerator.Current;
		}

		public override bool IsReadOnly
		{
			// todo
			get { return true; }
		}

		public override Type PropertyType
		{
			get { return propertyType; }
		}

		public override void ResetValue(object component)
		{
			throw new NotSupportedException();
		}

		public override void SetValue(object component, object value)
		{
			throw new NotSupportedException();
		}

		public override bool ShouldSerializeValue(object component)
		{
			// todo
			return false;
		}
	}
}
