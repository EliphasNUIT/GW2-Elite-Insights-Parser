namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public static class DefenseStatistics
    {

        public class FinalDefense
        {
            public long DamageTaken { get; set; }
            public int BlockedCount { get; set; }
            public int EvadedCount { get; set; }
            public int InvulnedCount { get; set; }
            public int DamageInvulned { get; set; }
            public int DamageBarrier { get; set; }
            public int InterruptedCount { get; set; }
        }

        public class FinalDefenseAll : FinalDefense
        {
            public int DodgeCount { get; set; }
            public int DownCount { get; set; }
            public int DownDuration { get; set; }
            public int DeadCount { get; set; }
            public int DeadDuration { get; set; }
            public int DcCount { get; set; }
            public int DcDuration { get; set; }
        }
    }
}
