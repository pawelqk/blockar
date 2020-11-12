using Controls;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using VirtualObjects;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToInstantiate;
    private readonly Logger logger = new Logger(Debug.unityLogger);
    private ARRaycastManager arRaycastmanager;
    private VirtualObjectsManager virtualObjectsManager;
    private TouchDetector touchDetector;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        arRaycastmanager = GetComponent<ARRaycastManager>();
        virtualObjectsManager = new VirtualObjectsManager(gameObjectToInstantiate);
        touchDetector = new TouchDetector(virtualObjectsManager, arRaycastmanager, logger);
    }

    // Update is called once per frame
    void Update()
    {
        if (!touchDetector.CheckForTouch())
            return;

        touchDetector.ResolveLastTouch();
    }
}
