using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        //Configuration file access
        private readonly IConfiguration _configuration;


        // We won't use this outside of this class, so we can scope it to this namespace.
        //We could create a seperate class and namespace if we wanted.

        //***We will go back and create seperate classes for these and a DB table also
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }   
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userID, string userName, string firstName, string lastName, string city)
            {
                UserId = userID;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(
            AuthenticationRequestBody authenticationRequestBody)
        {
            //1st: validate the username/password***
            var user = ValidateUserCredentials(
                authenticationRequestBody.Username, authenticationRequestBody.Password);

            if(user == null)
            {
                return Unauthorized();
            }

            //2nd: create a secret***
            //The secreet needs to be an array of bytes.
            //we retrieve the secretkey from the config file then use "Encoding.ASCII.GetBytes()"
            //to convert the secretkey into an array of bytes
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));

            //We need to sign the token***
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //What will be in the token? Claims info on who the user is
            //"sub", "given_name", and "family_name" are standard claim names. we could name the claims 
            //whatever we wanted to but lets follow the claims.
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            //Finally, the actual token
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            return new CityInfoUser(
                1,
                userName ?? "",
                "Rhea",
                "Doggie",
                "Manila");
        }
    }
}
