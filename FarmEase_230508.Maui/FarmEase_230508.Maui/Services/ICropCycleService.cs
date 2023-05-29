using FarmEase_230508.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Services {
    public interface ICropCycleService {
        Task<string> CreateCropCycle(CropCreateCommand command);

        Task<string> GetCropCycleById(int Id);

        Task<string> GetCropCycleTasksById(int Id);

        Task<string> GetCrops();
    }
}
