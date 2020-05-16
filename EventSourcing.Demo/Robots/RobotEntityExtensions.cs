using System;

namespace EventSourcing.Demo.Robots
{
    public static class RobotEntityExtensions
    {
        public static RobotApplication? GetRobotApplication(this RobotImported.RobotEntity source)
        {
            return source.AkaApplicationTest?.GetRobotApplication();
        }
        
        public static RobotApplication GetRobotApplication(this RobotImported.Application source)
        {
            return source switch
            {
                RobotImported.Application.Assembly => RobotApplication.Assembly,
                RobotImported.Application.MaterialHandling => RobotApplication.MaterialHandling,
                RobotImported.Application.MachineTending => RobotApplication.MachineTending,
                RobotImported.Application.MaterialRemoval => RobotApplication.MaterialRemoval,
                RobotImported.Application.Quality => RobotApplication.Quality,
                RobotImported.Application.Welding => RobotApplication.Welding,
                RobotImported.Application.Other => RobotApplication.Other,
                RobotImported.Application.Finishing => RobotApplication.Finishing,
                RobotImported.Application.Dispensing => RobotApplication.Dispensing,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
    }
}