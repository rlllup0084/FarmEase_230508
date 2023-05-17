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
            var tasks = cropTasks.OrderBy(o=>o.MainSeq).Select(o => new Tasks {
                CropId = o.CropId.Oid,
                ParentId = o.ParentId != null ? o.ParentId.Oid : null,
                Seq = o.Seq,
                MainSeq = o.MainSeq,
                RecType = o.RecType,
                RecValue = o.RecValue,
                Title = o.Title,
                Description = o.Description,
                Notes = o.Notes,
                Days = o.Days
            }).ToList();
            return Ok(tasks);
        }

        [HttpGet("/CustomCropTasks/CropCycles")]
        public IActionResult GetCropCycles() {
            ISecurityStrategyBase security = securityProvider.GetSecurity();
            var user = security.User;
            using IObjectSpace newObjectSpace = objectSpaceFactory.CreateObjectSpace<CropCycle>();
            ApplicationUser applicationUser = newObjectSpace.GetObject((ApplicationUser)user);
            var cropCycles = newObjectSpace.GetObjects<CropCycle>(CriteriaOperator.Parse("[Owner] = ?", applicationUser));
            var cycles = cropCycles.Select(o => new CropCycleModel {
                CropCycleId = o.Oid,
                Owner = o.Owner.UserName,
                CropId = o.CropId.Oid,
                StartDate = o.StartDate
            }).ToList();
            return Ok(cycles);
        }

        [HttpGet("/CustomCropTasks/CropCycleTasks/{cropCycleId}")]
        public IActionResult GetCropCyclesTaskByCycleId(int cropCycleId) {
            using IObjectSpace newObjectSpace = objectSpaceFactory.CreateObjectSpace<CropCycleTask>();
            var cropCycles = newObjectSpace.GetObjects<CropCycleTask>(CriteriaOperator.Parse("[CropCycleId] = ?", cropCycleId));
            var cyclesTasks = cropCycles.OrderBy(o => o.MainSeq).Select(o => new CropCycleTaskModel {
                CropCycleId = o.CropCycleId.Oid,
                TaskId = o.CropCycleId.Oid,
                ParentId = o.ParentId != null ? o.ParentId.Oid : null,
                Title = o.Title,
                Description = o.Description,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                Seq = o.Seq,
                MainSeq = o.MainSeq,
                RecType = o.RecType,
                RecValue = o.RecValue,
                Status = o.Status
            }).ToList();
            return Ok(cyclesTasks);
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
            return Ok(newCropCycle.Oid);
        }
    }

    public class Tasks {
        public Guid CropId { get; set; }
        public int? ParentId { get; set; }
        public int Seq { get; set; }
        public int MainSeq { get; set; }
        public RecurrenceType RecType { get; set; }
        public int RecValue { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int Days { get; set; }
    }

    public class CropCycleTaskModel {
        public int CropCycleId { get; set; }
        public int TaskId { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Seq { get; set; }
        public int MainSeq { get; set; }
        public RecurrenceType RecType { get; set; }
        public int RecValue { get; set; }
        public CropCycleTaskStatus Status { get; set; }
    }

    public class CropCycleModel {
        public int CropCycleId { get; set; }
        public string Owner { get; set; }
        public Guid CropId { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class CreateCropCycleCommand {
        public Guid CropId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
