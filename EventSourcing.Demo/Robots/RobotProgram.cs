using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aggregail;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    public static class RobotProgram
    {
        public static async Task RunAsync(IEventStore store)
        {
            var text = await File.ReadAllTextAsync("robots.json");
            var response = JsonConvert.DeserializeObject<EntityListResponse<RobotImported.RobotEntity>>(text);

            var olympus = Guid.Parse("1ab17979-ff43-e911-a970-000d3a391cda");
            foreach (var entity in response.Value.Where(e => e.C2RurEnduserValue == olympus))
            {
                await ImportRobotAsync(store, entity);
            }

            Console.WriteLine("[Robots]");
            await foreach (var robotId in Robot.IdsAsync(store))
            {
                Console.WriteLine(robotId);
            }
        }

        private static async Task ImportRobotAsync(IEventStore store, RobotImported.RobotEntity entity)
        {
            var olympusControlCorpGulf = new EndUserId(Guid.Parse("1ab17979-ff43-e911-a970-000d3a391cda"));

            var robot = Robot.Import(entity);
            await robot.CommitAsync(store);
            
            robot.Unregister(olympusControlCorpGulf);
            await robot.CommitAsync(store);

            robot.Register(olympusControlCorpGulf, null, RobotApplication.Dispensing);
            await robot.CommitAsync(store);

            robot.Edit(olympusControlCorpGulf, "Machinist", RobotApplication.MachineTending);
            await robot.CommitAsync(store);

            // var json = JsonConvert.SerializeObject(robot, Formatting.Indented, new StringEnumConverter());
            // Console.WriteLine(json);

            // var aggregate = await Robot.FromAsync(store, robot.Id);
            // var registration = aggregate!.LatestRegistration;
            // var registration = aggregate!.LatestRegistrationFor(olympusControlCorpGulf);
            // var registeredRobot = new RegisteredRobot(aggregate!, registration!);

            // var json1 = JsonConvert.SerializeObject(registeredRobot, Formatting.Indented, new StringEnumConverter());
            // Console.WriteLine(json1);
            
            // robot.Unregister(olympusControlCorpGulf);
            // await robot.CommitAsync(store);

            // aggregate = await Robot.FromAsync(store, robot.Id);
            // var distributorRobot = new DistributorRobot(aggregate!);

            // var json2 = JsonConvert.SerializeObject(distributorRobot, Formatting.Indented, new StringEnumConverter());
            // Console.WriteLine(json2);
            
            // robot.Register(olympusControlCorpGulf, null, RobotApplication.Welding);
            // await robot.CommitAsync(store);

            // aggregate = await Robot.FromAsync(store, robot.Id);
            // registration = aggregate!.LatestRegistrationFor(olympusControlCorpGulf);
            // registeredRobot = new RegisteredRobot(aggregate!, registration!);

            // var json3 = JsonConvert.SerializeObject(registeredRobot, Formatting.Indented, new StringEnumConverter());
            // Console.WriteLine(json3);
        }
    }
}