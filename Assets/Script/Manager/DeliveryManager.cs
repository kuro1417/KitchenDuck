using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeComplted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }
    
    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;

    private const string HIGHSCRORE = "HighScore";
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimeMax = 7f;
    private int waitingRecipeMax = 6;
    private int recipeSuccess;
    [SerializeField] private TextMeshProUGUI highScoreText;
    private void Awake()
    {
       Instance= this;


        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        if (!IsServer) {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimeMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
               
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
                //RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                
                //waitingRecipeSOList.Add(waitingRecipeSO);

                //OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
        UpdateHighScoreText();
    }
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for(int i=0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //has Same number of ingredient
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList){
                    //Cycling through all ingredient in Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //Cycling through all ingrediant in Plate
                        if(plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            // Ingredient Matches
                            ingredientFound =true; 
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //this Recipe ingredient was not found on Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    //Player delivery correct recipe
                    DeliveryCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        // No Matches found
        //Player did not delivery a correct recipe
        DeliveryIncorrectRecipeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryIncorrectRecipeServerRpc()
    {
        DeliveryIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliveryIncorrectRecipeClientRpc()
    {
        float MinusTimmer = KitchenGameManager.Instance.GetGamnePlayingTimmer();
        MinusTimmer -= 5;
        KitchenGameManager.Instance.setGamnePlayingTimmer(MinusTimmer);
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliveryCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        recipeSuccess++;
        float PlayingTimmer = KitchenGameManager.Instance.GetGamnePlayingTimmer();
        PlayingTimmer += 10;
        KitchenGameManager.Instance.setGamnePlayingTimmer(PlayingTimmer);
        CheckHighScore();
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeComplted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return recipeSuccess;
    }

    public void CheckHighScore()
    {
        if (recipeSuccess > PlayerPrefs.GetInt(HIGHSCRORE, 0))
        {
            PlayerPrefs.SetInt(HIGHSCRORE, recipeSuccess);
            PlayerPrefs.Save();
        }
    }

    public void UpdateHighScoreText()
    {
        highScoreText.text = (PlayerPrefs.GetInt(HIGHSCRORE, 0)).ToString();
    }
}