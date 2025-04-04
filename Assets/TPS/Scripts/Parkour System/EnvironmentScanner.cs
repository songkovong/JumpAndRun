using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0);
    [SerializeField] Vector3 middleRayOffset = new Vector3(0, 0.95f, 0); // 0.5
    [SerializeField] Vector3 topRayOffset = new Vector3(0, 1.65f, 0); // 1.3
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 5f;
    [SerializeField] LayerMask obstacleLayer;

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();
        var forwardOrigin = transform.position + forwardRayOffset;
        var middleOrigin = transform.position + middleRayOffset;
        var topOrigin = transform.position + topRayOffset;

        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, out hitData.forwardHit, forwardRayLength, obstacleLayer);
        hitData.middleHitFound = Physics.Raycast(middleOrigin, transform.forward, out hitData.middleHit, forwardRayLength, obstacleLayer);
        hitData.topHitFound = Physics.Raycast(topOrigin, transform.forward, out hitData.topHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.white);
        Debug.DrawRay(middleOrigin, transform.forward * forwardRayLength, (hitData.middleHitFound) ? Color.red : Color.white);
        Debug.DrawRay(topOrigin, transform.forward * forwardRayLength, (hitData.topHitFound) ? Color.red : Color.white);


        if(hitData.topHitFound)
        {
            hitData.effectiveHit = hitData.topHit;
            hitData.effectiveHitFound = true;
        }
        else if (hitData.middleHitFound)
        {
            hitData.effectiveHit = hitData.middleHit;
            hitData.effectiveHitFound = true;
        }
        else if (hitData.forwardHitFound)
        {
            hitData.effectiveHit = hitData.forwardHit;
            hitData.effectiveHitFound = true;
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
        
        var FRayLength = 0.3f;
        var HRayLength = 3f;
        var heightOrigin = transform.position + transform.forward * FRayLength + (Vector3.up * HRayLength);
        var rayLength = HRayLength - 0.25f;

        hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, rayLength, obstacleLayer);

        Debug.DrawRay(heightOrigin, Vector3.down * rayLength, (hitData.heightHitFound) ? Color.red : Color.white);

        /*if(hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;

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
    public bool forwardHitFound;
    public bool middleHitFound;
    public bool topHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit middleHit;
    public RaycastHit topHit;
    public RaycastHit heightHit;

    public bool effectiveHitFound;
    public RaycastHit effectiveHit;
}
