using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Models {
    public class CropCycleTaskEntity {
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
        public string Status { get; set; }
        public string Notes { get; set; }
        public int Days { get; set; }
    }
}
