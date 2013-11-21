using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	internal sealed class WeakReference<T>
	{
		private readonly WeakReference reference;

		public WeakReference(WeakReference reference)
		{
			this.reference = reference;
		}

		public WeakReference(T referencedObject)
		{
			this.reference = new WeakReference(referencedObject);
		}

		public bool IsAlive
		{
			get { return reference.IsAlive; }
		}

		public T Target
		{
			get { return (T)reference.Target; }
		}
	}
}
