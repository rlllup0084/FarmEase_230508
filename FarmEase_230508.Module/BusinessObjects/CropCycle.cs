using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AssociationAttribute = DevExpress.Xpo.AssociationAttribute;
using RequiredAttribute = DevExpress.ExpressApp.Model.RequiredAttribute;

namespace FarmEase_230508.Module.BusinessObjects {
    [DefaultClassOptions]
    public class CropCycle : XPObject {
        private Crop _CropId;
        private ApplicationUser _Owner;
        private Land _LandId;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private TerminationTypeEnum _TerminationType;
        private TerminationReason _TerminationReasonId;
        private double _ForcastedCost;
        private double _AcctualCost;
        private double _ForcastedRevenue;
        private double _AcctualRevenue;
        private double _ForcastedYield;
        private UnitOfMeasure _ForcastedYieldUom;
        private double _AcctualYield;
        private UnitOfMeasure _ActualYieldUom;
        private int _Progress;

        [Required]
        public Crop CropId {
            get { return _CropId; }
            set { SetPropertyValue(nameof(CropId), ref _CropId, value); }
        }

        [Association("User-CropCycles")]
        [Editable(false)]
        public ApplicationUser Owner {
            get { return _Owner; }
            set { SetPropertyValue(nameof(Owner), ref _Owner, value); }
        }

        [Required]
        public Land LandId {
            get { return _LandId; }
            set { SetPropertyValue(nameof(LandId), ref _LandId, value); }
        }

        public DateTime StartDate {
            get { return _StartDate; }
            set { SetPropertyValue(nameof(StartDate), ref _StartDate, value); }
        }

        public DateTime EndDate {
            get { return _EndDate; }
            set { SetPropertyValue(nameof(EndDate), ref _EndDate, value); }
        }

        public TerminationTypeEnum TerminationType {
            get { return _TerminationType; }
            set { SetPropertyValue(nameof(TerminationType), ref _TerminationType, value); }
        }

        public TerminationReason TerminationReasonId {
            get { return _TerminationReasonId; }
            set { SetPropertyValue(nameof(TerminationReasonId), ref _TerminationReasonId, value); }
        }

        public double ForcastedCost {
            get { return _ForcastedCost; }
            set { SetPropertyValue(nameof(ForcastedCost), ref _ForcastedCost, value); }
        }

        public double AcctualCost {
            get { return _AcctualCost; }
            set { SetPropertyValue(nameof(AcctualCost), ref _AcctualCost, value); }
        }

        public double ForcastedRevenue {
            get { return _ForcastedRevenue; }
            set { SetPropertyValue(nameof(ForcastedRevenue), ref _ForcastedRevenue, value); }
        }

        public double AcctualRevenue {
            get { return _AcctualRevenue; }
            set { SetPropertyValue(nameof(AcctualRevenue), ref _AcctualRevenue, value); }
        }

        public double ForcastedYield {
            get { return _ForcastedYield; }
            set { SetPropertyValue(nameof(ForcastedYield), ref _ForcastedYield, value); }
        }

        public UnitOfMeasure ForcastedYieldUom {
            get { return _ForcastedYieldUom; }
            set { SetPropertyValue(nameof(ForcastedYieldUom), ref _ForcastedYieldUom, value); }
        }

        public double AcctualYield {
            get { return _AcctualYield; }
            set { SetPropertyValue(nameof(AcctualYield), ref _AcctualYield, value); }
        }

        public UnitOfMeasure ActualYieldUom {
            get { return _ActualYieldUom; }
            set { SetPropertyValue(nameof(ActualYieldUom), ref _ActualYieldUom, value); }
        }

        public int Progress {
            get { return _Progress; }
            set { SetPropertyValue(nameof(Progress), ref _Progress, value); }
        }

        [Association("CropCycle-Tasks"), Aggregated]
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

        public CropCycle(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
        }

        protected override void OnSaving() {
            if (Session.IsNewObject(this)) {
                var currentUser = Session.GetObjectByKey<ApplicationUser>((Guid)SecuritySystem.CurrentUserId);
                if (currentUser != null) {
                    CreatedBy = currentUser.UserName;
                    Owner = currentUser;
                }
                CreatedDate = DateTime.UtcNow;
            } else {
                var currentUser = Session.GetObjectByKey<ApplicationUser>((Guid)SecuritySystem.CurrentUserId);
                if (currentUser != null) {
                    ModifiedBy = currentUser.UserName;
                }
                ModifiedDate = DateTime.UtcNow;
            }

            base.OnSaving();
        }
    }
}