using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VoiceAPI.Configure;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Payload.Auths;
using VoiceAPI.Models.Responses.Accounts;
using VoiceAPI.Models.Responses.Auths;

namespace VoiceAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IMapper _mapper;

        private readonly IAccountRepository _accountRepository;
        private readonly IAdminRepository _adminRepository;

        public AuthService(IOptionsSnapshot<JwtConfig> options, IMapper mapper, 
            IAccountRepository accountRepository, 
            IAdminRepository adminRepository)
        {
            this._jwtConfig = options.Value;
            this._mapper = mapper;

            _accountRepository = accountRepository;
            _adminRepository = adminRepository;
        }

        public JwtTokenDTO GenerateToken(Account account)
        {
            var claims = new[]
{
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, account.Email ?? ""),
                new Claim(ClaimTypes.MobilePhone, account.PhoneNumber ?? ""),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime dateTime = DateTime.UtcNow.AddDays(30);

            var token = new JwtSecurityToken(claims: claims, expires: dateTime, signingCredentials: signIn);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var result = _mapper.Map<JwtTokenDTO>(account);

            result.JwtToken = jwtToken;

            return result;
        }

        public JwtTokenAdminDTO GenerateToken(Admin admin)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, "ADMIN"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime dateTime = DateTime.UtcNow.AddDays(30);

            var token = new JwtSecurityToken(claims: claims, expires: dateTime, signingCredentials: signIn);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var result = _mapper.Map<JwtTokenAdminDTO>(admin);

            result.JwtToken = jwtToken;

            return result;
        }

        public async Task<GenericResult<JwtTokenDTO>> LoginByPassword(LoginByPasswordPayload payload)
        {
            if (payload.IsUsePhoneNumber)
            {
                if (payload.PhoneNumber == null)
                    return GenericResult<JwtTokenDTO>.Error("V400_10",
                        "Số điện thoại không thể để trống.");

                var account = await _accountRepository
                    .GetByPhoneNumberAndPassword(payload.PhoneNumber, payload.Password);

                if (account == null)
                    return GenericResult<JwtTokenDTO>.Error("V400_11",
                        "Tài khoản hoặc mật khẩu không chính xác.");
                
                if (account.Status is AccountStatusEnum.BLOCKED or AccountStatusEnum.DELETED) 
                    return GenericResult<JwtTokenDTO>.Error("V400_60",
                        "Tài khoản này đã bị khóa hoặc bị xóa.");

                var jwtToken = GenerateToken(account);

                return GenericResult<JwtTokenDTO>.Success(jwtToken);
            }
            else
            {
                if (payload.Email == null)
                    return GenericResult<JwtTokenDTO>.Error("V400_12",
                        "Email không thể để trống.");

                var account = await _accountRepository
                    .GetByEmailAndPassword(payload.Email, payload.Password);

                if (account == null)
                    return GenericResult<JwtTokenDTO>.Error("V400_11",
                        "Tài khoản hoặc mật khẩu không chính xác.");
                
                if (account.Status is AccountStatusEnum.BLOCKED or AccountStatusEnum.DELETED) 
                    return GenericResult<JwtTokenDTO>.Error("V400_60",
                        "Tài khoản này đã bị khóa hoặc bị xóa.");

                var jwtToken = GenerateToken(account);

                return GenericResult<JwtTokenDTO>.Success(jwtToken);
            }
        }

        public async Task<GenericResult<JwtTokenAdminDTO>> AdminLoginByPassword(AdminLoginByPasswordPayload payload)
        {
            var admin = await _adminRepository
                .GetByEmailAndPassword(payload.Email, payload.Password);

            if (admin == null) 
                return GenericResult<JwtTokenAdminDTO>.Error("V400_11",
                        "Tài khoản hoặc mật khẩu không chính xác.");

            var jwtToken = GenerateToken(admin);

            return GenericResult<JwtTokenAdminDTO>.Success(jwtToken);
        }

        public async Task<GenericResult<AccountDTO>> GetMe(Guid id)
        {
            var account = await _accountRepository.GetById(id);

            if (account == null)
                return GenericResult<AccountDTO>.Error((int)HttpStatusCode.NotFound,
                    "Account is not found.");

            var response = _mapper.Map<AccountDTO>(account);

            return GenericResult<AccountDTO>.Success(response);
        }

        public async Task<GenericResult<Admin>> GetMeAdmin(Guid id)
        {
            var admin = await _adminRepository.GetById(id);

            if (admin == null)
                return GenericResult<Admin>.Error((int)HttpStatusCode.NotFound,
                    "Admin is not found.");

            return GenericResult<Admin>.Success(admin);
        }

        public async Task<GenericResult<JwtTokenDTO>> LoginByGoogle(LoginByGooglePayload payload)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(payload.GoogleId); // Get user by request's guid
            var targetAccount = await _accountRepository.GetByEmail(userRecord.Email);

            if (targetAccount != null) // if email existed in local database
            {
                if (targetAccount.Status == Entities.Enums.AccountStatusEnum.BLOCKED
                        || targetAccount.Status == Entities.Enums.AccountStatusEnum.DELETED)
                    return GenericResult<JwtTokenDTO>.Error((int)HttpStatusCode.BadRequest,
                                                "V400_44",
                                                "Tài khoản này đã bị khóa hoặc bị xóa.");

                FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(payload.AccessToken); // Verify & get FirebaseToken from AccessToken

                token.Claims.TryGetValue("email", out object email); // Get email form accessToken
                if (userRecord.Email.Equals(email))
                {
                    var jwtToken = GenerateToken(targetAccount);

                    var claim = new Dictionary<string, object> { { "email", userRecord.Email } };
                    await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(payload.GoogleId, claim);

                    return GenericResult<JwtTokenDTO>.Success(jwtToken);
                } else
                {
                    return GenericResult<JwtTokenDTO>.Error((int)HttpStatusCode.BadRequest,
                                                "V400_45",
                                                "Email nhận từ GoogleID và AccessToken không khớp.");
                }
            } else
            {
                return GenericResult<JwtTokenDTO>.Error((int)HttpStatusCode.BadRequest,
                                                "V400_46",
                                                "Email này chưa tồn tại trong hệ thống.");
            }
        }

        public async Task<GenericResult<JwtTokenDTO>> RegisterByGoogle(RegisterByGooglePayload payload)
        {
            FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(payload.AccessToken); // Verify & get FirebaseToken from AccessToken
            token.Claims.TryGetValue("email", out object email); // Get email from accessToken

            if (payload.Account.Email.Equals(email)) // Check if email from accessToken and email from payload is matched
            {
                var targetAccount = _mapper.Map<Account>(payload.Account);

                _accountRepository.Create(targetAccount);
                await _accountRepository.SaveAsync();

                var jwtToken = GenerateToken(targetAccount);

                return GenericResult<JwtTokenDTO>.Success(jwtToken);
            } else
            {
                return GenericResult<JwtTokenDTO>.Error((int)HttpStatusCode.BadRequest,
                                            "V400_46",
                                            "Emai từ AccessToken và Payload không khớp.");
            }
        }
    }
}
