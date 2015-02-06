using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
    /// <summary>Mapping class holds information about mapping of TSource type
    /// to some DependencyProperty.</summary>
    /// <typeparam name="TSource">Mapping source type.</typeparam>
    internal sealed class Mapping<TSource>
    {
        /// <summary>Property that will be set.</summary>
        internal DependencyProperty Property { get; set; }
        /// <summary>Function that computes value for property from TSource type.</summary>
        internal Func<TSource, object> F { get; set; }
    }
}
