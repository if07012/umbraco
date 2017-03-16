using System;

namespace Voxteneo.Core.Domains.Contracts
{
    public interface ISoftDelete
    {
        bool IsDelete { get; set; }
    }
    public interface IAuditTrailEntity
    {
        byte RelationType { get; set; }
        byte ActionType { get; set; }
        DateTime ActionDate { get; set; }

        string Field { get; set; }
        string Username { get; set; }

        string NewValue { get; set; }
        string OldValue { get; set; }

        int RelationId { get; set; }
    }
}