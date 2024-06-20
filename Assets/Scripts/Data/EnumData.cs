using System;

namespace Data
{
    public enum ECharacterType
    {
        None,
        Hit,
        Frost,
        Kane,
        Storm,
        E
    }
    
    public enum EPetType
    {
        None,
        Scorchwing, // 스코치윙
        Icebound, // 아이스바운드
        Aerion, // 에리온
        Electra // 일렉트라
    }
    
    public enum ActorType
    {
        None,
        User,
        AI
    }
    
    public enum GestureType
    {
        None,
        DoubleTap,
        ScrollDown,
        Pinch
    }

    public enum EVocaType
    {
        Kung,
        Tang,
        Fuck
    }

    [Flags]
    public enum EBattleMapKind
    {
        None,
        Day,
        Night
    }
}