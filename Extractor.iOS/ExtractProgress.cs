namespace Extractor.iOS
{
    public struct ExtractProgress
    {
        public int CurrentStage { get; private set; }
        public int CurrentStageProgressPercent { get; private set; }
        public string CurrentStageName { get; private set; }
        public int TotalStages { get; private set; }

        public ExtractProgress(
            int currentStage,
            int currentStageProgressPercent,
            string currentStageName,
            int totalStages)
        {
            this.CurrentStage = currentStage;
            this.CurrentStageProgressPercent = currentStageProgressPercent;
            this.CurrentStageName = currentStageName;
            this.TotalStages = totalStages;
        }
    }
}
