using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParent : MonoBehaviour
{
    public Sprite hit;
    Vector3 pos = Vector3.zero;
    float speed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    bool isDestroyCommanded = false;
    // Update is called once per frame
    void Update()
    {
        speed = GameManager.Instance.EnemyBlockSpeed;

        if (pos.x < GameManager.Instance.LefLimitPos) Destroy(gameObject);

        if (!isDestroyCommanded)
        {
            pos = new Vector3(pos.x - speed * Time.deltaTime, pos.y, pos.z);
            transform.position = pos;

            if (LuckCount <= 0 && !isDestroyCommanded)
            {
                DoDestroy();
                isDestroyCommanded = true;
            }
        }
    }

    void DoDestroy()
    {
        GameManager.Instance.AddScore(1);

        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().sprite = hit;
            Destroy(child.gameObject.GetComponent<BoxCollider2D>());
        }
        Destroy(gameObject, 0.5f);
    }


    [HideInInspector]
    public int LuckCount = int.MaxValue;
}
