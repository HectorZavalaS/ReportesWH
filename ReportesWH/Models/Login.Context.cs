﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReportesWH.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class siixsem_main_dbEntities : DbContext
    {
        public siixsem_main_dbEntities()
            : base("name=siixsem_main_dbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<siixsem_users_t> siixsem_users_t { get; set; }
    
        public virtual ObjectResult<validate_user_Result> validate_user(string user, string pass)
        {
            var userParameter = user != null ?
                new ObjectParameter("user", user) :
                new ObjectParameter("user", typeof(string));
    
            var passParameter = pass != null ?
                new ObjectParameter("pass", pass) :
                new ObjectParameter("pass", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<validate_user_Result>("validate_user", userParameter, passParameter);
        }
    }
}