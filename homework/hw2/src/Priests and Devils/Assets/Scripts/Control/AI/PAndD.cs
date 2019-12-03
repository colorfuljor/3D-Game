using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAndD
{
    private const int pathNum = 5;

    private PAndDNode beginNode;
    private PAndDNode endNode;
    private PAndDNode nextNode;
    private PAndDNode currentNode;

    public bool BFSearch(PAndDNode begin, PAndDNode end)
    {
        Queue<PAndDNode> openList = new Queue<PAndDNode>();
        List<PAndDNode> closeList = new List<PAndDNode>();
        openList.Enqueue(begin);

        this.beginNode = begin;
        this.endNode = end;

        while (openList.Count > 0)
        {
            this.currentNode = openList.Peek();

            if (this.currentNode.Equals(end))
            {
                setNextToMove();
                return true;
            }
            openList.Dequeue();
            closeList.Add(this.currentNode);

            for (int i = 0; i < pathNum; i++)
            {
                PAndDNode newNode = new PAndDNode(this.currentNode);
                if (newNode.Move(i) && !closeList.Contains(newNode) && newNode.IsValid())
                {
                    openList.Enqueue(newNode);
                }
            }
        }

        return false;
    }

    private void setNextToMove()
    {
        nextNode = this.currentNode;
        while (!nextNode.GetParent().Equals(beginNode))
        {
            nextNode = nextNode.GetParent();
        }
    }

    public PAndDNode GetNextToMove()
    {
        return nextNode;
    }
}
