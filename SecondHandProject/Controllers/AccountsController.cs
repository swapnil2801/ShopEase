using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecondHandProject.APIServices;
using SecondHandProject.Models;
using System.Data;
using System.Data.SqlClient;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace SecondHandProject.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IHostingEnvironment _en;
        private readonly string _connStr;
        public AccountsController(IConfiguration config, IHostingEnvironment en)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
            _en = en;
        }

        [HttpPost]
        public async Task<ActionResult<RegisterUserResultModel>> RegisterUser([FromBody] RegisterUserModel model)
        {
            try
            {
                using(SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("UserName", model.UserName, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("UserEmail", model.UserEmail, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("UserPassword", HashPassword.HashUserPassword(model.UserPassword), dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("Id",0    , dbType: DbType.Int32, direction: ParameterDirection.Output);
                    string procedure = "RegisterUser";
                    dbcon.Open();

                    var result = dbcon.Query<RegisterUserResultModel>(procedure, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var obj = new RegisterUserResultModel() 
                    {
                        UserId = result.UserId,
                        UserName = result.UserName,
                        UserEmail = result.UserEmail,
                        CodeGenerated = result.CodeGenerated
                    };
                    SendEmail email = new SendEmail();
                    await email.VerifyEmail(result.UserEmail, result.CodeGenerated);
                    DeleteUnverifiedUser();
                    return Ok(obj);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EmailVerification([FromBody] EmailVerificationModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Email", model.UserEmail, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("Code", model.CodeGenerated, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    
                    string procedure = "VerifyEmail";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);
                    
                    return Ok("Your Email is an verified successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<SignInResultModel> SignInUser([FromBody] SignInModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Email", model.UserEmail, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("Password",HashPassword.HashUserPassword(model.UserPassword), dbType: DbType.String, direction: ParameterDirection.Input);

                    string procedure = "SignInProc";
                    dbcon.Open();

                    var result = dbcon.Query<SignInResultModel>(procedure, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var obj = new SignInResultModel()
                    {
                        Id = result.Id,
                        UserName = result.UserName,
                        UserEmail = result.UserEmail,
                        EmailIsComfirmed = result.EmailIsComfirmed
                    };

                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetCodeModel>> getGeneratedCode([FromBody] GetCodeModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Email", model.UserEmail, dbType: DbType.String, direction: ParameterDirection.Input);
                   
                    string procedure = "GetCodeProc";
                    dbcon.Open();

                    var result = dbcon.Query<GetCodeModel>(procedure, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var obj = new GetCodeModel()
                    {
                        UserId = result.UserId,
                        UserEmail = result.UserEmail,
                        CodeGenerated = result.CodeGenerated,
                        UpadatedDate = result.UpadatedDate
                    };
                    SendEmail email = new SendEmail();
                    await email.VerifyEmail(result.UserEmail, result.CodeGenerated);
                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Email", model.Email, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("Code", model.Code, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("NewPassword", model.NewPassword, dbType: DbType.String, direction: ParameterDirection.Input);


                    string procedure = "ResetPassword";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);

                    return Ok("Your Password has been changed successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddUserContacts([FromBody] ContactsModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("UserId", model.UserId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("TelNumber", model.TelNumber, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("FacebookUrl", model.FacebookUrl, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("InstagramUrl", model.InstagramUrl, dbType: DbType.String, direction: ParameterDirection.Input);


                    string procedure = "AddContactsProc";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);

                    return Ok("Your contact details has been changed successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet ("{id}")]
        public ActionResult<ContactsModel> GetContactDetails(int id)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Id", id, dbType: DbType.Int32, direction: ParameterDirection.Input);

                    string procedure = "GetContactDetailsProc";
                    dbcon.Open();

                    var result = dbcon.Query<ContactsModel>(procedure, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var obj = new ContactsModel()
                    {
                        UserId = result.UserId,
                        TelNumber = result.TelNumber,
                        FacebookUrl = result.FacebookUrl,
                        InstagramUrl = result.InstagramUrl
                    };

                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddUserImage([FromBody] UserImageModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("UserId", model.UserId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("UserImage",UploadImageClass.UploadProfileImage(model.ImageArray), dbType: DbType.String, direction: ParameterDirection.Input);
                    string procedure = "AddUserImageProc";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);
                    DeleteUnusedProfileImage();
                    return Ok("Your Image has been Added successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userid}")]
        public ActionResult<GetUserImage> GetUserImage(int Id)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("UserId", Id, dbType: DbType.Int32, direction: ParameterDirection.Input);

                    string procedure = "GetUserImagePorc";
                    dbcon.Open();

                    var result = dbcon.Query<GetUserImage>(procedure, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if(result == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var obj = new GetUserImage()
                        {
                            UserImage = result.UserImage,
                        };

                        return Ok(obj);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public ActionResult DeleteUnusedProfileImage()
        {
            var UserImagePath = Path.Combine(_en.WebRootPath, "ProfileImages");
            List<string> filelist = Directory.GetFiles(UserImagePath,"*.*",SearchOption.AllDirectories).ToList();
            List<GetUserImage> ImageList = new List<GetUserImage>();
            using (SqlConnection dbcon = new SqlConnection(_connStr))
            {
                string tQuery = "Select UserImage from UserImages";
                dbcon.Open();
                var result = dbcon.ExecuteReader(tQuery, commandType: CommandType.Text);
                while(result.Read())
                {
                    var i = new GetUserImage()
                    {
                        UserImage = result.IsDBNull(result.GetOrdinal("UserImage")) ? null : Convert.ToString(result["UserImage"]).Remove(0, 15)
                    };
                    ImageList.Add(i);
                }
                foreach(var item in filelist) 
                {
                    string ImageName = item.Substring(UserImagePath.Length + 1);
                    var selected = ImageList.Find(p => p.UserImage == ImageName);
                    if (selected != null)
                    {
                        System.IO.File.Delete(item);
                    }
                }
            }
            return Ok("Process is done");
        }


        public ActionResult DeleteUnverifiedUser()
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Id", 1, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    
                    string procedure = "EmailNotConfirmedProc";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);
                    return Ok("Process is done");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddUserAddress([FromBody] AddressModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("UserId", model.UserId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("ApartmentInfo", model.ApartmentInfo, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("BuildingInfo", model.BuildingInfo, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("StreetName", model.StreetName, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("StreetNo", model.StreetNo, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("CityName", model.CityName, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("PostalCode", model.PostalCode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("CountryName", model.CountryName, dbType: DbType.String, direction: ParameterDirection.Input);


                    string procedure = "AddUserAddress";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);

                    return Ok("Your Address details has been changed successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userid}")]
        public ActionResult<AddressModel> GetUserAddressInfo(int id)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("UserId", id, dbType: DbType.Int32, direction: ParameterDirection.Input);

                    string procedure = "GetUserAddressProc";
                    dbcon.Open();

                    var result = dbcon.Query<AddressModel>(procedure, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var obj = new AddressModel()
                    {
                        UserId = result.UserId,
                        ApartmentInfo = result.ApartmentInfo,
                        BuildingInfo = result.BuildingInfo,
                        StreetName = result.StreetName,
                        StreetNo = result.StreetNo,
                        CityName = result.CityName,
                        PostalCode = result.PostalCode,
                        CountryName = result.CountryName
                    };

                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        

    }
}
