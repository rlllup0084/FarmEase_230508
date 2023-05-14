using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using FarmEase_230508.Module.BusinessObjects;
using DevExpress.CodeParser;
using Newtonsoft.Json;

namespace FarmEase_230508.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {

    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        //string name = "MyName";
        //DomainObject1 theObject = ObjectSpace.FirstOrDefault<DomainObject1>(u => u.Name == name);
        //if(theObject == null) {
        //    theObject = ObjectSpace.CreateObject<DomainObject1>();
        //    theObject.Name = name;
        //}

        //UploadMultipleCropTasks();

#if !RELEASE
        ApplicationUser sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "User");
        if (sampleUser == null) {
            sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
            sampleUser.UserName = "User";
            // Set a password if the standard authentication type is used
            sampleUser.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        }
        PermissionPolicyRole defaultRole = CreateDefaultRole();
        sampleUser.Roles.Add(defaultRole);

        CreateFarmerRole();

        ApplicationUser userAdmin = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "Admin");
        if (userAdmin == null) {
            userAdmin = ObjectSpace.CreateObject<ApplicationUser>();
            userAdmin.UserName = "Admin";
            // Set a password if the standard authentication type is used
            userAdmin.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)userAdmin).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(userAdmin));
        }
        // If a role with the Administrators name doesn't exist in the database, create this role
        PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
        if (adminRole == null) {
            adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            adminRole.Name = "Administrators";
        }
        adminRole.IsAdministrative = true;
        userAdmin.Roles.Add(adminRole);
        ObjectSpace.CommitChanges(); //This line persists created object(s).
#endif
    }

    private void UploadMultipleCropTasks() {
        // Load and deserialize the JSON data from a file or any other source
        string jsonData = File.ReadAllText("D:\\Repos\\CropTasks Upload.json");
        List<CropTaskTemp> dataObjects = JsonConvert.DeserializeObject<List<CropTaskTemp>>(jsonData);

        // Create and save the data objects
        foreach (var dataObject in dataObjects) {
            Crop crop = ObjectSpace.GetObjectByKey<Crop>(dataObject.CropId);
            CropTask cropTask = ObjectSpace.GetObjectByKey<CropTask>(dataObject.ParentId);
            CropTask newEntity = ObjectSpace.CreateObject<CropTask>();
            newEntity.CropId = crop;
            newEntity.Title = dataObject.Title;
            newEntity.Description = dataObject.Description;
            newEntity.ParentId = cropTask;
            newEntity.Seq = dataObject.Seq;
            newEntity.Days = dataObject.Days;
            newEntity.Notes = dataObject.Notes;
            // Set other properties as needed
        }

        ObjectSpace.CommitChanges(); // Save the changes to the database
    }

    public override void UpdateDatabaseBeforeUpdateSchema() {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
    private PermissionPolicyRole CreateDefaultRole() {
        PermissionPolicyRole defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default");
        if(defaultRole == null) {
            defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            defaultRole.Name = "Default";

			defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
			defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
			defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
			defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
        }

        return defaultRole;
    }

    private void CreateFarmerRole() {
        PermissionPolicyRole farmerRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Farmer");
        if (farmerRole == null) {
            farmerRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            farmerRole.Name = "Farmer";

            farmerRole.AddObjectPermissionFromLambda<Farm>(SecurityOperations.ReadWriteAccess, frm => frm.Owner.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            farmerRole.AddObjectPermissionFromLambda<Farm>(SecurityOperations.Delete, frm => frm.Owner.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);

            farmerRole.AddObjectPermissionFromLambda<Land>(SecurityOperations.ReadWriteAccess, lnd => lnd.Owner.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            farmerRole.AddObjectPermissionFromLambda<Land>(SecurityOperations.Delete, lnd => lnd.Owner.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
        }

    }

    public class GuidConverter : JsonConverter<Guid> {
        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.String && Guid.TryParse((string)reader.Value, out Guid result)) {
                return result;
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer) {
            writer.WriteValue(value.ToString());
        }
    }

    public class CropTaskTemp {
        public Guid CropId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
        public int Seq { get; set; }
        public int Days { get; set; }
        public string Notes { get; set; }
    }
}
