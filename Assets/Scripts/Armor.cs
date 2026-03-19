public struct ArmorStat
{
    public float defence;
    public float speed;

    public ArmorStat(float defence, float speed)
    {
        this.defence = defence;
        this.speed = speed;
    }
}
public static class Armor
{
    public static ArmorStat[] armorStats =
    {
        new ArmorStat(1.05f, 1),
        new ArmorStat(1.1f, 0.95f),
        new ArmorStat(1.2f, 0.85f)
    };
}
