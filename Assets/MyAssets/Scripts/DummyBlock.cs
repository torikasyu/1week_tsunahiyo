using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBlock : MonoBehaviour
{
    [SerializeField]
    Sprite PlacedSprite;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeFace()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = PlacedSprite;
        gameObject.tag = "Block";

        var parent = gameObject.transform.parent.gameObject;
        parent.GetComponent<BlockParent>().LuckCount--;
    }
}
