using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Models {

    public class CropData {
        public CropData(string name, string notes, string id) {
            Name = name;
            Notes = notes;
            Id = id;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}
