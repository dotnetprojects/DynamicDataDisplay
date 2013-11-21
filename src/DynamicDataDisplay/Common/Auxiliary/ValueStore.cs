using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
	public sealed class ValueStore : CustomTypeDescriptor, INotifyPropertyChanged
	{
		public ValueStore(params string[] propertiesNames)
		{
			foreach (var propertyName in propertiesNames)
			{
				this[propertyName] = "";
			}
		}

		private Dictionary<string, object> cache = new Dictionary<string, object>();

		public object this[string propertyName]
		{
			get { return cache[propertyName]; }
			set { SetValue(propertyName, value); }
		}

		public ValueStore SetValue(string propertyName, object value)
		{
			cache[propertyName] = value;
			PropertyChanged.Raise(this, propertyName);

			return this;
		}

		private PropertyDescriptorCollection collection;
		public override PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptor[] propertyDescriptors = new PropertyDescriptor[cache.Count];
			var keys = cache.Keys.ToArray();
			for (int i = 0; i < keys.Length; i++)
			{
				propertyDescriptors[i] = new ValueStorePropertyDescriptor(keys[i]);
			}

			collection = new PropertyDescriptorCollection(propertyDescriptors);
			return collection;
		}

		private sealed class ValueStorePropertyDescriptor : PropertyDescriptor
		{
			private readonly string name;

			public ValueStorePropertyDescriptor(string name)
				: base(name, null)
			{
				this.name = name;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(ValueStore); }
			}

			public override object GetValue(object component)
			{
				ValueStore store = (ValueStore)component;
				return store[name];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
