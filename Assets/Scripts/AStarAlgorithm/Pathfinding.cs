using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace pathFinding
{
    public class Pathfinding : MonoBehaviour
    {
        //PathRequestManager requestManager;

        //public Transform seeker, target;

        Grid grid;
        public GameObject cube;
        void Awake()
        {
            //requestManager = GetComponent<PathRequestManager>();
            grid = GetComponent<Grid>();
        }

        private void Update()
        {
        }

        public bool IsMovementPoint(Vector3 point)
        {
            Node targetNode = grid.NodeFromWorldPoint(point);
            return targetNode.walkable ? true : false;
            //return false;
        }


        //public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        //{
        //    StartCoroutine(FindPath(startPos, targetPos));

        //}

        public void FindPath(PathRequest request, Action<PathResult> callback)
        {
            Vector3[] wayPoint = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = grid.NodeFromWorldPoint(request.pathStart);

            //Instantiate(cube, startNode.worldPosition, Quaternion.identity).name = "startNode";
            //Instantiate(cube, request.pathStart, Quaternion.identity).name = "PathStart";

            Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

            //if (startNode == null || targetNode == null)
            //    yield return null;

            if (startNode.walkable && targetNode.walkable)
            {
                Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
                HashSet<Node> closeSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();

                    closeSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;
                        //openSet.Add(currentNode);
                        //Instantiate(cube, currentNode.worldPosition, Quaternion.identity);
                        break;
                    }

                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        // 2024-04-26 -> 10시 59분
                        // walkable이더라도 일단 넣고 경로 이동을 할 때 재판단하자.
                        // 다음 목표가 walkable일 경우 그 다음 목표와 비교했을 때.
                        // x + 2일 경우 점프해서 넘어가고, y+2일 경우 점프를 한다. 
                        // 만약 다음과 같은 상황일 땐 어떠한가?
                        // □ ■ <- 왼쪽이 다음 목적지 2
                        // ■ ■ <- 오른쪽이 다음 목적지 1, 
                        // ■ □ 
                        // 위의 상황에서는 비교하였을 때 x는 +1이지만
                        // y가 +2이므로 y+ 2위치까지 점프를 해서 이동하게 하자


                        if (!neighbour.walkable || closeSet.Contains(neighbour))
                            continue;

                        var checkX = neighbour.gridX;
                        var checkY = neighbour.gridY;

                        if (checkX >= 100 && checkX < 0 && checkY >= 100 && checkY < 0) continue;

                        #region Debug
                        //if (!grid[checkX - 1, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY - 1].walkable) continue;
                        //if (!grid[checkX - 1, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY].walkable) continue;
                        //if (!grid[checkX - 1, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY + 1].walkable) continue;
                        //if (!grid[checkX, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY - 1].walkable) continue;
                        //if (!grid[checkX, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY + 1].walkable) continue;
                        //if (!grid[checkX + 1, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY - 1].walkable) continue;
                        //if (!grid[checkX + 1, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY].walkable) continue;
                        //if (!grid[checkX + 1, neighbour.gridY].walkable || !grid[neighbour.gridX, checkY + 1].walkable) continue;
                        #endregion

                        // 대각선 이동 시, 해당 이동 항향 ex) -1, -1 위치의 경우 (0, -1), (-1, 0)위치 둘 다 열려있는지 체크 해야함.
                        int gCostDistance = GetDistance(currentNode, neighbour);
                        int newMovementCostToNeighbour = currentNode.gCost + gCostDistance + neighbour.movementPenalty;
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            //yield return null;
            if (pathSuccess)
            {
                wayPoint = RetracePath(startNode, targetNode);
                pathSuccess = wayPoint.Length > 0;
            }

            // 여기서 보통은 움직이게 함.
            if(callback != null)    
                callback(new PathResult(wayPoint, pathSuccess, request.callback));
            else
            {
                Debug.Log("움직임을 직접 구현해야하는 로직");
            }
        }

        Vector3[] RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector3[] wayPoints = SimplifyPath(path);
            Array.Reverse(wayPoints);

            return wayPoints;
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> wayPoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for(int i = 0; i <  path.Count; i++)
            {
                #region legarcy
                //Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                //if (directionNew != directionOld)
                //    wayPoints.Add(path[i].worldPosition);
                //directionOld = directionNew;
                #endregion
                wayPoints.Add(path[i].worldPosition);
            }
            return wayPoints.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (disX > disY)
                return 14 * disY + 10 * (disX - disY);
            return 14 * disX + 10 * (disY - disX);
        }

    }

}