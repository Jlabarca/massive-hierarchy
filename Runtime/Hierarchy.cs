namespace Massive
{
	public struct Hierarchy : IAutoFree<Hierarchy>
	{
		public ListModel<Entifier> Childs;

		public Entifier Parent;
	}
}
