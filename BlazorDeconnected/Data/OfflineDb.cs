using Blazor.IndexedDB;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace BlazorDeconnected.Data
{
    public class OfflineDb : IndexedDb
    {

        public OfflineDb(IJSRuntime jsRuntime, string name, int version)
              : base(jsRuntime, name, version)
        { }


        // Stores (tables) — Id clé primaire auto si long/int, ou propriété [Key]
        public IndexedSet<FormDraft> Drafts { get; set; }
        public IndexedSet<OutboxItem> Outbox { get; set; }
    }

    public class FormDraft
    {
        [Key] public long Id { get; set; }                // auto-incrément si long
        [Required] public string DraftKey { get; set; } = default!;  // ex: "person-form"
        [Required] public string JsonData { get; set; } = default!;
        public DateTime SavedAt { get; set; }
    }

    public class OutboxItem
    {
        [Key] public long Id { get; set; }                // auto-incrément
        [Required] public string ClientKey { get; set; } = default!; // idempotency key (GUID côté client)
        [Required] public string Endpoint { get; set; } = default!;
        [Required] public string Method { get; set; } = "POST";
        [Required] public string JsonBody { get; set; } = default!;
        public int Attempts { get; set; }
        public DateTime QueuedAt { get; set; }
    }
}
