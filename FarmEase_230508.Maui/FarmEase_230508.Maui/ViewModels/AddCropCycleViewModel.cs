using DevExpress.Maui.Scheduler;
using FarmEase_230508.Maui.Data;
using FarmEase_230508.Maui.Models;
using Newtonsoft.Json;
using Plugin.LocalNotification;

namespace FarmEase_230508.Maui.ViewModels {
    public enum CropTypeEnum {
        Corn,
        Cassava
    }
    public class AddCropCycleViewModel : BaseViewModel {
        private DateTime startDate = DateTime.Now;
        private CropData crop;

        bool isInProcess;
        public List<CropData> Crops { get; }

        CropCycleDatabase database;
        CropCycleTaskDatabase database2;

        public AddCropCycleViewModel() {
            database = new CropCycleDatabase();
            database2 = new CropCycleTaskDatabase();
            Title = "Add Crop Cycle";
            Crops = new List<CropData> {
                new CropData("Corn", "The crop cycle schedule for corn can vary depending on various factors such as the specific corn variety, climate, and local growing conditions.", "8eba8dea-9414-44b5-a83f-37262d4c3fcb"),
                new CropData("Cassava", "The crop cycle schedule for cassava can vary depending on various factors such as climate, cultivar, and intended use of the crop.", "4cdc549d-d5dc-4440-ad87-530380401d35")
            };
            CreateCommand = new Command(ExecuteCreateCommand);
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
                CropId = Guid.Parse(this.crop.Id)
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
                        Seq = cropCycleTask.Seq,
                        MainSeq = cropCycleTask.MainSeq,
                        RecType = cropCycleTask.RecType,
                        RecValue = cropCycleTask.RecValue,
                        Status = isValidStatus ? status : CropCycleStatus.NotStarted
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
}
