namespace Assets.Gamelogic.Core
{
    public static class ComponentUtils
    {
        public static uint QuantizeAngle(float angle)
        {
            return (uint)(angle * SimulationSettings.AngleQuantisationFactor);
        }

        public static float DequantizeAngle(uint angle)
        {
            return angle / SimulationSettings.AngleQuantisationFactor;
        }
    }
}
