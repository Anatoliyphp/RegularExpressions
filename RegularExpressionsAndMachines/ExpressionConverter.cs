using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RegularExpressionsAndMachines
{
	public abstract class ExpressionConverter
	{
		internal const string FINAL_STATE = "F";
		public virtual Dictionary<string, Dictionary<string, string>> ConvertExpressions(List<string> strings)
		{
			throw new NotImplementedException();
		}

		public void PrintMachineMinimizationFormat(Dictionary<string, Dictionary<string, string>> states, StreamWriter output)
		{
			output.Write("  ");
			foreach (string state in states.Keys)
			{
				output.Write($"{state} ");
			}

			output.WriteLine();

			List<string> inputSignals = new List<string>();
			foreach (var transition in states.Values)
			{
				foreach (string inputSignal in transition.Values)
				{
					if (!inputSignals.Contains(inputSignal))
					{
						inputSignals.Add(inputSignal);
					}
				}
			}
			
			inputSignals.Sort();

			foreach (string inputSignal in inputSignals)
			{
				output.Write($"{inputSignal} ");
				foreach (var transition in states.Values)
				{
					if (transition.ContainsValue(inputSignal))
					{
						string state = transition.Where(t => t.Value == inputSignal).Select(t => t.Key).First();
						output.Write($"{state} ");
					}
					else
					{
						output.Write("% ");
					}
				}
				output.WriteLine();
			}
		}

		public void PrintMachine(Dictionary<string, Dictionary<string, string>> states, StreamWriter output)
		{
			foreach (KeyValuePair<string, Dictionary<string, string>> state in states)
			{
				output.Write($"{state.Key} ");
				foreach (KeyValuePair<string, string> stateAndTransition in state.Value.OrderBy(s => s.Value))
				{
					if (stateAndTransition.Key.Contains('\''))
					{
						output.Write($"{stateAndTransition.Key.Split("'")[0]}({stateAndTransition.Value}) ");
					}
					else
					{
						output.Write($"{stateAndTransition.Key}({stateAndTransition.Value}) ");
					}
				}
				output.WriteLine();
			}
		}

		internal Dictionary<string, Dictionary<string, string>> ParseNewStates(Dictionary<string, Dictionary<string, string>> states)
		{
			int newStatesCount = 0;
			while ( states.Count != newStatesCount )
			{
				newStatesCount = states.Count;
				Dictionary<string, Dictionary<string, string>> newStates =
					new Dictionary<string, Dictionary<string, string>>();
				foreach ( var state in states )
				{
					foreach ( var stateToTransition in state.Value )
					{
						if ( !states.ContainsKey( SortState(stateToTransition.Key) ) )
						{
							KeyValuePair<string, Dictionary<string, string>>
								result = GetFromDestinationStates(stateToTransition.Key, states);
							if (!newStates.ContainsKey(result.Key))
							{
								newStates.Add(result.Key, result.Value);
							}
						}
					}
				}
				foreach ( var state in newStates )
				{
					states.Add( SortState(state.Key), state.Value );
				}
			}

			return states;
		}

		internal KeyValuePair<string, Dictionary<string, string>> GetFromDestinationStates(string toState, Dictionary<string, Dictionary<string, string>> states)
		{
			KeyValuePair<string, Dictionary<string, string>>
				result = new KeyValuePair<string, Dictionary<string, string>>(toState, new Dictionary<string, string>());

			foreach (char letter in toState)
			{
				if (letter == '\'')
				{
					continue;
				}

				foreach (var element in states[letter.ToString()])
				{
					if (!result.Value.Select( v => v.Value).Contains(element.Value))
					{
						string elementKey = element.Key.Contains("'") ? element.Key.Split("'")[0] : element.Key;
						if (result.Value.ContainsKey(elementKey))
						{
							result.Value.Add(elementKey + "'", element.Value);
						}
						else
						{
							result.Value.Add(elementKey, element.Value);
						}
					}
					else
					{
						string elementKey = element.Key.Contains("'") ? element.Key.Split("'")[0] : element.Key;
						if (!result.Value.First(v => v.Value == element.Value).Value.Contains(elementKey))
						{
							string oldKey = result.Value.Where(r => r.Value == element.Value).First().Key;
							string key = string.Empty;
							string value = string.Empty;
							if (!oldKey.Contains(elementKey))
							{
								if (oldKey.Contains("'"))
								{
									oldKey = oldKey.Split("'")[0];
									key = oldKey + elementKey;
									value = element.Value;
									result.Value.Remove(oldKey + "'");
								}
								else
								{
									key = oldKey + elementKey;
									value = element.Value;
									result.Value.Remove(oldKey);
								}
								result.Value.Add(SortState(key), value);
							}
						}
					}
				}
			}

			return result;
		}

		internal string SortState(string state)
		{
			if (state.Contains('\''))
			{
				state = state.Split("'")[0];
			}
			char[] stateArray = state.ToArray();
			stateArray = stateArray.ToList().Distinct().ToArray();
			Array.Sort(stateArray);
			return new string(stateArray);
		}
	}
}
