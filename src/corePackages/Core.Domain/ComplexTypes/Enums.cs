using System.ComponentModel.DataAnnotations;

namespace Core.Domain.ComplexTypes.Enums
{
    public enum RecordStatu
    {
        None = 0,
        Active = 1,
        Passive = 2,
    }

    public enum FileType
    {
        None,
        Xls,
        Xlsx,
        Doc,
        Pps,
        Pdf,
        Img,
        Mp4,
    }

    public enum CultureType
    {
        None = 0,
        [Display(Name = "en-US")]
        US = 1,
        [Display(Name = "tr-TR")]
        TR = 2,
    }

    public enum AuthenticatorType
    {
        None = 0,
        Email = 1,
        Otp = 2,
    }

    public enum ConfirmationTypes
    {
        None = 0,
    }
}
