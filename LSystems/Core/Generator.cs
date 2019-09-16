using System;
using System.Collections.Generic;
using System.Threading;

namespace LSystems.Core
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
			if (CurrentGeneration.Count < 10000)
			{
				//Generation is too small, costs of thread creation would be high
				AdvanceSingleThreaded();
			}
			else
			{
				AdvanceMultiThreaded();
			}
			
			
		}

		public void AdvanceNGenerations(int n)
		{
			for(int i = 0; i < n; i++)
				AdvanceGeneration();
		}

		private void AdvanceSingleThreaded()
		{
			var task = new AdvanceTask(0, CurrentGeneration.Count, this);
			task.Start();
			CurrentGeneration = new Generation<T>(task.NextGeneration, CurrentGeneration.Age+1);
		}

		private void AdvanceMultiThreaded()
		{
			int threadCount = Environment.ProcessorCount -1;
			int taskSize = CurrentGeneration.Count / threadCount;
			
			var tasks = new AdvanceTask[threadCount];
			for (int i = 0; i < threadCount-1; i++)
			{
				tasks[i] = new AdvanceTask(i * taskSize, taskSize, this);
			}

			int lastTaskIndex = (tasks.Length - 1) * taskSize;
			tasks[tasks.Length-1] = 
				new AdvanceTask(lastTaskIndex, CurrentGeneration.Count - lastTaskIndex, this);

			var threads = new Thread[threadCount];
			
			for(int i = 0; i < threads.Length; i++)
			{
				threads[i] = new Thread(tasks[i].Start);
			}

			foreach(var thread in threads)
				thread.Start();
			
			for(int i = 0; i < tasks.Length; i++)
			{
				var task = tasks[i];
				while (!task.Completed)
					threads[i].Join();
					
			}

			var nextGenerationModules = new List<Module<T>>();
			foreach(var task in tasks)
				nextGenerationModules.AddRange(task.NextGeneration);
			
			CurrentGeneration = new Generation<T>(nextGenerationModules, CurrentGeneration.Age+1);
		}

		private class AdvanceTask
		{
			public bool Completed { get; private set; } = false;
			public List<Module<T>> NextGeneration { get; private set; }

			private int _startIndex;
			private int _count;
			private Generator<T> _generator;
			
			public AdvanceTask(int startIndex, int count, Generator<T> generator)
			{
				_startIndex = startIndex;
				_count = count;
				_generator = generator;
			}
			
			public void Start()
			{
				NextGeneration = new List<Module<T>>();
				
				for (int i = _startIndex; i < _startIndex + _count; i++)
				{
					bool existsRule = false;
					var genIndex= new GenerationIndex<T>(_generator.CurrentGeneration, i);
					foreach (var rule in _generator._rules)
					{
						if (rule.CanBeApplied(genIndex))
						{
							NextGeneration.AddRange(rule.Apply(_generator.CurrentGeneration[i]));
							existsRule = true;
							break;
						}
					}
					if(!existsRule) //Default 'copy' rule
						NextGeneration.Add(_generator.CurrentGeneration[i]);
				}

				Completed = true;
			}
		}
	}
}