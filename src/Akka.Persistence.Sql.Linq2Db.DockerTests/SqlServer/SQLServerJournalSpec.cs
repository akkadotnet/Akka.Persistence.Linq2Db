﻿using System;
using Akka.Persistence.Sql.Linq2Db.Config;
using Akka.Persistence.Sql.Linq2Db.Db;
using Akka.Persistence.Sql.Linq2Db.Journal.Types;
using Akka.Persistence.Sql.Linq2Db.Tests.Docker.Docker;
using Akka.Persistence.TCK.Journal;
using LinqToDB;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Persistence.Sql.Linq2Db.Tests.Docker.SqlServer
{
    [Collection("SqlServerSpec")]
    public class SQLServerJournalSpec : JournalSpec
    {

        

        public static Configuration.Config Initialize(SqlServerFixture fixture)
        {
            DockerDbUtils.Initialize(fixture.ConnectionString);
            return config;
        }

        private static Configuration.Config config =>
            SQLServerJournalSpecConfig.Create(DockerDbUtils.ConnectionString,
                "journalSpec");
        public SQLServerJournalSpec(ITestOutputHelper outputHelper, SqlServerFixture fixture)
            : base(Initialize(fixture), "SQLServer", outputHelper)
        {
            var connFactory = new AkkaPersistenceDataConnectionFactory(new JournalConfig(config.GetConfig("akka.persistence.journal.linq2db")));
            using (var conn = connFactory.GetConnection())
            {
                try
                {
                    conn.GetTable<JournalRow>().Delete();
                }
                catch (Exception e)
                {
                   
                }
                try
                {
                    conn.GetTable<JournalMetaData>().Delete();
                }
                catch (Exception e)
                {
                   
                }
            }

            Initialize();
        }
        // TODO: hack. Replace when https://github.com/akkadotnet/akka.net/issues/3811
        protected override bool SupportsSerialization => false;
    }
}