namespace EventProvider;

public static class Topics
{
    public const string UserStreaming = "user-streaming";
    public const string TaskStreaming = "task-streaming";
    public const string TaskLifeCycle = "task-life-cycle";
    public const string Billing = "billing-streaming";
}

public static class EventNames
{
    public const string UserCreated = "user-created";
    public const string UserUpdated = "user-updated";
    public const string UserDeleted = "user-deleted";
    
    public const string TaskCreated= "task-created";
    
    public const string TaskReassigned = "task-reassigned";
    public const string TaskAdded = "task-added";
    public const string TaskCompleted = "task-completed";
    
    public const string TransactionApplied = "transaction-created";
    public const string BillingCycleClosed = "billing-cycle-updated";
    public const string BillingCycleOpened = "billing-cycle-created";
}