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
        public virtual DbSet<AGR_PERIZIE_DETT_TEMP_MVC> AGR_PERIZIE_DETT_TEMP_MVC { get; set; }
        public virtual DbSet<AGR_PERIZIE_DETT_TEMP_MVC_vw> AGR_PERIZIE_DETT_TEMP_MVC_vw { get; set; }
        public virtual DbSet<AGR_Parti> AGR_Parti { get; set; }
        public virtual DbSet<WEB_AGR_Parti_vw> WEB_AGR_Parti_vw { get; set; }
        public virtual DbSet<WEB_AGR_Danni_vw> WEB_AGR_Danni_vw { get; set; }
        public virtual DbSet<WEB_AGR_Gravita_vw> WEB_AGR_Gravita_vw { get; set; }
        public virtual DbSet<AGR_PERIZIE_TEMP_MVC> AGR_PERIZIE_TEMP_MVC { get; set; }
        public virtual DbSet<AGR_PerizieExpGrim_Temp_MVC> AGR_PerizieExpGrim_Temp_MVC { get; set; }
        public virtual DbSet<WEB_AUTO_ListaSpedizioni_vw> WEB_AUTO_ListaSpedizioni_vw { get; set; }
        public virtual DbSet<WEB_AUTO_FOTO> WEB_AUTO_FOTO { get; set; }
        public virtual DbSet<AGR_Perizie_MVC_Flat_vw> AGR_Perizie_MVC_Flat_vw { get; set; }
        public virtual DbSet<AAA_Prova> AAA_Prova { get; set; }
        public virtual DbSet<WEB_Auto_ListaPerizieXSpedizione_vw> WEB_Auto_ListaPerizieXSpedizione_vw { get; set; }
        public virtual DbSet<AGR_DatiRotabiliInUSo_vw> AGR_DatiRotabiliInUSo_vw { get; set; }
        public virtual DbSet<WEB_AUTO_PDF> WEB_AUTO_PDF { get; set; }
        public virtual DbSet<WEB_AUTO_ListaSpedizioni_2_vw> WEB_AUTO_ListaSpedizioni_2_vw { get; set; }
        public virtual DbSet<AGR_ModelliAuto_CAB_vw> AGR_ModelliAuto_CAB_vw { get; set; }
        public virtual DbSet<AGR_TrasportatoriGrimaldi_vw> AGR_TrasportatoriGrimaldi_vw { get; set; }
        public virtual DbSet<WEB_ListaPerizieFlat_MVC_vw> WEB_ListaPerizieFlat_MVC_vw { get; set; }
        public virtual DbSet<WEB_ListaPerizieFlat_DEF_vw> WEB_ListaPerizieFlat_DEF_vw { get; set; }
        public virtual DbSet<WEB_ListaPerizieFlat_TMP_vw> WEB_ListaPerizieFlat_TMP_vw { get; set; }
        public virtual DbSet<AGR_ModelliAuto_vw> AGR_ModelliAuto_vw { get; set; }
        public virtual DbSet<AGR_FilesTxt> AGR_FilesTxt { get; set; }
        public virtual DbSet<WEB_AUTO_ListaSpedizioni_CMN_vw> WEB_AUTO_ListaSpedizioni_CMN_vw { get; set; }
        public virtual DbSet<AGR_Parti_SDU> AGR_Parti_SDU { get; set; }
        public virtual DbSet<AGR_Gravita_SDU> AGR_Gravita_SDU { get; set; }
        public virtual DbSet<AGR_Danni_SDU> AGR_Danni_SDU { get; set; }
        public virtual DbSet<AGR_PERIZIE_DETT_TEMP_MVC_SDU_vw> AGR_PERIZIE_DETT_TEMP_MVC_SDU_vw { get; set; }
        public virtual DbSet<BKP_AGR_PERIZIE_TEMP_MVC> BKP_AGR_PERIZIE_TEMP_MVC { get; set; }
        public virtual DbSet<BKP_AGR_PERIZIE_DETT_TEMP_MVC> BKP_AGR_PERIZIE_DETT_TEMP_MVC { get; set; }
        public virtual DbSet<BKP_AGR_PerizieExpGrim_Temp_MVC> BKP_AGR_PerizieExpGrim_Temp_MVC { get; set; }
        public virtual DbSet<BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw> BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw { get; set; }
        public virtual DbSet<WEB_ListaPerizieFlat_DEF_ALL_vw> WEB_ListaPerizieFlat_DEF_ALL_vw { get; set; }
        public virtual DbSet<WEB_AUTO_FOTO_X_EMAIL> WEB_AUTO_FOTO_X_EMAIL { get; set; }
        public virtual DbSet<AGR_Perizie_TEMP_MVC_STANDBY> AGR_Perizie_TEMP_MVC_STANDBY { get; set; }
        public virtual DbSet<WEB_AUTO_ListaSpedizioni_3_vw> WEB_AUTO_ListaSpedizioni_3_vw { get; set; }
    }
}
