using SQLite;

namespace FarmEase_230508.Maui.Models {
    public enum CropCycleStatus {
        NotStarted,
        InProgress,
        Completed,
        Cancelled
    }
    public class CropCycleData {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CropCycleId { get; set; }
        public string Owner { get; set; }
        public Guid CropId { get; set; }
        public string CropName { get; set; }
        public DateTime StartDate { get; set; }
        public CropCycleStatus Status { get; set; }
    }
}
