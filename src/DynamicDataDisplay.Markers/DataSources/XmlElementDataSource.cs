using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class XmlElementDataSource : PointDataSourceBase
	{
		private readonly XmlElement xmlElement;
		public XmlElement XmlElement
		{
			get { return xmlElement; }
		}

		public XmlElementDataSource(XmlElement xmlElement)
		{
			if (xmlElement == null)
				throw new ArgumentNullException("xmlElement");

			this.xmlElement = xmlElement;
		}

		public override IEnumerable GetData(int startingIndex)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable GetDataCore()
		{
			var xmlElement = this.xmlElement;
			do
			{
				yield return ((XmlText)xmlElement.FirstChild).Data;
				xmlElement = (XmlElement)xmlElement.NextSibling;
			}
			while (xmlElement != null);
		}

		public override object GetDataType()
		{
			return typeof(XmlNode);
		}
	}
}
