using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class EditPassword : BaseViewModel
    {
        public string Password { get; set; }   
        
        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }

        public EditPassword(UserApi user)
        {
          
            Save = new CustomCommand(async() => 
            {  
                Core.Hash.HashCheck.CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.SaltHash = passwordSalt;
                await ChangeUser(user);
                UIManager.CloseWindow(this);
            });

            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }

        private async Task ChangeUser(UserApi userApi)
        {
            var user = await Api.PutAsync<UserApi>(userApi, "User");
        }
    }
}
