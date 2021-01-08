using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

namespace VirtualObjects
{
    public class FirebaseWithCloudAnchorDb : IDatabase, IStateContext
    {
        private FirebaseWrapper firebaseWrapper;
        private CloudAnchorsWrapper cloudAnchorsWrapper;
        private DatabaseStateBase databaseState;
        private ICollection<IDatabaseObserver> observers;
        private Logger logger;

        private string pendingSaveSessionName;
        private IDictionary<string, VirtualObjectData> pendingSaveObjectsData;

        private IGetReqCommand pendngGetReq;
        private IDictionary<string, VirtualObjectData> retrievedObjectsData;
        private IList<string> retrievedSessionsNames;

        public FirebaseWithCloudAnchorDb(FirebaseWrapper firebaseWrapper, CloudAnchorsWrapper cloudAnchorsWrapper, Logger logger)
        {
            this.firebaseWrapper = firebaseWrapper;
            this.cloudAnchorsWrapper = cloudAnchorsWrapper;
            this.logger = logger;
            this.observers = new List<IDatabaseObserver>();
            GenericState<FirebaseWithCloudAnchorDb>.InitState<SReady>(this);
        }

        public void SaveToDb(string sessionName, IDictionary<string, VirtualObjectData> objectsData)
        {
            logger.Log("SaveToDb()", $"sessionName={sessionName}");
            databaseState.Save(sessionName, objectsData);
        }

        public void GetAllFromDb(string sessionName)
        {
            logger.Log("GetAllFromDb()", $"sessionName={sessionName}");
            databaseState.GetData(new GetAllObjectsDataInSession { db = this, sessionName = sessionName });
        }

        public void GetSessionsFromDb()
        {
            logger.Log("GetSessionsFromDb()", "");
            databaseState.GetData(new GetSessionsReq { db = this });
        }

        public IDictionary<string, VirtualObjectData> GetRetrievedObjectsData()
        {
            return retrievedObjectsData;
        }

        public IList<string> GetRetrievedSessionsList()
        {
            return retrievedSessionsNames;
        }

        public void Update()
        {
            databaseState.Update();
        }

        public void SetState<ConcreteContext>(GenericState<ConcreteContext> state) where ConcreteContext : IStateContext
        {
            this.databaseState = (DatabaseStateBase)(object)state;
        }

        public void AttachDbObserver(IDatabaseObserver observer)
        {
            observers.Add(observer);
        }

        public void DetachDbObserver(IDatabaseObserver observer)
        {
            observers.Remove(observer);
        }

        private void NotifyObservers(DatabaseStatus status)
        {
            foreach(var observer in observers)
                observer.Notify(status);
        }

        private void HostAnchors()
        {
            cloudAnchorsWrapper.HostAnchors(pendingSaveObjectsData);
        }

        private void UpdateAnchorsHosting()
        {
            cloudAnchorsWrapper.UpdateHosting();
        }

        private bool IsHostingCompleted()
        {
            return cloudAnchorsWrapper.IsHostingCompleted;
        }

        private bool IsHostingSuccessful()
        {
            return cloudAnchorsWrapper.IsHostingOK;
        }

        private void UploadToFirebase()
        {
            firebaseWrapper.UploadToDb(pendingSaveSessionName, pendingSaveObjectsData);
        }

        private bool IsUploadToFirebaseCompleted()
        {
            return firebaseWrapper.IsUploadCompleted;
        }

        private bool IsUploadToFirebaseSuccessful()
        {
            return firebaseWrapper.IsUploadSuccessful;
        }

        private void GetDataFromFirebaseDb()
        {
            pendngGetReq.Execute();
        }

        private bool IsGetFromFirebaseCompleted()
        {
            return firebaseWrapper.IsGetCompleted;
        }

        private bool IsGetFromFirebaseSuccessful()
        {
            return firebaseWrapper.IsGetSuccessful;
        }

        private void GetFirebaseRetrievedData()
        {
            pendngGetReq.SaveRetrievedData();
        }

        private void ResolveCloudAnchors()
        {
            cloudAnchorsWrapper.ResolveAnchors(retrievedObjectsData);
        }

        private void UpdateAnchorsResolving()
        {
            cloudAnchorsWrapper.UpdateResolving();
        }

        private bool IsAnchorsResolutionCompleted()
        {
            return cloudAnchorsWrapper.IsResolvingCompleted;
        }

        private bool IsAnchorsResolutionSuccessful()
        {
            return cloudAnchorsWrapper.IsResolvingOK;
        }

        protected enum GetReqType
        {
            sessionsList = 0,
            sessionObjectsData
        }

        protected interface IGetReqCommand
        {
            void Execute();
            void SaveRetrievedData();
            GetReqType GetType();
        }
        protected class GetSessionsReq : IGetReqCommand
        {
            public FirebaseWithCloudAnchorDb db;
            void IGetReqCommand.Execute()
            {
                db.firebaseWrapper.GetSessionsList();
            }
            void IGetReqCommand.SaveRetrievedData()
            {
                db.retrievedSessionsNames = db.firebaseWrapper.RetrievedSessionsNames;
            }
            GetReqType IGetReqCommand.GetType()
            {
                return GetReqType.sessionsList;
            }
        }
        protected class GetAllObjectsDataInSession : IGetReqCommand
        {
            public FirebaseWithCloudAnchorDb db;
            public string sessionName;
            void IGetReqCommand.Execute()
            {
                db.firebaseWrapper.GetObjectsData(sessionName);
            }
            void IGetReqCommand.SaveRetrievedData()
            {
                db.retrievedObjectsData = db.firebaseWrapper.RetrievedObjectsData;
            }
            GetReqType IGetReqCommand.GetType()
            {
                return GetReqType.sessionObjectsData;
            }
        }

