/**************************************************************
Defalut Code
**************************************************************/

/*using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 bottomRayOffset = new Vector3(0, 0.25f, 0);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 3f;
    [SerializeField] LayerMask obstacleLayer;
    

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();
        var bottomOrigin = transform.position + bottomRayOffset;

        hitData.bottomHitFound = Physics.Raycast(bottomOrigin, transform.forward, out hitData.bottomHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(bottomOrigin, transform.forward * forwardRayLength, (hitData.bottomHitFound) ? Color.red : Color.white);

        hitData.obstacleHit = hitData.bottomHit;
        hitData.obstacleHitFound = hitData.bottomHitFound;

        if(hitData.obstacleHitFound)
        {
            var heightOrigin = hitData.obstacleHit.point + Vector3.up * heightRayLength;

            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, 
                out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
            Debug.Log("Hit = " + hitData.obstacleHit.transform);
        }
        
        return hitData;
    }
}
        
public struct ObstacleHitData
{
    public bool bottomHitFound;
    public bool heightHitFound;
    public RaycastHit bottomHit;
    public RaycastHit heightHit;

    public bool obstacleHitFound;
    public RaycastHit obstacleHit;
}*/



/**************************************************************
Second Code
**************************************************************/

/*using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 topRayOffset = new Vector3(0, 1.65f, 0); // 1.3
    [SerializeField] Vector3 middleRayOffset = new Vector3(0, 0.95f, 0); // 0.5
    [SerializeField] Vector3 bottomRayOffset = new Vector3(0, 0.25f, 0);
    [SerializeField] Vector3 boxTopOffset = new Vector3(0, 1.55f, 0);
    [SerializeField] Vector3 boxMiddleffset = new Vector3(0, 1.05f, 0);
    [SerializeField] Vector3 boxBottomOffset = new Vector3(0, 0.55f, 0);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float forwardHeightRayLength = 0.35f;
    [SerializeField] float heightRayLength = 3f;
    [SerializeField] LayerMask obstacleLayer;
    

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();
        var bottomOrigin = transform.position + bottomRayOffset;
        var middleOrigin = transform.position + middleRayOffset;
        var topOrigin = transform.position + topRayOffset;

        hitData.bottomhitFound = Physics.Raycast(bottomOrigin, transform.forward, out hitData.bottomHit, forwardRayLength, obstacleLayer);
        hitData.middleHitFound = Physics.Raycast(middleOrigin, transform.forward, out hitData.middleHit, forwardRayLength, obstacleLayer);
        hitData.topHitFound = Physics.Raycast(topOrigin, transform.forward, out hitData.topHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(bottomOrigin, transform.forward * forwardRayLength, (hitData.bottomhitFound) ? Color.red : Color.white);
        Debug.DrawRay(middleOrigin, transform.forward * forwardRayLength, (hitData.middleHitFound) ? Color.red : Color.white);
        Debug.DrawRay(topOrigin, transform.forward * forwardRayLength, (hitData.topHitFound) ? Color.red : Color.white);

        if(hitData.topHitFound)
        {
            hitData.obstacleHit = hitData.topHit;
            hitData.obstacleHitFound = hitData.topHitFound;
        }
        else if (hitData.middleHitFound)
        {
            hitData.obstacleHit = hitData.middleHit;
            hitData.obstacleHitFound = hitData.middleHitFound;
        }
        else if (hitData.bottomhitFound)
        {
            hitData.obstacleHit = hitData.bottomHit;
            hitData.obstacleHitFound = hitData.bottomhitFound;
        }
        else
        {
            hitData.obstacleHitFound = false;
        }
        
        var heightOrigin = transform.position + transform.forward * forwardHeightRayLength + (Vector3.up * heightRayLength);
        var rayLength = heightRayLength - 0.25f;

        hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, rayLength, obstacleLayer);

        Debug.DrawRay(heightOrigin, Vector3.down * rayLength, (hitData.heightHitFound) ? Color.red : Color.white);

        return hitData;
    }
}
        
public struct ObstacleHitData
{
    public bool bottomhitFound;
    public bool middleHitFound;
    public bool topHitFound;
    public bool heightHitFound;
    public RaycastHit bottomHit;
    public RaycastHit middleHit;
    public RaycastHit topHit;
    public RaycastHit heightHit;

    public bool obstacleHitFound;
    public RaycastHit obstacleHit;
}
*/

/**************************************************************
Final Code
**************************************************************/


using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] float heightRayLength = 2.5f;
    [SerializeField] LayerMask obstacleLayer;
    float radius = 0.15f;
    float distance = 0.5f;
    Vector3 point1 = new Vector3(0, 0.4f, 0); // start 0.15 (0.4 - 0.15(radius))
    Vector3 point2 = new Vector3(0, 1.85f, 0); // end 2 (1.85 + 0.15(radius))

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();

        hitData.obstacleHitFound = Physics.CapsuleCast(transform.position + point1, transform.position + point2, 
            radius, transform.forward.normalized, out hitData.obstacleHit, distance, obstacleLayer);


        if(hitData.obstacleHitFound)
        {
            var heightOrigin = hitData.obstacleHit.point + transform.forward * 0.05f + Vector3.up * heightRayLength;

            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }
        
        return hitData;
    }
    private void OnDrawGizmos() // GPT script
    {
        // 캡슐 캐스트 파라미터
        Vector3 start = transform.position + point1;
        Vector3 end = transform.position + point2;
        Vector3 direction = transform.forward;

        // 시각화 색상
        Gizmos.color = Color.red;

        // 실제 cast 위치는 방향으로 distance만큼 나아간 곳
        Vector3 offset = direction.normalized * distance;

        // 이동된 캡슐의 위치
        Vector3 movedStart = start + offset;
        Vector3 movedEnd = end + offset;

        // 두 구를 그림 (양 끝에 원형)
        Gizmos.DrawWireSphere(movedStart, radius);
        Gizmos.DrawWireSphere(movedEnd, radius);

        // 측면 원통을 선으로 그림
        Gizmos.DrawLine(movedStart + Vector3.right * radius, movedEnd + Vector3.right * radius);
        Gizmos.DrawLine(movedStart - Vector3.right * radius, movedEnd - Vector3.right * radius);
        Gizmos.DrawLine(movedStart + Vector3.forward * radius, movedEnd + Vector3.forward * radius);
        Gizmos.DrawLine(movedStart - Vector3.forward * radius, movedEnd - Vector3.forward * radius);
    }
}
        
public struct ObstacleHitData
{
    public bool obstacleHitFound;
    public RaycastHit obstacleHit;
    public bool heightHitFound;
    public RaycastHit heightHit;
}