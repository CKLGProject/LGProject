using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace BehaviourTree
{
    public class WeightDecoratorNode : DecoratorNode, IWeightNode
    {
        private WeightDecoratorNode node;
        public UnityAction<float, float> onWeightChanged;
        // 이 노드로 들어가기 위한 가중치
        [SerializeField] private float weight;
        [SerializeField] private IWeightNode.AIWeightCost aiWeight;
        public float Weight
        {
            get { return weight; }
            set
            {
                if (weight != value)
                {
                    float original = weight;
                    weight = value;
                    onWeightChanged?.Invoke(original, weight);
                }
            }
        }

        public override Node Clone()
        {
            node = Instantiate(this);
            node.Weight = WeightSet();
            //node.Weight = Weight;
            node.child = child.Clone();
            
            return node;
        }

        private float WeightSet()
        {
            LGProject.FileManager.Instance.LoadData();
            AIAgent.Instance.SetData();
            //LGProject.FileManager.Instance.;
            switch (aiWeight)
            {
                case IWeightNode.AIWeightCost.None:
                    break;
                case IWeightNode.AIWeightCost.Guard:
                    weight = AIAgent.Instance.GuardPercent;
                    break;
                case IWeightNode.AIWeightCost.NormalAttack:
                    weight = AIAgent.Instance.AttackPercent;
                    break;
                case IWeightNode.AIWeightCost.DashAttack:
                    weight = AIAgent.Instance.AttackPercent;
                    break;
                case IWeightNode.AIWeightCost.Chasing:
                    weight = AIAgent.Instance.ChasingPercent;
                    break;
                case IWeightNode.AIWeightCost.Movement:
                    weight = AIAgent.Instance.NormalMovePercent;
                    break;
                default:
                    break;
            }
            return weight;
        }

        protected override void OnStart()
        {
            //node.weight = WeightSet();
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {

            return child.Update();
        }
    }
}