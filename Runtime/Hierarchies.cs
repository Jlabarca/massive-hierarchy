using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Hierarchies
	{
		private Entities Entities { get; }

		public DataSet<Hierarchy> Components { get; }

		public Allocator Allocator { get; }

		public Hierarchies(DataSet<Hierarchy> components, Entities entities, Allocator allocator)
		{
			Components = components;
			Entities = entities;
			Allocator = allocator;

			Components.BeforeRemoved += RemoveHierarchy;

			void RemoveHierarchy(int id)
			{
				ref var hierarchy = ref Components.Get(id);

				if (hierarchy.Parent != Entifier.Dead)
				{
					RemoveChild(hierarchy.Parent, Entities.GetEntifier(id));
				}

				foreach (var child in hierarchy.Childs.GetEnumerator(Allocator))
				{
					ref var childHierarchy = ref Components.Get(child.Id);
					childHierarchy.Parent = Entifier.Dead;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AddHierarchy(Entifier entifier)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, entifier);

			if (Components.Add(entifier.Id))
			{
				Components.Get(entifier.Id).Childs = Allocator.AllocListModel<Entifier>();
				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddChild(Entifier parent, Entifier child)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			EntityNotAliveException.ThrowIfEntityDead(Entities, child);

			ref var childHierarchy = ref Components.Get(parent.Id);

			if (childHierarchy.Parent == parent)
			{
				return;
			}

			if (childHierarchy.Parent != Entifier.Dead)
			{
				RemoveChild(childHierarchy.Parent, child);
			}

			childHierarchy.Parent = parent;

			Components.Get(parent.Id).Childs.Add(Allocator, child);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveChild(Entifier parent, Entifier child)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			EntityNotAliveException.ThrowIfEntityDead(Entities, child);

			if (Components.Get(parent.Id).Childs.Remove(Allocator,child))
			{
				Components.Get(child.Id).Parent = Entifier.Dead;
			}
		}
	}
}
