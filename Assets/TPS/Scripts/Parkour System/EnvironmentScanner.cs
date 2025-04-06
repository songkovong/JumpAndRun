using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 topRayOffset = new Vector3(0, 1.65f, 0); // 1.3
    [SerializeField] Vector3 middleRayOffset = new Vector3(0, 0.95f, 0); // 0.5
    [SerializeField] Vector3 bottomRayOffset = new Vector3(0, 0.25f, 0);
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
            hitData.effectiveHit = hitData.topHit;
            hitData.effectiveHitFound = hitData.topHitFound;
        }
        else if (hitData.middleHitFound)
        {
            hitData.effectiveHit = hitData.middleHit;
            hitData.effectiveHitFound = hitData.middleHitFound;
        }
        else if (hitData.bottomhitFound)
        {
            hitData.effectiveHit = hitData.bottomHit;
            hitData.effectiveHitFound = hitData.bottomhitFound;
        }
        else
        {
            hitData.effectiveHitFound = false;
        }

        /*if(hitData.effectiveHitFound)
        {
            var heightOrigin = hitData.effectiveHit.point + Vector3.up * heightRayLength;

            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, 
                out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
            Debug.Log("Hit = " + hitData.effectiveHit.transform);
        }*/
        
        var heightOrigin = transform.position + transform.forward * forwardHeightRayLength + (Vector3.up * heightRayLength);
        var rayLength = heightRayLength - 0.25f;

        hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, rayLength, obstacleLayer);

        Debug.DrawRay(heightOrigin, Vector3.down * rayLength, (hitData.heightHitFound) ? Color.red : Color.white);

        /*if(hitData.bottomhitFound)
        {
            var heightOrigin = hitData.bottomHit.point + Vector3.up * heightRayLength;

            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, 
                out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }

        if(hitData.middleHitFound)
        {
            var heightOrigin = hitData.middleHit.point + Vector3.up * heightRayLength;
            
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, 
                out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }*/

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

    public bool effectiveHitFound;
    public RaycastHit effectiveHit;
}
