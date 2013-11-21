
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Resources;

namespace Microsoft.Research.DynamicDataDisplay.MarkupExtensions
{
	/// <summary>
	/// Represents a markup extension, which allows to get an access to application resource files.
	/// </summary>
	[MarkupExtensionReturnType(typeof(string))]
	public class ResourceExtension : MarkupExtension
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceExtension"/> class.
		/// </summary>
		public ResourceExtension() { }

		private string resourceKey;
		//[ConstructorArgument("resourceKey")]
		public string ResourceKey
		{
			get { return resourceKey; }
			set
			{
				if (resourceKey == null)
					throw new ArgumentNullException("resourceKey");

				resourceKey = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceExtension"/> class.
		/// </summary>
		/// <param name="resourceKey">The resource key.</param>
		public ResourceExtension(string resourceKey)
		{
			if (resourceKey == null)
				throw new ArgumentNullException("resourceKey");

			this.resourceKey = resourceKey;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Strings.UIResources.ResourceManager.GetString(resourceKey);
		}
	}
}
