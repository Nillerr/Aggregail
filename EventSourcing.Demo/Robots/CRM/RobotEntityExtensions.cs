namespace EventSourcing.Demo.Robots.CRM
{
    public static class RobotEntityExtensions
    {
        public static RobotProduct RobotProduct(this RobotImported.RobotEntity source) =>
            RobotProducts.FromSerialNumber(source.C2RurName);
    }
}