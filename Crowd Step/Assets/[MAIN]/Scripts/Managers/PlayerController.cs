using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Dreamteck.Splines;
using DG.Tweening;
using MoreMountains.Feedbacks;


namespace CrowdStep
{
    public class PlayerController : MonoBehaviour
    {
/*        [BoxGroup("Character Data"), SerializeField]
        private GameObject characterPrefab;*/

        [BoxGroup("Character Data"), SerializeField]
        private Character character;
        public Character Character => character;

        [BoxGroup("Character Data"), SerializeField]
        private Transform characterHolder;
        public Transform CharacterHolder => characterHolder;

        [BoxGroup("Character Data"), SerializeField]
        private float ascendSpeed = 7f;

        [BoxGroup("Spline Data"), SerializeField]
        private SplineFollower splineFollower;
        public SplineFollower SplineFollower => splineFollower;

        [BoxGroup("Spline Data"), SerializeField]
        private float moveSpeed = 7f;

/*        [BoxGroup("Flock Agent Data"), SerializeField]
        private GameObject agentPrefab;*/

        [BoxGroup("Flock Agent Data"), SerializeField]
        private float agentascendSpeed = 7f;

        //[BoxGroup("Flock Agent Data"), SerializeField]
        //private float agentascendDecSpeed = 0.03f;

        [BoxGroup("Flock Agent Data"), SerializeField]
        private MMFeedbacks lose_FB;

        [BoxGroup("Flock Agent Data"), SerializeField]
        private MMFeedbacks layingSteps_FB;
        
        /*[BoxGroup("Flock Agent Data"), SerializeField]
        private MMFeedbacks spawnAgent_FB;*/

        [BoxGroup("Flock Data"), SerializeField]
        private List<FlockAgent> flockAgents;
        public List<FlockAgent> FlockAgents => flockAgents;

        [BoxGroup("Flock Data"), SerializeField, ReadOnly]
        private List<FlockAgent> flockAgentsPool;
        public List<FlockAgent> FlockAgentsPool => flockAgentsPool;



        [BoxGroup("Flock Data"), SerializeField]
        private float agentSpawnRadius = 1f;

        [BoxGroup("Flock Step Data"), SerializeField]
        private float placeStepDelay = 0.2f;

        [BoxGroup("Flock Data"), SerializeField]
        private float flockSpread_Dist = 0.8f;

        [BoxGroup("Flock Data"), SerializeField]
        private float flockSpreadSpeed = 3f;

        [BoxGroup("Flock Data"), SerializeField]
        private float flockRadius = 0.24f;

        //[BoxGroup("Flock Data"), SerializeField]
        //private float flockSqueezeSpeed = 1f;

        [BoxGroup("Input Data"), SerializeField, ReadOnly]
        private GameInput input;

        [BoxGroup("States"), ReadOnly, SerializeField]
        private bool flockAscending;

        [BoxGroup("States"), ReadOnly, SerializeField]
        private bool layingSteps;

        [BoxGroup("States"), ReadOnly, SerializeField]
        private bool levelFinished;

        [BoxGroup("States"), ReadOnly, SerializeField]
        private bool canApplyFlockBehavior = false;


        private int agentTotalCount = 0;

        private void FixedUpdate()
        {
            flockAscending = false;
            if (input.ScreenDown && flockAgents.Count > 0 && !GameManager.instance.GameOver && !levelFinished && character.CanHandleMove)
            {
                Ascend();
                //AgentAscend();
            }
            /*else
            {
                for (int i = 0; i < flockAgents.Count; i++)
                {
                    flockAgents[i].Rb.isKinematic = false;
                }
            }*/

            character.Rb.velocity = Vector3.ClampMagnitude(character.Rb.velocity, ascendSpeed + 30f);


            if (canApplyFlockBehavior)
            {
                ApplyFlockBehavior();
            }
        }


        /// <summary>
        /// init player controller
        /// </summary>
        public void Init()
        {
            if(!GameManager.instance.debugging) splineFollower.spline = GameManager.instance.GameLevel.Spline;
            SpawnCharacter();
            GameManager.instance.FollowCam.Follow = character.transform;
            GameManager.instance.FollowCam.LookAt = character.transform;
            //StartCoroutine(nameof(AwaitFollowLeader));
        }


