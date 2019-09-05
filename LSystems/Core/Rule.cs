using System.Collections.Generic;

namespace LSystems.Core
{
	/// <summary>
	/// Represents rule of LSystem
	/// </summary>
	public sealed class Rule<T>
	{
		/// <summary>
		/// Delegate used when there is need to check if module parameters satisfy some condition
		/// </summary>
		/// <param name="args">Values of parameters of module</param>
		public delegate bool ParamCondition(params T[] args);

		/// <summary>
		/// Delegate used when there is need to check position of Module with respect to other Modules in Generation
		/// </summary>
		/// <param name="index">Position of checked module in some generation</param>
		public delegate bool ContextCondition(GenerationIndex<T> index);

		private readonly int _sourceId;
		private readonly ParamCondition[] _paramConditions;
		private readonly ContextCondition[] _contextConditions;
		private readonly ModuleFactory<T>[] _nextGenerationFactories;

		public Rule(int sourceId, ModuleFactory<T>[] nextGenerationFactories, ParamCondition[] paramConditions, 
			ContextCondition[] contextConditions)
		{
			_sourceId = sourceId;
			_nextGenerationFactories = nextGenerationFactories ?? new ModuleFactory<T>[0]; //TODO: Consider throwing exception here
			_paramConditions = paramConditions ?? new ParamCondition[0];
			_contextConditions = contextConditions ?? new ContextCondition[0];
		}
		
		/// <summary>
		/// Checks if module at given index meets all conditions of this rule.
		/// </summary>
		/// <param name="index">Determines module which is checked against conditions of rule</param>
		/// <returns>True if module at given index meets all conditions of rule, false otherwise</returns>
		internal bool CanBeApplied(GenerationIndex<T> index)
		{
			//Check ids
			if (_sourceId != index.Module.Id) return false;

			//Check all context conditions
			foreach(var condition in _contextConditions)
				if (!condition(index))
					return false;
			
			//Check parametric conditions
			foreach(var condition in _paramConditions)
				if (!condition(index.Module.Parameters))
					return false;
			
			//All conditions passed
			return true;
		}

		/// <summary>
		/// Creates new modules based on this rule
		/// Doesn't check if Rule is compatible with given Module
		/// </summary>
		/// <param name="module">Module used for creating new Modules</param>
		/// <returns>List of Modules created by this rule</returns>
		internal List<Module<T>> Apply(Module<T> module)
		{
			var nextGenModules = new List<Module<T>>();
			foreach(var factory in _nextGenerationFactories)
				nextGenModules.Add(factory.Create(module));

			return nextGenModules;
		}
	}
}