        // State machine

        protected abstract class DatabaseStateBase : GenericState<FirebaseWithCloudAnchorDb>
        {
            protected override void OnEnter()
            {
            }

            protected override void OnExit()
            {
            }

            public virtual void Save(string sessionName, IDictionary<string, VirtualObjectData> objectsData)
            {
                throw new NotSupportedException($"{GetType().Name}::Save() should not be called. Logic error");
            }

            public virtual void GetData(IGetReqCommand getReq)
            {
                throw new NotSupportedException($"{GetType().Name}::GetData() should not be called. Logic error");
            }

            public virtual void Update()
            {
            }
        }

        protected class SReady : DatabaseStateBase
        {
            protected override void OnEnter()
            {
                context.logger.Log("SReady::OnEnter()", "");
            }

            public override void Save(string sessionName, IDictionary<string, VirtualObjectData> objectsData)
            {
                context.pendingSaveSessionName = sessionName;
                context.pendingSaveObjectsData = objectsData;
                Change<SHostingAnchors>();
            }

            public override void GetData(IGetReqCommand getReq)
            {
                context.pendngGetReq = getReq;
                Change<SGettingFirebaseData>();
            }
        }

        protected class SBusy : DatabaseStateBase
        {
            protected DatabaseStatus busyStatus;

            public override void Save(string sessionName, IDictionary<string, VirtualObjectData> objectsData)
            {
                context.NotifyObservers(busyStatus);
            }

            public override void GetData(IGetReqCommand getReq)
            {
                context.NotifyObservers(busyStatus);
            }
        }

        protected class SHostingAnchors : SBusy
        {
            protected override void OnEnter()
            {
                context.logger.Log("SHostingAnchors::OnEnter()", "");
                context.HostAnchors();
                busyStatus = DatabaseStatus.saveOngoing;
                context.NotifyObservers(DatabaseStatus.saveOngoing);
            }

            public override void Update()
            {
                context.UpdateAnchorsHosting();

                if (!context.IsHostingCompleted())
                    return;

                if (context.IsHostingSuccessful())
                    Change<SUploadingToFirebase>();
                else
                    Change<SSaveFailed>();
            }
        }

        protected class SUploadingToFirebase : SBusy
        {
            protected override void OnEnter()
            {
                context.logger.Log("SUploadingToFirebase::OnEnter()", "");
                context.UploadToFirebase();
                busyStatus = DatabaseStatus.saveOngoing;
                context.NotifyObservers(DatabaseStatus.saveOngoing);
            }

            public override void Update()
            {
                if (!context.IsUploadToFirebaseCompleted())
                    return;

                if (context.IsUploadToFirebaseSuccessful())
                {
                    context.NotifyObservers(DatabaseStatus.saveDone);
                    Change<SReady>();
                }
                else
                {
                    Change<SSaveFailed>();
                }
            }
        }

        protected class SSaveFailed : SBusy
        {
            protected override void OnEnter()
            {
                context.logger.Log("SSaveFailed::OnEnter()", "");
                busyStatus = DatabaseStatus.saveError;
                context.NotifyObservers(DatabaseStatus.saveError);

                Change<SReady>();  // immediate change to Ready state
            }
        }

        protected class SGettingFirebaseData : SBusy
        {
            protected override void OnEnter()
            {
                context.logger.Log("SRetrivingFirebaseData::OnEnter()", "");
                context.GetDataFromFirebaseDb();
                busyStatus = DatabaseStatus.getOngoing;
                context.NotifyObservers(DatabaseStatus.getOngoing);
            }

            public override void Update()
            {
                if (!context.IsGetFromFirebaseCompleted())
                    return;

                if (context.IsGetFromFirebaseSuccessful())
                {
                    context.GetFirebaseRetrievedData();
                    if (context.pendngGetReq.GetType() == GetReqType.sessionsList)
                    {
                        context.NotifyObservers(DatabaseStatus.getSessionsListDone);
                        Change<SReady>();
                    }
                    else
                        Change<SGettingCloudAnchors>(); 
                }
                else
                {
                    Change<SGetFailed>();
                }
            }
        }

        protected class SGettingCloudAnchors : SBusy
        {
            protected override void OnEnter()
            {
                context.logger.Log("SGettingCloudAnchors::OnEnter()", "");
                context.ResolveCloudAnchors();
                busyStatus = DatabaseStatus.getOngoing;
                context.NotifyObservers(DatabaseStatus.getOngoing);
            }

            public override void Update()
            {
                context.UpdateAnchorsResolving();

                if (!context.IsAnchorsResolutionCompleted())
                    return;

                if (context.IsAnchorsResolutionSuccessful())
                {
                    context.NotifyObservers(DatabaseStatus.getObjectsDataDone);
                    Change<SReady>();
                }
                else
                {
                    Change<SGetFailed>();
                }
            }
        }
        protected class SGetFailed : SBusy
        {
            protected override void OnEnter()
            {
                context.logger.Log("SGetFailed::OnEnter()", "");
                busyStatus = DatabaseStatus.getError;
                context.NotifyObservers(DatabaseStatus.getError);

                Change<SReady>();  // immediate change to Ready state
            }
        }

    }
}
