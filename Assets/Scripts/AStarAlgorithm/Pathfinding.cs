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

        private Grid _grid;
        public GameObject cube;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }
        
        public bool IsMovementPoint(Vector3 point)
        {
            Node targetNode = _grid.NodeFromWorldPoint(point);
            return targetNode.Walkable;
            //return false;
        }


        //public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        //{
        //    StartCoroutine(FindPath(startPos, targetPos));

        //}

        public void FindPath(PathRequest request, Action<PathResult> callback)
        {
            try
            {
                Vector3[] wayPoint = Array.Empty<Vector3>();
                bool pathSuccess = false;

                Node startNode = _grid.NodeFromWorldPoint(request.PathStart);

                //Instantiate(cube, startNode.worldPosition, Quaternion.identity).name = "startNode";
                //Instantiate(cube, request.pathStart, Quaternion.identity).name = "PathStart";

                Node targetNode = _grid.NodeFromWorldPoint(request.PathEnd);

                //if (startNode == null || targetNode == null)
                //    yield return null;

                if (startNode.Walkable && targetNode.Walkable)
                {
                    Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
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

                        foreach (Node neighbour in _grid.GetNeighbours(currentNode))
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


                            if (!neighbour.Walkable || closeSet.Contains(neighbour))
                                continue;

                            var checkX = neighbour.GridX;
                            var checkY = neighbour.GridY;

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
                            int newMovementCostToNeighbour = currentNode.GCost + gCostDistance + neighbour.MovementPenalty;
                            if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                            {
                                neighbour.GCost = newMovementCostToNeighbour;
                                neighbour.HCost = GetDistance(neighbour, targetNode);
                                neighbour.Parent = currentNode;

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
                if (callback != null)
                    callback(new PathResult(wayPoint, pathSuccess, request.Callback));
                else
                {
                    Debug.Log("움직임을 직접 구현해야하는 로직");
                }
            }
            catch
            {
                //Debug.Log("현재 보드에 있지 않음.");
            }
        }

        private Vector3[] RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            Vector3[] wayPoints = SimplifyPath(path);
            Array.Reverse(wayPoints);

            return wayPoints;
        }

        private Vector3[] SimplifyPath(List<Node> path)
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
                wayPoints.Add(path[i].WorldPosition);
            }
            return wayPoints.ToArray();
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int disX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int disY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (disX > disY)
                return 14 * disY + 10 * (disX - disY);
            return 14 * disX + 10 * (disY - disX);
        }

    }

}