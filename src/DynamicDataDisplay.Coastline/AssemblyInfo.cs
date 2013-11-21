using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts")]
[assembly: Dependency("DynamicDataDisplay", LoadHint.Always)]
[assembly: AllowPartiallyTrustedCallers]