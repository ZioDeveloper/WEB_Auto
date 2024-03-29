//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class BKP_AGR_PERIZIE_TEMP_MVC
    {
        public string ID { get; set; }
        public string BARCODE { get; set; }
        public string IDSpedizione { get; set; }
        public string IDPerito { get; set; }
        public string IDTipoPerizia { get; set; }
        public Nullable<System.DateTime> DataPerizia { get; set; }
        public string IDNazione { get; set; }
        public Nullable<int> IDModello { get; set; }
        public string Telaio { get; set; }
        public Nullable<short> NumFoto { get; set; }
        public string IDScheda { get; set; }
        public string Note { get; set; }
        public string NoteConc { get; set; }
        public Nullable<int> Flags { get; set; }
        public Nullable<System.DateTime> DRichiesta { get; set; }
        public string VRichiesta { get; set; }
        public Nullable<System.DateTime> DDefinizione { get; set; }
        public string VDefinizione { get; set; }
        public Nullable<System.DateTime> DContab { get; set; }
        public string VContab { get; set; }
        public string Stato { get; set; }
        public Nullable<int> FileNumber { get; set; }
        public string IDUtente { get; set; }
        public Nullable<System.DateTime> DataModUtente { get; set; }
        public Nullable<System.DateTime> DataModPerito { get; set; }
        public Nullable<System.DateTime> DataSpedizione { get; set; }
        public Nullable<System.DateTime> DataArrivo { get; set; }
        public Nullable<short> FlagControllo { get; set; }
        public Nullable<double> IRichiesta { get; set; }
        public Nullable<double> IDefinizione { get; set; }
        public Nullable<double> IContab { get; set; }
        public string IDMeteo { get; set; }
        public string IDShip { get; set; }
        public string IDPortL { get; set; }
        public string IDPortD { get; set; }
        public Nullable<System.DateTime> DataImbarco { get; set; }
        public Nullable<System.DateTime> DataSbarco { get; set; }
        public string IDLocalitaPerizia { get; set; }
        public Nullable<double> TotaleItems { get; set; }
        public int NumPDF { get; set; }
        public bool IsClosed { get; set; }
        public Nullable<int> IDOperatore { get; set; }
        public bool IsTransferred { get; set; }
        public string MachineName { get; set; }
    }
}
