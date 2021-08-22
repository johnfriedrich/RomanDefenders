using UnityEngine;

namespace Entities {
    public class DeathAngelJobTest : MonoBehaviour {

        ////TODO: Check if it gains actual performance
        //[BurstCompile]
        //public struct MovementJob : IJobParallelForTransform {

        //    public float moveSpeed;
        //    public float offset;
        //    public float deltaTime;
 
        //    public void Execute(int index, TransformAccess transform) {
        //        Vector3 pos = transform.position;
        //        pos += moveSpeed* deltaTime * (transform.rotation* new Vector3(0f, 0f, 1f));
        //        transform.position = pos;
        //    }
        //}

        ////Transforms to move
        //TransformAccessArray transforms;
        //MovementJob moveJob;
        //JobHandle moveHandle;


        //void Update () {
        //    moveHandle.Complete();

        //    moveJob = new MovementJob() {
        //        moveSpeed = 1,
        //        offset = 3,
        //        deltaTime = Time.deltaTime
        //    };

        //    moveHandle = moveJob.Schedule(transforms);
        //    JobHandle.ScheduleBatchedJobs();
        //}
    }
}
