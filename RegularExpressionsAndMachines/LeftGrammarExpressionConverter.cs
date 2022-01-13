using System.Collections.Generic;
using System.Linq;

namespace RegularExpressionsAndMachines
{
	public class LeftGrammarExpressionConverter: ExpressionConverter
	{

		public override Dictionary<string, Dictionary<string, string>> ConvertExpressions(List<string> strings)
		{
			Dictionary<string, Dictionary<string, string>>
				states = new Dictionary<string, Dictionary<string, string>>();

			foreach (string s in strings)
			{
				string state = s.Split(" -> ").First();
				List<string> statesToTransition = s.Split(" -> ")[1].Split(" | ").ToList();
				List<KeyValuePair<string, KeyValuePair<string, string>>> statesToMerge =
					new List<KeyValuePair<string, KeyValuePair<string, string>>>();
				foreach (string transitionAndState in statesToTransition)
				{
					string fromState = transitionAndState.Length == 1 ? FINAL_STATE : transitionAndState[0].ToString();
					string transition = fromState == FINAL_STATE ? transitionAndState[0].ToString() : transitionAndState[1].ToString();
					if (states.ContainsKey(fromState))
					{
						bool statesMerged = false;
						Dictionary<string, string> stateTransitions = states[fromState];
						foreach (KeyValuePair<string, string> stateToTransition in stateTransitions)
						{
							if (stateToTransition.Value == transition)
							{
								string value = stateToTransition.Value;
								string key = SortState(stateToTransition.Key + state);
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
							states[fromState].Add(state, transition);
						}
					}
					else
					{
						states.Add(fromState, new Dictionary<string, string> {{state, transition}});
					}
					
					foreach (var stateFromMegre in statesToMerge)
					{
						states[fromState].Remove(stateFromMegre.Key);
						states[fromState].Add(stateFromMegre.Value.Value, stateFromMegre.Value.Key);
					}
					statesToMerge.Clear();
				}
			}

			ParseNewStates(states);

			return states;
		}
	}
}
