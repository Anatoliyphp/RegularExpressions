using System.Collections.Generic;
using System.Linq;

namespace RegularExpressionsAndMachines
{
	public class RightGrammarExpressionConverter: ExpressionConverter
	{
		public override Dictionary<string, Dictionary<string, string>> ConvertExpressions(List<string> strings)
		{
			Dictionary<string, Dictionary<string, string>>
				states = new Dictionary<string, Dictionary<string, string>>();
			foreach (string s in strings)
			{
				string state = s.Split(" -> ").First();
				List<string> statesToTransition = s.Split(" -> ")[1].Split(" | ").ToList();
				foreach (string transitionAndState in statesToTransition)
				{
					List<KeyValuePair<string, KeyValuePair<string, string>>> statesToMerge =
						new List<KeyValuePair<string, KeyValuePair<string, string>>>();
					string toState = transitionAndState.Length == 1 ? FINAL_STATE : transitionAndState[1].ToString();
					if (states.ContainsKey(state))
					{
						bool statesMerged = false;
						Dictionary<string, string> stateTransitions = states[state];
						foreach (KeyValuePair<string, string> stateToTransition in stateTransitions)
						{
							if (stateToTransition.Value == transitionAndState[0].ToString())
							{
								string value = stateToTransition.Value;
								string key = SortState(stateToTransition.Key + toState);
								statesToMerge
									.Add(
										new KeyValuePair<string, KeyValuePair<string, string>>(
											stateToTransition.Key,
											new KeyValuePair<string, string>(
												value, key
												)
											)
										);
								statesMerged = true;
							}
						}

						if (!statesMerged)
						{
							states[state].Add(toState, transitionAndState[0].ToString());
						}
					}
					else
					{
						states.Add(state, new Dictionary<string, string>{{toState, transitionAndState[0].ToString()}});
					}
					
					Dictionary<string, string> stateTransitionsToMerge = states[state];

					foreach (var stateFromMegre in statesToMerge)
					{
						states[state].Remove(stateFromMegre.Key);
						states[state].Add(stateFromMegre.Value.Value, stateFromMegre.Value.Key);
					}
					
				}
			}

			states.Add(FINAL_STATE, new Dictionary<string, string>());

			ParseNewStates(states);

			return states;
		}
	}
}
