using System;
using Aggregail;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    public sealed class RobotImported
    {
        public static readonly EventType<RobotImported> EventType = "RobotImported";

        public RobotEntity Entity { get; set; }

        public static RobotImported Create(RobotEntity entity) =>
            new RobotImported
            {
                Entity = entity
            };

        public enum Application
        {
            Assembly = 964110000,
            MaterialHandling = 964110001,
            MachineTending = 964110002,
            MaterialRemoval = 964110004,
            Quality = 964110005,
            Welding = 964110006,
            Other = 964110008,
            Finishing = 964110009,
            Dispensing = 964110010,
        }

        public sealed class RobotEntity
        {
            [JsonProperty("@odata.etag", Required = Required.Always)]
            public string OdataEtag { get; set; }

            [JsonProperty("c2rur_calculatedendofwarranty", Required = Required.AllowNull)]
            public DateTimeOffset? C2RurCalculatedendofwarranty { get; set; }

            [JsonProperty("statecode", Required = Required.Always)]
            public long Statecode { get; set; }

            [JsonProperty("statuscode", Required = Required.Always)]
            public long Statuscode { get; set; }

            [JsonProperty("_c2rur_customer_value", Required = Required.AllowNull)]
            public Guid? C2RurCustomerValue { get; set; }

            [JsonProperty("_c2rur_product_value", Required = Required.AllowNull)]
            public Guid? C2RurProductValue { get; set; }

            [JsonProperty("createdon", Required = Required.Always)]
            public DateTimeOffset Createdon { get; set; }

            [JsonProperty("aka_street", Required = Required.AllowNull)]
            public string? AkaStreet { get; set; }

            [JsonProperty("c2rur_zippostalcode", Required = Required.AllowNull)]
            public string? C2RurZippostalcode { get; set; }

            [JsonProperty("_ownerid_value", Required = Required.Always)]
            public Guid OwneridValue { get; set; }

            [JsonProperty("new_soldtoenduserdate", Required = Required.AllowNull)]
            public DateTimeOffset? NewSoldtoenduserdate { get; set; }

            [JsonProperty("c2rur_invoicelineid", Required = Required.AllowNull)]
            public Guid? C2RurInvoicelineid { get; set; }

            [JsonProperty("modifiedon", Required = Required.Always)]
            public DateTimeOffset Modifiedon { get; set; }

            [JsonProperty("c2rur_country", Required = Required.AllowNull)]
            public string? C2RurCountry { get; set; }

            [JsonProperty("versionnumber", Required = Required.Always)]
            public long Versionnumber { get; set; }

            [JsonProperty("c2rur_warrantyexceeded", Required = Required.Always)]
            public bool C2RurWarrantyexceeded { get; set; }

            [JsonProperty("timezoneruleversionnumber", Required = Required.AllowNull)]
            public long? Timezoneruleversionnumber { get; set; }

            [JsonProperty("_modifiedby_value", Required = Required.Always)]
            public Guid ModifiedbyValue { get; set; }

            [JsonProperty("c2rur_invoicedate", Required = Required.AllowNull)]
            public DateTimeOffset? C2RurInvoicedate { get; set; }

            [JsonProperty("_c2rur_invoice_value", Required = Required.AllowNull)]
            public Guid? C2RurInvoiceValue { get; set; }

            [JsonProperty("c2rur_city", Required = Required.AllowNull)]
            public string? C2RurCity { get; set; }

            [JsonProperty("c2rur_salesorderid", Required = Required.AllowNull)]
            public string? C2RurSalesorderid { get; set; }

            [JsonProperty("c2rur_endofwarranty", Required = Required.AllowNull)]
            public DateTimeOffset? C2RurEndofwarranty { get; set; }

            [JsonProperty("c2rur_name", Required = Required.Always)]
            public string C2RurName { get; set; }

            [JsonProperty("_createdby_value", Required = Required.Always)]
            public Guid CreatedbyValue { get; set; }

            [JsonProperty("_owningbusinessunit_value", Required = Required.Always)]
            public Guid OwningbusinessunitValue { get; set; }

            [JsonProperty("_owninguser_value", Required = Required.Always)]
            public Guid OwninguserValue { get; set; }

            [JsonProperty("c2rur_robotsid", Required = Required.Always)]
            public Guid C2RurRobotsid { get; set; }

            [JsonProperty("aka_subapplication_test", Required = Required.AllowNull)]
            public long? AkaSubapplicationTest { get; set; }

            [JsonProperty("_ur_ur_softwareversion_value", Required = Required.AllowNull)]
            public object? UrUrSoftwareversionValue { get; set; }

            [JsonProperty("importsequencenumber", Required = Required.AllowNull)]
            public long? Importsequencenumber { get; set; }

            [JsonProperty("ur_zippostalcode2", Required = Required.AllowNull)]
            public string? UrZippostalcode2 { get; set; }

            [JsonProperty("_ur_enduser2installation_value", Required = Required.AllowNull)]
            public Guid? UrEnduser2InstallationValue { get; set; }

            [JsonProperty("c2rur_warrantycorrectioninmonth", Required = Required.AllowNull)]
            public long? C2RurWarrantycorrectioninmonth { get; set; }

            [JsonProperty("ur_servicecontract1", Required = Required.AllowNull)]
            public bool? UrServicecontract1 { get; set; }

            [JsonProperty("ur_companyname2", Required = Required.AllowNull)]
            public string? UrCompanyname2 { get; set; }

            [JsonProperty("_c2rur_systemintegrator_value", Required = Required.AllowNull)]
            public Guid? C2RurSystemintegratorValue { get; set; }

            [JsonProperty("c2rur_application", Required = Required.AllowNull)]
            public long? C2RurApplication { get; set; }

            [JsonProperty("_modifiedonbehalfby_value", Required = Required.AllowNull)]
            public Guid? ModifiedonbehalfbyValue { get; set; }

            [JsonProperty("aka_soldrobottype", Required = Required.AllowNull)]
            public long? AkaSoldrobottype { get; set; }

            [JsonProperty("new_demo", Required = Required.AllowNull)]
            public bool? NewDemo { get; set; }

            [JsonProperty("ur_stateprovince2", Required = Required.AllowNull)]
            public string? UrStateprovince2 { get; set; }

            [JsonProperty("_createdonbehalfby_value", Required = Required.AllowNull)]
            public Guid? CreatedonbehalfbyValue { get; set; }

            [JsonProperty("overriddencreatedon", Required = Required.AllowNull)]
            public DateTimeOffset? Overriddencreatedon { get; set; }

            [JsonProperty("c2rur_stateprovince", Required = Required.AllowNull)]
            public string? C2RurStateprovince { get; set; }

            [JsonProperty("_owningteam_value", Required = Required.AllowNull)]
            public Guid? OwningteamValue { get; set; }

            [JsonProperty("aka_application_test", Required = Required.AllowNull)]
            public Application? AkaApplicationTest { get; set; }

            [JsonProperty("_new_servicehandledby_value", Required = Required.AllowNull)]
            public Guid? NewServicehandledbyValue { get; set; }

            [JsonProperty("ur_servicecontract1date", Required = Required.AllowNull)]
            public DateTimeOffset? UrServicecontract1Date { get; set; }

            [JsonProperty("ur_servicecontract3", Required = Required.AllowNull)]
            public bool? UrServicecontract3 { get; set; }

            [JsonProperty("ur_systemintegrator2", Required = Required.AllowNull)]
            public string? UrSystemintegrator2 { get; set; }

            [JsonProperty("c2rur_industry", Required = Required.AllowNull)]
            public long? C2RurIndustry { get; set; }

            [JsonProperty("_aka_originatingopportunity_value", Required = Required.AllowNull)]
            public Guid? AkaOriginatingopportunityValue { get; set; }

            [JsonProperty("_ur_new_softwareversion_value", Required = Required.AllowNull)]
            public Guid? UrNewSoftwareversionValue { get; set; }

            [JsonProperty("ur_servicecontract3date", Required = Required.AllowNull)]
            public DateTimeOffset? UrServicecontract3Date { get; set; }

            [JsonProperty("ur_c2rur_softwareversion", Required = Required.AllowNull)]
            public string? UrC2RurSoftwareversion { get; set; }

            [JsonProperty("utcconversiontimezonecode", Required = Required.AllowNull)]
            public object? Utcconversiontimezonecode { get; set; }

            [JsonProperty("ur_servicecontract2date", Required = Required.AllowNull)]
            public DateTimeOffset? UrServicecontract2Date { get; set; }

            [JsonProperty("_c2rur_enduser_value", Required = Required.AllowNull)]
            public Guid? C2RurEnduserValue { get; set; }

            [JsonProperty("ur_countryregion2", Required = Required.AllowNull)]
            public string? UrCountryregion2 { get; set; }

            [JsonProperty("ur_companyname", Required = Required.AllowNull)]
            public string? UrCompanyname { get; set; }

            [JsonProperty("ur_servicecontract2", Required = Required.AllowNull)]
            public bool? UrServicecontract2 { get; set; }

            [JsonProperty("_ur_endusercontact_value", Required = Required.AllowNull)]
            public Guid? UrEndusercontactValue { get; set; }

            [JsonProperty("new_additionalinformation", Required = Required.AllowNull)]
            public string? NewAdditionalinformation { get; set; }

            [JsonProperty("ur_city2", Required = Required.AllowNull)]
            public string? UrCity2 { get; set; }
        }
    }
}