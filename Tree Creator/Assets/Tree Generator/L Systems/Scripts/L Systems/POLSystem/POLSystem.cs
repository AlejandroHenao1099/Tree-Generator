using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using org.matheval;
using UnityEngine;

namespace LindenmayerSystems
{
    public class POLSystem
    {
        private string axiom;
        private int derivations;
        private POLRules[] rules;
        private char[] ignore;
        private GlobalVariable[] globalVariables;
        private Dictionary<KeyValuePair<int, string>, string> memoization;

        public POLSystem(string axiom, int derivations, POLRules[] rules, char[] ignore,
                GlobalVariable[] globalVariables)
        {
            this.axiom = axiom;
            this.derivations = derivations;
            this.rules = rules;
            this.ignore = ignore;
            this.globalVariables = globalVariables;
            memoization = new Dictionary<KeyValuePair<int, string>, string>();
            CheckGlobalVariables();
        }

        private void CheckGlobalVariables()
        {
            for (int i = 0; i < globalVariables.Length; i++)
            {
                if (!Regex.IsMatch(globalVariables[i].value, "[+*/)^(a-z]+"))
                    continue;
                for (int j = 0; j < globalVariables.Length; j++)
                {
                    if (j == i)
                        continue;

                    var currentVariable = globalVariables[j].name;
                    if (!globalVariables[i].value.Contains(currentVariable))
                        continue;
                    globalVariables[i].value =
                        globalVariables[i].value.Replace(currentVariable, globalVariables[j].value);
                }
                var expression = new Expression(globalVariables[i].value);
                globalVariables[i].value = expression.Eval().ToString();
            }
        }

        public string CreateDP()
        {
            return IterateAxiom(axiom, 0);
        }

        private string RecursionPOLSystem(KeyValuePair<int, string> currentState)
        {
            if (memoization.ContainsKey(currentState))
                return memoization[currentState];

            if (currentState.Key >= derivations)
            {
                var result = currentState.Value;
                memoization.Add(currentState, result);
                return result;
            }

            var production = CheckPOLRules(currentState.Value);
            var resultProduction = IterateAxiom(production, currentState.Key + 1);
            memoization.Add(currentState, resultProduction);
            return resultProduction;
        }

        private string IterateAxiom(string currentAxiom, int iterationValue)
        {
            var result = new StringBuilder();
            var counter = 0;
            var parameter = "";
            for (int i = 0, len = currentAxiom.Length; i < len; i++)
            {
                if (CheckIgnoreChars(currentAxiom[i]))
                {
                    if (i + 1 >= len || currentAxiom[i + 1] != '(')
                        result.Append(currentAxiom[i]);
                    else
                    {
                        counter = currentAxiom.IndexOf(')', i) - i;
                        parameter = currentAxiom.Substring(i, counter + 1);
                        result.Append(parameter);
                        i += counter;
                    }
                    continue;
                }
                else if (i + 1 >= len || currentAxiom[i + 1] != '(')
                {
                    parameter = currentAxiom[i].ToString();
                    counter = 0;
                }
                else
                {
                    counter = currentAxiom.IndexOf(')', i) - i;
                    parameter = currentAxiom.Substring(i, counter + 1);
                }

                result.Append(RecursionPOLSystem(new KeyValuePair<int, string>(iterationValue, parameter)));
                i += counter;
            }
            return result.ToString();
        }

        private string CheckPOLRules(string input)
        {
            foreach (var item in rules)
                if (item.VerifyInput(input))
                    return item.GetOutput(input, globalVariables);
            return input;
        }

        private bool CheckIgnoreChars(char element)
        {
            foreach (var item in ignore)
                if (item == element)
                    return true;
            return false;
        }

        public string Create()
        {
            var result = axiom;
            var newResult = new StringBuilder();
            for (int i = 0; i < derivations; i++)
            {
                for (int j = 0, len = result.Length; j < len; j++)
                {
                    if (CheckIgnoreChars(result[j]))
                    {
                        newResult.Append(result[j]);
                        continue;
                    }
                    var startIndex = j;
                    var counter = result.IndexOf(')', startIndex) - startIndex;

                    var parameter = result.Substring(j, counter + 1);
                    newResult.Append(CheckPOLRules(parameter));
                    j += counter;
                }
                result = newResult.ToString();
                newResult.Clear();
            }
            return result;
        }
    }
}