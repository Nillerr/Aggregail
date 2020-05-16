using System.Collections.Generic;

namespace EventSourcing.Demo.Robots.CRM
{
    public static class RobotProducts
    {
        private static class Series
        {
            public const char CB = '3';
            public const char E = '5';
        }

        private static class Model
        {
            public const char UR3 = '3';
            public const char UR5 = '5';
            public const char UR10 = '0';
            public const char UR16 = '6';
        }
        
        private static readonly Dictionary<(int, int), RobotProduct> Models = new Dictionary<(int, int), RobotProduct>
        {
            [(Series.CB, Model.UR3)] = RobotProduct.UR3,
            [(Series.CB, Model.UR5)] = RobotProduct.UR5,
            [(Series.CB, Model.UR10)] = RobotProduct.UR10,
            
            [(Series.E, Model.UR3)] = RobotProduct.UR3e,
            [(Series.E, Model.UR5)] = RobotProduct.UR5e,
            [(Series.E, Model.UR10)] = RobotProduct.UR10e,
            [(Series.E, Model.UR16)] = RobotProduct.UR16e,
        }; 
        
        public static RobotProduct FromSerialNumber(string serialNumber)
        {
            const int seriesIndex = 4;
            const int typeIndex = 5;
            
            // $ - '0' works because numbers are always sequential in character sets
            var series = serialNumber[seriesIndex];
            var model = serialNumber[typeIndex];

            return Models[(series, model)];
        }
    }
}