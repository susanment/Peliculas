using BlazorPeliculas.Shared.DTO;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<Usuarios> userManager;
        private readonly SignInManager<Usuarios> signInManager;
        private readonly IConfiguration configuration;

        public CuentasController(UserManager<Usuarios> userManager,
            SignInManager<Usuarios> signInManager,
            IConfiguration configuration
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("crear")]
        public async Task<ActionResult<UserTokenDTO>> CreateUser([FromBody] UserInfoDTO model)
        {
            var usuario = new Usuarios() { Email = model.Email };
            var resultado = await userManager.CreateAsync(usuario, model.Password);
            if (resultado.Succeeded)
            {
                return BuildToken(model);
            }
            else
            {
                return BadRequest(resultado.Errors.FirstOrDefault());
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] UserInfoDTO model)
        {
            var resultado = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            
            if (resultado.Succeeded)
            {
                return BuildToken(model);
            }
            else
            {
                return BadRequest("Intento de login fallido.");
            }
        }

        //permite crear un json web token a partir de lo que sea
        private UserTokenDTO BuildToken(UserInfoDTO userInfoDTO)
        {
            //Esto no es secreto por lo tanto no debe exponer se algo sensible(pass, tarjeta c)
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfoDTO.Email),
                new Claim("mivalor","Lo que yo quiera")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtkey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration= DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
                );
            return new UserTokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };

        }
    }
}
