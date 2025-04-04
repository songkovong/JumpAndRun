using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    [SerializeField] bool rotateToObstacle;
    [SerializeField] float posActionDelay;

    
    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;
    [SerializeField] Vector3 matchPosWeight = new Vector3(0, 1, 0);

    public Quaternion TargetRotation {get; set;} // Dont show in Inspector
    public Vector3 MatchPos {get; set;} // Dont show in Inspector

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y;
        Debug.Log("Height = " + height);

        if(height < minHeight || height > maxHeight) return false;

        /*if(rotateToObstacle){
            //TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
            TargetRotation = Quaternion.LookRotation(-hitData.effectiveHit.normal);
        }*/

        // Just Y Rotation
        if (rotateToObstacle)
        {
            // 장애물의 법선 반대 방향 (정면이 그쪽을 향하게)
            var targetDirection = -hitData.effectiveHit.normal;

            // Y축 평면으로 투영해서 수직 회전 제거
            targetDirection.y = 0f;

            if (targetDirection != Vector3.zero)
            {
                // 현재 오브젝트의 up 방향을 기준으로 Y축만 회전 생성
                TargetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            }
        }

        if(enableTargetMatching) MatchPos = hitData.heightHit.point;

        return true;
    }

    public string Animname => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PosActionDelay => posActionDelay;

    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;

    public Vector3 MatchPosWeight => matchPosWeight;
}
