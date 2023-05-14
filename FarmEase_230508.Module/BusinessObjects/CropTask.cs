using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AssociationAttribute = DevExpress.Xpo.AssociationAttribute;
using RequiredAttribute = DevExpress.ExpressApp.Model.RequiredAttribute;

namespace FarmEase_230508.Module.BusinessObjects {
    [DefaultClassOptions]
    public class CropTask : XPObject, ITreeNode {
        private Crop _CropId;
        private CropTask _ParentId;
        private int _Seq = 0;
        private string _Title;
        private string _Description;
        private string _Notes;
        private int _Days;

        [Association("Crop-Tasks")]
        public Crop CropId {
            get { return _CropId; }
            set { SetPropertyValue(nameof(CropId), ref _CropId, value); }
        }

        [Association("Parent-Tasks")]
        public CropTask ParentId {
            get { return _ParentId; }
            set { SetPropertyValue(nameof(ParentId), ref _ParentId, value); }
        }

        public int Seq {
            get { return _Seq; }
            set { SetPropertyValue(nameof(Seq), ref _Seq, value); }
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

        [Size(SizeAttribute.Unlimited)]
        public string Notes {
            get { return _Notes; }
            set { SetPropertyValue(nameof(Notes), ref _Notes, value); }
        }

        public int Days {
            get { return _Days; }
            set { bool modified = SetPropertyValue(nameof(Days), ref _Days, value);
                if (!IsLoading && !IsSaving && CropId != null && modified) {
                    CropId.UpdateTotalDays(true);
                }
                if (!IsLoading && !IsSaving && ParentId != null && modified) {
                    ParentId.UpdateTotalDays(true);
                }
            }
        }

        public int? TotalDays {
            get {
                if (!IsLoading && !IsSaving)
                    UpdateTotalDays(false);
                return _Days;
            }
        }

        public void UpdateTotalDays(bool forceChangeEvents) {
            int? oldTotalDays = _Days;
            int tempTotal = 0;
            foreach (CropTask detail in Tasks)
                tempTotal += detail.Days;
            _Days = tempTotal == 0 ? oldTotalDays.Value : tempTotal;
            if (forceChangeEvents)
                OnChanged(nameof(Days), oldTotalDays, _Days);
        }

        [Association("Parent-Tasks")]
        public XPCollection<CropTask> Tasks {
            get { return GetCollection<CropTask>(nameof(Tasks)); }
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

        public string Name => Title;

        public ITreeNode Parent => ParentId;

        public IBindingList Children => Tasks;

        #endregion

        public CropTask(Session session)
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