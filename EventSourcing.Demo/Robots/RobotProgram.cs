using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aggregail;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventSourcing.Demo.Robots
{
    public static class RobotProgram
    {
        public static async Task RunAsync(IEventStore store)
        {
            var text = await File.ReadAllTextAsync("robots.json");
            var response = JsonConvert.DeserializeObject<EntityListResponse<RobotImported.RobotEntity>>(text);

            var olympus = Guid.Parse("1ab17979-ff43-e911-a970-000d3a391cda");
            
            var tasks = response.Value
                .Where(e => e.C2RurEnduserValue == olympus)
                .Select(entity => ImportRobotAsync(store, entity));
            
            await Task.WhenAll(tasks);

            Console.WriteLine("[Robots]");
            await foreach (var robotId in Robot.IdsAsync(store))
            {
                await PrintRobotAsync(store, robotId);
            }
        }

        private static async Task ImportRobotAsync(IEventStore store, RobotImported.RobotEntity entity)
        {
            var olympusControlCorpGulf = new EndUserId(Guid.Parse("1ab17979-ff43-e911-a970-000d3a391cda"));

            var robot = Robot.Import(entity);

            if (await Robot.ExistsAsync(store, robot.Id))
            {
                Console.WriteLine($"Deleting robot {robot.Id}");
                await Robot.DeleteFromAsync(store, robot.Id);
                Console.WriteLine($"Deleted robot {robot.Id}");
            }

            await robot.CommitAsync(store);
            
            for (var i = 0; i < 1; i++)
            {
                for (var j = 0; j < 3_333; j++)
                {
                    robot.Unregister(olympusControlCorpGulf);
                    
                    var registeredApplication = Enums.GetRandomValue<RobotApplication>();
                    robot.Register(olympusControlCorpGulf, null, registeredApplication);
                    
                    var editedApplication = Enums.GetRandomValue<RobotApplication>();
                    var name = $"{editedApplication:G} {editedApplication:G}";
                    robot.Edit(olympusControlCorpGulf, name, editedApplication);
                }
            
                await robot.CommitAsync(store);
            }
        }

        private static async Task PrintRobotAsync(IEventStore store, RobotId robotId)
        {
            var robot = await Robot.FromAsync(store, robotId);
            var registeredRobot = new RegisteredRobot(robot!, robot!.LatestRegistration!);
            Console.WriteLine(JsonConvert.SerializeObject(registeredRobot, Formatting.Indented, new StringEnumConverter()));
        }
    }
}