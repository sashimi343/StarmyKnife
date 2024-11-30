using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    public enum Base64CharacterSet
    {
        [Display(Name = "Standard (RFC 4648)")]
        Standard,
        [Display(Name = "URL Safe (RFC 4648 Sec. 5)")]
        UrlSafe,
        [Display(Name = "Filename Safe")]
        FilenameSafe
    }

}
