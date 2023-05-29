using FarmEase_230508.Maui.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Data {
    public class CropCycleDatabase {
        SQLiteAsyncConnection Database;
        public CropCycleDatabase() {
        }

        async Task Init() {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<CropCycleData>();
            
        }

        public async Task<List<CropCycleData>> GetCropCycleByOwner(string owner) {
            await Init();
            return await Database.Table<CropCycleData>().Where(t => t.Owner == owner).ToListAsync();

            // SQL queries are also possible
            //return await Database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public async Task<CropCycleData> GetCropCycleByCropCycleId(int cropCycleId) {
            await Init();
            return await Database.Table<CropCycleData>().Where(i => i.CropCycleId == cropCycleId).FirstOrDefaultAsync();
        }

        public async Task<int> SaveCropCycleAsync(CropCycleData cropCycle) {
            await Init();
            if (cropCycle.ID != 0) {
                return await Database.UpdateAsync(cropCycle);
            } else {
                return await Database.InsertAsync(cropCycle);
            }
        }

        public async Task<int> DeleteCropCycleAsync(CropCycleData cropCycle) {
            await Init();
            return await Database.DeleteAsync(cropCycle);
        }

        
    }
}
