using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShootBlock : MonoBehaviour
{
    Vector3 pos = Vector3.zero;
    bool isMoveRight = true;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        speed = GameManager.Instance.ShootBlockSpeed;
    }
    float speed = 0;
    // Update is called once per frame
    void Update()
    {
        if (isMoveRight)
        {
            pos = new Vector3(pos.x + speed * Time.deltaTime, pos.y, pos.z);
            transform.position = pos;
        }

        if (pos.x > GameManager.Instance.RightLimitPos)
        {
            GameManager.Instance.canShoot = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Bullet") && other.gameObject.CompareTag("Dummy"))
        {
            other.gameObject.GetComponent<DummyBlock>().ChangeFace();
            GameManager.Instance.canShoot = true;
            GameManager.Instance.PlaySound(3);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Block"))
        {
            isMoveRight = false;
            gameObject.tag = "Untagged";
            GameManager.Instance.canShoot = true;
            GameManager.Instance.PlaySound(4);

            var pos = transform.position;
            gameObject.transform.DOMove(new Vector3(pos.x - 1f, pos.y - 2f), 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
               {
                   Destroy(gameObject);
               });

        }
    }
}
