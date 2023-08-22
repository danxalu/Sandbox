using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerationWorld : MonoBehaviour
{
    public int width1World;
    public int width2World;

    public float sizeBlock;
    public float soilMinHeight;

    public GameObject Soil;
    public GameObject GrassSoil;

    public GameObject logP;
    public GameObject logQ;
    public GameObject Branch;

    private GameObject newObject;

    System.Random random = new System.Random();

    private void Start()
    {
        generateSoil();
        generateTrees();
    }

    private void generateSoil()
    {
        for(int x = 0; x < width1World; x++)
        {
            for(int z = 0; z < width2World; z++)
            {
                newObject = Instantiate(Soil, new Vector3(x * sizeBlock, soilMinHeight, z * sizeBlock), Quaternion.identity);
                newObject = Instantiate(Soil, new Vector3(x * sizeBlock, soilMinHeight + sizeBlock / 2, z * sizeBlock), Quaternion.identity);
                newObject = Instantiate(GrassSoil, new Vector3(x * sizeBlock, soilMinHeight + sizeBlock, z * sizeBlock), Quaternion.identity);
            }
        }
    }

    private void generateTrees()
    {
        int xTree = 0;
        int zTree = 0;
        int heightTree = 0;
        int thicknessTree = 0;
        int lastBareTrunk = 0;

        int curAmountTrees = 0;
        int maxAmountTrees = random.Next(15, 30);

        float soilHeight = soilMinHeight;

        LayerMask layerMaskSoilHeight = 1 << LayerMask.NameToLayer("Soil");
        LayerMask layerMaskTreeExists = 1 << LayerMask.NameToLayer("Ground");

        while (curAmountTrees < maxAmountTrees)
        {
            xTree = random.Next(6, 15);
            zTree = random.Next(6, 15);

            void newTree()
            {
                heightTree = random.Next(5, 13);
                if (heightTree > 8) thicknessTree = random.Next(2, 4);
                else thicknessTree = 2;
                lastBareTrunk = random.Next((int)(heightTree * 0.25f), (int)(heightTree * 0.4f));
            }

            if (curAmountTrees == 0) newTree();

            //Determining the value of soilHeight
            Ray ray = new Ray(new Vector3(xTree, 50, zTree) * sizeBlock, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, layerMaskSoilHeight))
                soilHeight = hit.collider.transform.position.y + hit.transform.GetChild(0).localScale.y;

            //Generation of next tree
            Ray ray1; RaycastHit hit1;
            Ray ray2; RaycastHit hit2;

            float currentY = soilHeight;

            for (int h = 0; h < heightTree; h++)
            {
                //Trunk
                for (int x = 0; x < thicknessTree - 2; x++)
                    newObject = Instantiate(logP, new Vector3(xTree + x, currentY, zTree - 1) * sizeBlock, Quaternion.identity);

                for (int z = 0; z < thicknessTree - 2; z++)
                    for (int x = -1; x < thicknessTree - 1; x++)
                        newObject = Instantiate(logP, new Vector3(xTree + x, currentY, zTree + z) * sizeBlock, Quaternion.identity);

                for (int x = 0; x < thicknessTree - 2; x++)
                    newObject = Instantiate(logP, new Vector3(xTree + x, currentY, zTree + (thicknessTree - 2)) * sizeBlock, Quaternion.identity);

                newObject = Instantiate(logQ, new Vector3(xTree + (thicknessTree - 2), currentY, zTree + (thicknessTree - 2)) * sizeBlock, Quaternion.identity);
                newObject.transform.Rotate(0, 0, 0);
                newObject = Instantiate(logQ, new Vector3(xTree + (thicknessTree - 2), currentY, zTree) * sizeBlock, Quaternion.identity);
                newObject.transform.Rotate(0, 90, 0);
                newObject = Instantiate(logQ, new Vector3(xTree, currentY, zTree) * sizeBlock, Quaternion.identity);
                newObject.transform.Rotate(0, 180, 0);
                newObject = Instantiate(logQ, new Vector3(xTree, currentY, zTree + (thicknessTree - 2)) * sizeBlock, Quaternion.identity);
                newObject.transform.Rotate(0, 270, 0);

                currentY += sizeBlock;

                //Leaf
                if (h >= lastBareTrunk)
                {
                    int branchLength;
                    if (heightTree > 9) branchLength = random.Next(1, 4);
                    else if (heightTree > 6) branchLength = random.Next(1, 3);
                    else branchLength = 1;

                    for (int z = 0; z < thicknessTree - 1; z++)
                    {
                        for (int x = 0; x < branchLength; x++)
                        {
                            newObject = Instantiate(Branch, new Vector3(xTree + (thicknessTree - 2) + 2 + x * 2, currentY, zTree + (thicknessTree - 2) - z) * sizeBlock, Quaternion.identity); //0
                            newObject.transform.Rotate(0, 270, 0);
                        }
                        //Extra branches
                        if (branchLength > 1 && z < 2)
                        {
                            for (int x = 0; x < branchLength - 1; x++)
                            {
                                newObject = Instantiate(Branch, new Vector3(xTree + (thicknessTree - 2) + x * 2 + 3, currentY, zTree + (thicknessTree - 2) + z * 2 + 3) * sizeBlock, Quaternion.identity); //180
                                newObject.transform.Rotate(0, 180, 0);
                            }
                        }
                    }

                    for (int x = 0; x < thicknessTree - 1; x++)
                    {
                        for (int z = 0; z < branchLength; z++)
                        {
                            newObject = Instantiate(Branch, new Vector3(xTree + (thicknessTree - 2) - x, currentY, zTree - 2 - z * 2) * sizeBlock, Quaternion.identity); //90
                            newObject.transform.Rotate(0, 0, 0);
                        }
                        //Extra branches
                        if (branchLength > 1 && x < 2)
                        {
                            for (int z = 0; z < branchLength - 1; z++)
                            {
                                newObject = Instantiate(Branch, new Vector3(xTree + (thicknessTree - 2) + x * 2 + 3, currentY, zTree - z * 2 - 3) * sizeBlock, Quaternion.identity); //180
                                newObject.transform.Rotate(0, 270, 0);
                            }
                        }
                    }

                    for (int z = 0; z < thicknessTree - 1; z++)
                    {
                        for (int x = 0; x < branchLength; x++)
                        {
                            newObject = Instantiate(Branch, new Vector3(xTree - 2 - x * 2, currentY, zTree + z) * sizeBlock, Quaternion.identity); //180
                            newObject.transform.Rotate(0, 90, 0);
                        }
                        //Extra branches
                        if (branchLength > 1 && z < 2)
                        {
                            for (int x = 0; x < branchLength - 1; x++)
                            {
                            newObject = Instantiate(Branch, new Vector3(xTree - x * 2 - 3, currentY, zTree - z * 2 - 3) * sizeBlock, Quaternion.identity); //180
                            newObject.transform.Rotate(0, 0, 0);
                            }
                        }
                    }

                    for (int x = 0; x < thicknessTree - 1; x++)
                    {
                        for (int z = 0; z < branchLength; z++)
                        {
                            newObject = Instantiate(Branch, new Vector3(xTree + x, currentY, zTree + (thicknessTree - 2) + 2 + z * 2) * sizeBlock, Quaternion.identity); //270
                            newObject.transform.Rotate(0, 180, 0);
                        }
                        //Extra branches
                        if (branchLength > 1 && x < 2)
                        {
                            for (int z = 0; z < branchLength - 1; z++)
                            {
                                newObject = Instantiate(Branch, new Vector3(xTree - x * 2 - 3, currentY, zTree + (thicknessTree - 2) + z * 2 + 3) * sizeBlock, Quaternion.identity); //180
                                newObject.transform.Rotate(0, 90, 0);
                            }
                        }
                    }

                    currentY += sizeBlock * 0.25f;
                }
            }

            newTree();
            //Position of next tree
            Collider[] treeExists = { };
            do
            {
                int moveXZ = random.Next(0, 3);
                if (moveXZ == 0) xTree += RandomPlusMinus(7 + thicknessTree, 13 + thicknessTree);
                else if (moveXZ == 1) zTree += RandomPlusMinus(7 + thicknessTree, 13 + thicknessTree);
                else
                {
                    xTree += RandomPlusMinus(7 + thicknessTree, 13 + thicknessTree);
                    zTree += RandomPlusMinus(7 + thicknessTree, 13 + thicknessTree);
                }
                treeExists = Physics.OverlapSphere(new Vector3(xTree + thicknessTree / 2, soilHeight + thicknessTree / 2 + 7, zTree + thicknessTree / 2), thicknessTree / 2 + 6, layerMaskTreeExists);


                ray1 = new Ray(new Vector3(xTree, 50, zTree) * sizeBlock, Vector3.down);
                ray2 = new Ray(new Vector3(xTree + (thicknessTree - 2), 50, zTree + (thicknessTree - 2)) * sizeBlock, Vector3.down);
                Physics.Raycast(ray1, out hit1, layerMaskSoilHeight);
                Physics.Raycast(ray2, out hit2, layerMaskSoilHeight);

            }
            while (treeExists.Length != 0 || xTree > width1World || xTree < 0 || zTree > width2World || zTree < 0 ||
                   hit1.collider.transform.position.y != hit2.collider.transform.position.y);

            curAmountTrees += 1;
        }
    }

    private int RandomPlusMinus(int a, int b)
    {
        int plusMinus = random.Next(0, 2);
        int answer = random.Next(a, b);
        if (plusMinus == 1) answer *= -1;
        return answer;
    }

    public int ChooseInt(int[] options)
    {
        int choice = random.Next(0, options.Length + 1);
        return options[choice];
    }


    private void Update()
    {
        
    }
}
