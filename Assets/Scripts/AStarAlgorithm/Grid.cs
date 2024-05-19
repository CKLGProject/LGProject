using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace pathFinding
{
    public class Grid : MonoBehaviour
    {
        public static Grid Instance { get; private set; }

        //public bool onlyDisplayPathGizmos;
        public bool displayGridGizmos;
        public bool displayGridOnPlatformGizmos;
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public TerrainType[] walkableRegions;
        public int obstacleProximityPenalty = 10;
        public Node[,] grid;
        private LayerMask _walkableMask;
        private readonly Dictionary<int, int> _walkableRegionsDictionary = new();

        // 이동할 수 있는 포인트를 넣어두자
        public readonly List<Node> WalkableNodeList = new();

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

        private float _nodeDiameter;
        private int _gridSizeX;
        private int gridSizeY;

        private int _penaltyMin = int.MaxValue;
        private int _penaltyMax = int.MinValue;

        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
            {
                Instance = this;

                _nodeDiameter = nodeRadius * 2;
                _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
                gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);

                foreach (TerrainType region in walkableRegions)
                {
                    _walkableMask.value |= region.terrainMask.value;
                    _walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
                }

                CreateGrid();
            }
        }

        public int MaxSize => _gridSizeX * gridSizeY;

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

        private void CreateGrid()
        {
            grid = new Node[_gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

            for (int x = _gridSizeX - 1; x >= 0; x--)
            {
                for (int y = gridSizeY - 1; y >= 0; y--)
                {try
                    {
                        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + nodeRadius) + Vector3.up * (y * _nodeDiameter + nodeRadius);
                        bool walkable = (!Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                        int movementPenalty = 0;

                        // raycast

                        Ray ray = new Ray(worldPoint + Vector3.forward * 5, Vector3.back);
                        if (Physics.Raycast(ray, out RaycastHit hit, 10, _walkableMask))
                        {
                            _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                        if (!walkable)
                            movementPenalty += obstacleProximityPenalty;

                        grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                        // 플랫폼일 경우
                        if (hit.transform != null && hit.transform.gameObject.layer == 6 )
                        {
                            grid[x, y + 1].Platform = true;
                            grid[x, y + 1].MovementPenalty = 0;
                            WalkableNodeList.Add(grid[x, y + 1]);
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


        private void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[_gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[_gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].MovementPenalty;
                }


                for (int x = 1; x < _gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].MovementPenalty + grid[addIndex, y].MovementPenalty;
                }
            }

            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                grid[x, 0].MovementPenalty = blurredPenalty;

                for (int y = 1; y < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    grid[x, y].MovementPenalty += blurredPenalty;

                    if (blurredPenalty > _penaltyMax)
                    {
                        _penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < _penaltyMin)
                    {
                        _penaltyMin = blurredPenalty;
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
                    var checkX = checkNode.GridX + x;
                    var checkY = checkNode.GridY + y;
                    if (checkX >= 0 && checkX < _gridSizeX + 1 && checkY >= 0 && checkY <= gridSizeY + 1
                        && grid[checkX, checkY].Walkable && !closeSet.Contains(grid[checkX, checkY]))
                    {
                        if (!grid[checkNode.GridX, checkY].Walkable && !grid[checkX, checkNode.GridY].Walkable) continue;
                        if (!grid[checkNode.GridX, checkY].Walkable || !grid[checkX, checkNode.GridY].Walkable) continue;

                        Node neighborNode = grid[checkX, checkY];
                        int MoveCost = checkNode.GCost + (checkNode.GridX - checkX == 0 || checkNode.GridY - checkY == 0 ? 10 : 14);

                        if (MoveCost < neighborNode.GCost || !openSet.Contains(neighborNode))
                        {
                            neighborNode.GCost = MoveCost;
                            neighborNode.HCost = (Mathf.Abs(neighborNode.GridX - targetNode.GridX) + Mathf.Abs(neighborNode.GridY - targetNode.GridY)) * 10;
                            neighborNode.Parent = checkNode;

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

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;


                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        if (!grid[checkX, node.GridY].Walkable || !grid[node.GridX, checkY].Walkable)
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

            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            try
            {
                if (!grid[x, y].Walkable)
                {
                    for (int ix = x - 1; ix <= x + 1; ix++)
                        for (int iy = y - 1; iy <= y + 1; iy++)
                            if (grid[ix, iy].Walkable)
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
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    //Gizmos.color = new Color(0, 0, 0, 0.25f);
                    Gizmos.color = Color.Lerp(new Color(Color.white.r, Color.white.g, Color.white.b, 1f),
                        new Color(Color.black.r, Color.black.g, Color.black.b, 1f),
                        Mathf.InverseLerp(_penaltyMin, _penaltyMax, n.MovementPenalty));

                    //Gizmos.color = (n.walkable) ? new Color(Gizmos.gray.r, Gizmos.color.g, Gizmos.color.b, 1f) : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                    if ((n.GridX + n.GridY) % 2 == 0)
                        Gizmos.color = (n.Walkable) ? Color.gray : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                    else
                        Gizmos.color = (n.Walkable) ? Color.white : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                    //if (path != null)
                    //    if (path.Contains(n))
                    //        Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter));
                }
            }
            if(grid != null && displayGridOnPlatformGizmos)
            {
                foreach (Node n in grid)
                {
                    if(n.Platform)
                    {
                        //Gizmos.color = new Color(0, 0, 0, 0.25f);
                        Gizmos.color = Color.Lerp(new Color(Color.white.r, Color.white.g, Color.white.b, 1f),
                            new Color(Color.black.r, Color.black.g, Color.black.b, 1f),
                            Mathf.InverseLerp(_penaltyMin, _penaltyMax, n.MovementPenalty));

                        //Gizmos.color = (n.walkable) ? new Color(Gizmos.gray.r, Gizmos.color.g, Gizmos.color.b, 1f) : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                        if ((n.GridX + n.GridY) % 2 == 0)
                            Gizmos.color = (n.Walkable) ? Color.gray : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                        else
                            Gizmos.color = (n.Walkable) ? Color.white : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
                        //if (path != null)
                        //    if (path.Cxontains(n))
                        //        Gizmos.color = Color.black;
                        Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter));
                    }
                }
            }
        }

        private int totalCount;
        private int count;

        public Vector3 GetRandPoint(Vector3 Point)
        {
            int rand = 0;
            count = 0;
            while (true)
            {
                count = 0;
                rand = UnityEngine.Random.Range(0, WalkableNodeList.Count);
                // 랜덤 좌표를 받아왔다면? 해당 좌표가 플레이어의 위치와 얼마나 차이가 나는지 체크해야한다.
                float distance = Vector3.Distance(WalkableNodeList[rand].WorldPosition, Point);
                if (Mathf.Abs(distance) > 3f)
                {
                    break;
                }
            }
            totalCount += count;
            return WalkableNodeList[rand].WorldPosition; ;
        }

        [Serializable]
        public class TerrainType
        {
            public LayerMask terrainMask;
            public int terrainPenalty;
        }
    }
}