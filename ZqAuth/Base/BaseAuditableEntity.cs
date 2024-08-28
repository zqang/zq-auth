﻿namespace ZqAuth.Base;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }
    
    public DateTimeOffset LastModified { get; set; }

}