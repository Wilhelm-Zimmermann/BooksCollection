using backend.Dtos;
using backend.Entities;
using backend.Exceptions;
using backend.Repositories;
using backend.Utils;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _repository;
        private readonly IFileManager _fileManager;
        public UserService(IUsersRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
        }

        public async Task CreateUserAsync(CreateUserDto createUserDto)
        {
            //var encodedPassword = PasswordHash.EncodePasswordToBase64(createUserDto.Password);
            var formatedFileName = DateTime.Now.Ticks + createUserDto.ProfilePhoto.FileName;

            User user = new()
            {
                Email = createUserDto.Email,
                Name = createUserDto.Name,
                Password = createUserDto.Password,
                ProfilePhoto = createUserDto.ProfilePhoto.FileName,
            };

            await _fileManager.SaveImage(createUserDto.ProfilePhoto, formatedFileName, "Users");
            await _repository.CreateUserAsync(user);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var userExists = await _repository.GetUserByIdAsync(id);

            if(userExists is null)
            {
                throw new AppException("User not found");
            }

            return userExists;
        }

        public async Task<string> Login(string email, string password)
        {
            var userExists = await _repository.GetUserByEmailAsync(email);

            if(userExists is null)
            {
                throw new AppException("Email/Password might be invalid");
            }

            //var passwordDecrypted = PasswordHash.DecodeFrom64(password);

            if(userExists.Password != password)
            {
                throw new AppException("Email/Password might be invalid");
            }

            var token = TokenService.GenerateToken(userExists);

            return token;
        }
    }
}
