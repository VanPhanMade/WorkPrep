using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlaneMovement : MonoBehaviour
{
    #region INPUT_FIELDS
    [SerializeField] private InputActionAsset _actionAsset;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private UIDocument _document;

    private InputActionMap _inputActionMap;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _maintainSpeedAction;

    public Action<Vector3> OnMove;
    public Action<Vector3> OnLook;
    public Action OnMaintainSpeedOn;
    public Action OnMaintainSpeedOff;

    private VisualElement _crossHair;

    private bool _bMaintainSpeed;
    #endregion

    #region MONOBEHAVIOUR
    private void Awake()
    {

#if UNITY_EDITOR
        if(_actionAsset == null)
        {
            // Attach with direct pathing
            string assetPath = "Assets/InputSystem_Actions.inputactions";
            _actionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(assetPath);

            // Error, not cached in editor and hard path not matching
            if (_actionAsset == null)
            {
                Debug.LogError("Couldn't attach InputActionAsset for PlaneMovement script");
            }
        }

        if(_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
            if( _rigidbody == null)
            {
                Debug.LogError("Missing rigidBody on PlaneMovement obj");
            }
        }
#endif

        _inputActionMap = _actionAsset.FindActionMap("Player", throwIfNotFound :true );

        _moveAction = _inputActionMap.FindAction("Move", throwIfNotFound :true );
        _lookAction = _inputActionMap.FindAction("Look", throwIfNotFound: true);
        _maintainSpeedAction = _inputActionMap.FindAction("Sprint", throwIfNotFound: true);

    }

    private void OnEnable()
    {
        _inputActionMap.Enable();
        OnMove += PlaneMove;
        OnLook += PlaneLook;
        OnMaintainSpeedOn += PlaneStartMaintainSpeed;
        OnMaintainSpeedOff += PlaneStopMaintainSpeed;
        _maintainSpeedAction.started += StartMaintainSpeed;
        _maintainSpeedAction.canceled += StopMaintainSpeed;
        _maintainSpeedAction.Enable();
    }

    private void OnDisable()
    {

        _inputActionMap.Disable();
        OnMove -= PlaneMove;
        OnLook -= PlaneLook;
        OnMaintainSpeedOn -= PlaneStartMaintainSpeed;
        OnMaintainSpeedOff -= PlaneStopMaintainSpeed;
        _maintainSpeedAction.started -= StartMaintainSpeed;
        _maintainSpeedAction.canceled -= StopMaintainSpeed;
        _maintainSpeedAction.Disable();
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Look(_lookAction.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        Move(_moveAction.ReadValue<Vector2>());
    }
    #endregion

    #region PLANE_MOVEMENT
    // --- Events ---
    private void Move(Vector2 _axisInput)
    {
        OnMove?.Invoke(_axisInput);
    }

    private void Look(Vector2 _axisInput)
    {
        OnLook?.Invoke(_axisInput);
    }

    private void StartMaintainSpeed(InputAction.CallbackContext ctx)
    {
        OnMaintainSpeedOn?.Invoke();
    }

    private void StopMaintainSpeed(InputAction.CallbackContext ctx)
    {
        OnMaintainSpeedOff?.Invoke();
    }


    // --- Action implementations ---
    private void PlaneMove(Vector3 _handleDirection)
    {
        Vector3 direction = (((_handleDirection.y) * transform.forward) + ((_handleDirection.x) * transform.right)).normalized;

        transform.position += direction * Time.deltaTime;


        Debug.Log("Running");
    }

    private void PlaneLook(Vector3 _handleDirection)
    {


        Debug.Log("Looking");
    }

    private void PlaneStartMaintainSpeed()
    {
        _bMaintainSpeed = true;
    }

    private void PlaneStopMaintainSpeed()
    {
        _bMaintainSpeed = false;
    }
    #endregion
}
