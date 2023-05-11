using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;

namespace FarmEase_230508.Blazor.Server.Controllers {
    public partial class HideProtectedContentController : ViewController {
        private AppearanceController appearanceController;
        public HideProtectedContentController() {
            InitializeComponent();
        }
        protected override void OnActivated() {
            base.OnActivated();
            appearanceController = Frame.GetController<AppearanceController>();
            if (appearanceController != null) {
                appearanceController.CustomApplyAppearance += appearanceController_CustomApplyAppearance;
            }
        }

        private void appearanceController_CustomApplyAppearance(object sender, ApplyAppearanceEventArgs e) {
            if (e.AppearanceObject.Visibility == null || e.AppearanceObject.Visibility == ViewItemVisibility.Show) {
                SecurityStrategy security = Application.GetSecurityStrategy();
                if (View is ListView) {
                    if (e.Item is ColumnWrapper) {
                        if (!security.CanRead(View.ObjectTypeInfo.Type,
                            ((ColumnWrapper)e.Item).PropertyName)) {
                            e.AppearanceObject.Visibility = ViewItemVisibility.Hide;
                        }
                    }
                }
                if (View is DetailView) {
                    if (e.Item is PropertyEditor) {
                        object targetObject = e.ContextObjects.Length > 0 ? e.ContextObjects[0] : null;
                        if (targetObject != null && !security.CanRead(targetObject, ((PropertyEditor)e.Item).PropertyName)) {
                            e.AppearanceObject.Visibility = ViewItemVisibility.Hide;
                        }
                    }
                }
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated() {
            if (appearanceController != null) {
                appearanceController.CustomApplyAppearance -= appearanceController_CustomApplyAppearance;
            }
            base.OnDeactivated();
        }
    }
}
