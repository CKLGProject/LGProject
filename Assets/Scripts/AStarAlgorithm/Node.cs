using UnityEngine;

namespace pathFinding
{
    public class Node : IHeapItem<Node>
    {
        public bool Walkable;
        public Vector3 WorldPosition;
        public int GridX;
        public int GridY;
        public int MovementPenalty;

        public int GCost;
        public int HCost;
        public Node Parent;

        public bool Platform;

        public Node(bool walkable, Vector3 worldPos, int gridX, int gridY, int penalty)
        {
            Walkable = walkable;
            WorldPosition = worldPos;
            GridX = gridX;
            GridY = gridY;
            MovementPenalty = penalty;
        }

        public int FCost => GCost + HCost;

        public int HeapIndex { get; set; }

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);
            
            if (compare == 0) 
                compare = HCost.CompareTo(other.HCost);

            return -compare;
        }

        //public static implicit operator Node(BT.Selector v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}