namespace FarmEase_230508.Maui.Models {
    public class CropCycleEntity {
        public int Oid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TerminationType { get; set; }
        public double ForcastedCost { get; set; }
        public double AcctualCost { get; set; }
        public double ForcastedRevenue { get; set; }
        public double AcctualRevenue { get; set; }
        public double ForcastedYield { get; set; }
        public double AcctualYield { get; set; }
        public int Progress { get; set; }
    }
}
