namespace Extractor.iOS
{
    public struct ExtractProgressInfo
    {
        public int CurrentStage { get; private set; }
        public double CurrentStagePercent { get; private set; }
        public string CurrentStageName { get; private set; }
        public int TotalStages { get; private set; }

        public ExtractProgressInfo(
            int currentStage,
            double currentStagePercent,
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
