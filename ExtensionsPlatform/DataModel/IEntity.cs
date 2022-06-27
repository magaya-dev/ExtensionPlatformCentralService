using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.DataModel
{
    public interface IEntity : IVersionable, IPartionable
    {
    }

    public interface IVersionable
    {
        string ETag { get; set; }
    }

    public interface IPartionable
    {
        string PartitionKey { get; }
    }
}
