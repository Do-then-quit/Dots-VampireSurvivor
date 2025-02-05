using Unity.Entities;

public struct WaterNodeComponent : IComponentData
{
    public float Height;    // 현재 높이 (예: 기준 0)
    public float Velocity;  // 높이 변화 속도
    public float Damping;   // 감쇠 계수 (0 ~ 1, 1이면 감쇠 없음)
}
