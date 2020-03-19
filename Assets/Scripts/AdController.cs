using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class AdController : MonoBehaviour
{
    private string _gameId = "Fussy Clucks";
    private string _placementId = "rewardedVideo";
    private string loadScene = "Level 1";

    private void Awake()
    {
        //True gives us test mode.
        Advertisement.Initialize(_gameId, true);
    }

    public void RewardAdTriggered()
    {
        #if UNITY_ADS
                RewardVideoAd();
        #endif
    }

    private void RewardVideoAd()
    {
        if (Advertisement.IsReady(_placementId))
        {
            Debug.Log("Advert Ready");

            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;

            Advertisement.Show(_placementId, options);
        }
        else
        {
            Debug.Log("Advert Not Ready");
            return;
        }
    }

    /* Handler for the result from the advert*/
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Failed:
                Debug.Log("Failed");
                break;
            case ShowResult.Finished:
                Debug.Log("Finished");
                SceneManager.LoadScene(loadScene);
                break;
            case ShowResult.Skipped:
                Debug.Log("Skipped");
                SceneManager.LoadScene(loadScene);
                break;
        }
    }
}