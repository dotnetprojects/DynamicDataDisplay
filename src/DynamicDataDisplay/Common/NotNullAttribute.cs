using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
	[Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
	internal sealed class NotNullAttribute : Attribute
	{
	}
}
