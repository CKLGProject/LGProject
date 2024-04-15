using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AIBehaviourTree
{
    public abstract class Node
    {
        public enum State
        {
            Running,
            Failure,
            Success,
        }

        public State state = State.Running;
        public bool started = false;

        public State Update()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state == State.Failure || state == State.Success)
            {
                OnStart();
                started = false;
            }

            return state;
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();

    }

}