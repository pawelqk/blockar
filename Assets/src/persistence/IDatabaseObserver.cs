namespace VirtualObjects
{
    public enum DatabaseStatus
    {
        actionCurrentlyNotPossible = 0,
        saveOngoing,
        saveDone,
        saveError,
        getOngoing,
        getObjectsDataDone,
        getSessionsListDone,
        getError
    }

    public interface IDatabaseObserver
    {
        void Notify(DatabaseStatus status);
    }
}
