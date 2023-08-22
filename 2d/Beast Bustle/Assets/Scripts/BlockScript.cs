using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    private string[] blockNameM;

    //Grass and soil
    private bool runTimerGrass = false;

    //Gravity
    private bool Moving = false;
    private Vector2 bottomBlock;

    static public string[] stickToOtherAndSmooth = { "Soil1", "Soil2", "Rock1", "Rock2" };
    static public string[] smoothWithItself = { "Leaf1" };

    private void Start()
    {
        blockNameM = GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' });
        defineGrass();
    }

    private void defineGrass()
    {
        LayerMask layerMask = 1 << gameObject.layer;
        Collider2D collider = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + GeneratorWorld.sizeBlock * 0.75f), Vector2.up, Mathf.Infinity, layerMask).collider;

        if (blockNameM[0] == "Soil2" && collider == null)
            GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/Soil1")[int.Parse(blockNameM[1])];

        else if (blockNameM[0] == "Soil1" && collider != null && collider.CompareTag("Block"))
            GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/Soil2")[int.Parse(blockNameM[1])];
    }

    private IEnumerator Grass()
    {
        yield return new WaitForSeconds(30f);

        if (runTimerGrass)
        {
            defineGrass();
            runTimerGrass = false;
        }
    }

    //Checking of a changing the sprite and a transition to a modification the collider
    private string lastBlockName;
    private void checkChangeSprite()
    {
        if (blockNameM[1] != lastBlockName)
        {
            PolygonCollider();
            lastBlockName = blockNameM[1];
        }
    }

    //Polygon Collider modification
    private void PolygonCollider()
    {
        float sB = GeneratorWorld.sizeBlock;
        if (blockNameM[1] == "0")
            GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] { new Vector2(-sB, -sB), new Vector2(sB, -sB), new Vector2(sB, sB) });
        if (blockNameM[1] == "1")
            GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] { new Vector2(-sB, -sB), new Vector2(-sB, sB), new Vector2(sB, sB), new Vector2(sB, -sB) });
        if (blockNameM[1] == "2")
            GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] { new Vector2(-sB, sB), new Vector2(-sB, -sB), new Vector2(sB, -sB)});
        if (blockNameM[1] == "3")
            GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] { new Vector2(-sB, -sB), new Vector2(0, sB), new Vector2(sB, -sB) });
        if (blockNameM[1] == "4")
            GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] { new Vector2(sB, sB), new Vector2(-sB, sB), new Vector2(sB, -sB) });
        if (blockNameM[1] == "5")
            GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] { new Vector2(sB, sB), new Vector2(-sB, sB), new Vector2(-sB, -sB) });
        if (blockNameM[1] == "6")
            GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] { new Vector2(-sB, sB), new Vector2(0, -sB), new Vector2(sB, sB) });
    }

    private void Update()
    {
        blockNameM = GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        checkChangeSprite();

        //Growth of the grass
        LayerMask layerMask = 1 << gameObject.layer;
        LayerMask layerMaskOther;
        if (gameObject.layer == LayerMask.NameToLayer("Ground")) layerMaskOther = 1 << LayerMask.NameToLayer("Ground_BG");
            else layerMaskOther = 1 << LayerMask.NameToLayer("Ground");
        Collider2D collider = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + GeneratorWorld.sizeBlock * 0.75f), Vector2.up, Mathf.Infinity, layerMask).collider;

        if (!runTimerGrass && (blockNameM[0] == "Soil2" && collider == null || (blockNameM[0] == "Soil1" && collider != null && collider.CompareTag("Block"))))
        {
            StartCoroutine(Grass());
            runTimerGrass = true;
        }

        if (runTimerGrass && (blockNameM[0] == "Soil1" && collider == null || (blockNameM[0] == "Soil2" && collider != null && collider.CompareTag("Block"))))
            runTimerGrass = false;

        //Block smoothing and physics
        Collider2D colliderNear = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f, layerMaskOther);
        Collider2D colliderNearY1 = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - GeneratorWorld.sizeBlock), 0.1f, layerMask);
        Collider2D colliderNearY2 = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + GeneratorWorld.sizeBlock), 0.1f, layerMask);
        Collider2D colliderNearX1 = Physics2D.OverlapCircle(new Vector2(transform.position.x - GeneratorWorld.sizeBlock, transform.position.y), 0.1f, layerMask);
        Collider2D colliderNearX2 = Physics2D.OverlapCircle(new Vector2(transform.position.x + GeneratorWorld.sizeBlock, transform.position.y), 0.1f, layerMask);

        if (stickToOtherAndSmooth.Contains(blockNameM[0]))
        {
            bool cNearX1;
            bool cNearX2;
            bool cNearY1;
            bool cNearY2;
            if (colliderNearX1 != null && stickToOtherAndSmooth.Contains(colliderNearX1.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0])) cNearX1 = true; else cNearX1 = false;
            if (colliderNearX2 != null && stickToOtherAndSmooth.Contains(colliderNearX2.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0])) cNearX2 = true; else cNearX2 = false;
            if (colliderNearY1 != null) cNearY1 = true; else cNearY1 = false;
            if (colliderNearY2 != null) cNearY2 = true; else cNearY2 = false;
            if (!cNearY2)
            {
                if (!cNearX1 && !cNearX2) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[3];
                else if (!cNearX1) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[0];
                else if (!cNearX2) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[2];
                else GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[1];
            }
            else if (!cNearY1)
            {
                if (!cNearX1 && !cNearX2) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[6];
                else if (!cNearX1) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[4];
                else if (!cNearX2) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[5];
                else GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[1];
            }
            else GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[1];
        }

        if (smoothWithItself.Contains(blockNameM[0]))
        {
            if (colliderNearY2 == null && colliderNearY1 != null && colliderNearY1.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0] == blockNameM[0])
            {
                if (colliderNearX1 == null && colliderNearX2 == null) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[3];
                else if (colliderNearX1 == null && colliderNearX2 != null && colliderNearX2.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0] == blockNameM[0]) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[0];
                else if (colliderNearX2 == null && colliderNearX1 != null && colliderNearX1.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0] == blockNameM[0]) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[2];
                else GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[1];
            }
            else if (colliderNearY1 == null && colliderNearY2 != null && colliderNearY2.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0] == blockNameM[0])
            {
                if (colliderNearX1 == null && colliderNearX2 == null) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[6];
                else if (colliderNearX1 == null && colliderNearX2 != null && colliderNearX2.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0] == blockNameM[0]) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[4];
                else if (colliderNearX2 == null && colliderNearX1 != null && colliderNearX1.GetComponent<SpriteRenderer>().sprite.name.Split(new char[] { '_' })[0] == blockNameM[0]) GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[5];
                else GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[1];
            }
            else GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + blockNameM[0])[1];
        }

        if(!Moving)
        {
            if(colliderNear == null && colliderNearY1 == null && colliderNearY2 == null && colliderNearX1 == null && colliderNearX2 == null)
            {
                bottomBlock = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - GeneratorWorld.sizeBlock), Vector2.down, Mathf.Infinity, layerMask).collider.transform.position;
                Moving = true;
            }
        }
        else
        {
            if (new Vector2(transform.position.x, transform.position.y) != bottomBlock + new Vector2(0, GeneratorWorld.sizeBlock))
                transform.position = Vector2.MoveTowards(transform.position, bottomBlock + new Vector2(0, GeneratorWorld.sizeBlock), Time.fixedDeltaTime * 75);
            else Moving = false;
        }
    }
    ////////////////
}
