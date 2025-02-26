//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Category
    {
        public int CategoryID { get; set; }


        [Required(ErrorMessage = "You can't leave it blank.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Enter minimum 2 or maximum 40 char.")]
        [Display(Name = "Category Name", Description = "Name of the category")]

        public string CategoryName { get; set; }
    }
}
