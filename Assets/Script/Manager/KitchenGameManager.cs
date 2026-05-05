using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class KitchenGameManager : NetworkBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePause;
    public event EventHandler OnLocalGameUnPause;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnPaused;
    public event EventHandler OnLocalReadyPlayerChanged;
   private enum State
    {
        WaitingToStart,
        CowndownToStart,
        GamePlaying,
        GameOver,
    }

    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private NetworkVariable<float> CowndownToStartTimmer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> GamePlayingTimmer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> GamePlayingTimmerMax = new NetworkVariable<float>(100f);
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private bool isLocalPlayerReady;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool autoTestGamePausedState;
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary= new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractActions += GameInput_OnInteractActions;
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;

            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;

            OnMultiplayerGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractActions(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            Time.timeScale = 1f;
            OnLocalReadyPlayerChanged?.Invoke(this, EventArgs.Empty);

            setPlayerReadyServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void setPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allPlayerReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) 
            { 
                allPlayerReady= false;
                break;
            }
        }

        if (allPlayerReady)
        {
            state.Value = State.CowndownToStart;
        }

    }
    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CowndownToStart:
                CowndownToStartTimmer.Value -= Time.deltaTime;
                if (CowndownToStartTimmer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    GamePlayingTimmer.Value = GamePlayingTimmerMax.Value;
                }
                break;
            case State.GamePlaying:
                GamePlayingTimmer.Value -= Time.deltaTime;
                if (GamePlayingTimmer.Value < 0f)
                {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }
    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsGameCountDownToStartActive()
    {
        return state.Value == State.CowndownToStart;
    }
    public float GetCountDownToStartTimer()
    {
        return CowndownToStartTimmer.Value;
    }
    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }
    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }
    public bool IsLocalGamePause()
    {
        return isLocalGamePaused;
    }
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public float GetGamnePlayingTimmer()
    {
        return GamePlayingTimmer.Value;
    }

    public float GetGamePlayingTimerNomalized()
    {
        return 1 - (GamePlayingTimmer.Value / GamePlayingTimmerMax.Value);
    }

    public void setGamnePlayingTimmer(float newTimer)
    {
        if (IsServer)
        {
            GamePlayingTimmer.Value = newTimer;
        }

        SetGamnePlayingTimmerServerRpc(newTimer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetGamnePlayingTimmerServerRpc(float newTimer)
    {
        GamePlayingTimmer.Value = newTimer;
    }

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();

            OnLocalGamePause?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnPauseGameServerRpc();

            OnLocalGameUnPause?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                //this Player Is Pause
                isGamePaused.Value = true;
                return;
            }
        }
        //all player are unpause
        isGamePaused.Value = false;
    }

    public bool IsThisPlayerPause(ulong clientId)
    {
        return playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId];
    }
}
