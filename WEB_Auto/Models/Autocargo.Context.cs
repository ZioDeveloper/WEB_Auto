﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WEB_Auto.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class wisedbEntities : DbContext
    {
        public wisedbEntities()
            : base("name=wisedbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AGR_Periti_WEB> AGR_Periti_WEB { get; set; }
        public virtual DbSet<Periti> Periti { get; set; }
        public virtual DbSet<AGR_Spedizioni> AGR_Spedizioni { get; set; }
        public virtual DbSet<AGR_SpedizioniWEB_vw> AGR_SpedizioniWEB_vw { get; set; }
        public virtual DbSet<AGR_Meteo> AGR_Meteo { get; set; }
        public virtual DbSet<AGR_TipiPerizia> AGR_TipiPerizia { get; set; }
        public virtual DbSet<AGR_SpedizioniWEB_Decoded_vw> AGR_SpedizioniWEB_Decoded_vw { get; set; }
        public virtual DbSet<AGR_Porti> AGR_Porti { get; set; }
        public virtual DbSet<AGR_ModelliAuto> AGR_ModelliAuto { get; set; }
        public virtual DbSet<AGR_TrasportatoriGrimaldi> AGR_TrasportatoriGrimaldi { get; set; }
        public virtual DbSet<AGR_TipoRotabile> AGR_TipoRotabile { get; set; }
    }
}
