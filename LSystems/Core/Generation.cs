using System;
using System.Collections.Generic;

namespace LSystems.Core
{
	/// <summary>
	/// Grups info about all modules inside one generation and it's age
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Generation<T>
	{
		public readonly List<Module<T>> Modules;
		public readonly int Age;

		public Generation(List<Module<T>> modules, int age)
		{
			Modules = modules ?? throw new ArgumentException("modules cannot be null");
			Age = age;
		}
		
		public Module<T> this[int index] => Modules[index];
		
		public int Count => Modules.Count;
	}

	/// <summary>
	/// Determines specific module inside one generation
	/// </summary>
	/// <typeparam name="T">Type of data that is stored in modules parameters</typeparam>
	public struct GenerationIndex<T>
	{
		public readonly Generation<T> Generation;
		public readonly int Index;

		public Module<T> Module => Generation.Modules[Index];

		public Module<T> this[int index] => Generation.Modules[index];
		
		public int Count => Generation.Modules.Count;
		
		public GenerationIndex(Generation<T> data, int index)
		{
			Generation = data ?? throw new ArgumentException("data cannot be null");
			if(index < 0 || index >= data.Modules.Count)
				throw new ArgumentException("position must be greater than 0 and less than data.Modules.Count");
			Index = index;
		}
	}
}