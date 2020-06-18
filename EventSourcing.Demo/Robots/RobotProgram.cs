using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventSourcing.Demo.Robots
{
    public static class RobotProgram
    {
        public static async Task RunAsync(IRobotStore store)
        {
            var text = await File.ReadAllTextAsync("robots.json");
            var response = JsonConvert.DeserializeObject<EntityListResponse<RobotImported.RobotEntity>>(text);

            var olympus = Guid.Parse("1ab17979-ff43-e911-a970-000d3a391cda");
            
            var tasks = response.Value
                .Where(e => e.C2RurEnduserValue == olympus)
                .Select(entity => ImportRobotAsync(store, entity));
            
            await Task.WhenAll(tasks);

            Console.WriteLine("[Robots]");
            await foreach (var robotId in store.IdsAsync())
            {
                await PrintRobotAsync(store, robotId);
            }
        }

        private static async Task ImportRobotAsync(IRobotStore store, RobotImported.RobotEntity entity)
        {
            var olympusControlCorpGulf = new EndUserId(Guid.Parse("1ab17979-ff43-e911-a970-000d3a391cda"));

            var robot = Robot.Import(entity, "nije");

            if (await store.ExistsAsync(robot.Id))
            {
                Console.WriteLine($"Deleting robot {robot.Id}");
                await store.DeleteAsync(robot.Id);
                Console.WriteLine($"Deleted robot {robot.Id}");
            }

            await store.CommitAsync(robot);
            
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
            
                await store.CommitAsync(robot);
            }
        }

        private static async Task PrintRobotAsync(IRobotStore store, RobotId robotId)
        {
            var robot = await store.AggregateAsync(robotId);
            var registeredRobot = new RegisteredRobot(robot!, robot!.LatestRegistration!);
            Console.WriteLine(JsonConvert.SerializeObject(registeredRobot, Formatting.Indented, new StringEnumConverter()));
        }
    }
}