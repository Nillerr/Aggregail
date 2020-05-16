using System;
using System.Threading.Tasks;
using Aggregail;
using EventSourcing.Demo.Robots.CRM;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    public static class RobotProgram
    {
        public static async Task RunAsync(IEventStore store)
        {
            var entity = new RobotEntity
            {
                Id = Guid.NewGuid(),
                SerialNumber = "82309810"
            };
            
            var endUser1 = new EndUserId(Guid.Parse("9827f1ee-327e-47ed-a051-8003f1cd5963"));
            var endUser2 = new EndUserId(Guid.Parse("7d74de82-0782-4895-a5bb-647ffb59dc83"));

            var robot = Robot.Import(entity);
            await robot.CommitAsync(store);
            
            robot.Register(endUser1, "EU1 Robot Name");
            await robot.CommitAsync(store);
            
            robot.Unregister(endUser1);
            await robot.CommitAsync(store);
            
            robot.Register(endUser2, "EU2 Robot Name");
            await robot.CommitAsync(store);
            
            robot.Edit(endUser2, "EU2 Robot Name - Edited");
            await robot.CommitAsync(store);

            var aggregate = await Robot.FromAsync(store, robot.Id);
            var registration = aggregate!.LatestRegistrationFor(endUser1);
            var registeredRobot = new RegisteredRobot(aggregate!, registration!);
            Console.WriteLine(JsonConvert.SerializeObject(registeredRobot, Formatting.Indented));
        }
    }
}