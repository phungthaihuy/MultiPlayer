using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawn;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawn = null;
    }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter baseCounter;
    }

    private Vector3 lastInteraction;
    private BaseCounter baseCounter;
    private KitchenObject kitchenObject;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private LayerMask playersCollisionLayerMask;
    [SerializeField] private Transform playerHoldPoint;
    [SerializeField] private List<Vector3> playerPositionList;

    public bool isWalking;

    private void Start()
    {
        GameInput.Instance.OnInteract += Instance_OnInteract;
        GameInput.Instance.OnInteractAlternate += Instance_OnInteractAlternate;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = playerPositionList[(int)OwnerClientId];

        OnAnyPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }

    private void Instance_OnInteractAlternate(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (baseCounter != null)
        {
            baseCounter.InteractAlternate(this);
        }
    }

    private void Instance_OnInteract(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (baseCounter != null)
        {
            //Debug.Log("---");
            baseCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();
        HandleInteraction();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = GameInput.Instance.GetVectorMovementNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float raycastDistance = 1.5f;
        if (moveDir != Vector3.zero)
        {
            lastInteraction = moveDir;
        }

        if (Physics.Raycast(transform.position, lastInteraction, out RaycastHit raycastHit, raycastDistance, counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != this.baseCounter)
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

    private void HandleMovement()
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            return;
        }
        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;

        Vector2 inputVector = GameInput.Instance.GetVectorMovementNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, playersCollisionLayerMask);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x !=0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, playersCollisionLayerMask);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, playersCollisionLayerMask);

                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        
        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * 10f);

        //Debug.Log(moveDistance);
    }

    private void SetSelectedCounter(BaseCounter baseCounter)
    {
        this.baseCounter = baseCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            baseCounter = baseCounter
        });
    }

    public Transform GetKitchenObjectFollower()
    {
        return playerHoldPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
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
