using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using SmallyGames.Menus;
using UnityEngine.SceneManagement;
using TMPro;
using MoreMountains.NiceVibrations;
using SmallyGames.Settings;


namespace CrowdStep
{
    public class GameManager : MonoBehaviour
    {
        [BoxGroup("Static Data"), ShowInInspector, Tooltip("Game manager singleton instance")]                     
        public static GameManager instance;

        [BoxGroup("Resources Data"), SerializeField, Tooltip("Resources Manager holding shopping category")]
        private ResourcesManager resourcesManager;
        public ResourcesManager ResourcesManager => resourcesManager;


        [BoxGroup("Player Controller Data"), SerializeField, Tooltip("player controller controlling the spline and flock")]                
        private PlayerController playerController;
        public PlayerController PlayerController => playerController;

        [BoxGroup("Level Data"), SerializeField, Tooltip("game level to be spawned from shop category")]
        private Transform levelHolder;

        [BoxGroup("Level Data"), SerializeField, Tooltip("game level to be spawned from shop category")]                       
        private Level gameLevel;
        public Level GameLevel => gameLevel;

        [BoxGroup("Camera Data"), SerializeField, Tooltip("main camera controlled my cinemachine camera rig")]                       
        private Camera mainCamera;
        public Camera MainCamera => mainCamera;

        [BoxGroup("CameraData"), SerializeField, Tooltip("Virtual Camera responsible for following character")]
        private CinemachineVirtualCamera followCam;
        public CinemachineVirtualCamera FollowCam => followCam;

        [BoxGroup("Camera Data"), SerializeField, Tooltip("virtual camera to activate after player wins")]
        private CinemachineVirtualCamera winCam;
        public CinemachineVirtualCamera WinCam => winCam;

        [BoxGroup("Camera Data"), SerializeField, Tooltip("virtual camera to activate after player loses")]
        private CinemachineVirtualCamera loseCam;
        public CinemachineVirtualCamera LoseCam => loseCam;

        [BoxGroup("Game Menu Data"), SerializeField, Tooltip("level slider from smally tools")]
        private LevelSliderManager levelSlider;

        [BoxGroup("Game Menu Data"), SerializeField, Tooltip("reward coins manager responsible for animating diamond on collect and on game over")]
        private RewardCoinsManager rewardCoinsManager;
        public RewardCoinsManager RewardCoinsManager => rewardCoinsManager;

        [BoxGroup("Game Over Data"), SerializeField, Tooltip("extra gameover win design Text to show how much we multiply be win reward")]
        private TextMeshProUGUI multiplierText;

        [BoxGroup("Game States"), ShowInInspector, ReadOnly] //if character has crossed finish line
        public bool FinishLineCrossed { get; set; }

        [BoxGroup("Game States"), ShowInInspector, ReadOnly] //is character above reward step
        public bool AboveStep { get; set; }

        [BoxGroup("Game States"), ShowInInspector, ReadOnly] //has the game started
        public bool GameStarted { get; set; }

        [BoxGroup("Game States"), ShowInInspector, ReadOnly] //Is the game over
        public bool GameOver { get; set; }

        public bool debugging;
        /// <summary>
        /// singleton and game manager initialized in start
        /// </summary>
        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }

            Init();
        }


        /// <summary>
        /// game manager init functoin initing 
        /// player controller and level slider
        /// </summary>
        private void Init()
        {
            if(!debugging) SpawnLevel();
            playerController.Init();
            levelSlider.Init(playerController.SplineFollower.transform, gameLevel.FinishLine, 
                ResourcesManager.LevelCat.SelectedItem + 1);
        }

        private void SpawnLevel()
        {
            var levItem = ResourcesManager.LevelCat.InventoryItems[ResourcesManager.LevelCat.SelectedItem];
            gameLevel = Instantiate(levItem.Prefab, levelHolder, false).GetComponent<Level>();
        }

        /// <summary>
        /// start game called from screen button
        /// and initiates moving the spline
        /// </summary>
        public void StartGame()
        {
            GameStarted = true;
            playerController.BeginMoveCrowd();
        }


        /// <summary>
        /// called from each checkpoint script 
        /// </summary>
        /// <param name="finishLine"> is the checkpoint calling this function the finish line?</param>
        public void CheckPointCrossed(bool finishLine)
        {
            if (finishLine)
            {
                FinishLineCrossed = true;
                //Debug.Log("crossed finishLine");
                /*playerController.Character.Rb.*/
            }
        }


        /// <summary>
        /// Game over function 
        /// </summary>
        /// <param name="win">did the player win or lose</param>
        public void OnGameOver(bool win)
        {
            GameOver = true;

            if (win)
            {
                var reward = (int)(100 * playerController.Character.GameOverRewardMultiplier);
                multiplierText.text = $"x{playerController.Character.GameOverRewardMultiplier}";
                Doozy.Engine.GameEventMessage.SendEvent(StaticStrings.PlayerWin_DoozyEvent);
                rewardCoinsManager.OnGameOver(reward);
                ///multiplierText.text = $"x{reward}";
                StartCoroutine(nameof(AwaitShowRewardT));

                if (resourcesManager.LevelCat.SelectedItem >= resourcesManager.LevelCat.InventoryItems.Length - 1)
                {
                    resourcesManager.LevelCat.SelectedItem = 0;
                }
                else
                {
                    resourcesManager.LevelCat.SelectedItem += 1;

                }
            }
            else
            {
                Doozy.Engine.GameEventMessage.SendEvent(StaticStrings.PlayerLose_DoozyEvent);
            }
        }

        /// <summary>
        /// delay before animating reward coins
        /// </summary>
        /// <returns></returns>
        IEnumerator AwaitShowRewardT()
        {
            yield return new WaitForSeconds(.5f);
            rewardCoinsManager.AnimateRewardCoins();
            multiplierText.GetComponentInParent<Doozy.Engine.UI.UIView>().Show();
        }

        /// <summary>
        /// called from game over view buttons to reload scene
        /// </summary>
        /// <param name="scene"></param>
        public void LoadLevel(string scene)
        {
            SceneManager.LoadScene(scene);
        }


        /// <summary>
        /// haptic function called from other scripts to add vibration to the games
        /// </summary>
        /// <param name="t"></param>
        public void PlayHaptic(HapticTypes t)
        {
            if (resourcesManager.Settings.ViberateOn)
            {
                MMVibrationManager.Haptic(t);
            }
        }
    }

    /// <summary>
    /// player input class
    /// </summary>
    [System.Serializable]
    public class GameInput
    {
        [SerializeField]
        private bool screenDown;
        public bool ScreenDown => screenDown = SimpleInput.GetButton(StaticStrings.Ascend_Input);
    }
}
