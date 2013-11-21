using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.UndoSystem
{
	public sealed class LambdaUndoAction : UndoAction
	{
		private readonly Action doAction;
		private readonly Action undoAction;

		public LambdaUndoAction(Action doAction, Action undoAction)
		{
			if (doAction == null)
				throw new ArgumentNullException("doHander");
			if (undoAction == null)
				throw new ArgumentNullException("undoAction");

			this.doAction = doAction;
			this.undoAction = undoAction;
		}

		public override void Do()
		{
			doAction();
		}

		public override void Undo()
		{
			undoAction();
		}
	}
}
