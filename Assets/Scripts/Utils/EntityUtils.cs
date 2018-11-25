using System.Text.RegularExpressions;
using UnityEngine;

public class EntityUtils {

    public static readonly int MaxEntityValue = 500;
    public static readonly int MinEntityValue = 0;

    /// <summary>
    /// returns the closest gameobject with a collider on it that is an instance of ParentObject
    /// ignores own entities and dead entities
    /// </summary>
    public static GameObject GetNearestEntity(Collider[] collider, Entity entity) {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = entity.gameObject.transform.position;
        foreach (Collider potentialTarget in collider) {
            Entity enemyEntity = potentialTarget.GetComponent<Entity>();
            if (enemyEntity != null && potentialTarget.gameObject != entity.gameObject.transform.gameObject && entity.Owner != enemyEntity.Owner && !enemyEntity.OnWall && enemyEntity.IsAlive() && potentialTarget.gameObject.activeInHierarchy) {
                //More would be too expensive
                if (collider.Length > 3) {
                    bestTarget = potentialTarget.gameObject;
                    break;
                }
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr) {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.gameObject;
                }
            }
        }
        return bestTarget;
    }

    /// <summary>
    /// returns the closest gameobject with a collider on it that is an instance of ParentObject
    /// ignores own buildings and dead buildings
    /// </summary>
    public static GameObject GetNearestBuilding(Collider[] collider, Entity entity) {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = entity.gameObject.transform.position;
        foreach (Collider potentialTarget in collider) {
            ParentBuilding enemyBuilding = potentialTarget.GetComponent<ParentBuilding>();
            if (enemyBuilding != null && potentialTarget.gameObject != entity.gameObject.transform.gameObject && entity.Owner != enemyBuilding.Owner && enemyBuilding.IsAlive() && potentialTarget.gameObject.activeInHierarchy && enemyBuilding.enabled) {
                    Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr) {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget.gameObject;
                    }
            }
        }
        return bestTarget;
    }

    /// <summary>
    /// returns the closest gameobject with a collider on it that is an instance of ParentObject and of the givent type
    /// ignores enemy entities and dead entities
    /// </summary>
    public static GameObject GetNearestEntityByType(int radius, ParentObjectNameEnum entityTypeToFind, Transform center) {
        Collider[] collider = Physics.OverlapSphere(center.position, radius, 256);
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = center.position;
        foreach (Collider potentialTarget in collider) {
            if (potentialTarget.GetComponent<ParentObject>() != null && potentialTarget.GetComponent<ParentObject>().ObjectName == entityTypeToFind) {
                if (potentialTarget.gameObject != center.gameObject.transform.gameObject && potentialTarget.gameObject.GetComponent<Entity>().Owner == OwnerEnum.Player && potentialTarget.gameObject.GetComponent<Entity>().IsAlive() && !potentialTarget.gameObject.GetComponent<Entity>().OnWall) {
                    Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr) {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget.gameObject;
                    }
                }
            }
        }
        return bestTarget;
    }

    public static bool ValidateData(CharacterSaveData characterSaveData) {
        if (!Regex.IsMatch(characterSaveData.CharacterName, @"^[a-zA-Z]+$") && characterSaveData.CharacterName != "") {
            ActionText.Instance.SetActionText("Cannot load Data due to illegal value " + characterSaveData.CharacterName + " of " + nameof(characterSaveData.CharacterName), 5f);
            return false;
        }
        ActionText.Instance.SetActionText("Data has been loaded successfully!", 2f);
        return true;
    }

    public static bool CheckValue(float value) {
        return value >= MinEntityValue && value <= MaxEntityValue;
    }

    private static bool CheckValue(string name, object value) {
        if (!Regex.IsMatch(value.ToString(), @"^[a-zA-Z]+$")) {
            float finalValue = float.Parse(value.ToString());
            if (!CheckValue(finalValue)) {
                ActionText.Instance.SetActionText("Cannot load Data due to illegal value " + finalValue + " of " + name, 5f);
                return false;
            }
            return true;
        }
        //Returns true for enums values
        return true;
    }

}
