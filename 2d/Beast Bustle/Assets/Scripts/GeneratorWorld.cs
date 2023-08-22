using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratorWorld : MonoBehaviour
{
    public GameObject Block;
    private GameObject newBlock;

    static public float sizeBlock;

    public int worldWidth;
    public int worldHeightDown;

    private int yMountain;
    private int xTree;

    //Kinds of the blocks that aren't darkened
    static public string[] excBlocksBG = {"Tree1", "Leaf1"};

    //Creation of the block
    private void createBlock(string material, string type, Vector2 pos)
    {
        newBlock = Instantiate(Block, pos, Quaternion.identity);
        newBlock.name = "Block";
        newBlock.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Blocks/" + material)[1];

        if (type == "front")
        {
            newBlock.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            newBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
            newBlock.layer = LayerMask.NameToLayer("Ground");
        } 
        else if (type == "back")
        {
            if (!excBlocksBG.Contains(material)) newBlock.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
                else newBlock.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            newBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Ground_BG";
            newBlock.layer = LayerMask.NameToLayer("Ground_BG");
        }
    }

    private void Start()
    {
        sizeBlock = Block.GetComponent<SpriteRenderer>().size.x * Block.transform.localScale.x;

        System.Random random = new System.Random();

        //Generation of Earth crust
        int ySoil = random.Next(3, 9);
        for (int k = 0; k < worldWidth; k++)
        {
            ySoil += random.Next(-1, 2);
            if (ySoil < 3) ySoil = 3;
            if (ySoil > 8) ySoil = 8;

            for (int i = 0; i > -ySoil; i--)
            {
                    createBlock("Soil2", "front", new Vector2(k * sizeBlock, i * sizeBlock));
                    createBlock("Soil2", "back", new Vector2(k * sizeBlock, i * sizeBlock));
            }

            for (int i = -ySoil; i > -worldHeightDown; i--)
            {

                int num_material = UnityEngine.Random.Range(1, 2);
                createBlock("Rock" + num_material, "front", new Vector2(k * sizeBlock, i * sizeBlock));
                createBlock("Rock" + num_material, "back", new Vector2(k * sizeBlock, i * sizeBlock));

            }
        }


        yMountain = random.Next(1, 9);
        xTree = random.Next(2, 6);
        int xBranch = -1;
        LayerMask layerMaskF = 1 << LayerMask.NameToLayer("Ground");
        LayerMask layerMaskB = 1 << LayerMask.NameToLayer("Ground_BG");

        //Generation of mountains
        for (int k = 0; k < worldWidth; k++)
        {
            int yMountainDelta = random.Next(0, 2);
            if (yMountainDelta == 1) yMountain += random.Next(-1, 2);

            if (yMountain < 0) yMountain = 0;
            if (yMountain > 12) yMountain = 12;

            if (yMountain > 0)
            {
                for (int i = 0; i < yMountain + 1; i++)
                {
                    if (i < yMountain)
                    {
                        createBlock("Soil2", "front", new Vector2(k * sizeBlock, i * sizeBlock));
                        createBlock("Soil2", "back", new Vector2(k * sizeBlock, i * sizeBlock));
                    }
                    else
                    {
                        if (k == xTree)
                        {
                                int heightTreeWhole = random.Next(4, 13);
                                int heightTreeBare = random.Next(3, (int)Math.Round(heightTreeWhole/1.5f));
                                for (int t = 0; t < heightTreeWhole; t++)
                                {
                                    createBlock("Tree1", "back", new Vector2(k * sizeBlock, (yMountain + t) * sizeBlock));
                                    if (t == heightTreeBare)
                                        {
                                            for(int q = -2; q < 3; q++)
                                                if(q != 0) createBlock("Leaf1", "back", new Vector2((k + q) * sizeBlock, (yMountain + t) * sizeBlock));
                                        }
                                    if (t > heightTreeBare)
                                    {
                                        if (t == heightTreeBare + 1) xBranch = random.Next(0, 2);

                                        if (xBranch == 0)
                                        {
                                            createBlock("Tree1", "back", new Vector2((k - 1)* sizeBlock, (yMountain + t) * sizeBlock));
                                            for (int q = -2; q < 3; q++)
                                               if (q != 0 && q != -1) createBlock("Leaf1", "back", new Vector2((k + q) * sizeBlock, (yMountain + t) * sizeBlock));
                                            xBranch = 1;
                                        }
                                        else if (xBranch == 1)
                                        {
                                            createBlock("Tree1", "back", new Vector2((k + 1) * sizeBlock, (yMountain + t) * sizeBlock));
                                            for (int q = -2; q < 3; q++)
                                                if (q != 0 && q != 1) createBlock("Leaf1", "back", new Vector2((k + q) * sizeBlock, (yMountain + t) * sizeBlock));
                                            xBranch = 0;
                                        }
                                    }
                                    for (int q = -2; q < 3; q++)
                                        createBlock("Leaf1", "back", new Vector2((k + q) * sizeBlock, (yMountain + heightTreeWhole) * sizeBlock));
                            }
                                xTree += random.Next(6, 10);
                        }
                    } 
                }
            }

        }


    }

    private void Update()
    {

    }
}
