namespace Extractor.iOS
{
    public struct ExtractProgress
    {
        public int CurrentStage { get; private set; }
        public int CurrentStagePercent { get; private set; }
        public string CurrentStageName { get; private set; }
        public int TotalStages { get; private set; }

        public ExtractProgress(
            int currentStage,
            int currentStagePercent,
            string currentStageName,
            int totalStages)
        {
            this.CurrentStage = currentStage;
            this.CurrentStagePercent = currentStagePercent;
            this.CurrentStageName = currentStageName;
            this.TotalStages = totalStages;
        }
    }
}
