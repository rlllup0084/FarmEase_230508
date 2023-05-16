using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.General;
using FarmEase_230508.Module.BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmEase_230508.Blazor.Server.API {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomCropTasksController : ControllerBase {
        IObjectSpaceFactory objectSpaceFactory;
        ISecurityProvider securityProvider;

        public CustomCropTasksController(IObjectSpaceFactory objectSpaceFactory, ISecurityProvider securityProvider) {
            this.objectSpaceFactory = objectSpaceFactory;
            this.securityProvider = securityProvider;
        }

        [HttpGet("/CustomCropTasks/{cropId}")]
        public IActionResult GetCropTasksByCropId(Guid cropId) {
            using IObjectSpace newObjectSpace = objectSpaceFactory.CreateObjectSpace<CropTask>();
            var cropTasks = newObjectSpace.GetObjects<CropTask>(CriteriaOperator.Parse("[CropId] = ?", cropId));
            var tasks = cropTasks.Select(o => new Tasks {
                CropId = o.CropId.Oid,
                ParentId = o.ParentId != null ? o.ParentId.Oid : null,
                Seq = o.Seq,
                RecType = o.RecType,
                RecValue = o.RecValue,
                Title = o.Title,
                Description = o.Description,
                Notes = o.Notes,
                Days = o.Days
            }).ToList();
            return Ok(tasks);
        }

        [HttpPost("/CustomCropTasks/CropCycle/Create")]
        public IActionResult CreateCropCycle([FromBody] CreateCropCycleCommand command) {
            ISecurityStrategyBase security = securityProvider.GetSecurity();
            var user = security.User;
            using IObjectSpace newObjectSpace = objectSpaceFactory.CreateObjectSpace<CropCycle>();
            var newCropCycle = newObjectSpace.CreateObject<CropCycle>();
            newCropCycle.CropId = newObjectSpace.GetObjectByKey<Crop>(command.CropId);
            newCropCycle.Owner = newObjectSpace.GetObject((ApplicationUser)user);
            newCropCycle.StartDate = command.StartDate;

            IOrderedEnumerable<CropTask> cropTasks = newCropCycle.CropId.Tasks.OrderBy(o => o.MainSeq);

            DateTime std = command.StartDate;
            int lastDays = 0;
            CropCycleTask parent = null;
            foreach (var item in cropTasks)
            {
                var newCropCycleTask = newObjectSpace.CreateObject<CropCycleTask>();
                DateTime startDate = std;
                if (item.Tasks.Count == 0)
                {
                    startDate = std.AddDays(lastDays);
                    lastDays = item.Days;
                    newCropCycleTask.ParentId = parent;

                } else {
                    parent = newCropCycleTask;
                }
                newCropCycleTask.CropCycleId = newCropCycle;
                newCropCycleTask.TaskId = newObjectSpace.GetObject(item);
                newCropCycleTask.Title = item.Title;
                newCropCycleTask.Description = item.Description;
                newCropCycleTask.StartDate = startDate;
                newCropCycleTask.EndDate = startDate.AddDays(item.Days);
                newCropCycleTask.Seq = item.Seq;
                newCropCycleTask.MainSeq = item.MainSeq;
                newCropCycleTask.RecType = item.RecType;
                newCropCycleTask.RecValue = item.RecValue;
                newCropCycleTask.Status = CropCycleTaskStatus.NotStarted;

                newCropCycle.Tasks.Add(newCropCycleTask);
            }
            newObjectSpace.CommitChanges();
            return Ok();
        }
    }

    public class Tasks {
        public Guid CropId { get; set; }
        public int? ParentId { get; set; }
        public int Seq { get; set; }
        public RecurrenceType RecType { get; set; }
        public int RecValue { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int Days { get; set; }
    }

    public class CreateCropCycleCommand {
        public Guid CropId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
