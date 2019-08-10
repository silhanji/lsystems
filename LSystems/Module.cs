using System;

namespace LSystems
{
	/// <summary>
	/// Represents one module of LSystem generation.
	/// </summary>
	/// <typeparam name="T">Type of parameters attached to module</typeparam>
	public struct Module<T> : IEquatable<Module<T>>
	{
		public readonly int Id;
		public readonly T[] Parameters;
		
		public Module(int id)
		{
			Id = id;
			Parameters = new T[0];
		}

		public Module(int id, params T[] parameters)
		{
			Id = id;
			Parameters = parameters;
		}
		
		public bool Equals(Module<T> other)
		{
			if (Id != other.Id || Parameters.Length != other.Parameters.Length)
				return false;

			for (int i = 0; i < Parameters.Length; i++)
			{
				if (!Parameters[i].Equals(other.Parameters[i]))
					return false;
			}

			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Module<T>)
			{
				Module<T> other = (Module<T>) obj;
				return Equals(other);
			}

			return false;
		}

		//TODO: Consider different implementation
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Module<T> m1, Module<T> m2)
		{
			return m1.Equals(m2);
		}

		public static bool operator !=(Module<T> m1, Module<T> m2)
		{
			return !m1.Equals(m2);
		}
	}
}