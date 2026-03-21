using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.User
{
    public class ResetPasswordByTokenViewModel : CommonFields
    {
        public string? ResetToken { get; set; }
        public string? NewPasswordHash { get; set; }
    }
}
