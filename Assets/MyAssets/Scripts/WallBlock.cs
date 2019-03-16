using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlock : MonoBehaviour
{
    float speed = 0;
    Vector3 pos = Vector3.zero;
    float limitX = 0;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        limitX = GameManager.Instance.LefLimitPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speed = GameManager.Instance.EnemyBlockSpeed;

        pos = new Vector3(pos.x - speed * Time.deltaTime, pos.y, pos.z);
        transform.position = pos;

        if (pos.x < limitX)
        {
            Vector3 newPos = new Vector3(GameManager.Instance.RightLimitPos, transform.position.y, transform.position.z);
            transform.position = newPos;
            pos = newPos;
            //Destroy(gameObject);
        }
    }
}