        /// <summary>
        /// responsible for spawning main character
        /// </summary>
        private void SpawnCharacter()
        {
            //characterPrefab
            var charItem = GameManager.instance
                .ResourcesManager.CharacterCat
                .InventoryItems[GameManager.instance
                .ResourcesManager.CharacterCat.SelectedItem];
            character = Instantiate(charItem.Prefab, characterHolder, false).GetComponent<Character>();
            character.OnCharacterLand += OnCharacterLand;
            character.OnStepLand += OnFinishStepLand;
            character.OnObstacleHit += OnObstacleHit;
            character.Init();
            character.FlockCount_Text.text = (flockAgents.Count + 1).ToString();
            //onSpawnAgent_FB?.PlayFeedbacks(character.transform.position + (Vector3.up * 0.25f));
        }

        public void CharacterCrossedGate(int count)
        {
            //Debug.Log(count) ;
            for (int i = 0; i < count; i++)
            {

                //Debug.Log(i);
                SpawnAgent();
            }

            character.FlockCount_Text.text = (flockAgents.Count + 1).ToString();
            OnCharacterLand();
        }


        public void OnCharacterLand()
        {
            
            if (!canApplyFlockBehavior)
            {
                DOVirtual.Float(0f, 1f, 1f, (x) => { })
                .OnStart(() => canApplyFlockBehavior = true)
                .OnComplete(() => canApplyFlockBehavior = false);
            }
        }

        /// <summary>
        /// responsible for spawning flock agents
        /// </summary>
        private void SpawnAgent()
        {
            var spawnPos = Random.insideUnitSphere * agentSpawnRadius;
            spawnPos.y = 0f;

            FlockAgent agent = flockAgentsPool.Find((x) => !x.gameObject.activeInHierarchy);

            if (agent == null)
            {
                var agentItem = GameManager.instance
                .ResourcesManager.CharacterCat
                .InventoryItems[GameManager.instance
                .ResourcesManager.CharacterCat.SelectedItem];
                agent = Instantiate(agentItem.PrefabSecondary, spawnPos + (character.transform.position + Vector3.back * 3),
                Quaternion.Euler(Vector3.zero), characterHolder).GetComponent<FlockAgent>();
            }
            else
            {
                flockAgentsPool.Remove(agent);
                agent.gameObject.SetActive(true);
                agent.transform.SetParent(characterHolder);
                agent.transform.position = spawnPos + (character.transform.position + Vector3.back * 3);
                agent.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }


            //spawnAgent_FB?.PlayFeedbacks(agent.transform.position + (Vector3.up * 1f));


            //agent.Connector.connectedBody = flockBody;
            agent.name = $"Agent {agentTotalCount++}";
            agent.CanHandleMove = true;
            agent.Init();
            //onSpawnAgent_FB?.PlayFeedbacks(agent.transform.position + (Vector3.up * 0.25f));
            agent.Anim.SetBool(StaticStrings.Move_AnimParam, true);
            flockAgents.Add(agent);

            


        }

        /// <summary>
        /// moves player upwards to simulate the step climbing 
        /// effect
        /// </summary>
        private void Ascend()
        {
            flockAscending = true;
            character.Ascend(ascendSpeed);
            character.Anim.SetBool(StaticStrings.Move_AnimParam, true);

            if (!layingSteps && flockAgents.Count > 0)
            {
                StartCoroutine(nameof(AwaitLayingSteps));
            }

        }

        /*private void AgentAscend()
        {
            var vel = agentascendSpeed;
            for (int i = 0; i < flockAgents.Count; i++)
            {
                //var fa = flockAgents[i];
                var agent = flockAgents[i];
                //agent.Rb.isKinematic = true;
                //if (!fa.CanHandleMove) continue;

                agent.Ascend(vel);
               
            }
        }*/

