using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeVisualizer.Components.Algorithm;
using TreeVisualizer.Models;
using TreeVisualizer.DTOS;
using TreeVisualizer.Repositories;
using TreeVisualizer.Utils;
namespace TreeVisualizer.Services
{
    public class TreeService
    {
        TreeRepository _treeRepository; 
        public TreeService() 
        {
            _treeRepository = new TreeRepository();
        }
        #region APICode
        public ResponseEntity<int> CreateOrUpdate(TreeCreateDTO treeDTO, NodeUserControl? root, int? Id)
        {
            try
            {
                string serializedData = TreeHelper.SerializeTree(root);
                treeDTO.SerializedData = serializedData;
                int response = 0;
                if (Id == null)
                {
                    response = Create(treeDTO);
                    return new ResponseEntity<int>
                    {
                        Status = response >= 0, // Nếu response >= 0 thì Status là true (thành công)
                        ResponseCode = response >= 0 ? 201 : 500, // Nếu thành công là 201, ngược lại là 500
                        StatusMessage = response >= 0 ? "Succeed" : "Failed", // Tin nhắn tương ứng
                        Data = response >= 0 ? response : -1 // Dữ liệu là ID nếu thành công, ngược lại là -1
                    };
                }
                else
                {
                    response = Update(Id.Value, treeDTO);
                    return new ResponseEntity<int>
                    {
                        Status = response >= 0, // Nếu response >= 0 thì Status là true (thành công)
                        ResponseCode = response >= 0 ? 200 : 500, // Nếu thành công là 200, ngược lại là 500
                        StatusMessage = response >= 0 ? "Succeed" : "Failed", // Tin nhắn tương ứng
                        Data = response >= 0 ? response : -1 // Dữ liệu là ID nếu thành công, ngược lại là -1
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateOrUpdate: {ex.Message}");
                throw;
            }
        }
        public ResponseEntity<Tree> GetById(int id)
        {
            try
            {
                Tree? tree = _treeRepository.GetById(id);

                if (tree == null)
                {
                    return new ResponseEntity<Tree>
                    {
                        Status = false,
                        ResponseCode = 404, // Not Found
                        StatusMessage = $"Tree with ID {id} not found.",
                        Data = null
                    };
                }
                return new ResponseEntity<Tree>
                {
                    Status = true,
                    ResponseCode = 200, // OK
                    StatusMessage = "Succeed",
                    Data = tree
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetById: {ex.Message}");
                return new ResponseEntity<Tree>
                {
                    Status = false,
                    ResponseCode = 500, // Internal Server Error
                    StatusMessage = $"An unexpected error occurred while fetching tree by ID: {ex.Message}",
                    Data = null
                };
            }
        }
        public ResponseEntity<List<Tree>> GetByUserId(int userId)
        {
            try
            {
                List<Tree> trees = _treeRepository.GetByUserId(userId);

                if (trees == null || !trees.Any()) // Check for null list or empty list
                {
                    return new ResponseEntity<List<Tree>>
                    {
                        Status = true, // Still a success that we looked, just no data
                        ResponseCode = 200, // OK
                        StatusMessage = "No trees found for this user.",
                        Data = new List<Tree>() // Return an empty list instead of null for consistency
                    };
                }
                return new ResponseEntity<List<Tree>>
                {
                    Status = true,
                    ResponseCode = 200, // OK
                    StatusMessage = "Succeed",
                    Data = trees
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByUserId: {ex.Message}");
                return new ResponseEntity<List<Tree>>
                {
                    Status = false,
                    ResponseCode = 500, // Internal Server Error
                    StatusMessage = $"An unexpected error occurred while fetching trees by user ID: {ex.Message}",
                    Data = null
                };
            }
        }
        public ResponseEntity<bool> Delete(int id)
        {
            try
            {
                bool IsExist = _treeRepository.GetById(id) != null;
                bool deleted = _treeRepository.Delete(id);

                if (!IsExist)
                {
                    return new ResponseEntity<bool>
                    {
                        Status = false,
                        ResponseCode = 404, // Not Found or Bad Request if ID is invalid
                        StatusMessage = $"{id} Not found ",
                        Data = false
                    };
                }
                return new ResponseEntity<bool>
                {
                    Status = true,
                    ResponseCode = 200, // OK
                    StatusMessage = "Succeed",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete: {ex.Message}");
                return new ResponseEntity<bool>
                {
                    Status = false,
                    ResponseCode = 500, // Internal Server Error
                    StatusMessage = $"{ex.Message}",
                    Data = false
                };
            }
        }

        public int Create(TreeCreateDTO treeDTO)
        {
            try
            {
                return _treeRepository.Create(treeDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create: {ex.Message}");
                return -1; // Indicating failure
            }


        }
        public int Update(int id, TreeCreateDTO treeDTO)
        {
            try
            {
                return _treeRepository.Update(id, treeDTO);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update: {ex.Message}");
                return -1; // Indicating failure
            }
        }
        #endregion
        #region LogicCode
        #endregion
    }
}