using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using System.ComponentModel.DataAnnotations;
using AggregatedAttribute = DevExpress.Xpo.AggregatedAttribute;
using AssociationAttribute = DevExpress.Xpo.AssociationAttribute;
using RequiredAttribute = DevExpress.ExpressApp.Model.RequiredAttribute;

namespace FarmEase_230508.Module.BusinessObjects {
    [DefaultClassOptions]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    public class CropCycleTask : XPObject {
        private CropCycle _CropCycleId;
        private CropTask _TaskId;
        private CropCycleTask _ParentId;
        private string _Title;
        private string _Description;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private int _Progress;
        private int _MainSeq;
        private int _Seq;
        private RecurrenceType _RecType;
        private int _RecValue;
        private CropCycleTaskStatus _Status;
        private string _Notes;
        private int _Days;

        [Association("CropCycle-Tasks")]
        public CropCycle CropCycleId {
            get { return _CropCycleId; }
            set { SetPropertyValue(nameof(CropCycleId), ref _CropCycleId, value); }
        }

        public CropTask TaskId {
            get { return _TaskId; }
            set { SetPropertyValue(nameof(TaskId), ref _TaskId, value); }
        }

        [Association("Parent-Tasks")]
        public CropCycleTask ParentId {
            get { return _ParentId; }
            set { SetPropertyValue(nameof(ParentId), ref _ParentId, value); }
        }

        [Required]
        public string Title {
            get { return _Title; }
            set { SetPropertyValue(nameof(Title), ref _Title, value); }
        }

        public string Description {
            get { return _Description; }
            set { SetPropertyValue(nameof(Description), ref _Description, value); }
        }

        public int Days {
            get { return _Days; }
            set { SetPropertyValue(nameof(Days), ref _Days, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Notes {
            get { return _Notes; }
            set { SetPropertyValue(nameof(Notes), ref _Notes, value); }
        }

        public int MainSeq {
            get { return _MainSeq; }
            set { SetPropertyValue(nameof(MainSeq), ref _MainSeq, value); }
        }

        public int Seq {
            get { return _Seq; }
            set { SetPropertyValue(nameof(Seq), ref _Seq, value); }
        }

        public RecurrenceType RecType {
            get { return _RecType; }
            set { SetPropertyValue(nameof(RecType), ref _RecType, value); }
        }

        public int RecValue {
            get { return _RecValue; }
            set { SetPropertyValue(nameof(RecValue), ref _RecValue, value); }
        }

        [Required]
        public DateTime StartDate {
            get { return _StartDate; }
            set { SetPropertyValue(nameof(StartDate), ref _StartDate, value); }
        }

        [Required]
        public DateTime EndDate {
            get { return _EndDate; }
            set { SetPropertyValue(nameof(EndDate), ref _EndDate, value); }
        }

        public int Progress {
            get { return _Progress; }
            set { SetPropertyValue(nameof(Progress), ref _Progress, value); }
        }

        public CropCycleTaskStatus Status {
            get { return _Status; }
            set { SetPropertyValue(nameof(Status), ref _Status, value); }
        }

        [Association("Parent-Tasks"), Aggregated]
        public XPCollection<CropCycleTask> Tasks {
            get { return GetCollection<CropCycleTask>(nameof(Tasks)); }
        }

        #region Audit Fields

        private string _CreatedBy;
        private DateTime _CreatedDate;
        private string _ModifiedBy;
        private DateTime _ModifiedDate;

        [Editable(false)]
        public string CreatedBy {
            get { return _CreatedBy; }
            set { SetPropertyValue(nameof(CreatedBy), ref _CreatedBy, value); }
        }

        [Editable(false)]
        public DateTime CreatedDate {
            get { return _CreatedDate; }
            set { SetPropertyValue(nameof(CreatedDate), ref _CreatedDate, value); }
        }

        [Editable(false)]
        public string ModifiedBy {
            get { return _ModifiedBy; }
            set { SetPropertyValue(nameof(ModifiedBy), ref _ModifiedBy, value); }
        }

        [Editable(false)]
        public DateTime ModifiedDate {
            get { return _ModifiedDate; }
            set { SetPropertyValue(nameof(ModifiedDate), ref _ModifiedDate, value); }
        }

        #endregion


        public CropCycleTask(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
        }

        //protected override void OnSaving() {
        //    if (Session.IsNewObject(this)) {
        //        var currentUser = Session.GetObjectByKey<ApplicationUser>((Guid)SecuritySystem.CurrentUserId);
        //        if (currentUser != null) {
        //            CreatedBy = currentUser.UserName;
        //        }
        //        CreatedDate = DateTime.UtcNow;
        //    } else {
        //        var currentUser = Session.GetObjectByKey<ApplicationUser>((Guid)SecuritySystem.CurrentUserId);
        //        if (currentUser != null) {
        //            ModifiedBy = currentUser.UserName;
        //        }
        //        ModifiedDate = DateTime.UtcNow;
        //    }

        //    base.OnSaving();
        //}
    }
}