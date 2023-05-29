using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Models {
    public class CropCreateCommand {
        public Guid CropId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
