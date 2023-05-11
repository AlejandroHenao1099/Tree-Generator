using System.Collections.Generic;
using UnityEngine;

namespace LindenmayerSystems
{
    public class SOLSystem
    {
        private string axiom;
        private int derivations;
        private SOLRules[] rules;
        private char[] ignore;
        // private Dictionary<KeyValuePair<int, char>, string> memoization;

        public SOLSystem(string axiom, int derivations, SOLRules[] rules, char[] ignore)
        {
            this.axiom = axiom;
            this.derivations = derivations;
            this.rules = rules;
            this.ignore = ignore;
            // memoization = new Dictionary<KeyValuePair<int, char>, string>();
        }

        // public string Create()
        // {
        //     string result = "";
        //     for (int i = 0, len = axiom.Length; i < len; i++)
        //         result = string.Format("{0}{1}",
        //                 result, RecursionSOLSystem(new KeyValuePair<int, char>(0, axiom[i])));
        //     return result;
        // }

        // private string RecursionSOLSystem(KeyValuePair<int, char> currentState)
        // {
        //     if (memoization.ContainsKey(currentState))
        //         return memoization[currentState];

        //     if (CheckIgnoreChars(currentState.Value))
        //         return currentState.Value.ToString();

        //     if (currentState.Key >= derivations)
        //     {
        //         var result = currentState.Value.ToString();
        //         memoization.Add(currentState, result);
        //         return result;
        //     }

        //     var newDerivation = CheckSOLRules(currentState.Value, Random.value);
        //     string resultNewDerivation = "";
        //     for (int i = 0, len = newDerivation.Length; i < len; i++)
        //     {
        //         var res = RecursionSOLSystem(new KeyValuePair<int, char>(currentState.Key + 1, newDerivation[i]));
        //         resultNewDerivation = string.Format("{0}{1}", resultNewDerivation, res);
        //     }
        //     memoization.Add(currentState, resultNewDerivation);
        //     return resultNewDerivation;
        // }

        public string Create()
        {
            string result = axiom;
            string newResult = "";
            for (int i = 0; i < derivations; i++)
            {
                for (int j = 0, len = result.Length; j < len; j++)
                {
                    if (CheckIgnoreChars(result[j]))
                    {
                        newResult = string.Format("{0}{1}", newResult, result[j]);
                        continue;
                    }
                    newResult = string.Format("{0}{1}", newResult, CheckSOLRules(result[j], Random.value));
                }
                result = newResult;
                newResult = "";
            }
            return result;
        }

        private string CheckSOLRules(char input, float probability)
        {
            foreach (var item in rules)
                if (item.Input == input)
                    return item.GetOutput(probability);
            return input.ToString();
        }

        private bool CheckIgnoreChars(char element)
        {
            foreach (var item in ignore)
                if (item == element)
                    return true;
            return false;
        }
    }
}