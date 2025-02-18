


//공격방식
public enum AttackType
{
    projectile,
    melee,
    hitscan
}

public enum AlertState
{
    Stay,
    Searching, //탐지거리 0.5
    Aleat, //탐지거리 1
    Track //추적
}

public enum TrackType
{
    FullTrack,
    HalfTrack,
    NoneTrack
}

public enum Faction 
{
    Friendly,
    Slavika,
    Zombie
}

