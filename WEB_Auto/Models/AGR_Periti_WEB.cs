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
    
    public partial class AGR_Periti_WEB
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string IDPerito { get; set; }
        public string IDPorto { get; set; }
        public string IDVero { get; set; }
        public int Classe { get; set; }
        public string Descr { get; set; }
        public Nullable<int> IDRuolo { get; set; }
        public string EmailAddress { get; set; }
        public string Lang { get; set; }
        public string Pwd { get; set; }
    
        public virtual AGR_Periti_WEB AGR_Periti_WEB1 { get; set; }
        public virtual AGR_Periti_WEB AGR_Periti_WEB2 { get; set; }
    }
}
