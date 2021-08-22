using System.Collections;
using Manager;
using ObjectManagement;
using Parent;
using Sound;
using UnityEngine;

namespace Buildings {
    public class Constructing : MonoBehaviour {

        private float oldY;
        [SerializeField]
        private float yToGround;
        private GameObject dustParticles;
        private Vector3 oldPosition;

        private void OnEnable() {
            var building = gameObject.GetComponent<ParentBuilding>();
            dustParticles = PoolHolder.Instance.GetObject(ParentObjectNameEnum.UpgradeParticles);
            var particles = dustParticles.GetComponent<ParticleSystem>();
            EventManager.Instance.BuildingStarted(building);
            building.NavMeshObstacle.enabled = true;
            Sound.Sound.Instance.PlaySoundClipWithSource(new SoundHolder(SoundEnum.BuildingConstruction, 1, 0), building.AudioSource, building.BuildTime);
            oldY = transform.position.y;
            yToGround = building.MeshRenderer.bounds.size.y;
            transform.position = new Vector3(transform.position.x, transform.position.y - yToGround, transform.position.z);
            var shape = particles.shape;
            shape.meshRenderer = (MeshRenderer)building.MeshRenderer;
            dustParticles.SetActive(true);
            StartCoroutine(MoveToPosition(transform, new Vector3(transform.position.x, oldY, transform.position.z), building.BuildTime));
            EventManager.Instance.OnGameResetEvent += ResetObject;
        }

        private void ResetObject() {
            transform.position = oldPosition;
            StopAllCoroutines();
            enabled = false;
        }

        private IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove) {
            var currentPos = transform.position;
            oldPosition = currentPos;
            var t = 0f;
            while (t < 1) {
                t += Time.deltaTime / timeToMove;
                transform.position = Vector3.Lerp(currentPos, position, t);
                yield return null;
            }
            gameObject.GetComponent<ParentBuilding>().enabled = true;
            var particles = dustParticles.GetComponent<ParticleSystem>();
            dustParticles.SetActive(false);
            var shape = particles.shape;
            shape.meshRenderer = null;
            enabled = false;
        }
    }
}
