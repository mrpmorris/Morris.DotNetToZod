using System.CodeDom.Compiler;

namespace Morris.Zod;

internal static class IndentedTextWriterWriteIndentedBlockExtension
{
	public static IDisposable WriteIndentedBlock(this IndentedTextWriter writer, string beforeBlock, string afterBlock)
	{
		writer.WriteLine(beforeBlock);
		writer.Indent++;
		return new DisposableAction(() =>
		{
			writer.Indent--;
			writer.WriteLine(afterBlock);
		});
	}

	private class DisposableAction : IDisposable
	{
		private readonly Action Action;

		public DisposableAction(Action action)
		{
			Action = action;
		}

		void IDisposable.Dispose()
		{
			Action();
		}
	}
}
