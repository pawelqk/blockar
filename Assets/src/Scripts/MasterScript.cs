using Controller.UI;
using Controls;
using Controls.Detection;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using VirtualObjects;
using UI;
using Materials;

[RequireComponent(typeof(ARRaycastManager))]
public class MasterScript : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToInstantiate;
    private readonly Logger logger = new Logger(Debug.unityLogger);
    private ARRaycastManager arRaycastmanager;
    private VirtualObjectsManager virtualObjectsManager;
    private MaterialManager materialManager;
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
        virtualObjectsManager = new VirtualObjectsManager(gameObjectToInstantiate, logger);
        materialManager = new MaterialManager(gameObjectToInstantiate);
        uiControls = new UIControls();
        controller = new MasterController(virtualObjectsManager, materialManager, uiControls);
        uiControls.SetController(controller);
        planeTouchHandler = new PlaneTouchHandler(virtualObjectsManager, controller);
        virtualObjectTouchHandler = new VirtualObjectTouchHandler(virtualObjectsManager, controller);
        touchDetector = new TouchDetector(planeTouchHandler, virtualObjectTouchHandler, arRaycastmanager, logger);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfBackButtonPressed();

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
