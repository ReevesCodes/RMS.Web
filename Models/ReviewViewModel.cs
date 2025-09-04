using RMS.Data.Entities; 
using System.Collections.Generic;

namespace RMS.Web.Models
{
    public class ReviewViewModel
    {
        public DateTime CreatedAt {get ; set;}     
        
        
        public string AboutText { get; set; } 

        public string ReviewerName {get;set;}
    }
}
