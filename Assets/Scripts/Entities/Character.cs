using System.Collections;
using Entities.Skills;
using SaveLoad;
using UI.HUD;
using UnityEngine;
using LogType = UI.HUD.LogType;

namespace Entities {
    public class Character : Entity {

        private string characterName = string.Empty;
        private bool casting;
        [SerializeField]
        private int revivalCost;
        [SerializeField]
        private SkinnedMeshRenderer clothRenderer;
        private IEnumerator castCoroutine;

        public string CharacterName => characterName;

        public Color HairColor {
            get => MeshRenderer.materials[3].color;

            set => MeshRenderer.materials[3].color = value;
        }

        public Color EyeColor {
            get => MeshRenderer.materials[1].color;

            set => MeshRenderer.materials[1].color = value;
        }

        public Color SkinColor {
            get => MeshRenderer.materials[2].color;

            set => MeshRenderer.materials[2].color = value;
        }

        public Color TeamColor {
            get => MeshRenderer.materials[4].color;

            set {
                MeshRenderer.materials[4].color = value;
                clothRenderer.material.color = value;
            }
        }

        public int RevivalCost => revivalCost;

        public bool Casting => casting;

        public void CancelCast() {
            StopCoroutine(castCoroutine);
            entityBehaviourClass.CancelCast();
            casting = false;
        }

        public void Cast(GameObject skill, Transform target) {
            SkillBehaviour skillBehaviour = skill.GetComponent<SkillBehaviour>();
            Sound.Sound.Instance.PlaySoundClipWithSource(skillBehaviour.CastSoundHolder, audioSource, skillBehaviour.CastTime);
            entityBehaviourClass.Cast(skill, target);
            casting = true;
            castCoroutine = ResetCasting(skillBehaviour.CastTime);
            StartCoroutine(castCoroutine);
        }

        public override void MoveTo(Vector3 target, bool final) {
            if (casting) {
                EventLog.Instance.AddAction(LogType.Error, "Cannot move while casting", transform.position);
                return;
            }
            base.MoveTo(target, final);
        }

        public new CharacterSaveData Save() {
            return new CharacterSaveData(characterName, HairColor, EyeColor, SkinColor, TeamColor);
        }

        public void Load(CharacterSaveData saveData) {
            characterName = saveData.CharacterName;
            HairColor = saveData.HairColor;
            EyeColor = saveData.EyeColor;
            SkinColor = saveData.SkinColor;
            TeamColor = saveData.TeamColor;
        }

        protected override void OnEnable() {
            base.OnEnable();
            casting = false;
        }

        protected override void Reset() {
            base.Reset();
        }

        protected override void Update() {
            base.Update();
        }

        private IEnumerator ResetCasting(float castTime) {
            yield return new WaitForSeconds(castTime);
            casting = false;
        }
    }
}
