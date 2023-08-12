using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class StoveCounter : BaseCounter
{
    [SerializeField] private FryingKitchenObjectSO[] fryingKitchenObjectSOArray;
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private BurnedKitchenObjectSO[] burnedKitchenObjectSOArray;
    private NetworkVariable<float> burnedTimer = new NetworkVariable<float>(0f);

    private FryingKitchenObjectSO fryingKitchenObjectSO;
    private BurnedKitchenObjectSO burnedKitchenObjectSO;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public event EventHandler<OnProgressBarChangedEventArgs> OnProgressBarChanged;
    public class OnProgressBarChangedEventArgs : EventArgs
    {
        public float progressBarNomalized;
    }

    public enum State { Idle, Frying, Fired, Burned};
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);

    // Start is called before the first frame update
    //void Start()
    //{
    //    state.Value = State.Idle;
    //}

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burnedTimer.OnValueChanged += BurnedTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }
    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingKitchenObjectSO != null ? fryingKitchenObjectSO.maxFryingTimer : 1f;

        OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
        {
            progressBarNomalized = (float)fryingTimer.Value / fryingTimerMax
        });
    }
    private void BurnedTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burnedTimerMax = burnedKitchenObjectSO != null ? burnedKitchenObjectSO.maxBurnedTimer : 1f;

        OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
        {
            progressBarNomalized = (float)burnedTimer.Value / burnedTimerMax
        });
    }
    private void State_OnValueChanged (State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });
        if (state.Value == State.Idle || state.Value == State.Burned)
        {
            OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
            {
                progressBarNomalized = 0f
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (state.Value)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer.Value += Time.deltaTime;
                //OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
                //{
                //    progressBarNomalized = (float)fryingTimer.Value / fryingTimerMax
                //});
                if (fryingTimer.Value > fryingKitchenObjectSO.maxFryingTimer)
                {
                    KitchenObjectSO fryingKitchenObjectSO = GetOutputFryingKitchenobjectSO(GetKitchenObject().GetKitchenObjectSO());

                    KitchenObject.DestroyKitchenObjectMultiPlayer(GetKitchenObject());
                    //GetKitchenObject().DestroyKitchenObject();

                    KitchenObject.SpawnKitchenObject(fryingKitchenObjectSO, this);
                    //Transform kitchenObjectTransform = Instantiate(fryingKitchenObjectSO.prefab);
                    //kitchenObjectTransform.localPosition = Vector3.zero;

                    //kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);

                    state.Value = State.Fired;
                    //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    //{
                    //    state = state
                    //});

                    burnedTimer.Value = 0f;
                    SetBurnedKitchenObjectSOClientRpc(
                        KitchenGameMultiPlayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );
                    
                    //burnedKitchenObjectSO = GetBurnedKitchenObjectSO(GetKitchenObject().GetKitchenObjectSO());
                }

                break;
            case State.Fired:
                burnedTimer.Value += Time.deltaTime;
                //OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
                //{
                //    progressBarNomalized = (float)burnedTimer.Value / fryingKitchenObjectSO.maxFryingTimer
                //});
                if (burnedTimer.Value > burnedKitchenObjectSO.maxBurnedTimer)
                {
                    KitchenObjectSO burnedKitchenObjectSO = GetOutputBurnedKitchenobjectSO(GetKitchenObject().GetKitchenObjectSO());

                    KitchenObject.DestroyKitchenObjectMultiPlayer(GetKitchenObject());
                    //GetKitchenObject().DestroyKitchenObject();

                    KitchenObject.SpawnKitchenObject(burnedKitchenObjectSO, this);
                    //Transform kitchenObjectTransform = Instantiate(burnedKitchenObjectSO.prefab);
                    //kitchenObjectTransform.localPosition = Vector3.zero;

                    //kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);

                    state.Value = State.Burned;
                    //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    //{
                    //    state = state
                    //});
                }
                break;
            case State.Burned:
                break;
            default:
                break;
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasKitchenObjectSOCanFried(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);

                InteractLogicPlaceObjectOnStoveCounterServerRpc(
                    KitchenGameMultiPlayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                    );
            }
            else
            {
                //player k giu KO
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStateIdleServerRpc();
                //state.Value = State.Idle;
               
                //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                //{
                //    state = state
                //});

                //OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
                //{
                //    progressBarNomalized = 0f
                //});
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddingredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObjectMultiPlayer(GetKitchenObject());
                        //GetKitchenObject().DestroyKitchenObject();

                        SetStateIdleServerRpc();
                        //state.Value = State.Idle;

                        //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        //{
                        //    state = state
                        //});

                        //OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
                        //{
                        //    progressBarNomalized = 0f
                        //});
                    }
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnStoveCounterServerRpc(int kitchenObjectSOIndex)
    {
        state.Value = State.Frying;
        fryingTimer.Value = 0f;

        SetFryingKitchenObjectSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetFryingKitchenObjectSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiPlayer.Instance.GetKitchenObjectSOFormIndex(kitchenObjectSOIndex);
        fryingKitchenObjectSO = GetFryingKitchenObjectSO(kitchenObjectSO);
        //burnedKitchenObjectSO = GetBurnedKitchenObjectSO(kitchenObjectSO);

        //state.Value = State.Frying; chuyen len ServerRpc

        //fryingTimer.Value = 0f; chuyen len ServerRpc

        //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        //{
        //    state = state
        //});
    }
    [ClientRpc]
    private void SetBurnedKitchenObjectSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiPlayer.Instance.GetKitchenObjectSOFormIndex(kitchenObjectSOIndex);
        burnedKitchenObjectSO = GetBurnedKitchenObjectSO(kitchenObjectSO);
    }

    private FryingKitchenObjectSO GetFryingKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        foreach (FryingKitchenObjectSO fryingKitchenObjectSO in fryingKitchenObjectSOArray)
        {
            if (fryingKitchenObjectSO.input == kitchenObjectSO)
            {
                return fryingKitchenObjectSO;
            }
        }
        return null;
    }

    private BurnedKitchenObjectSO GetBurnedKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        foreach (BurnedKitchenObjectSO burnedKitchenObjectSO in burnedKitchenObjectSOArray)
        {
            if (burnedKitchenObjectSO.input == kitchenObjectSO)
            {
                return burnedKitchenObjectSO;
            }
        }
        return null;
    }

    private KitchenObjectSO GetOutputFryingKitchenobjectSO(KitchenObjectSO kitchenObjectSO)
    {
        FryingKitchenObjectSO fryingKitchenObjectSO = GetFryingKitchenObjectSO(kitchenObjectSO);

        if (fryingKitchenObjectSO != null)
        {
            return fryingKitchenObjectSO.output;
        }
        return null;
    }
    private KitchenObjectSO GetOutputBurnedKitchenobjectSO(KitchenObjectSO kitchenObjectSO)
    {
        BurnedKitchenObjectSO burnedKitchenObjectSO = GetBurnedKitchenObjectSO(kitchenObjectSO);

        if (burnedKitchenObjectSO != null)
        {
            return burnedKitchenObjectSO.output;
        }
        return null;
    }

    private bool HasKitchenObjectSOCanFried(KitchenObjectSO kitchenObjectSO)
    {
        FryingKitchenObjectSO fryingKitchenObjectSO = GetFryingKitchenObjectSO(kitchenObjectSO);

        return fryingKitchenObjectSO != null;
    }
}
