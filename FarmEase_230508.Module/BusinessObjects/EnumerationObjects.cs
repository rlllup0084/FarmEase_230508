namespace FarmEase_230508.Module.BusinessObjects {
    #region TerminationType
    public enum TerminationTypeEnum {
        None,
        Success,
        Failed,
        Cancelled
    }
    #endregion

    #region CropCycleTaskStatus
    public enum CropCycleTaskStatus {
        NotStarted,
        InProgress,
        Completed,
        Cancelled
    }
    #endregion

}