        IEnumerator AwaitLayingSteps()
        {
            layingSteps = true;
            yield return new WaitForSeconds(0.035f);
            var tempArr = flockAgents.ToArray();
            for (int i = tempArr.Length - 1; i >= 0; i--)
            {
                if (!flockAscending)
                {
                    character.Anim.SetBool(StaticStrings.Move_AnimParam, false);
                    if (GameManager.instance.AboveStep)
                    {
                        OnLevelFinished();
                    }
                    break;
                }
                var agent = tempArr[i];
                flockAgents.Remove(agent);
                var pos = character.transform.localPosition - (Vector3.up * 0.15f);
                agent.CanHandleMove = false;
                agent.transform.localEulerAngles = new Vector3(-90f, 0f, 90f);
                agent.Rb.isKinematic = true;
                pos.x = 0.5f;
                agent.transform.localPosition =  pos;
                agent.transform.SetParent(null);
                //agent.Anim.SetBool(StaticStrings.Move_AnimParam, false);
                agent.OnStep();
                character.FlockCount_Text.text = (flockAgents.Count + 1).ToString();
                GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);
                FlockAgentsPool.Add(agent);
                layingSteps_FB.PlayFeedbacks(character.transform.position);
                yield return new WaitForSeconds(placeStepDelay);
            }
            /*while (flockAscending && flockAgents.Count > 0)
            {
                
            }*/
            layingSteps = false;
        }

        /// <summary>
        /// responsible for moving enter crowd with spline
        /// </summary>
        public void BeginMoveCrowd()
        {
            splineFollower.followSpeed = moveSpeed;
            character.CanHandleMove = true;
            character.Anim.SetBool(StaticStrings.Move_AnimParam, true);
            for (int i = 0; i < flockAgents.Count; i++)
            {
                flockAgents[i].Anim.SetBool(StaticStrings.Move_AnimParam, true);
            }
        }

        /// <summary>
        /// handles what happens when we step on reward stairs
        /// </summary>
        private void OnFinishStepLand()
        {
            if (GameManager.instance.GameOver)
            {
                Debug.Log("Game already over");
                return;
            }
            OnWin();
            splineFollower.followSpeed = 0;
            character.Anim.SetTrigger(StaticStrings.Win_AnimParam);
            character.Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            //character.Rb.isKinematic = true;
            character.Rb.constraints = RigidbodyConstraints.FreezeRotation;
            //character.Rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
            character.transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f);

