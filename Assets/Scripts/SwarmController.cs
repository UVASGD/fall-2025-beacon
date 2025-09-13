using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour
{
    public List<Transform> swarmModel = new List<Transform>();
    public bool isMoving = false;
    public Vector3 moveDirection;
    public float idleDriftRadius = 2f;
    public float idleMoveSpeed = 1f;
    public float idleRotationSpeed = 1f;

    private void Start()
    {
        RandomizePositionsOfAllModels();
    }

    private void Update()
    {
        if (isMoving)
        {
            foreach (Transform model in swarmModel)
            {
                model.rotation = Quaternion.LookRotation(moveDirection);
                //model.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            foreach (Transform model in swarmModel)
            {
                Vector3 center = transform.position;
                Vector3 offset = new Vector3(
                    Mathf.PerlinNoise(Time.time + model.GetInstanceID()*100f, 0f) - 0.5f,
                    0f,
                    Mathf.PerlinNoise(Time.time * 0.5f, model.GetInstanceID() * 100f) - 0.5f
                );

                offset *= idleDriftRadius * 2f;

                Vector3 target = center + offset;
                Vector3 direction = (target - model.position).normalized; direction.y = 0f;
                model.position += model.forward * idleMoveSpeed * Time.deltaTime;
                model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(direction), Time.deltaTime * idleRotationSpeed);
            }
        }
    }

    public void RandomizePositionsOfAllModels()
    {
        foreach(Transform model in swarmModel)
        {
            model.position = RandomPositionInSwarm();
        }
    }

    public Vector3 RandomPositionInSwarm()
    {
        Vector2 randomPoint = Random.insideUnitCircle * idleDriftRadius;
        return transform.position + randomPoint.x * Vector3.right + randomPoint.y * Vector3.forward;
    }
}

