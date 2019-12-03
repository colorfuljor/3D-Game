using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAndDNode
{
    private const int maxPriests = 3;
    private const int maxDevils = 3;

    int priestsNum;
    int devilsNum;
    private PAndDNode parent;

    public PAndDNode(int priestsNum, int devilsNum)
    {
        if (priestsNum <= maxPriests && devilsNum <= maxDevils)
        {
            this.priestsNum = priestsNum;
            this.devilsNum = devilsNum;
            this.parent = null;
        }
        else
        {
            Debug.Log("传参错误");
        }
    }

    public PAndDNode(PAndDNode node)
    {
        this.priestsNum = node.priestsNum;
        this.devilsNum = node.devilsNum;
        this.parent = node.parent;
    }

    public int GetPriestsNum()
    {
        return priestsNum;
    }

    public int GetDevilsNum()
    {
        return devilsNum;
    }

    public PAndDNode GetParent()
    {
        return parent;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType().Equals(this.GetType()) == false)
        {
            return false;
        }

        PAndDNode node = (PAndDNode)obj;
        if (node.priestsNum == this.priestsNum && node.devilsNum == this.devilsNum)
        {
            return true;

        }
        return false;
    }

    public override int GetHashCode()
    {
        return this.priestsNum.GetHashCode() + this.devilsNum.GetHashCode();
    }

    public bool IsValid()
    {
        return ((priestsNum >= devilsNum || priestsNum  == 0) && (priestsNum <= devilsNum || priestsNum == maxPriests));
    }

    public bool MoveDD()
    {
        if (devilsNum >= 2)
        {
            this.parent = new PAndDNode(this);
            devilsNum -= 2;
            return true;
        }
        return false;
    }

    public bool MoveDP()
    {
        if (priestsNum > 0 && devilsNum > 0)
        {
            this.parent = new PAndDNode(this);
            devilsNum--;
            priestsNum--;
            return true;
        }
        return false;
    }

    public bool MovePP()
    {
        if (priestsNum >= 2)
        {
            this.parent = new PAndDNode(this);
            priestsNum -= 2;
            return true;
        }
        return false;
    }

    public bool MoveD()
    {
        if (devilsNum > 0)
        {
            this.parent = new PAndDNode(this);
            devilsNum--;
            return true;
        }
        return false;
    }

    public bool MoveP()
    {
        if (priestsNum > 0)
        {
            this.parent = new PAndDNode(this);
            priestsNum--;
            return true;
        }
        return false;
    }

    public bool Move(int path)
    {
        switch (path) {
            case 0:
                return MoveDP();
            case 1:
                return MovePP();
            case 2:
                return MoveDD();
            case 3:
                return MoveP();
            case 4:
                return MoveD();
            default:
                return false;
        }
    }
}
