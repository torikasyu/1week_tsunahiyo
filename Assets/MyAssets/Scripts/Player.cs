using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public GameObject blockA;

    bool canMove = true;
    GameObject HoldBallet;
    bool canShoot()
    {
        return GameManager.Instance.canShoot;
    }
    int TopLimitPos;
    int BottomLimitPos;

    // Start is called before the first frame update
    void Start()
    {
        HoldBallet = transform.Find("HoldBallet").gameObject;
        TopLimitPos = GameManager.Instance.TopLimitPos;
        BottomLimitPos = GameManager.Instance.BottomLimitPos;
    }

    bool move = false;
    float posY = 0;

    bool isDead = false;
    // Update is called once per frame
    void Update()
    {
        posY = transform.position.y;

        if (canMove && !isDead)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                move = true;
                posY++;
                if (posY > TopLimitPos)
                {
                    posY = TopLimitPos;
                    move = false;
                }
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                move = true;
                posY--;
                if (posY < BottomLimitPos)
                {
                    posY = BottomLimitPos;
                    move = false;
                }
            }

            if (move)
            {
                canMove = false;
                transform.DOMoveY(posY, 0.1f).OnComplete(() =>
                {
                    canMove = true;
                });
            }
        }

        if (canShoot() && !isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var pos = transform.position;
                pos = new Vector3(pos.x + 1, Snap(pos.y), pos.z);
                Instantiate(blockA, pos, Quaternion.identity);
                GameManager.Instance.canShoot = false;
                GameManager.Instance.PlaySound(0);
            }
        }
        if (!isDead)
        {
            HoldBallet.SetActive(canShoot());
        }
        else
        {
            HoldBallet.SetActive(false);
        }
    }

    float Snap(float value)
    {
        return Mathf.Floor(value / 1.0f) * 1.0f;
    }
    [SerializeField]
    Sprite hit;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Block"))
        {
            GetComponent<Animator>().SetBool("isHurt", true);
            isDead = true;
            //GetComponent<SpriteRenderer>().sprite = hit;
            //Destroy(gameObject, 1f);
            GameManager.Instance.GameOverProcess();
        }
    }
}

