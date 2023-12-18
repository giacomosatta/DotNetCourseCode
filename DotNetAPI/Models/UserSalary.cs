using System.ComponentModel.DataAnnotations;

namespace DotnetAPI.Models;

public partial class UserSalary
{
    [Key]
    public int UserId { get; set; }
    public decimal Salary { get; set; }
    public decimal? AvgSalary { get; set; }
}