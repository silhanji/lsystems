using System;
using System.Collections.Generic;

namespace LSystems
{
	public class Generator<T>
	{
		public Generation<T> CurrentGeneration { get; private set; }
		
		private Rule<T>[] _rules;
		
		
		public Generator(Generation<T> axiom, Rule<T>[] rules)
		{
			CurrentGeneration = axiom ?? throw new ArgumentException("axiom cannot be null");
			_rules = rules ?? throw new ArgumentException("rules cannot be null");
		}

		public void AdvanceGeneration()
		{
			var nextGenModules = new List<Module<T>>();
			for (int i = 0; i < CurrentGeneration.Count; i++)
			{
				var genIndex= new GenerationIndex<T>(CurrentGeneration, i);
				foreach (var rule in _rules)
				{
					if (rule.CanBeApplied(genIndex))
						nextGenModules.AddRange(rule.Apply(CurrentGeneration[i]));
				}
			}
			CurrentGeneration = new Generation<T>(nextGenModules, CurrentGeneration.Age+1);
		}

		public void AdvanceNGenerations(int n)
		{
			for(int i = 0; i < n; i++)
				AdvanceGeneration();
		}
		
	}
}