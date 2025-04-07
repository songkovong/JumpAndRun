using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;

    bool inAction = false;

    EnvironmentScanner environmentScanner;
    Animator animator;
    PlayerController playerController;

    void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        environmentScanner.ObstacleCheck();
        if(Input.GetButtonDown("Jump") && !inAction)
        {
            var hitData = environmentScanner.ObstacleCheck();

            // if(hitData.obstacleHitFound)
            if(hitData.obstacleHitFound && hitData.heightHitFound)
            {
                if(hitData.obstacleHit.transform.Equals(hitData.heightHit.transform))
                {
                    foreach(var action in parkourActions)
                    {
                        if(action.CheckIfPossible(hitData, transform))
                        {
                            StartCoroutine(DoParkourAction(action));
                            break;
                        }
                    }
                }
            }
            /*if(hitData.effectiveHit.transform.Equals(hitData.heightHit.transform))
            {
                foreach(var action in parkourActions)
                {
                    if(action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }*/

            /*if(hitData.forwardHitFound)
            {
                foreach(var action in parkourActions)
                {
                    if(action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }*/
        }
    }

    IEnumerator DoParkourAction(ParkourAction action)
    {
        inAction = true;
        playerController.SetControl(false);

        // smoothly
        animator.CrossFade(action.Animname, 0.05f); // 0.2f is too slow
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if(!animState.IsName(action.Animname))
            Debug.LogError("The parkour animation is wrong!");

        // Run in parallel unlike Waitforseconds
        float timer = 0f;
        while(timer <= animState.length)
        {
            timer += Time.deltaTime;

            // Rotate the player towards the obstacle
            if(action.RotateToObstacle) 
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, 
                    playerController.RotationSpeed * Time.deltaTime);

            if(action.EnableTargetMatching) {
                MatchTarget(action);
            }

            yield return null;
        }

        yield return new WaitForSeconds(action.PosActionDelay);

        playerController.SetControl(true);
        inAction = false;
    }

    public bool returnInAction()
    {
        return inAction;
    }

    void MatchTarget(ParkourAction action)
    {
        int layerIndex = 0;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (animator.isMatchingTarget || animator.IsInTransition(layerIndex) || stateInfo.normalizedTime < 0.1f) return;

        // MatchTarget to animator
        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart, 
            new MatchTargetWeightMask(action.MatchPosWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}