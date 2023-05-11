using System.Collections.Generic;

namespace LindenmayerSystems
{
    public class DOLSystem
    {
        private string axiom;
        private int derivations;
        private DOLRules[] rules;
        private char[] ignore;
        private Dictionary<KeyValuePair<int, char>, string> memoization;

        public DOLSystem(string axiom, int derivations, DOLRules[] rules, char[] ignore)
        {
            this.axiom = axiom;
            this.derivations = derivations;
            this.rules = rules;
            this.ignore = ignore;
            memoization = new Dictionary<KeyValuePair<int, char>, string>();
        }

        public string Create()
        {
            string result = "";
            for (int i = 0, len = axiom.Length; i < len; i++)
                result = string.Format("{0}{1}",
                        result, RecursionDOLSystem(new KeyValuePair<int, char>(0, axiom[i])));
            return result;
        }

        private string RecursionDOLSystem(KeyValuePair<int, char> currentState)
        {
            if (memoization.ContainsKey(currentState))
                return memoization[currentState];

            if (CheckIgnoreChars(currentState.Value))
                return currentState.Value.ToString();

            if (currentState.Key >= derivations)
            {
                var result = currentState.Value.ToString();
                memoization.Add(currentState, result);
                return result;
            }

            var newDerivation = CheckDOLRules(currentState.Value);
            string resultNewDerivation = "";
            for (int i = 0, len = newDerivation.Length; i < len; i++)
            {
                var res = RecursionDOLSystem(new KeyValuePair<int, char>(currentState.Key + 1, newDerivation[i]));
                resultNewDerivation = string.Format("{0}{1}", resultNewDerivation, res);
            }
            memoization.Add(currentState, resultNewDerivation);
            return resultNewDerivation;
        }

        private string CheckDOLRules(char input)
        {
            foreach (var item in rules)
                if (item.Input == input)
                    return item.Output;
            return input.ToString();
        }

        private bool CheckIgnoreChars(char element)
        {
            foreach (var item in ignore)
                if (item == element)
                    return true;
            return false;
        }

        public string NaiveCreate()
        {
            string result = axiom;
            string newResult = "";
            for (int i = 0; i < derivations; i++)
            {
                bool usingRule = false;
                for (int j = 0, len = result.Length; j < len; j++)
                {
                    usingRule = false;
                    foreach (var item in rules)
                        if (item.Input == result[j])
                        {
                            newResult = string.Format("{0}{1}", newResult, item.Output);
                            usingRule = true;
                        }
                    if (!usingRule)
                        newResult = string.Format("{0}{1}", newResult, result[j]);
                }
                result = newResult;
                newResult = "";
            }
            return result;
        }
    }
}