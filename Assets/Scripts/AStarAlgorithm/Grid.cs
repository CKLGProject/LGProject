using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace pathFinding
{
    public class Grid : MonoBehaviour
    {
        private static Grid instance = null;
        public static Grid Instance
        {
            get
            {
                return instance; 
            }
        }

        //public bool onlyDisplayPathGizmos;
        public bool displayGridGizmos;
        public bool displayGridOnPlatformGizmos;
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public TerrainType[] walkableRegions;
        public int obstacleProximityPenalty = 10;
        public Node[,] grid;
        LayerMask walkableMask;
        Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

        // 이동할 수 있는 포인트를 넣어두자
        public List<Node> walkableNodeList = new List<Node>();

        public Node this[int a, int b]
        {
            get
            {
                try
                {
                    return grid[a, b];
                }
                catch
                {
                    Debug.LogError($" a = {a}, b = {b} ");
                    return grid[0, 0];
                }
            }
        }

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        int penaltyMin = int.MaxValue;
        int penaltyMax = int.MinValue;

        void Awake()
        {
            if (instance != null)
                Destroy(this);
            else
            {
                instance = this;

                nodeDiameter = nodeRadius * 2;
                gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
                gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

                foreach (TerrainType region in walkableRegions)
                {
                    walkableMask.value |= region.terrainMask.value;
                    walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
                }

                CreateGrid();
            }
        }

        public int MaxSize
        {
            get
            {
                return gridSizeX * gridSizeY;
            }
        }

        private void Update()
        {
            //for(int x = 0; x < gridSizeX; x++)
            //{
            //    for(int y = 0; y < gridSizeY; y++ )
            //    {

            //        Debug.DrawLine(grid[x, y].worldPosition + Vector3.forward * 50, grid[x, y].worldPosition + Vector3.back * 50);
            //    }
            //}
        }

        void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

            for (int x = gridSizeX - 1; x >= 0; x--)
            {
                for (int y = gridSizeY - 1; y >= 0; y--)
                {try
                    {
                        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                        bool walkable = (!Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                        int movementPenalty = 0;

                        // raycast

                        Ray ray = new Ray(worldPoint + Vector3.forward * 5, Vector3.back);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 10, walkableMask))
                        {
                            walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                        if (!walkable)
                            movementPenalty += obstacleProximityPenalty;

                        grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                        // 플랫폼일 경우
                        if (hit.transform != null && hit.transform.gameObject.layer == 6 )
                        {
                            grid[x, y + 1].platform = true;
                            grid[x, y + 1].movementPenalty = 0;
                            walkableNodeList.Add(grid[x, y + 1]);
                        }
                    }
                    catch
                    {
                        Debug.Log($"{x}, {y}");
                    }
                }
            }

            // 노드를 중심으로 가중치를 부가하는 노드.
            //BlurPenaltyMap(3);
        }


        void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                }


                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                grid[x, 0].movementPenalty = blurredPenalty;

                for (int y = 1; y < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    grid[x, y].movementPenalty += blurredPenalty;

                    if (blurredPenalty > penaltyMax)
                    {
                        penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < penaltyMin)
                    {
                        penaltyMin = blurredPenalty;
                    }
                }
            }
        }

        public void OpenListAdd(Node checkNode, Node targetNode, ref Heap<Node> openSet, ref HashSet<Node> closeSet)
        {

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    var checkX = checkNode.gridX + x;
                    var checkY = checkNode.gridY + y;
                    if (checkX >= 0 && checkX < gridSizeX + 1 && checkY >= 0 && checkY <= gridSizeY + 1
                        && grid[checkX, checkY].walkable && !closeSet.Contains(grid[checkX, checkY]))
                    {
                        if (!grid[checkNode.gridX, checkY].walkable && !grid[checkX, checkNode.gridY].walkable) continue;
                        if (!grid[checkNode.gridX, checkY].walkable || !grid[checkX, checkNode.gridY].walkable) continue;

                        Node neighborNode = grid[checkX, checkY];
                        int MoveCost = checkNode.gCost + (checkNode.gridX - checkX == 0 || checkNode.gridY - checkY == 0 ? 10 : 14);

                        if (MoveCost < neighborNode.gCost || !openSet.Contains(neighborNode))
                        {
                            neighborNode.gCost = MoveCost;
                            neighborNode.hCost = (Mathf.Abs(neighborNode.gridX - targetNode.gridX) + Mathf.Abs(neighborNode.gridY - targetNode.gridY)) * 10;
                            neighborNode.parent = checkNode;

                            openSet.Add(neighborNode);
                        }
                    }
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if ((x == 0 && y == 0) /*|| (x == -1 && y == -1) || (x == 1 && y == -1) || (x == -1 && y == 1) || (x == 1 && y == 1)*/)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;


                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        if (!grid[checkX, node.gridY].walkable || !grid[node.gridX, checkY].walkable)
                            continue;
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = ((worldPosition.x - transform.position.x )+ gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = ((worldPosition.y - transform.position.y) + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            try
            {
                if (!grid[x, y].walkable)
                {
                    for (int ix = x - 1; ix <= x + 1; ix++)
                        for (int iy = y - 1; iy <= y + 1; iy++)
                            if (grid[ix, iy].walkable)
                                return grid[ix, iy];
                }
            }
            catch
            {
                return null;
            }

            return grid[x, y];
        }

        //public List<Node> path;
        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    //Gizmos.color = new Color(0, 0, 0, 0.25f);
                    Gizmos.color = Color.Lerp(new Color(Color.white.r, Color.white.g, Color.white.b, 1f),
                        new Color(Color.black.r, Color.black.g, Color.black.b, 1f),
                        Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));

                    //Gizmos.color = (n.walkable) ? new Color(Gizmos.gray.r, Gizmos.color.g, Gizmos.color.b, 1f) : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                    if ((n.gridX + n.gridY) % 2 == 0)
                        Gizmos.color = (n.walkable) ? Color.gray : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                    else
                        Gizmos.color = (n.walkable) ? Color.white : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                    //if (path != null)
                    //    if (path.Contains(n))
                    //        Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
                }
            }
            if(grid != null && displayGridOnPlatformGizmos)
            {
                foreach (Node n in grid)
                {
                    if(n.platform)
                    {
                        //Gizmos.color = new Color(0, 0, 0, 0.25f);
                        Gizmos.color = Color.Lerp(new Color(Color.white.r, Color.white.g, Color.white.b, 1f),
                            new Color(Color.black.r, Color.black.g, Color.black.b, 1f),
                            Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));

                        //Gizmos.color = (n.walkable) ? new Color(Gizmos.gray.r, Gizmos.color.g, Gizmos.color.b, 1f) : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                        if ((n.gridX + n.gridY) % 2 == 0)
                            Gizmos.color = (n.walkable) ? Color.gray : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                        else
                            Gizmos.color = (n.walkable) ? Color.white : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                        //if (path != null)
                        //    if (path.Cxontains(n))
                        //        Gizmos.color = Color.black;
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
                    }
                }
            }
        }
        int totalCount = 0;
        int count = 0;

        public Vector3 GetRandPoint(Vector3 Point)
        {
            int rand = 0;
            count = 0;
            while (true)
            {
                count = 0;
                rand = UnityEngine.Random.Range(0, walkableNodeList.Count);
                // 랜덤 좌표를 받아왔다면? 해당 좌표가 플레이어의 위치와 얼마나 차이가 나는지 체크해야한다.
                float distance = Vector3.Distance(walkableNodeList[rand].worldPosition, Point);
                if (Mathf.Abs(distance) > 3f)
                {
                    break;
                }
            }
            totalCount += count;
            return walkableNodeList[rand].worldPosition; ;
        }

        [Serializable]
        public class TerrainType
        {
            public LayerMask terrainMask;
            public int terrainPenalty;
        }
    }
}