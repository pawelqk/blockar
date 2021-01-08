using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace VirtualObjects
{
    public class FirebaseWrapper
    {
        private const int SUCCESSFUL_HTTP_CODE = 200;
        private const string URL_BASE = "https://blockar-default-rtdb.firebaseio.com/sessions/";
        private UnityWebRequestAsyncOperation asyncOperation;
        private Logger logger;

        private volatile bool isUploadCompleted;
        private volatile bool isUploadSuccessful;

        private volatile bool isGetCompleted;
        private volatile bool isGetSuccessful;
        private IDictionary<string, VirtualObjectData> retrievedObjectsData;
        private IList<string> retrievedSessionsNames;

        public FirebaseWrapper(Logger logger)
        {
            this.logger = logger;
            this.retrievedObjectsData = new Dictionary<string, VirtualObjectData>();
            this.retrievedSessionsNames = new List<string>();
        }

        public bool IsUploadCompleted { get => isUploadCompleted;}
        public bool IsUploadSuccessful { get => isUploadSuccessful;}
        public bool IsGetCompleted { get => isGetCompleted;}
        public bool IsGetSuccessful { get => isGetSuccessful;}
        public IDictionary<string, VirtualObjectData> RetrievedObjectsData { get => retrievedObjectsData;}
        public IList<string> RetrievedSessionsNames { get => retrievedSessionsNames;}

        public void UploadToDb(string sessionName, IDictionary<string, VirtualObjectData> objectsData)
        {
            logger.Log("UploadToDb()", $"sessionName={sessionName}");

            isUploadCompleted = false;
            isUploadSuccessful = true;
            var objectsDataJson = ConvertDataToJson(objectsData);
            logger.Log("UploadToDb()", $"objectsDataJson={objectsDataJson}");

            var req = UnityWebRequest.Put(URL_BASE + sessionName + ".json", objectsDataJson);
            req.SetRequestHeader("Content-Type", "application/json");
            asyncOperation = req.SendWebRequest();
            asyncOperation.completed += OnUploadCompleted;
            if (asyncOperation.isDone)
                OnUploadCompleted(asyncOperation);
        }

        private void OnUploadCompleted(AsyncOperation ao)
        {
            if (isUploadCompleted)
                return;

            logger.Log("OnUploadCompleted()", $"responseCode={asyncOperation.webRequest.responseCode}, respText={asyncOperation.webRequest.downloadHandler.text}");
            if(asyncOperation.webRequest.responseCode != SUCCESSFUL_HTTP_CODE)
                isUploadSuccessful = false;
            isUploadCompleted = true;
            asyncOperation.webRequest.Dispose();
        }

        private string ConvertDataToJson(IDictionary<string, VirtualObjectData> objectsData)
        {
            var objectsDataDTO = new Dictionary<string, VirtualObjectDataDTO>();
            foreach(var objectData in objectsData)
                objectsDataDTO[objectData.Key] = VirtualObjectDataDTO.FromVirtualObjectData(objectData.Value);

            return "{\"objectsData\":" + JsonConvert.SerializeObject(objectsDataDTO) + "}";
        }

        public void GetObjectsData(string sessionName)
        {
            logger.Log("GetObjectsData()", $"sessionName={sessionName}");
            isGetCompleted = false;
            isGetSuccessful = true;
            var req = UnityWebRequest.Get(URL_BASE + sessionName + ".json");
            asyncOperation = req.SendWebRequest();
            asyncOperation.completed += OnGetObjectsDataCompleted;
            if (asyncOperation.isDone)
                OnGetObjectsDataCompleted(asyncOperation);
        }

        private void OnGetObjectsDataCompleted(AsyncOperation ao)
        {
            if (isGetCompleted)
                return;

            logger.Log("OnGetObjectsDataCompleted()", $"responseCode={asyncOperation.webRequest.responseCode}, respText={asyncOperation.webRequest.downloadHandler.text}");
            if (asyncOperation.webRequest.responseCode != SUCCESSFUL_HTTP_CODE)
                isGetSuccessful = false;

            if (isGetSuccessful)
                ConvertGetResponseToObjectsData(asyncOperation.webRequest.downloadHandler.text);

            isGetCompleted = true;
            asyncOperation.webRequest.Dispose();
        }

        private void ConvertGetResponseToObjectsData(string resp)
        {
            retrievedObjectsData.Clear();
            if (string.IsNullOrEmpty(resp) || resp.ToLower().Equals("null"))
                return;

            try
            {
                var getObjectsDataResult = JObject.Parse(resp);
                var objectsDataJsonObjects = getObjectsDataResult["objectsData"].Children();
                retrievedObjectsData.Clear();
                var objectsDataDTO = new Dictionary<string, VirtualObjectDataDTO>();
                foreach (var jsonObjectData in objectsDataJsonObjects.Values())
                {
                    var objectData = jsonObjectData.ToObject<VirtualObjectDataDTO>();
                    objectsDataDTO[objectData.guid] = objectData;
                    retrievedObjectsData[objectData.guid] = VirtualObjectData.FromDTO(objectData);
                    logger.Log("ConvertGetResponseToObjectsData()", $"objData: {retrievedObjectsData[objectData.guid]}");
                }

                foreach (var retrievedObjData in retrievedObjectsData)
                {
                    var parentingObjectGuid = objectsDataDTO[retrievedObjData.Key].parentingObjectGuid;
                    retrievedObjData.Value.ParentingObject = parentingObjectGuid is null ? null : retrievedObjectsData[parentingObjectGuid];
                }
            }
            catch(Exception e)
            {
                logger.LogError("ConvertGetResponseToObjectsData()", $"Exception caught: {e}");
                isGetSuccessful = false;
            }
        }

        public void GetSessionsList()
        {
            logger.Log("GetSessionsList()", "");
            isGetCompleted = false;
            isGetSuccessful = true;
            var req = UnityWebRequest.Get(URL_BASE + ".json?shallow=true");
            asyncOperation = req.SendWebRequest();
            asyncOperation.completed += OnGetSessionsListCompleted;
            if (asyncOperation.isDone)
                OnGetSessionsListCompleted(asyncOperation);
        }

        private void OnGetSessionsListCompleted(AsyncOperation ao)
        {
            if (isGetCompleted)
                return;

            logger.Log("OnGetSessionsListCompleted()", $"responseCode={asyncOperation.webRequest.responseCode}, respText={asyncOperation.webRequest.downloadHandler.text}");
            if (asyncOperation.webRequest.responseCode != SUCCESSFUL_HTTP_CODE)
                isGetSuccessful = false;

            if (isGetSuccessful)
                ConvertGetResponseToSessionsList(asyncOperation.webRequest.downloadHandler.text);

            isGetCompleted = true;
            asyncOperation.webRequest.Dispose();
        }

        private void ConvertGetResponseToSessionsList(string resp)
        {
            retrievedSessionsNames.Clear();
            if (string.IsNullOrEmpty(resp) || resp.ToLower().Equals("null"))
                return;

            try
            {
                var getSessionsResult = JObject.Parse(resp);
                retrievedSessionsNames = getSessionsResult.Properties().Select(p => p.Name).ToList();
            }
            catch (Exception e)
            {
                logger.LogError("ConvertGetResponseToSessionsList()", $"Exception caught: {e}");
                isGetSuccessful = false;
            }
        }
    }
}
