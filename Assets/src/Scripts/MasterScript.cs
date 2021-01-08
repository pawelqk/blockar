using Controller.UI;
using Controls;
using Controls.Detection;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using VirtualObjects;
using UI;
using Materials;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
public class MasterScript : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject gameObjectToInstantiate;
    private readonly Logger logger = new Logger(Debug.unityLogger);

    private ARRaycastManager arRaycastmanager;
    private LayerMask virtualObjectsLayerMask;
    private ARAnchorManager arAnchorManager;
    private VirtualObjectsCreator virtualObjectsCreator;
    private VirtualObjectsStore virtualObjectsStore;
    private VirtualObjectsManager virtualObjectsManager;
    private MaterialManager materialManager;
    private IDatabase databaseCtrl;
    private IUIControls uiControls;
    private IController controller;
    private PlaneTouchHandler planeTouchHandler;
    private VirtualObjectTouchHandler virtualObjectTouchHandler;
    private TouchDetector touchDetector;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        arRaycastmanager = GetComponent<ARRaycastManager>();
        virtualObjectsLayerMask = LayerMask.GetMask("VirtualObjects");
        arAnchorManager = GetComponent<ARAnchorManager>();
        virtualObjectsCreator = new VirtualObjectsCreator(arAnchorManager, gameObjectToInstantiate, virtualObjectsLayerMask, logger);
        virtualObjectsStore = new VirtualObjectsStore(arAnchorManager, logger);
        virtualObjectsManager = new VirtualObjectsManager(virtualObjectsCreator, virtualObjectsStore, logger);
        materialManager = new MaterialManager(gameObjectToInstantiate);
        databaseCtrl = new FirebaseWithCloudAnchorDb(new FirebaseWrapper(logger), new CloudAnchorsWrapper(arAnchorManager, logger), logger);
        uiControls = new UIControls();
        controller = new MasterController(virtualObjectsManager, materialManager, uiControls, databaseCtrl, logger);
        uiControls.SetController(controller);
        planeTouchHandler = new PlaneTouchHandler(virtualObjectsManager, controller);
        virtualObjectTouchHandler = new VirtualObjectTouchHandler(virtualObjectsManager, controller);
        touchDetector = new TouchDetector(planeTouchHandler, virtualObjectTouchHandler, arRaycastmanager, logger);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfBackButtonPressed();

        uiControls.Update();
        databaseCtrl.Update();

        if (!touchDetector.CheckForTouch())
            return;

        touchDetector.ResolveLastTouch();
    }

    private void CheckIfBackButtonPressed()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
    }
}
