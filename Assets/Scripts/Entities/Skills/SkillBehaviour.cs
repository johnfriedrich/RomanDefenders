using Manager;
using ObjectManagement;
using Parent;
using Sound;
using UnityEngine;

namespace Entities.Skills {
    public class SkillBehaviour : MonoBehaviour {

        [SerializeField]
        private Sprite uiSprite;
        [SerializeField]
        private ParentObjectNameEnum impactType;
        [SerializeField]
        private SoundEnum impactSound;
        [SerializeField]
        private SoundHolder castSoundHolder;
        [SerializeField]
        private SoundHolder flySoundHolder;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        protected new string name;
        [SerializeField]
        private string description;
        [SerializeField]
        protected float damage;
        [SerializeField]
        private float speed;
        protected Transform target;
        private Vector3 targetPos;
        [SerializeField]
        protected float cooldownAmount;
        [SerializeField]
        protected float radius;
        [SerializeField]
        private float castTime;
        [SerializeField]
        private float range;
        [SerializeField]
        private GameObject smallEffect;
        [SerializeField]
        private GameObject bigEffect;
        private bool inactive;
        private Entity caster;

        public string Name => name;

        public float CooldownAmount => cooldownAmount;

        public GameObject SmallEffect => smallEffect;

        public GameObject BigEffect => bigEffect;

        public float Radius => radius;

        public string Description => description;

        public float Range => range;

        public Sprite UiSprite => uiSprite;

        public SoundHolder CastSoundHolder => castSoundHolder;

        public float CastTime => castTime;

        public void Cancel() {
            EventManager.Instance.CancelSkill(this);
            Remove();
        }

        public virtual void SetDestination(Transform target, Entity caster) {
            this.caster = caster;
            this.target = target;
            targetPos = target.position;
            GetComponent<AudioSource>().volume = Sound.Sound.Instance.SoundVolume;
            Sound.Sound.Instance.PlaySoundClipWithSource(flySoundHolder, audioSource, 0, true);
        }

        protected virtual void Impact() {
            audioSource.loop = false;
            ParticleSystem particles = PrefabHolder.Instance.Get(impactType).GetComponent<ParticleSystem>();
            particles.gameObject.transform.position = transform.position;
            ParticleSystem.ShapeModule newShape = particles.shape;
            newShape.radius = radius;
            particles.Play();
            Sound.Sound.Instance.PlaySoundClipWithSource(new SoundHolder(impactSound, 1, 0), audioSource, 0);
        }

        protected virtual void Update() {
            if (target == null) {
                return;
            }
            Move();
        }

        private void Remove() {
            inactive = true;
            bigEffect.SetActive(false);
            smallEffect.SetActive(false);
            Destroy(gameObject, 3);
        }

        private void Move() {
            transform.LookAt(targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (transform.position == targetPos && !inactive) {
                Impact();
                Remove();
            }
        }
    }
}
