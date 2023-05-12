using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraSpreadsheet.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AssociationAttribute = DevExpress.Xpo.AssociationAttribute;
using RequiredAttribute = DevExpress.ExpressApp.Model.RequiredAttribute;

namespace FarmEase_230508.Module.BusinessObjects {
    [DefaultClassOptions]
    [DefaultProperty("Name")]
    public class Land : XPObject {
        private string _Name;
        private string _Description;
        private ApplicationUser _Owner;
        private Farm _Farm;
        private decimal _Area;
        private UnitOfMeasure _AreaUom;

        [Required]
        [RuleUniqueValue]
        public string Name {
            get { return _Name; }
            set { SetPropertyValue(nameof(Name), ref _Name, value); }
        }

        public string Description {
            get { return _Description; }
            set { SetPropertyValue(nameof(Description), ref _Description, value); }
        }

        [Required]
        [Association("Farm-Lands")]
        public Farm Farm {
            get { return _Farm; }
            set { SetPropertyValue(nameof(Farm), ref _Farm, value); }
        }

        [ModelDefault("EditMaskType", "Simple")]
        [ModelDefault("EditMask", "N2")]
        [ModelDefault("DisplayFormat", "{0:N2}")]
        public decimal Area {
            get { return _Area; }
            set { SetPropertyValue(nameof(Area), ref _Area, value); }
        }

        public UnitOfMeasure AreaUom {
            get { return _AreaUom; }
            set { SetPropertyValue(nameof(AreaUom), ref _AreaUom, value); }
        }

        [Association("User-Lands")]
        [Editable(false)]
        public ApplicationUser Owner {
            get { return _Owner; }
            set { SetPropertyValue(nameof(Owner), ref _Owner, value); }
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

        public Land(Session session)
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