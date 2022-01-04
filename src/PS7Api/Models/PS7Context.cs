﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PS7Api.Models;

public class Ps7Context : IdentityDbContext<User>
{
    public Ps7Context(DbContextOptions<Ps7Context> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentAnomaly> DocumentAnomalies { get; set; }
}