            GameManager.instance.WinCam.LookAt = character.transform;
            GameManager.instance.WinCam.Follow = character.transform;
            GameManager.instance.WinCam.gameObject.SetActive(true);
        }

        private void OnObstacleHit()
        {
            if (GameManager.instance.GameOver)
            {
                Debug.Log("Game already over");
                return;
            }
            //OnLose();
            splineFollower.followSpeed = 0;
            GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.RigidImpact);
            //character.Anim.SetTrigger(StaticStrings.Lose_AnimParam);
            character.Anim.SetBool(StaticStrings.Move_AnimParam, false);
            //character.Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            DetFlock(5);
            DOVirtual.Float(0, 15f, 0.5f, (x) => 
            {
                var vel = Vector3.up * (x/2);
                splineFollower.followSpeed = x;
                character.Rb.velocity = vel;
            })
                .OnStart(()=>
                {
                    splineFollower.applyDirectionRotation = false;
                    splineFollower.direction = Spline.Direction.Backward;
                    character.CanHandleMove = false;
                })
                .OnComplete(()=>
                {
                    splineFollower.applyDirectionRotation = true;
                    splineFollower.direction = Spline.Direction.Forward;
                    character.CanHandleMove = true;
                    if (flockAgents.Count <= 0)
                    {
                        Debug.Log("Flock Finished, Player Lose");
                        OnLose();
                        character.CharacterDead = true;
                        character.Anim.SetTrigger(StaticStrings.Lose_AnimParam);
                        character.Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                        character.Rb.constraints = RigidbodyConstraints.FreezeRotation;
                        GameManager.instance.LoseCam.LookAt = character.transform;
                        GameManager.instance.LoseCam.Follow = character.transform;
                        GameManager.instance.LoseCam.gameObject.SetActive(true);
                        lose_FB.PlayFeedbacks(character.transform.position);
                    }
                });
            //character.Rb.constraints = RigidbodyConstraints.FreezeRotation;


            //GameManager.instance.LoseCam.LookAt = character.transform;
           //// GameManager.instance.LoseCam.Follow = character.transform;
            //GameManager.instance.LoseCam.gameObject.SetActive(true);
        }

        private void DetFlock(int amount)
        {
            if (flockAgents.Count > 0)
            {
                if (amount > flockAgents.Count)
                {
                    amount = flockAgents.Count;
                }
                var tempArr = flockAgents.ToArray();

                for (int i = 0; i < amount; i++)
                {
                    var agent = tempArr[i];
                    flockAgents.Remove(agent);
                    agent.CanHandleMove = false;
                    agent.Rb.isKinematic = true;
                    agent.transform.SetParent(null);
                    //agent.Anim.SetBool(StaticStrings.Move_AnimParam, false);
                    agent.DeactivateAgent();
                    character.FlockCount_Text.text = (flockAgents.Count + 1).ToString();
                    GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);
                    FlockAgentsPool.Add(agent);
                }
            }
        }

        public void OnLevelFinished()
        {
            levelFinished = true;

            splineFollower.followSpeed = 0;
            character.Anim.SetBool(StaticStrings.Move_AnimParam, false);
        }

        /// <summary>
        /// responsible for implementing 
        /// flock behavior to our flock
        /// </summary>
        /// 
        private void ApplyFlockBehavior()
        {
            for (int i = 0; i < flockAgents.Count; i++)
            {
                var agent = flockAgents[i];
                if (!agent.CanHandleMove) continue;
                agent.Move(AvoidanceBehavior(flockAgents[i]), flockSpreadSpeed);
                //agent.Move(CohesionBehavior(flockAgents[i]), flockSqueezeSpeed);
            }
        }

        private void OnWin()
        {
            character.Anim.SetBool(StaticStrings.Move_AnimParam, false);
            for (int i = 0; i < flockAgents.Count; i++)
            {
                flockAgents[i].Anim.SetTrigger(StaticStrings.Win_AnimParam);
            }
            GameManager.instance.OnGameOver(true);
        }

        private void OnLose()
        {
            //character.Anim.SetBool(StaticStrings.Move_AnimParam, false);
            for (int i = 0; i < flockAgents.Count; i++)
            {
                flockAgents[i].Anim.SetTrigger(StaticStrings.Cry_AnimParam);
            }
            GameManager.instance.OnGameOver(false);
        }

        /// <summary>
        /// flock behavior that keeps flocks slightly away from each other
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        private Vector3 AvoidanceBehavior(FlockAgent agent)
        {
            var avoidanceMovement = Vector3.zero;
            if (flockAgents.Count < 1) return avoidanceMovement;
            //Debug.Log("Bow");
            var numAvoid = 0;

            for (int i = 0; i < flockAgents.Count + 1; i++)
            {
                if (i == flockAgents.Count)
                {
                    if (Vector3.Distance(character.transform.localPosition, agent.transform.localPosition) < flockSpread_Dist)
                    {
                        numAvoid++;
                        avoidanceMovement -= (character.transform.localPosition - agent.transform.localPosition).normalized;
                    }
                }
                else
                {
                    if (agent == flockAgents[i]) continue;
                    if (Vector3.Distance(flockAgents[i].transform.localPosition, agent.transform.localPosition) < flockSpread_Dist)
                    {
                        numAvoid++;
                        avoidanceMovement -= (flockAgents[i].transform.localPosition - agent.transform.localPosition).normalized;
                    }
                }

            }

            if (numAvoid > 0) avoidanceMovement /= numAvoid;

            return avoidanceMovement;
        }

        private Vector3 CohesionBehavior(FlockAgent agent)
        {
            var cohesionMovement = Vector3.zero;
            var cohese = true;
            var numCohese = 0;
            if (flockAgents.Count == 0) return cohesionMovement;

            for (int i = 0; i < flockAgents.Count; i++)
            {
                if (agent == flockAgents[i]) continue;
                cohesionMovement += flockAgents[i].transform.position;
                if (!(Vector3.Distance(flockAgents[i].transform.localPosition, agent.transform.localPosition) > flockRadius))
                {
                    cohese = false;
                }
            }

            cohesionMovement /= flockAgents.Count;

            cohesionMovement -= agent.transform.position;

            if (numCohese <= 2) cohese = false;

            if (!cohese) cohesionMovement = Vector3.zero;

            return cohesionMovement;
        }
    }
}
