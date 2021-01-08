using System.Collections.Generic;

namespace VirtualObjects
{
    public interface IDatabase
    {
        void Update();
        void SaveToDb(string sessionName, IDictionary<string, VirtualObjectData> objectsData);
        void GetAllFromDb(string sessionName);
        void GetSessionsFromDb();

        IDictionary<string, VirtualObjectData> GetRetrievedObjectsData();
        IList<string> GetRetrievedSessionsList();

        void AttachDbObserver(IDatabaseObserver observer);
        void DetachDbObserver(IDatabaseObserver observer);
    }
}
