using System.Collections.Generic;
using System.Text.RegularExpressions;
using org.matheval;

namespace LindenmayerSystems
{
    [System.Serializable]
    public struct POLRules
    {
        public string Input;
        public char[] localVariables;
        public OutputPOlSystem[] Outputs;

        public bool VerifyInput(string currentInput)
        {
            if (currentInput[0] == Input[0])
                return true;
            return false;
        }

        public string GetOutput(string currentInput, GlobalVariable[] globalVariables)
        {
            var parameters = ExtractParameters(currentInput);
            foreach (var item in Outputs)
            {
                if (item.EvaluateCondition(localVariables, parameters))
                    return item.GetOutput(globalVariables, localVariables, parameters);
            }
            return "";
        }

        private float[] ExtractParameters(string currentInput)
        {
            var matches = Regex.Matches(currentInput, "[.0-9]+");
            var result = new float[matches.Count];
            for (int i = 0; i < matches.Count; i++)
                result[i] = float.Parse(matches[i].Value);
            return result;
        }
    }

    [System.Serializable]
    public struct OutputPOlSystem
    {
        public string condition;
        public string Output;

        public string GetOutput(GlobalVariable[] globalVariables,
            char[] localVariables, params float[] variables)
        {
            if (Output.Length <= 1)
                return Output;

            var result = Output;
            if (globalVariables != null)
                for (int i = 0; i < globalVariables.Length; i++)
                    if (result.Contains(globalVariables[i].name))
                        result = result.Replace(globalVariables[i].name, globalVariables[i].value);

            if (localVariables != null)
                for (int i = 0, len = localVariables.Length; i < len; i++)
                    if (result.Contains(localVariables[i].ToString()))
                        result = result.Replace(localVariables[i].ToString(), variables[i].ToString());

            var operations = Regex.Matches(result, "[.0-9-+*^/]+");
            var expression = new Expression();
            for (int i = 0; i < operations.Count; i++)
            {
                if (!Regex.IsMatch(operations[i].Value, "[.0-9]+"))
                    continue;
                expression.SetFomular(operations[i].Value);
                var resultOperation = float.Parse(expression.Eval().ToString());
                var index = result.IndexOf(operations[i].Value);
                result = result.Remove(index, operations[i].Value.Length);
                result = result.Insert(index, resultOperation.ToString());
                expression.Gc();
            }
            return result;
        }

        public bool EvaluateCondition(char[] localVariables, params float[] variables)
        {
            if (condition == "" || condition == null) return true;
            var indexParameter = 0;
            foreach (var item in localVariables)
            {
                if (condition.Contains(item.ToString()))
                    break;
                indexParameter++;
            }

            var a = variables[indexParameter];
            var boolOperator = Regex.Match(condition, "[><=!]+");
            var b = Regex.Match(condition, "[.0-9]+");
            return EvaluateOperator(a, boolOperator.Value, float.Parse(b.Value));
        }

        private bool EvaluateOperator(float a, string boolOperator, float b)
        {
            switch (boolOperator)
            {
                case ">":
                    return a > b;
                case ">=":
                    return a >= b;
                case "<":
                    return a < b;
                case "<=":
                    return a <= b;
                case "==":
                    return a == b;
                case "!=":
                    return a != b;
                default:
                    return false;
            }
        }
    }

    [System.Serializable]
    public struct GlobalVariable
    {
        public string name;
        public string value;
    }
}

// 0312 643 21 64
// 318 796 95 78