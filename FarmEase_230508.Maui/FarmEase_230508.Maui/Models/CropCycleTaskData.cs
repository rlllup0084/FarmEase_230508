using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Models {
    public class CropCycleTaskData {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CropCycleId { get; set; }
        public int TaskId { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Seq { get; set; }
        public int MainSeq { get; set; }
        public string RecType { get; set; }
        public int RecValue { get; set; }
        public CropCycleStatus Status { get; set; }
    }
}
