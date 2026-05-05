using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static IHasProgess;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent, IHasProgess
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomthing;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }
    public static Player LocalInstance { get; private set; }

    public event EventHandler<OnProgessChangedEventArgs> OnProgessChanged;
    public event EventHandler OnPickUpSomeThing;

    public event EventHandler<OnSelectedCounterChangedArgs> OnSelectedCouterChanged;
    public class OnSelectedCounterChangedArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float powerDash = 4.72f;
    [SerializeField] private Transform PlayerHoldPoint;
    [SerializeField] private LayerMask countersLayersMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform playerBody;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;

    private bool isWalking;
    private bool isDashing = false;
    private float dashTimer;
    private float dashDuration = 0.2f;
    private float coolDown = 2f; //cooldown for Dash
    private float dashCooldownTimer = 0f;

    private Vector3 lastMoveInteract;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        //Instance = this;
    }
    private void Start()
    {
        GameInput.Instance.OnInteractActions += GameInput_OnInteractActions;
        GameInput.Instance.OnInteractAlternateActions += GameInput_OnInteractAlternateActions;
        GameInput.Instance.OnDash += GameInput_OnDash;
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.setPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        OnProgessChanged?.Invoke(this, new OnProgessChangedEventArgs
        {
            progressNormalized = dashCooldownTimer
        });
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
       if(clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroykitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnDash(object sender, EventArgs e)
    {
        if (isWalking && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = 0f;
            dashCooldownTimer = coolDown;
        }
    }

    private void GameInput_OnInteractAlternateActions(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying() || KitchenGameManager.Instance.IsGameOver()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractActions(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying() || KitchenGameManager.Instance.IsGameOver()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();     
        HandleInteractions();

        if (isDashing == true)
        {
            HandleDash();
        }
        
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
            OnProgessChanged?.Invoke(this, new OnProgessChangedEventArgs
            {
                progressNormalized = dashCooldownTimer / coolDown
            });
        }
    }

    // check player walking or not
    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    //tuong tac (Interact)
    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.getMovementNormalize();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float interactDistance = 2f;
        if (moveDir != Vector3.zero)
        {
            lastMoveInteract = moveDir;
        }
        if (Physics.Raycast(transform.position, lastMoveInteract, out RaycastHit raycastHit, interactDistance, collisionsLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Has ClearCounter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }
    //MOVERMENT
    //private void HandleMovementServerAuth()
    //{
    //    if (KitchenGameManager.Instance.IsGameOver()) return;
    //    Vector2 inputVector = GameInput.Instance.getMovementNormalize();
    //    HandleMovementServerRpc(inputVector);
    //}
    //[ServerRpc(RequireOwnership = false)]
    //private void HandleMovementServerRpc(Vector2 inputVector)
    //{
    //    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    //    float moveDistance = moveSpeed * Time.deltaTime;
    //    //playersize 
    //    float playerRadius = 0.8f;
    //    float playerHeight = 2f;
    //    //Check when polayer can move
    //    bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
    //    if (!canMove)
    //    {
    //        Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
    //        canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
    //        if (canMove)
    //        {
    //            moveDir = moveDirX;
    //        }
    //        else
    //        {
    //            //cannot move on x

    //            //atemp on z
    //            Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
    //            canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
    //            if (canMove)
    //            {
    //                moveDir = moveDirZ;
    //            }
    //            else
    //            {
    //                //cannot move on z
    //            }
    //        }
    //    }

    //    if (canMove)
    //    {
    //        transform.position += moveDir * moveDistance;
    //    }
    //    // is walking when moveDir(vector3) != 0
    //    isWalking = moveDir != Vector3.zero;
    //    //roltate player
    //    float rotateSpeed = 5f;
    //    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    //}
    private void HandleMovement()
    {
        if (KitchenGameManager.Instance.IsGameOver()) return;
        Vector2 inputVector = GameInput.Instance.getMovementNormalize();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        //playersize 
        float playerRadius = 0.8f;
        //Check when polayer can move
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = (moveDir.x < -.5f || moveDir.x >+.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                //cannot move on x

                //atemp on z
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
                else
                {
                    //cannot move on z
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        // is walking when moveDir(vector3) != 0
        isWalking = moveDir != Vector3.zero;
        //roltate player
        float rotateSpeed = 5f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
    private void HandleDash()
    {
            dashTimer += Time.deltaTime;

            Vector2 inputVector = GameInput.Instance.getMovementNormalize();
            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

            float dashDistance = moveSpeed * powerDash * Time.deltaTime;
            float playerRadius = 0.8f;
            float playerHeight = 2f;

            bool canDash = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, dashDistance, collisionsLayerMask);

            if (dashTimer > dashDuration || !canDash)
            {
                isDashing = false;
            }

            if (canDash)
            {
                transform.position += moveDir * dashDistance;
            }
    }
    
    /*private Vector3 GetMoveDirection(Vector2 inputVector)
    {
        string currentScene = Loader.GetCurrentScene();

        if (currentScene == "GameScenes")
        {
           return new Vector3(inputVector.x, 0f, inputVector.y);
        }
        else if (currentScene == "GameScenes2")
        {
            return playerBody.forward * inputVector.y + playerBody.right * inputVector.x;
        }
        else
        {
            return Vector3.zero;
        }
    }*/

    //tuong tac vs counter hien tai 
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCouterChanged?.Invoke(this, new OnSelectedCounterChangedArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return PlayerHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if(kitchenObject != null)
        {
            OnPickUpSomeThing?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomthing?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}