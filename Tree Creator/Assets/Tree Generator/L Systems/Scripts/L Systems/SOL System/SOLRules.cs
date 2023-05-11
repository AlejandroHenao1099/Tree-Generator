namespace LindenmayerSystems
{

    [System.Serializable]
    public struct SOLRules
    {
        public char Input;
        public OutputSOlSystem[] Outputs;

        public string GetOutput(float probability)
        {
            var acumulatedProbability = 0f;
            foreach (var item in Outputs)
            {
                if (probability <= item.probability + acumulatedProbability)
                    return item.Output;
                acumulatedProbability += item.probability;
            }
            return "";
        }
    }

    [System.Serializable]
    public struct OutputSOlSystem
    {
        public string Output;
        public float probability;
    }
}