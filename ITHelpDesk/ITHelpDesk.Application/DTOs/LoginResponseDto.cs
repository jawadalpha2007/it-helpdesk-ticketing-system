using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Application.DTOs
{
    public  class LoginResponseDto
    {
        public int Id { get; set; }
        public string token {  get; set; }= string.Empty;
        public string FullName { get; set; }=string.Empty;
        public string Email {  get; set; }=string.Empty; 
        public string Role {  get; set; }=string.Empty;

    }
}
