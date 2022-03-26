using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapCamera : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Make camera follow the player
        transform.position = new Vector3(player.position.x + offset.x,
            player.position.y + offset.y, this.transform.position.z + offset.z);
    }
}
