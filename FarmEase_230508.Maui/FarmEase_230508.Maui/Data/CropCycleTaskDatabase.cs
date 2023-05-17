using FarmEase_230508.Maui.Models;
using SQLite;

namespace FarmEase_230508.Maui.Data {
    public class CropCycleTaskDatabase {
        SQLiteAsyncConnection Database;
        public CropCycleTaskDatabase() {
        }

        async Task Init() {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<CropCycleTaskData>();
        }

        public async Task<List<CropCycleTaskData>> GetCropCycleTaskByCycleId(int cycleId) {
            await Init();
            return await Database.Table<CropCycleTaskData>().Where(t => t.CropCycleId == cycleId).ToListAsync();
        }

        public async Task<int> SaveCropCycleTaskAsync(CropCycleTaskData cropCycleTaskData) {
            await Init();
            if (cropCycleTaskData.ID != 0) {
                return await Database.UpdateAsync(cropCycleTaskData);
            } else {
                return await Database.InsertAsync(cropCycleTaskData);
            }
        }

        public async Task<int> DeleteCropCycleAsync(CropCycleTaskData cropCycleTaskData) {
            await Init();
            return await Database.DeleteAsync(cropCycleTaskData);
        }
    }
}
