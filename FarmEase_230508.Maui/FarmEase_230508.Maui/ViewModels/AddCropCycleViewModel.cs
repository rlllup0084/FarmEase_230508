using DevExpress.Maui.Scheduler;
using FarmEase_230508.Maui.Data;
using FarmEase_230508.Maui.Models;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace FarmEase_230508.Maui.ViewModels {
    public enum CropTypeEnum {
        Corn,
        Cassava
    }
    public class AddCropCycleViewModel : BaseViewModel {
        private DateTime startDate = DateTime.Now;
        private CropData crop;

        bool isInProcess;
        public List<CropData> Crops { get; private set; }

        CropCycleDatabase database;
        CropCycleTaskDatabase database2;

        public AddCropCycleViewModel() {
            database = new CropCycleDatabase();
            database2 = new CropCycleTaskDatabase();
            Title = "Add Crop Cycle";
            Crops = new List<CropData>();
            LoadCrops();
            CreateCommand = new Command(ExecuteCreateCommand);
        }

        private async void LoadCrops() {
            var response = await CropCycleService.GetCrops();
            CropResponse cropResponse = JsonConvert.DeserializeObject<CropResponse>(response);
            MainThread.BeginInvokeOnMainThread(() => {
                Crops.Clear();
                foreach (Crop crop in cropResponse.value) {
                    CropData cropData = new CropData(crop.Name, crop.Notes, crop.Oid);
                    Crops.Add(cropData);
                }
            });
        }

        public bool IsInProcess {
            get => isInProcess;
            set => SetProperty(ref isInProcess, value);
        }

        async void ExecuteCreateCommand() {
            IsInProcess = true;
            string owner = SecureStorage.GetAsync("auth_id").Result;
            var cropCreateCommand = new CropCreateCommand {
                StartDate = this.startDate,
                CropId = Guid.Parse(this.crop.Id),
            };
            var response = await CropCycleService.CreateCropCycle(cropCreateCommand);

            int cropCycleId;
            bool isValidCropCycleId = int.TryParse(response, out cropCycleId);

            if (isValidCropCycleId) {
                var cropCycleByIdResponse = await CropCycleService.GetCropCycleById(cropCycleId);
                var cropCycleEntity = JsonConvert.DeserializeObject<CropCycleEntity>(cropCycleByIdResponse);
                var cropCycleData = new CropCycleData {
                    CropCycleId = cropCycleEntity.Oid,
                    Owner = owner,
                    CropId = Guid.Parse(this.crop.Id),
                    CropName = this.crop.Name,
                    StartDate = cropCycleEntity.StartDate,
                    Status = CropCycleStatus.NotStarted
                };
                await database.SaveCropCycleAsync(cropCycleData);
                var request = new NotificationRequest {
                    NotificationId = cropCycleEntity.Oid,
                    Title = "A New Crop Cycle to Start",
                    Subtitle = $"Starting on {cropCycleEntity.StartDate.ToShortDateString}",
                    Description = $"Please don't forget to start the Crop Cycle on {cropCycleEntity.StartDate.ToShortDateString}. Thank you!",
                    Schedule = new NotificationRequestSchedule {
                        NotifyTime = cropCycleEntity.StartDate.Subtract(TimeSpan.FromHours(1))
                    }
                };

                await LocalNotificationCenter.Current.Show(request);
                // TODO: Retrieve CropCycleTasksById and save it to SQLLite
                var CropCycleTasksByIdResponse = await CropCycleService.GetCropCycleTasksById(cropCycleId);
                List<CropCycleTaskEntity> cropCycleTaskEntity = JsonConvert.DeserializeObject<List<CropCycleTaskEntity>>(CropCycleTasksByIdResponse);
                foreach (var cropCycleTask in cropCycleTaskEntity) {
                    CropCycleStatus status;
                    bool isValidStatus = Enum.TryParse(cropCycleTask.Status, out status);
                    var cropCycleTaskData = new CropCycleTaskData {
                        CropCycleId = cropCycleId,
                        TaskId = cropCycleTask.TaskId,
                        ParentId = cropCycleTask.ParentId,
                        Title = cropCycleTask.Title,
                        Description = cropCycleTask.Description,
                        StartDate = cropCycleTask.StartDate,
                        EndDate = cropCycleTask.EndDate,
                        Seq = cropCycleTask.Seq,
                        MainSeq = cropCycleTask.MainSeq,
                        RecType = cropCycleTask.RecType,
                        RecValue = cropCycleTask.RecValue,
                        Status = isValidStatus ? status : CropCycleStatus.NotStarted,
                        Notes = cropCycleTask.Notes,
                        Days = cropCycleTask.Days
                    };
                    await database2.SaveCropCycleTaskAsync(cropCycleTaskData);
                    RecurrenceType recurrenceType;
                    bool isValidRecurrenceType = Enum.TryParse(cropCycleTask.Status, out recurrenceType);
                    TimeSpan timeSpan = TimeSpan.Zero;
                    switch (recurrenceType) {
                        case RecurrenceType.Daily:
                            timeSpan = TimeSpan.FromDays(1);
                            // Use dailyTimeSpan as needed
                            break;

                        case RecurrenceType.Weekly:
                            timeSpan = TimeSpan.FromDays(7);
                            // Use weeklyTimeSpan as needed
                            break;

                        case RecurrenceType.Monthly:
                            timeSpan = TimeSpan.FromDays(30);
                            // Use monthlyTimeSpan as needed
                            break;

                        case RecurrenceType.Yearly:
                            timeSpan = TimeSpan.FromDays(365);
                            // Use yearlyTimeSpan as needed
                            break;

                        case RecurrenceType.Minutely:
                            timeSpan = TimeSpan.FromMinutes(1);
                            // Use minutelyTimeSpan as needed
                            break;

                        case RecurrenceType.Hourly:
                            timeSpan = TimeSpan.FromHours(1);
                            // Use hourlyTimeSpan as needed
                            break;

                        default:
                            break;
                    }
                    var request2 = new NotificationRequest {
                        NotificationId = (cropCycleId * 1000) + cropCycleTask.TaskId,
                        Title = cropCycleTask.Title,
                        Subtitle = $"Starting on {cropCycleTask.StartDate.ToShortDateString}",
                        Description = $"Please don't forget to start the task on {cropCycleTask.StartDate.ToShortDateString}. Thank you!",
                        Schedule = new NotificationRequestSchedule {
                            NotifyTime = cropCycleTask.StartDate.Subtract(TimeSpan.FromHours(1)),
                        }
                    };
                    if (cropCycleTask.RecValue > 0) {
                        request2.Schedule.NotifyRepeatInterval = timeSpan;
                    }
                    await LocalNotificationCenter.Current.Show(request2);
                }

            }

            IsInProcess = false;
            await Navigation.GoBackAsync();
        }

        public Command CreateCommand { get; }

        public DateTime StartDate {
            get => startDate;
            set => SetProperty(ref startDate, value);
        }
        public CropData Crop {
            get => crop;
            set => SetProperty(ref crop, value);
        }
    }

    public class Crop {
        public string Oid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int Days { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class CropResponse {
        public List<Crop> value { get; set; }
    }
}
