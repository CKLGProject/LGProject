using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace pathFinding
{
    public struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public readonly Action<Vector3[], bool> Callback;
        
        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
        {
            PathStart = start;
            PathEnd = end;
            Callback = callback;
        }
    }
    public struct PathResult
    {
        public Vector3[] Path;
        public bool Success;
        public Action<Vector3[], bool> Callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            Path = path;
            Success = success;
            Callback = callback;
        }
    }

    public class PathRequestManager : MonoBehaviour
    {

        //Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        //PathRequest currentPathRequest;

        private Queue<PathResult> _results = new Queue<PathResult>();
        private Pathfinding _pathfinding;

        //bool isProcessingPath;

        private static PathRequestManager _instance;

        private void Awake()
        {
            _instance = this;
            _pathfinding = GetComponent<Pathfinding>();
        }

        private void Update()
        {
            if(_results.Count > 0)
            {
                int itemsInQueue = _results.Count;
                lock(_results)
                {
                    for(int i =0; i < itemsInQueue; i++)
                    {
                        PathResult result = _results.Dequeue();
                        result.Callback(result.Path, result.Success);
                    }
                }
            }
        }

        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                _instance._pathfinding.FindPath(request, _instance.FinishedProcessingPath);
            };
            threadStart.Invoke();

        }

        public void FinishedProcessingPath(PathResult result)
        {
            //originalRequest.callback(path, success);
            //PathResult result = new PathResult(path, success, originalRequest.callback);
            lock (_results)
            {
                _results.Enqueue(result);
            }
        }


        //public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        //{
        //    PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //    instance.pathRequestQueue.Enqueue(newRequest);
        //    instance.TryProcessNext();
        //}

        //void TryProcessNext()
        //{
        //    if (!isProcessingPath && pathRequestQueue.Count > 0)
        //    {
        //        currentPathRequest = pathRequestQueue.Dequeue();
        //        isProcessingPath = true;
        //        Pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        //    }
        //}

        //public void FinishedProcessingPath(Vector3[] path, bool success)
        //{
        //    currentPathRequest.callback(path, success);
        //    isProcessingPath = false;
        //    TryProcessNext();
        //}

        // ������ ��ġ�� ��忡 �̵��� �� �ִ��� üũ
        public static bool IsMovementPoint(Vector3 point)
        {
            // ����Ʈ ���� �̵��� �����Ѱ�?.
            return _instance._pathfinding.IsMovementPoint(point) ? true : false;
        }


    }

}
