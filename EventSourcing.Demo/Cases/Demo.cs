using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Cases
{
    public static class Demo
    {
        public static async Task RunAsync(IEventStore store)
        {
            var id = new CaseId(Guid.NewGuid());

            await CreateCaseAsync(store, id);
            await ModifyCaseAsync(store, id);

            var @case = await Case.FromAsync(store, id);
            Console.WriteLine(JsonConvert.SerializeObject(@case, Formatting.Indented));
        }

        private static async Task CreateCaseAsync(IEventStore store, CaseId id)
        {
            var @case = Case.Create(id, "The Subject", "The Description", new CaseType.Support());
            await @case.CommitAsync(store);
        }

        private static async Task ModifyCaseAsync(IEventStore store, CaseId id)
        {
            var @case = await Case.FromAsync(store, id);
            if (@case == null)
            {
                throw new InvalidOperationException();
            }

            var incident = new CRM.Incident
            {
                Title = "Imported Subject",
                Description = "Imported Description",
                
                CaseNumber = new CaseNumber("TS012345"),
                
                Type = CRM.CaseType.Service,
                Origin = CRM.CaseOriginCode.CustomerPortalCreated,
                Status = CRM.CaseStatus.WaitingForDistributor
            };
            
            @case.Import(incident);
            @case.AssignToService();

            await @case.CommitAsync(store);
        }
    }
}