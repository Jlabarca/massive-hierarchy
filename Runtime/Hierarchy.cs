namespace Massive
{
	public struct Hierarchy : IAuto<Hierarchy>
	{
		public ListModel<Entifier> Childs;

		public Entifier Parent;
	}
}
