using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum TeleportType {
    InScene,
    BetweenScenes,
}

public class TeleportPoint : MonoBehaviour
{
    [SerializeField]
    string transtionAnimationName = "CrossFade";
    [SerializeField]
    Animator transitionAnimator = null;
    //[SerializeField]
    //string playerTagName;
    [SerializeField]
    TeleportType teleportType;
    [SerializeField]
    GameObject inScenedestination;
    [SerializeField]
    AreaMonsterGenerator destinationAreaMonsterGeneratorInScene;

    [SerializeField]
    string destinationSceneName;
    [SerializeField]
    Vector2 betweenScenesdestination;

    [SerializeField]
    float transtionTime = 0.25f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator LoadLevel(Collision2D collision) {
        if (transitionAnimator != null)
        {
            Internals.transitionName = transtionAnimationName;
            transitionAnimator.SetTrigger(transtionAnimationName + "_Start");
        }
        yield return new WaitForSeconds(transtionTime);
        switch (teleportType)
        {
            case TeleportType.BetweenScenes:
                Internals.teleported = true;
                Internals.teleportedLocation = betweenScenesdestination;
                Internals.allowBattle = true;
                Internals.lastBattleMonsterIndex = -1;
                SceneManager.LoadScene(destinationSceneName);
                break;
            case TeleportType.InScene:
                collision.gameObject.transform.position = inScenedestination.transform.position;
                PlayerCharacter character = collision.gameObject.GetComponent<PlayerCharacter>();
                if (destinationAreaMonsterGeneratorInScene != null) {
                    character.areaMonsterGenerator = destinationAreaMonsterGeneratorInScene;
                }
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(LoadLevel(collision));
        }
    }
}
