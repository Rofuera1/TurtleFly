using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    NotStarted,
    DiedOxygen,
    DiedKnock,
    Playing,
    Finishing,
    Finished
}
public class Main : MonoBehaviour
{
    public static Main Instance;

    // Public section for the inspector
    public PathCreation.PathCreator Path;
    public GameObject PlayerParentObj;
    public Transform BaloonTR;
    public GameObject TurtleTR;
    [Space]
    public float PlayerStartSpeed;
    public float PlayerHorizontalSpeed;
    public float RoadWidth;
    public int OxygenStartAmount;
    public int NeedleLooseOxygenPerHole;
    public int BasicLooseOxygenPerSecond;

    // Public properties section;
    public GameStates CurrentState { get; protected set; }

    // UI Section
    public delegate void OnValueChange(int newValue);
    private event OnValueChange OxygenChange;
    private event OnValueChange CoinsChange;
    public float OxygenPercentageCurrent { get => (float) _currentOxygen / (float) OxygenStartAmount; }

    // Points private section
    private int _currentOxygen;
    private int currentOxygen { get => _currentOxygen; set { if (OxygenChange != null) OxygenChange(value); } }
    private int _currentCoins;
    private int currentCoins { get => _currentCoins;  set { if (CoinsChange != null) CoinsChange(value); } }

    // Mechanics private section
    private float currentSpeed;

    private float currentPathDistance;
    private Vector3 currentNeededPosition;
    private Vector3 currentNeededPositionReference;

    private Vector3 startCameraOffset;
    private Vector3 cameraFollowReference;
    private Quaternion startCameraRotationOffset;

    private Vector2 touchPreviousPosition;
    private Vector2 touchCurrentPosition;
    private float touchHorizontalDeltaValue;
    private float horizontalOffset;

    private int needleHolesAmount;
    private int currentOxygenLooseExtra;

    private Vector3 ballonStartScale;

    private void Awake()
    {
        if (Instance)
            Destroy(Instance);
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        loadEvents();
        loadStartProperties();

        StartCoroutine(waitForKeyPressToStartGame());
    }

    private void loadStartProperties()
    {
        currentSpeed = PlayerStartSpeed;
        currentOxygen = OxygenStartAmount;

        ballonStartScale = BaloonTR.localScale;

        startCameraOffset = transform.position - PlayerParentObj.transform.position;
    }

    private void loadEvents()
    {
        OxygenChange += updateOxygenAmount;
        OxygenChange += updateBaloonScale;
        //OxygenChange += UIManager.Instance.UpdateOxygenUI;

        CoinsChange += updateCointsAmount;
        CoinsChange += UIManager.Instance.UpdateCoinsUI;
    }

    private void Update()
    {
        updateCameraPosition();
        updateCameraRotation();

        if (CurrentState > GameStates.DiedOxygen)
        {
            updatePlayerPosition();
            updatePlayerRotation();
        }

        if(CurrentState == GameStates.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchCurrentPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                touchPreviousPosition = touchCurrentPosition;
                touchCurrentPosition = Input.mousePosition;
                touchHorizontalDeltaValue = (touchCurrentPosition.x - touchPreviousPosition.x) * Time.deltaTime;
            }
            else
            {
                touchHorizontalDeltaValue = 0f;
            }
        }
    }

    private IEnumerator looseOxygenCoroutine()
    {
        while(CurrentState == GameStates.Playing && currentOxygen > 0f)
        {
            currentOxygen -= (BasicLooseOxygenPerSecond + currentOxygenLooseExtra);
            yield return new WaitForSeconds(1f);
        }

        if(currentOxygen <= 0f)
        {
            oxygenRanOut();
        }
    }

    private IEnumerator waitForKeyPressToStartGame()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;

        updateState(GameStates.Playing);
    }

    private void oxygenRanOut()
    {
        updateState(GameStates.DiedOxygen);
    }

    private void updatePlayerPosition()
    {
        currentPathDistance += currentSpeed * Time.deltaTime;
        horizontalOffset = Mathf.Clamp(horizontalOffset + touchHorizontalDeltaValue * PlayerHorizontalSpeed, -RoadWidth / 2, RoadWidth / 2);

        currentNeededPosition = Path.path.GetPointAtDistance(currentPathDistance) + Path.path.GetNormalAtDistance(currentPathDistance) * horizontalOffset;

        PlayerParentObj.transform.position = Vector3.SmoothDamp(PlayerParentObj.transform.position, currentNeededPosition, ref currentNeededPositionReference, 0.2f);
    }

    private void updatePlayerRotation()
    {

    }

    private void updateCameraRotation()
    {
    }

    private void updateState(GameStates newState)
    {
        CurrentState = newState;
        UIManager.Instance.ShowGameState(newState);

        if(CurrentState == GameStates.Playing)
        {
            startGamePlaying();
        }
    }

    private void startGamePlaying()
    {
        UIManager.Instance.StartTrackingOxygenUISlider();
        StartCoroutine(looseOxygenCoroutine());
    }

    private void updateCameraPosition()
    {
        transform.position = Vector3.SmoothDamp(transform.position, PlayerParentObj.transform.position + startCameraOffset, ref cameraFollowReference, 0.2f);
    }

    private void updateOxygenAmount(int newValue) => _currentOxygen = newValue;
    private void updateCointsAmount(int newValue) => _currentCoins = newValue;

    public void CollectedOxygen(GameObject oxygenBoostObj)
    {
        currentOxygen += oxygenBoostObj.GetComponent<CollectableOxygen>().OxygenAmount;

        StartCoroutine(SmoothCoroutines.SmoothDisablePop(oxygenBoostObj, 0.3f, 0.5f, EasingFunction.EaseInOutCirc));
        Destroy(oxygenBoostObj, 1f);
    }

    private void updateBaloonScale(int newValue)
    {
        StartCoroutine(SmoothCoroutines.SmoothScaleLerp(BaloonTR, Vector3.Lerp(ballonStartScale, Vector3.zero, 1f - OxygenPercentageCurrent), 1f, EasingFunction.Linear));
    }

    public void StartLoosingOxygenNeedleCause()
    {
        needleHolesAmount++;
        currentOxygenLooseExtra += NeedleLooseOxygenPerHole;
    }

    public void StopLoosingOxygenNeedleCause()
    {
        needleHolesAmount--;
        currentOxygenLooseExtra -= NeedleLooseOxygenPerHole;
    }

    public void KnockPlayerOff()
    {
        updateState(GameStates.DiedKnock);

        TurtleTR.transform.parent = null;
        Destroy(TurtleTR.transform.GetComponent<BoxCollider>());
        TurtleTR.GetComponent<MeshCollider>().enabled = true;
        TurtleTR.AddComponent<Rigidbody>();
    }
}
