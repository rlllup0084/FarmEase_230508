using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
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

        public CustomCropTasksController(IObjectSpaceFactory objectSpaceFactory) {
            this.objectSpaceFactory = objectSpaceFactory;
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
}
