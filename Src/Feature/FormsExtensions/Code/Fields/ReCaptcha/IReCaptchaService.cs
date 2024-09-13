﻿using System.Threading.Tasks;

namespace DEWAXP.Feature.FormsExtensions.Fields.ReCaptcha
{
    public interface IReCaptchaService
    {
        Task<bool> Verify(string response);
        bool VerifySync(string response);
    }
}
