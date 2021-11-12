namespace BytesPack.Sterling.Takeoff.IntegrationTests.MartenTestHarness
{
    using Marten;
    using System;
    using Weasel.Postgresql;
    using Xunit;

    [CollectionDefinition("integration")]
    public class MartenTestHarnessCollection : ICollectionFixture<DefaultStoreFixture>
    {

    }

    [Collection("integration")]
    public class MartenTestHarness : StoreContext<DefaultStoreFixture>
    {
        private DocumentStore store;
        private bool hasBuiltStore = false;
        private bool overrideSession;

        public MartenTestHarness(DefaultStoreFixture fixture) : base(fixture)
        {

        }

        /// <summary>
        /// Customize the store configuration for one off tests.
        /// The return value is the database schema
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected string StoreOptions(Action<StoreOptions> configure)
        {
            overrideSession = true;

            if (session != null)
            {
                session.Dispose();
                Disposables.Remove(session);
                session = null;
            }


            var options = new StoreOptions();
            options.Connection(ConnectionSource.ConnectionString);

            // Can be overridden
            options.AutoCreateSchemaObjects = AutoCreate.All;
            options.NameDataLength = 100;
            options.DatabaseSchemaName = "special";

            configure(options);

            store = new DocumentStore(options);
            Disposables.Add(store);

            store.Advanced.Clean.CompletelyRemoveAll();

            return options.DatabaseSchemaName;
        }

        protected override DocumentStore TheStore
        {
            get
            {
                if (store != null)
                {
                    return store;
                }

                if (!hasBuiltStore)
                {
                    base.TheStore.Advanced.Clean.DeleteAllDocuments();
                    base.TheStore.Advanced.Clean.DeleteAllEventData();
                    hasBuiltStore = true;
                }

                return base.TheStore;
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);

            if (overrideSession)
            {
                Fixture.Store.Advanced.Clean.CompletelyRemoveAll();
            }

            base.Dispose();
        }
    }
}
