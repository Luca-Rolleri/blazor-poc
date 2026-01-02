using Blazor.IndexedDB;
using BlazorDeconnected.Data;
using static BlazorDeconnected.Data.OfflineDb;

namespace BlazorDeconnected.Services
{
    public class OfflineStore
    {
        private readonly IIndexedDbFactory _factory;
        public OfflineStore(IIndexedDbFactory factory) => _factory = factory;

        private Task<OfflineDb> DbAsync()
              => _factory.Create<OfflineDb>("MyOfflineDb", 1);

        // Brouillons
        public async Task SaveDraftAsync(string draftKey, string json)
        {
            var db = await DbAsync();
            db.Drafts.Add(new FormDraft { DraftKey = draftKey, JsonData = json, SavedAt = DateTime.UtcNow });
            await db.SaveChanges();
        }


        public async Task<FormDraft?> GetDraftAsync(string draftKey)
        {
            var db = await DbAsync();
            return db.Drafts.SingleOrDefault(d => d.DraftKey == draftKey);
        }

        // Outbox
        public async Task EnqueueAsync(OutboxItem item)
        {
            var db = await DbAsync();
            db.Outbox.Add(item);
            await db.SaveChanges();
        }

        public async Task<List<OutboxItem>> GetQueueAsync()
        {
            var db = await DbAsync();
            return db.Outbox.ToList().OrderBy(x => x.QueuedAt).ToList();
        }

        public async Task RemoveAsync(long id)
        {
            var db = await DbAsync();
            var item = db.Outbox.SingleOrDefault(x => x.Id == id);
            if (item is not null) { db.Outbox.Remove(item); await db.SaveChanges(); }
        }

        public async Task MarkAttemptAsync(long id)
        {
            var db = await DbAsync();
            var item = db.Outbox.SingleOrDefault(x => x.Id == id);
            if (item is null) return;
            item.Attempts++;
            await db.SaveChanges();
        }


    }
}
