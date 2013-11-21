using System;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
    public class StandardDescription : Description
    {
        public StandardDescription() { }
        public StandardDescription(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            this.description = description;
        }

        protected override void AttachCore(UIElement element)
        {
            if (description == null)
            {
                string str = element.GetType().Name;
                description = str;
            }
        }

		private string description;
		public string DescriptionString {
			get { return description; }
			set { description = value; }
		}

        public sealed override string Brief
        {
            get { return description; }
        }

        public sealed override string Full
        {
            get { return description; }
        }
    }
}
