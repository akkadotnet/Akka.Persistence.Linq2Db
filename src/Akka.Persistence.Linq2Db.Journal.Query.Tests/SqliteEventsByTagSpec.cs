﻿//-----------------------------------------------------------------------
// <copyright file="SqliteEventsByTagSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2020 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2020 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Configuration;
using Akka.Persistence.Linq2Db.Journal.Query.Tests;
using Akka.Persistence.Query;
using Akka.Persistence.Sql.Linq2Db.Journal;
using Akka.Persistence.Sql.Linq2Db.Query;
using Akka.Persistence.TCK.Query;
using Akka.Util.Internal;
using LinqToDB;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Persistence.Sqlite.Tests.Query
{
    public class SqliteEventsByTagSpec : EventsByTagSpec
    {
        public static readonly AtomicCounter Counter = new AtomicCounter(0);

        public static Config Config(int id)
        {
            var connString = $"Filename=file:memdb-l2db-journal-eventsbytag-{id}.db;Mode=Memory;Cache=Shared";
            ConnectionContext.Remember(connString);
            return ConfigurationFactory.ParseString($@"
            akka.loglevel = INFO
            akka.persistence.journal.plugin = ""akka.persistence.journal.linq2db""
            akka.persistence.journal.linq2db {{
                event-adapters {{
                  color-tagger  = ""Akka.Persistence.TCK.Query.ColorFruitTagger, Akka.Persistence.TCK""
                }}
                event-adapter-bindings = {{
                  ""System.String"" = color-tagger
                }}
                plugin-dispatcher = ""akka.actor.default-dispatcher""
                auto-initialize = on
                provider-name = ""{ProviderName.SQLiteMS}""
                tables {{
                   journal {{
                     table-name = event_journal
                     metadata-table-name = journal_metadata
                     auto-init = true 
                     warn-on-auto-init-fail = false
                   }} 
                }}
                connection-string = ""{connString}""
                refresh-interval = 1s
            }}
            akka.persistence.query.journal.linq2db
            {{
                provider-name = ""{ProviderName.SQLiteMS}""
                connection-string = ""{connString}""
                tables {{
                   journal {{
                     table-name = event_journal
                     metadata-table-name = journal_metadata 
                     warn-on-auto-init-fail = false
                   }} 
                }}
            }}
            akka.test.single-expect-default = 10s")
                .WithFallback(Linq2DbReadJournal.DefaultConfiguration)
                .WithFallback(Linq2DbWriteJournal.DefaultConfiguration);
        }

        public SqliteEventsByTagSpec(ITestOutputHelper output) : base(Config(Counter.GetAndIncrement()), nameof(SqliteEventsByTagSpec), output)
        {
            ReadJournal = Sys.ReadJournalFor<Linq2DbReadJournal>(Linq2DbReadJournal.Identifier);
        }
    }
}
