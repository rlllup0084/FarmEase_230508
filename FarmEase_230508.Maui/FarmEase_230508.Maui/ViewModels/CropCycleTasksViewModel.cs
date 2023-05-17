using FarmEase_230508.Maui.Data;
using FarmEase_230508.Maui.Models;
using System.Collections.ObjectModel;
using System.Web;

namespace FarmEase_230508.Maui.ViewModels {
    public class CropCycleTasksListViewModel : BaseViewModel, IQueryAttributable {
        CropCycleTaskDatabase database;
        private ObservableCollection<CropCycleTaskData> items;

        public CropCycleTasksListViewModel() {
            Title = "Tasks";
            Items = new ObservableCollection<CropCycleTaskData>();
            database = new CropCycleTaskDatabase();
            SetNotStarted = new Command<CropCycleTaskData>(ExecuteSetNotStartedCommand);
            SetInProgress = new Command<CropCycleTaskData>(ExecuteSetInProgressCommand);
            SetCompleted = new Command<CropCycleTaskData>(ExecuteSetCompletedCommand);
            SetCancelled = new Command<CropCycleTaskData>(ExecuteSetCancelledCommand);
        }

        private async void ExecuteSetCancelledCommand(CropCycleTaskData obj) {
            obj.Status = CropCycleStatus.Cancelled;
            await database.SaveCropCycleTaskAsync(obj);
        }

        private async void ExecuteSetCompletedCommand(CropCycleTaskData obj) {
            obj.Status = CropCycleStatus.Completed;
            await database.SaveCropCycleTaskAsync(obj);
        }

        private async void ExecuteSetInProgressCommand(CropCycleTaskData obj) {
            obj.Status = CropCycleStatus.InProgress;
            await database.SaveCropCycleTaskAsync(obj);
        }

        private async void ExecuteSetNotStartedCommand(CropCycleTaskData obj) {
            obj.Status = CropCycleStatus.NotStarted;
            await database.SaveCropCycleTaskAsync(obj);
        }

        public Command<CropCycleTaskData> SetNotStarted { get; }
        public Command<CropCycleTaskData> SetInProgress { get; }
        public Command<CropCycleTaskData> SetCompleted { get; }
        public Command<CropCycleTaskData> SetCancelled { get; }
        public ObservableCollection<CropCycleTaskData> Items {
            get => items;
            set => SetProperty(ref items, value);
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query) {
            string id = HttpUtility.UrlDecode(query["id"] as string);
            await LoadCropCycleTasks(Convert.ToInt32(id));
        }

        private async Task LoadCropCycleTasks(int id) {
            var items = await database.GetCropCycleTaskByCycleId(id);
            MainThread.BeginInvokeOnMainThread(() => {
                Items.Clear();
                foreach (var item in items)
                    Items.Add(item);
            });
        }
    }
}
