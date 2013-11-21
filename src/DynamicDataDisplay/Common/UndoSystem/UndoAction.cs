using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.UndoSystem
{
	public abstract class UndoAction
	{
		public abstract void Do();
		public abstract void Undo();
	}
}
