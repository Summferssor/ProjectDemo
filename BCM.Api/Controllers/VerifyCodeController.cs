using System.DrawingCore.Imaging;
using System.IO;
using BCM.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BCM.Api.Controllers
{
    [EnableCors("CorsPolicy")] // ����
    [Route("BcmSystem/[controller]")]
    [ApiController]
    public class VerifyCodeController : Controller
    {
        /// <summary>
        /// 数字验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("num")]
        public FileContentResult NumberVerifyCode()
        {
            string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.NumberVerifyCode);
            Response.Cookies.Append("verify_code", code);
            byte[] codeImage = VerifyCodeHelper.GetSingleObj().CreateByteByImgVerifyCode(code, 100, 40);
            return File(codeImage, @"image/jpeg");
        }

        /// <summary>
        /// 字母验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("abc")]
        public FileContentResult AbcVerifyCode()
        {
            string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.AbcVerifyCode);
            Response.Cookies.Append("verify_code", code);
            var bitmap = VerifyCodeHelper.GetSingleObj().CreateBitmapByImgVerifyCode(code, 100, 40);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return File(stream.ToArray(), @"image/jpeg");
        }

        /// <summary>
        /// 混合验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("mix")]
        public FileContentResult MixVerifyCode()
        {
            string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.MixVerifyCode);
            Response.Cookies.Append("verify_code", code);

            var bitmap = VerifyCodeHelper.GetSingleObj().CreateBitmapByImgVerifyCode(code, 100, 40);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            return File(stream.ToArray(), @"image/jpeg");
        }

        /// <summary>
        /// 验证码验证
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("{code}")]
        public Message verify(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Message.Fail().Add("Content", "验证码为空");
            }
            var ckcode = Request.Cookies["verify_code"];
            Response.Cookies.Delete("verify_code");
            if (!code.Trim().Equals(ckcode.Trim()))
            {
                return Message.VerifyFail().Add("content","验证码错误");
            }
            return Message.Ok();
        }

    }
}