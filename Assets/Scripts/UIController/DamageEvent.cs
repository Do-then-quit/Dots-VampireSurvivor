using Unity.Mathematics;
using Unity.Collections;

public static class DamageEventManager
{
    // NativeQueue는 ECS Job과 메인 스레드(UI) 모두에서 사용하기 위해 static으로 관리합니다.
    // Allocator.Persistent를 사용하므로, 게임 종료 시 반드시 Dispose해야 합니다.
    public static NativeQueue<DamageEvent> DamageQueue;
}

public struct DamageEvent
{
    public float Damage;
    public float3 Position;
    public float4 BulletColor;
}