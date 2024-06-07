namespace BehaviourTree
{
    internal interface IWeightNode
    {
        public enum AIWeightCost
        {
            None,
            Guard,
            NormalAttack,
            DashAttack,
            Chasing,
            Movement,
        }

        public float Weight
        {
            get; set;
        }

    } 
}