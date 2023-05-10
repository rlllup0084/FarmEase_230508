using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AssociationAttribute = DevExpress.Xpo.AssociationAttribute;
using RequiredAttribute = DevExpress.ExpressApp.Model.RequiredAttribute;

namespace FarmEase_230508.Module.BusinessObjects {
    [DefaultClassOptions]
    [DefaultProperty("Name")]
    public class Farm : XPObject {
        private string _Name;
        private string _Description;

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

        [Association("User-Farms")]
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
        private ApplicationUser _Owner;

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

        [Association("Farm-Lands")]
        public XPCollection<Land> Lands {
            get { return GetCollection<Land>(nameof(Lands)); }
        }

        #endregion

        public Farm(Session session)
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