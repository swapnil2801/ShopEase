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
    public class ItemsController : ControllerBase
    {
        private readonly IHostingEnvironment _en;
        private readonly string _connStr;
        public ItemsController(IConfiguration config, IHostingEnvironment en)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
            _en = en;
        }

        [HttpGet]
        public ActionResult GetCategories()
        {
            try
            {
                List<CategoryModel> CategotyList = new List<CategoryModel>();

                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    string query = "select * from CategoriesTb";
                    dbcon.Open();

                    var result = dbcon.ExecuteReader(query, commandType:CommandType.Text);  
                    
                    while (result.Read())
                    {
                        var category = new CategoryModel()
                        {
                            Id = result.IsDBNull(result.GetOrdinal("Id")) ? 0 : Convert.ToInt32(result["Id"]),
                            CategoryName = result.IsDBNull(result.GetOrdinal("CategoryName")) ? null : Convert.ToString(result["CategoryName"])
                        };
                        CategotyList.Add(category);
                    }
                    return Ok(CategotyList);
                }
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddItem([FromBody] ItemModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("UserId", model.UserId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("CategoryId", model.CategoryId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("ItemTitle", model.ItemTitle, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("ItemDescription", model.ItemDescription, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("ItemPrice", model.ItemPrice, dbType: DbType.Decimal, direction: ParameterDirection.Input);


                    string procedure = "AddItemsProc";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);

                    return Ok("Your product has been Added successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public ActionResult UpdateItem([FromBody] ItemModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Id", model.Id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("CategoryId", model.CategoryId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("ItemTitle", model.ItemTitle, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("ItemDiscription", model.ItemDescription, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("ItemPrice", model.ItemPrice, dbType: DbType.Decimal, direction: ParameterDirection.Input);


                    string procedure = "UpdateItem";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);

                    return Ok("Your product has been Updated successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/{status}")]
        public ActionResult ChangeItemStatus(int id, string status)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("ItemId", id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("ItemStatus", status, dbType: DbType.String, direction: ParameterDirection.Input);
                   

                    string procedure = "ChangeItemStatus";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);

                    return Ok("Your item status has been changed successfully to "+status);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/{level}")]
        public ActionResult ChangeAnnoucementLevel(int id, int level)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("ItemId", id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("AnnouncementLevel", level, dbType: DbType.String, direction: ParameterDirection.Input);


                    string procedure = "ChangeAnnoucementLevel";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);

                    return Ok("Your announcement level status has been changed successfully to " + level);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddItemImage([FromBody] ItemsImagesModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("ItemId", model.ItemId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("ImageUrl", UploadImageClass.UploadItemImage(model.ImageArray), dbType: DbType.String, direction: ParameterDirection.Input);
                    string procedure = "AddItemImage";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);
                    return Ok("Product's image has been Added successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult UpdateItemImage([FromBody] ItemsImagesModel model)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Id", model.Id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("ImageUrl", UploadImageClass.UploadItemImage(model.ImageArray), dbType: DbType.String, direction: ParameterDirection.Input);
                    string procedure = "UpdateItemImageProc";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);
                    DeleteUnusedItemsImage();
                    return Ok("Product's image has been updated successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public ActionResult DeleteUnusedItemsImage()
        {
            var ItemImagePath = Path.Combine(_en.WebRootPath, "ItemImages");
            List<string> filelist = Directory.GetFiles(ItemImagePath, "*.*", SearchOption.AllDirectories).ToList();
            List<GetItemImage> ImageList = new List<GetItemImage>();
            using (SqlConnection dbcon = new SqlConnection(_connStr))
            {
                string tQuery = "Select ImageUrl from ItemImage";
                dbcon.Open();
                var result = dbcon.ExecuteReader(tQuery, commandType: CommandType.Text);
                while (result.Read())
                {
                    var i = new GetItemImage()
                    {
                        ImageUrl = result.IsDBNull(result.GetOrdinal("ImageUrl")) ? null : Convert.ToString(result["ImageUrl"]).Remove(0, 12)
                    };
                    ImageList.Add(i);
                }
                foreach (var item in filelist)
                {
                    string ImageName = item.Substring(ItemImagePath.Length + 1);
                    var selected = ImageList.Find(p => p.ImageUrl == ImageName);
                    if (selected != null)
                    {
                        System.IO.File.Delete(item);
                    }
                }
            }
            return Ok("Process is done");
        }

        [HttpGet("{itemid}")]
        public ActionResult GetItemImages(int itemid)
        {
            try
            {
                List<GetItemImage> ItemImageList = new List<GetItemImage>();

                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("ItemId", itemid, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    string procedure = "GetItemImage";
                    dbcon.Open();

                    var result = dbcon.ExecuteReader(procedure,param, commandType: CommandType.StoredProcedure);

                    while (result.Read())
                    {
                        var category = new GetItemImage()
                        {
                            Id = result.IsDBNull(result.GetOrdinal("Id")) ? 0 : Convert.ToInt32(result["Id"]),
                            ItemId = result.IsDBNull(result.GetOrdinal("ItemId")) ? 0 : Convert.ToInt32(result["ItemId"]),
                            ImageUrl = result.IsDBNull(result.GetOrdinal("ImageUrl")) ? null : Convert.ToString(result["ImageUrl"])
                        };
                        ItemImageList.Add(category);
                    }
                    return Ok(ItemImageList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<GetItemImage> GetItemImage(int Id)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Id", Id, dbType: DbType.Int32, direction: ParameterDirection.Input);

                    string procedure = "GetItemImageProc";
                    dbcon.Open();

                    var result = dbcon.Query<GetItemImage>(procedure, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (result == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var obj = new GetItemImage()
                        {
                            Id = result.Id,
                            ItemId = result.ItemId,
                            ImageUrl = result.ImageUrl,
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

        [HttpPost("{id}")]
        public ActionResult DeleteItemImage(int id)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Id", id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    string procedure = "DeleteItemImage";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);
                    return Ok("Item image has been deleted successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}")]
        public ActionResult DeleteItem(int id)
        {
            try
            {
                using (SqlConnection dbcon = new SqlConnection(_connStr))
                {
                    var param = new DynamicParameters();
                    param.Add("Id", id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    string procedure = "DeleteItem";
                    dbcon.Open();

                    var result = dbcon.Execute(procedure, param, commandType: CommandType.StoredProcedure);
                    return Ok("The product has been deleted successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
