//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using GTA.Native;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GTA
{
	public class PedGroup : PoolObject, IEnumerable<Ped>, IDisposable
	{
		public class Enumerator : IEnumerator<Ped>
		{
			#region Fields
			readonly PedGroup collection;
			Ped current;
			int currentIndex = -2;
			#endregion

			public Enumerator(PedGroup group)
			{
				collection = group;
			}

			public Ped Current => current;

			object IEnumerator.Current => current;

			public void Reset()
			{
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (currentIndex++ < (collection.MemberCount - 1))
				{
					current = currentIndex < 0 ? collection.Leader : collection.GetMember(currentIndex);

					if (!(current is null) && current.Exists())
					{
						return true;
					}

					return MoveNext();
				}

				return false;
			}
		}

		public PedGroup() : base(Function.Call<int>(Hash.CREATE_GROUP, 0))
		{
		}
		public PedGroup(int handle) : base(handle)
		{
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Function.Call(Hash.REMOVE_GROUP, Handle);
			}
		}

		public int MemberCount
		{
			get
			{
				long unknBool;
				int count;
				unsafe
				{
					Function.Call(Hash.GET_GROUP_SIZE, Handle, &unknBool, &count);
				}
				return count;
			}
		}

		public float SeparationRange
		{
			set => Function.Call(Hash.SET_GROUP_SEPARATION_RANGE, Handle, value);
		}

		public Formation Formation
		{
			set => Function.Call(Hash.SET_GROUP_FORMATION, Handle, value);
		}

		public void Add(Ped ped, bool leader)
		{
			Function.Call(leader ? Hash.SET_PED_AS_GROUP_LEADER : Hash.SET_PED_AS_GROUP_MEMBER, ped.Handle, Handle);
		}
		public void Remove(Ped ped)
		{
			Function.Call(Hash.REMOVE_PED_FROM_GROUP, ped.Handle);
		}

		public bool Contains(Ped ped)
		{
			return Function.Call<bool>(Hash.IS_PED_GROUP_MEMBER, ped.Handle, Handle);
		}

		public Ped Leader => new Ped(Function.Call<int>(Hash.GET_PED_AS_GROUP_LEADER, Handle));

		public Ped GetMember(int index)
		{
			return new Ped(Function.Call<int>(Hash.GET_PED_AS_GROUP_MEMBER, Handle, index));
		}

		public Ped[] ToArray(bool includingLeader = true)
		{
			return ToList(includingLeader).ToArray();
		}

		public List<Ped> ToList(bool includingLeader = true)
		{
			var result = new List<Ped>();

			if (includingLeader)
			{
				Ped leader = Leader;

				if (leader != null && leader.Exists())
				{
					result.Add(leader);
				}
			}

			for (int i = 0; i < MemberCount; i++)
			{
				Ped member = GetMember(i);

				if (member != null && member.Exists())
				{
					result.Add(member);
				}
			}

			return result;
		}

		/// <summary>
		/// Removes this <see cref="PedGroup"/>.
		/// </summary>
		public override void Delete()
		{
			Function.Call(Hash.REMOVE_GROUP, Handle);
		}

		/// <summary>
		/// Determines if this <see cref="PedGroup"/> exists.
		/// </summary>
		/// <returns><c>true</c> if this <see cref="PedGroup"/> exists; otherwise, <c>false</c>.</returns>
		public override bool Exists()
		{
			return Function.Call<bool>(Hash.DOES_GROUP_EXIST, Handle);
		}

		/// <summary>
		/// Determines if an <see cref="object"/> refers to the same group as this <see cref="PedGroup"/>.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> to check.</param>
		/// <returns><c>true</c> if the <paramref name="obj"/> is the same group as this <see cref="PedGroup"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is PedGroup group)
			{
				return Handle == group.Handle;
			}

			return false;
		}

		/// <summary>
		/// Determines if two <see cref="PedGroup"/>s refer to the same group.
		/// </summary>
		/// <param name="left">The left <see cref="Checkpoint"/>.</param>
		/// <param name="right">The right <see cref="Checkpoint"/>.</param>
		/// <returns><c>true</c> if <paramref name="left"/> is the same group as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		public static bool operator ==(PedGroup left, PedGroup right)
		{
			return left is null ? right is null : left.Equals(right);
		}
		/// <summary>
		/// Determines if two <see cref="PedGroup"/>s don't refer to the same group.
		/// </summary>
		/// <param name="left">The left <see cref="PedGroup"/>.</param>
		/// <param name="right">The right <see cref="PedGroup"/>.</param>
		/// <returns><c>true</c> if <paramref name="left"/> is not the same group as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		public static bool operator !=(PedGroup left, PedGroup right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts a <see cref="PedGroup"/> to a native input argument.
		/// </summary>
		public static implicit operator InputArgument(PedGroup value)
		{
			return new InputArgument((ulong)value.Handle);
		}

		public override int GetHashCode()
		{
			return Handle;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
		public IEnumerator<Ped> GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}
