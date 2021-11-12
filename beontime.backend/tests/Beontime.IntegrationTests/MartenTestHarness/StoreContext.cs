namespace BytesPack.Sterling.Takeoff.IntegrationTests.MartenTestHarness
{
    using Marten;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class StoreContext<T> : IDisposable where T : StoreFixture
    {
        protected DocumentTracking DocumentTracking { get; set; } = DocumentTracking.None;
        protected virtual DocumentStore TheStore => Fixture.Store;
        protected T Fixture { get; }

        protected IDocumentSession session;
        protected readonly IList<IDisposable> Disposables = new List<IDisposable>();

        protected IDocumentSession TheSession
        {
            get
            {
                if (session == null)
                {
                    session = TheStore.OpenSession(DocumentTracking);
                    Disposables.Add(session);
                }

                return session;
            }
        }

        protected StoreContext(T fixture)
        {
            Fixture = fixture;
        }

        protected async Task AppendEvent(Guid streamId, params object[] events)
        {
            TheSession.Events.Append(streamId, events);
            await TheSession.SaveChangesAsync();
        }

        public virtual void Dispose()
        {
            foreach (var disposable in Disposables)
            {
                GC.SuppressFinalize(this);
                disposable.Dispose();
            }
        }
    }
}
