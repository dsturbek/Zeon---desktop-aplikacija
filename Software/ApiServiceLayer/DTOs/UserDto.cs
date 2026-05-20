using Newtonsoft.Json;

namespace ApiServiceLayer.DTOs
{
    public class UserDto
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("id_trainer")]
        public int? IdTrainer { get; set; }

        [JsonProperty("name_surname")]
        public string NameSurname { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("specialization")]
        public string Specialization { get; set; }

        [JsonProperty("employment_date")]
        public string EmploymentDate { get; set; }
    }
}
